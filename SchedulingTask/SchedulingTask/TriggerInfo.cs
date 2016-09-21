using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingTask
{
    public struct TriggerInfo
    {
        //触发器名称
        public string TriggerName;
        //触发器组
        public string TriggerGroup;
        //循环秒
        public int SecondInterval;
        //Cron表达式
        public string CronExpression;
    }
}
