using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                try { System.IO.File.AppendAllText("crash.log", "[ThreadException] " + e.Exception.ToString() + Environment.NewLine); } catch { }
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try { System.IO.File.AppendAllText("crash.log", "[UnhandledException] " + e.ExceptionObject.ToString() + Environment.NewLine); } catch { }
            };

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                try { System.IO.File.AppendAllText("crash.log", "[MainException] " + ex.ToString() + Environment.NewLine); } catch { }
            }
        }
    }
}
