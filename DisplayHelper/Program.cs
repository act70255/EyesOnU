using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DisplayHelper
{
    internal static class Program
    {
        #region Windows API
        // Windows API
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// 還原視窗
        /// </summary>
        private const int SW_RESTORE = 9;

        /// <summary>
        /// 刪除其他相同進程
        /// </summary>
        static void SetSingleInstance()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            Debug.WriteLine($"當前進程名稱：{currentProcessName}，進程數量：{processes.Length}");
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
                IntPtr hWnd = process.MainWindowHandle;
                if (process.Id != Process.GetCurrentProcess().Id && hWnd != IntPtr.Zero)
                {
                    try
                    {
                        // 最大化視窗
                        SetForegroundWindow(process.MainWindowHandle);
                        // 還原最小化的視窗
                        ShowWindow(hWnd, SW_RESTORE);
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
        #endregion

        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string activeWindow = config.AppSettings.Settings["ActiveWindow"].Value;
            if (!string.IsNullOrEmpty(activeWindow))
            {
                SetProcessToForeground(activeWindow);
            }
            SetSingleInstance();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
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
                dict["說明"] = "無資料啟動";
                dict["範例"] = "測試資料0";
                dict["範例0"] = "測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0測試資料0";
                dict["範例1"] = "測試資料0";
                dict["範例2"] = "測試資料0";
                dict["範例3"] = "測試資料0";
                dict["範例4"] = "測試資料0";
            }
            Application.Run(new DisplayForm(dict));
        }
    }
}