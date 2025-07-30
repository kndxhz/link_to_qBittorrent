using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace qBHelper
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!IsRunAsAdmin())
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        Arguments = string.Join(" ",
                                    Environment.GetCommandLineArgs()
                                               .Skip(1)
                                               .Select(a => "\"" + a + "\"")),
                        Verb = "runas",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    return;
                }
                catch
                {
                    MessageBox.Show("本程序需要以管理员权限运行",
                                    "需要管理员权限",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }
            }

            Application.Run(new MainForm(Environment.GetCommandLineArgs()));
        }

        private static bool IsRunAsAdmin()
        {
            var id = WindowsIdentity.GetCurrent();
            var pr = new WindowsPrincipal(id);
            return pr.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}