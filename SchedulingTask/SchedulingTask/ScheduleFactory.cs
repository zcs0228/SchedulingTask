﻿using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlAdapter.XmlRepository;

namespace SchedulingTask
{
    public class ScheduleFactory
    {
        private readonly IScheduler scheduler;
        private XmlHelper xmlHelper = null;
        private readonly string filePath = AppDomain.CurrentDomain.BaseDirectory;

        public IScheduler Scheduler
        {
            get { return scheduler; }
        }

        public ScheduleFactory()
        {
            filePath += "schedule.config";
            xmlHelper = new XmlHelper(filePath);
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }
        /// <summary>
        /// 通过配置文件反射载入调度任务
        /// </summary>
        public void Initial()
        {
            #region 挂载添加引用程序集的事件
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
            #endregion

            XElement schedules = xmlHelper.QueryXElement("jobs").FirstOrDefault();
            if (schedules == null)
                return;
            foreach (XElement xe in schedules.Descendants("job"))
            {
                #region 获取任务信息
                string jobName = xe.Attribute("name").Value.Trim();//任务名称
                string jobGroup = xe.Attribute("group").Value.Trim();//任务组
                string triggerName = xe.Descendants("triggerName").FirstOrDefault().Value.Trim();//触发器名称
                string assemblyName = xe.Descendants("assemblyName").FirstOrDefault().Value.Trim();//程序集名称
                string nameSpace = xe.Descendants("nameSpace").FirstOrDefault().Value.Trim();//命名空间
                string className = xe.Descendants("className").FirstOrDefault().Value.Trim();//类名

                JobInfo jobInfo = new JobInfo
                {
                    JobName = jobName,
                    JobGroup = jobGroup
                };
                #endregion

                #region 获取触发器信息
                XElement triggers = xmlHelper.QueryXElement("triggers").FirstOrDefault();
                string triggerGroup = String.Empty;
                string cronExpression = String.Empty;
                int secondInterval = 0;
                foreach (XElement tr in triggers.Descendants("trigger"))
                {
                    string tempTriggerName = tr.Attribute("name").Value.Trim();
                    if (tempTriggerName == triggerName)
                    {
                        triggerGroup = tr.Descendants("TriggerGroup").FirstOrDefault().Value.Trim();
                        int.TryParse(tr.Descendants("SecondInterval").FirstOrDefault().Value.Trim(), out secondInterval);
                        cronExpression = tr.Descendants("CronExpression").FirstOrDefault().Value.Trim();
                    }
                }

                TriggerInfo triggerInfo = new TriggerInfo
                {
                    TriggerName = triggerName + "_" + jobName,//触发器名称由配置文件中的触发器名加任务名组成，防止重复报错
                    TriggerGroup = triggerGroup,
                    SecondInterval = secondInterval,
                    CronExpression = cronExpression
                };
                #endregion

                #region 获取参数信息
                Dictionary<string, object> param = new Dictionary<string, object>();
                XElement paramXElement = xe.Descendants("parameters").FirstOrDefault();
                foreach (XElement paramXe in paramXElement.Descendants())
                {
                    string paramName = paramXe.Name.ToString().Trim();
                    string paramValue = paramXe.Value.ToString().Trim();
                    param.Add(paramName, paramValue);
                }
                #endregion

                #region 反射生成实例
                Assembly assembly = Assembly.Load(assemblyName);
                object o = assembly.CreateInstance(nameSpace + "." + className, false);
                Type ot = o.GetType();
                #endregion

                AddScheduler(ot, jobInfo, triggerInfo, param);
            }
        }
        /// <summary>
        /// 反射载入调度任务
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="jobInfo"></param>
        /// <param name="triggerInfo"></param>
        /// <param name="jobParam"></param>
        /// <returns></returns>
        public bool AddScheduler(Type jobType, JobInfo jobInfo, TriggerInfo triggerInfo,
            Dictionary<string, object> jobParam)
        {
            try
            {
                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create(jobType)
                    .WithIdentity(jobInfo.JobName, jobInfo.JobGroup)
                    .Build();

                ITrigger trigger;
                if (triggerInfo.CronExpression == null || triggerInfo.CronExpression == String.Empty)
                {
                    // Trigger the job to run on the next round minute
                        trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerInfo.TriggerName, triggerInfo.TriggerGroup)
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(triggerInfo.SecondInterval)
                            .RepeatForever()
                            .WithMisfireHandlingInstructionNextWithExistingCount())
                        .Build();
                }
                else
                {
                    //使用Cron表达式生成触发器
                        trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerInfo.TriggerName, triggerInfo.TriggerGroup)
                        .StartNow().WithCronSchedule(triggerInfo.CronExpression)
                        .Build();
                }


                // set parameters
                foreach (var key in jobParam.Keys)
                {
                    job.JobDataMap.Put(key, jobParam[key]);
                }

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteErrLog("添加定时任务报错", se);
                return false;
            }
        }

        /// <summary>
        /// 手动载入调度任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobInfo"></param>
        /// <param name="triggerInfo"></param>
        /// <param name="jobParam"></param>
        /// <returns></returns>
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
                ITrigger trigger;
                if (triggerInfo.CronExpression == null || triggerInfo.CronExpression == String.Empty)
                {
                    trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerInfo.TriggerName, triggerInfo.TriggerGroup)
                    .StartNow()
                    .WithSchedule(SimpleScheduleBuilder.Create()
                        .WithIntervalInSeconds(triggerInfo.SecondInterval)
                        .RepeatForever())
                    .Build();
                }
                else
                {
                    //使用Cron表达式生成触发器
                    trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerInfo.TriggerName, triggerInfo.TriggerGroup)
                    .StartNow().WithCronSchedule(triggerInfo.CronExpression)
                    .Build();
                }

                // set parameters
                foreach (var key in jobParam.Keys)
                {
                    job.JobDataMap.Put(key, jobParam[key]);
                }

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);
                return true;
            }
            catch (SchedulerException se)
            {
                LogHelper.WriteErrLog("添加定时任务报错", se);
                return false;
            }
        }


        //添加引用程序集的事件
        private Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            //string path = @"E:\zcsgit\SchedulingTask\SchedulingTask\SchedulingTaskPlatform\bin\Debug\";
            //Assembly.LoadFrom(path + "Quartz.dll");
            return Assembly.Load("Quartz");
        }
    }
}
