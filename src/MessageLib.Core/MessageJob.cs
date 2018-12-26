using EasyNetQ;
using EasyNetQ.Topology;
using MessageLib.Core.ClassBean;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageLib.Core
{
    public class JobHelper
    {
        private readonly static object obj = new object();
        private static JobHelper instance;
        public static JobHelper GetInstance()
        {
            if (instance == null)
            {
                lock (obj)
                {
                    if (instance == null)
                    {
                        instance = new JobHelper();
                    }
                }
            }
            return instance;
        }
        private JobHelper()
        {
            if (factory == null)
                factory = new StdSchedulerFactory();
            if (scheduler == null)
                scheduler = factory.GetScheduler().Result;
        }

        private static ISchedulerFactory factory = null;
        private static IScheduler scheduler = null;
        public void Registrar(string jobName, string groupName, Action<IBus> action, IBus bus)
        {
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<MessageJob>().Build();
            job.JobDataMap.Put("action", action);
            job.JobDataMap.Put("bus", bus);
            //每5秒推送一次
            ITrigger trigger = TriggerBuilder.Create()
                 .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                 .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }

    public class MessageJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Action<IBus> action = context.JobDetail.JobDataMap.Get("action") as Action<IBus>;
            IBus bus = context.JobDetail.JobDataMap.Get("bus") as IBus;
            await Task.Run(() => { action.Invoke(bus); });
        }
    }

}
