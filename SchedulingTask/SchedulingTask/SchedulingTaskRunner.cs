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
        //private readonly IScheduler scheduler;
        private ScheduleFactory scheduleFactory;
        public ScheduleFactory ScheduleFactory
        {
            get { return scheduleFactory; }
        }

        public SchedulingTaskRunner()
        {
            //scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduleFactory = new ScheduleFactory();
            scheduleFactory.Initial();
            //scheduler = scheduleFactory.Scheduler;
        }

        public bool Start()
        {
            try
            {
                scheduleFactory.Scheduler.Start();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("启动定时任务报错：" + se);
                return false;
            }
        }

        public bool Shutdown()
        {
            try
            {
                scheduleFactory.Scheduler.Shutdown();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("关闭定时任务报错：" + se);
                return false;
            }
        }

        public bool PauseAll()
        {
            try
            {
                scheduleFactory.Scheduler.PauseAll();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("关闭所有定时任务报错：" + se);
                return false;
            }
        }

        public bool ResumeAll()
        {
            try
            {
                scheduleFactory.Scheduler.ResumeAll();
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("重启所有定时任务报错：" + se);
                return false;
            }
        }

        public bool PauseJob(string jobName, string groupName)
        {
            JobKey jobKey = new JobKey(jobName, groupName);
            try
            {
                scheduleFactory.Scheduler.PauseJob(jobKey);
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("关闭定时任务报错：" + se);
                return false;
            }
        }

        public bool ResumeJob(string jobName, string groupName)
        {
            JobKey jobKey = new JobKey(jobName, groupName);
            try
            {
                scheduleFactory.Scheduler.ResumeJob(jobKey);
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteInfoLog("重启定时任务报错：" + se);
                return false;
            }
        }
    }
}
