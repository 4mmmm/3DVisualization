using System;
using System.Windows.Forms;

namespace _3DVisualization
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
            LoginForm lf = new LoginForm();
            lf.ShowDialog();
            if (lf.DialogResult == DialogResult.OK)
            {
                Application.Run(new MainForm());
            }
        }
    }
}
