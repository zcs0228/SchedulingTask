using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingTask
{
    public sealed class SchedulingTaskRunner
    {
        private readonly IScheduler scheduler;

        public SchedulingTaskRunner()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public bool AddScheduler<T>(JobInfo jobInfo, TriggerInfo triggerInfo,
            Dictionary<string, object> jobParam) where T : IJob
        {
            try
            {
                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(jobInfo.JobName, jobInfo.JobGroup)
                    .Build();

                // Trigger the job to run on the next round minute
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerInfo.TriggerName, triggerInfo.TriggerGroup)
                    .StartNow()
                    .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(triggerInfo.SecondInterval).RepeatForever())
                    .Build();

                // set parameters
                foreach(var key in jobParam.Keys)
                {
                    job.JobDataMap.Put(key, jobParam[key]);
                }

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteLog("添加定时任务报错：" + se);
                return false;
            }
        }

        public bool Start()
        {
            try
            {
                scheduler.Start();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteLog("启动定时任务报错：" + se);
                return false;
            }
        }

        public bool Shutdown()
        {
            try
            {
                scheduler.Shutdown();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteLog("关闭定时任务报错：" + se);
                return false;
            }
        }

        public bool PauseAll()
        {
            try
            {
                scheduler.PauseAll();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteLog("关闭所有定时任务报错：" + se);
                return false;
            }
        }

        public bool ResumeAll()
        {
            try
            {
                scheduler.ResumeAll();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteLog("重启所有定时任务报错：" + se);
                return false;
            }
        }
    }
}
