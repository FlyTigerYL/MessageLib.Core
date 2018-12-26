using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageLib.Core;
using MessageLib.Core.ClassBean;
using EasyNetQ;
using EasyNetQ.Topology;

namespace MessageLib.Core
{
    public class QueueTask
    {
        //队列数目发布数量 
        private Queue<Item> queueData = new Queue<Item>(5000);
        private string itemType = ItemType.info;
        private bool isRunning { get; set; } = false;
        public QueueTask(string itemType)
        {
            this.itemType = itemType;
        }

        /// <summary>
        /// 可供外部使用的消息入列操作
        /// </summary>
        public void Push(Item item, IBus IBus)
        {
            queueData.Enqueue(item);
            Run(IBus);
        }

        public void Run(IBus IBus)
        {
            if (!isRunning)
            {
                isRunning = true;
                JobHelper.GetInstance().Registrar("job-" + itemType, "group-" + itemType, PublisMsg, IBus);
            }
            //这里需要个定时器来发送queueData，
        }
        /// <summary>
        /// 推送消息，实时推送直接调用即可
        /// </summary>
        /// <param name="BusInstance"></param>
        private void PublisMsg(IBus BusInstance)
        {
            try
            {
                if (queueData.Count > 0)
                {
                    string channelName = itemType;
                    var mqqueue = BusInstance.Advanced.QueueDeclare(string.Format("Queue.{0}", channelName));
                    var exchange = BusInstance.Advanced.ExchangeDeclare(string.Format("Exchange.{0}", channelName), ExchangeType.Direct);
                    var binding = BusInstance.Advanced.Bind(exchange, mqqueue, mqqueue.Name);

                    while (queueData.Count > 0)
                    {
                        Item item = queueData.Dequeue();
                        if (item != null)
                        {
                            var properties = new MessageProperties();
                            var Message = new Message<string>(Newtonsoft.Json.JsonConvert.SerializeObject(item));
                            Message.Properties.AppId = item.appid;
                            BusInstance.Advanced.Publish(exchange, mqqueue.Name, false, Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PublisMsg error：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busInstance"></param>
        /// <param name="action"></param>
        public void Read<T>(IBus busInstance, Action<Item> action) where T : Item
        {
            try
            {
                string channelName = itemType;
                var mqqueue = busInstance.Advanced.QueueDeclare(string.Format("Queue.{0}", channelName));
                var exchange = busInstance.Advanced.ExchangeDeclare(string.Format("Exchange.{0}", channelName), ExchangeType.Direct);
                var binding = busInstance.Advanced.Bind(exchange, mqqueue, mqqueue.Name);

                var Consume = busInstance.Advanced.Consume(mqqueue, registration => Task.Run(() =>
                {
                    registration.Add<string>((message, info) =>
                    {
                        Item data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Body);
                        action(data);
                    });
                }));
                //Consume.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read error：" + ex.Message);
            }
        }
    }
}
