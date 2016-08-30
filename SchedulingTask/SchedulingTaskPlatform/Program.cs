using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingTaskPlatform
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // get the name of our process
            string p = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(p);
            //if there is more than one process
            if (processes.Length > 1)
            {
                MessageBox.Show("程序已经在运行中", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            else
            {
                Application.Run(new SchedulingTaskFrom());
            }       
        }
    }
}
