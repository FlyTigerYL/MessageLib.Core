using System;
using System.Collections.Generic;
using System.Threading;
using EasyNetQ;
using MessageLib.Core.ClassBean;
using System.Threading.Tasks;

namespace MessageLib.Core
{
    public static class MessageQueue
    {
        private static IBus bus = null;

        //消息队列
        private static QueueTask NoticQueue = null;
        //日志队列
        private static QueueTask LogQueue = null;
        //自定义
        private static QueueTask InfoQueue = null;

        #region 同步锁
        private static readonly object obj = new object();
        #endregion

        public static void Init(string Connection)
        {
            if (NoticQueue == null)
                NoticQueue = new QueueTask(ItemType.notic);
            if (LogQueue == null)
                LogQueue = new QueueTask(ItemType.error);
            if (InfoQueue == null)
                InfoQueue = new QueueTask(ItemType.info);
            if (string.IsNullOrEmpty(MQBusBuilder.Connnection))
                MQBusBuilder.Connnection = Connection;
        }

        public static IBus BusInstance
        {
            get
            {
                if (bus == null)
                {
                    lock (obj)
                    {
                        if (bus == null || !bus.IsConnected)
                        {
                            bus = MQBusBuilder.CreateMessageBus();
                        }
                    }
                }
                return bus;
            }
        }


        /// <summary>
        /// 可供外部使用的消息入列操作
        /// </summary>
        public static void Push(Item item)
        {
            if (string.IsNullOrWhiteSpace(MQBusBuilder.Connnection) || BusInstance == null)
                return;
            if (item.type == ItemType.notic)
            {
                NoticQueue.Push(item, BusInstance);
            }
            if (item.type == ItemType.error)
            {
                LogQueue.Push(item, BusInstance);
            }
            if (item.type == ItemType.info)
            {
                InfoQueue.Push(item, BusInstance);
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="itemType">消息类型</param>
        /// <param name="dealAction">处理方法</param>
        public static void Read(string itemType, Action<Item> dealAction)
        {
            if (itemType == ItemType.notic)
            {
                NoticQueue.Read<NoticItem>(BusInstance, dealAction);
            }
            if (itemType == ItemType.error)
            {
                LogQueue.Read<ErrorItem>(BusInstance, dealAction);
            }
            if (itemType == ItemType.info)
            {
                InfoQueue.Read<Message>(BusInstance, dealAction);
            }
        }
    }
}
