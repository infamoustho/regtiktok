using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    internal static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0] == "--test-chrome")
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                System.Threading.SynchronizationContext.SetSynchronizationContext(null);
                string deviceID = args.Length > 1 ? args[1] : "2231c68c14017ece";
                string email = args.Length > 2 ? args[2] : "hahuutrinhlkln9r@trustmailold.us";
                Console.WriteLine("==================================================");
                Console.WriteLine("🚀 RUNNING CHROME FLOW TEST");
                Console.WriteLine("📱 Device: " + deviceID);
                Console.WriteLine("📧 Email:  " + email);
                Console.WriteLine("==================================================");

                try
                {
                    using (var form = new Form1())
                    {
                        bool res = Task.Run(async () => await form.RunChromeFlowStandaloneAsync(deviceID, email)).GetAwaiter().GetResult();
                        Console.WriteLine("==================================================");
                        Console.WriteLine("🏁 TEST RESULT: " + (res ? "SUCCESS ✅" : "FAILED ❌"));
                        Console.WriteLine("==================================================");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Lỗi ngoại lệ trong Test Chrome: " + ex.ToString());
                }
                return;
            }

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
