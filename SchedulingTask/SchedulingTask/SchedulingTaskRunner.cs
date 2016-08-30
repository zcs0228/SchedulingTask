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
                LogHelper.WriteLog("启动定时任务报错：" + se);
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
                LogHelper.WriteLog("关闭定时任务报错：" + se);
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
                LogHelper.WriteLog("关闭所有定时任务报错：" + se);
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
                LogHelper.WriteLog("重启所有定时任务报错：" + se);
                return false;
            }
        }
    }
}
