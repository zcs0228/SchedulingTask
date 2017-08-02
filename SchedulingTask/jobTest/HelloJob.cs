using Quartz;
using SchedulingTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jobTest
{
    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //get parameters
            JobDataMap data = context.JobDetail.JobDataMap;
            string SQL = data.GetString("sql");
            int count = data.GetInt("count");

            LogHelper.WriteInfoLog("---------------------------------" + System.DateTime.Now.ToString() + SQL + count.ToString() +
                "---------------------------------");
        }
    }
}
