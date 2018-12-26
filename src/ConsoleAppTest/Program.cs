using MessageLib.Core;
using MessageLib.Core.ClassBean;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace ConsoleAppTest
{
    class Program
    {
        static string mqConn = "your connection string";
        static void Main(string[] args)
        {
            //初始化队列
            MessageQueue.Init(mqConn);

            //TestTask();
            //info
            MessageQueue.Read(ItemType.info, m =>
            {
                Console.WriteLine("info Message：" + m.body);
            });
            Console.Write("Send：");
            while (true)
            { 
                string str = Console.ReadLine(); 
                var message = new Message("test", str);
                MessageQueue.Push(message);

            }
        }

        static void TestTask()
        {
            Console.WriteLine("只需要启动一次监听：<多次启动则为多个消费者，多个消费者，用完记得Consume.Dispose()>");
            //notic customer
            MessageQueue.Read(ItemType.notic, m =>
            {
                Console.WriteLine("Notic Message：" + m.body);
            });
            //notic producer
            Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var notic = new NoticItem("test1", "test1-" + i.ToString());
                    MessageQueue.Push(notic);
                }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var notic = new NoticItem("test2", "test2-" + i.ToString());
                    MessageQueue.Push(notic);
                }
            });

            //error customer
            MessageQueue.Read(ItemType.error, m =>
            {
                Console.WriteLine("Error Message：" + m.body);
            });

            //error producer
            Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var notic = new ErrorItem("test1", "test1-" + i.ToString());
                    MessageQueue.Push(notic);
                }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var notic = new ErrorItem("test2", "test2-" + i.ToString());
                    MessageQueue.Push(notic);
                }
            });
        }
    }
}
