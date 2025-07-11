using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace EyesOnU
{
    internal static class Program
    {
        // Windows API
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            SetSingleInstance();
            ApplicationConfiguration.Initialize();

            var dict = new Dictionary<string, string>();
            if (args.Length > 0)
            {
                Uri uri = new Uri(args[0]);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                foreach (string key in queryParams)
                {
                    dict[key] = queryParams[key];
                }
            }

            //var form = new Monitor();
            if (!dict.Any())
            {
                dict["message"] = "無資料";
                dict["test"] = "測試資料0";
            }
            var form = new DisplayForm(dict);
            Application.Run(form);
        }

        /// <summary>
        /// 刪除其他相同進程
        /// </summary>
        static void SetSingleInstance()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);

            // Kill others
            if (processes.Length > 1)
            {
                foreach (Process process in processes)
                {
                    if (process.Id != Process.GetCurrentProcess().Id)   //skip current
                    {
                        try
                        {
                            process.CloseMainWindow();
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"無法關閉進程：{ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查找指定進程名稱的程式，並將其主視窗設為前景
        /// </summary>
        /// <param name="processName">進程名稱（不含 .exe）</param>
        public static bool SetProcessToForeground(string processName)
        {
            // 獲取所有同名的進程
            Process[] processes = Process.GetProcessesByName(processName);
            // 檢查是否有其他進程（排除當前進程）
            foreach (Process process in processes)
            {
                if (process.Id != Process.GetCurrentProcess().Id && process.MainWindowHandle != IntPtr.Zero)
                {
                    try
                    {
                        // 最大化視窗
                        SetForegroundWindow(process.MainWindowHandle);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return false;
                    }
                }
            }
            return false; // 未找到其他進程
        }
    }
}