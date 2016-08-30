using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SchedulingTask;

namespace SchedulingTaskPlatform
{
    public partial class SchedulingTaskFrom : Form
    {
        private SchedulingTaskRunner taskRunner;
        public SchedulingTaskFrom()
        {
            taskRunner = new SchedulingTaskRunner();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            //HostFactory.Run(x =>
            //{
            //    x.UseLog4Net();

            //    x.Service<ServiceRunner>();

            //    x.SetDescription("QuartzDemo服务描述");
            //    x.SetDisplayName("QuartzDemo服务显示名称");
            //    x.SetServiceName("QuartzDemo服务名称");

            //    x.EnablePauseAndContinue();
            //});
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("sql", "select * from zcs");
            param.Add("count", 1);
            JobInfo jobInfo = new JobInfo
            {
                JobName = "a",
                JobGroup = "b"
            };
            TriggerInfo triggerInfo = new TriggerInfo
            {
                TriggerName = "a",
                TriggerGroup = "b",
                SecondInterval = 10
            };
            taskRunner.AddScheduler<HelloJob>(jobInfo, triggerInfo, param);
            taskRunner.Start();
        }



        #region 窗体隐藏到托盘操作事件
        //托盘图标左键事件
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //左键发生
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.notifyIcon1.Visible = true;
            }
        }
        //托盘退出按钮事件
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //点击"是(YES)"退出程序
            if (MessageBox.Show("确定要退出程序?", "安全提示",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                this.notifyIcon1.Visible = false;   //设置图标不可见
                taskRunner.Shutdown();
                this.Close();                  //关闭窗体
                this.Dispose();                //释放资源
                Application.Exit();            //关闭应用程序窗体
            }
        }
        //窗体关闭事件
        private void SchedulingTaskFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            //窗体关闭原因为单击“关闭”按钮或Alt+F4
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //取消关闭操作，表现为不关闭窗体
                this.Hide();
            }
        }
        #endregion
    }
}
