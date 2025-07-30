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
        /// �٭����
        /// </summary>
        private const int SW_RESTORE = 9;

        /// <summary>
        /// �R����L�ۦP�i�{
        /// </summary>
        static void SetSingleInstance()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            Debug.WriteLine($"��e�i�{�W�١G{currentProcessName}�A�i�{�ƶq�G{processes.Length}");
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
                            Debug.WriteLine($"�L�k�����i�{�G{ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �d����w�i�{�W�٪��{���A�ñN��D�����]���e��
        /// </summary>
        /// <param name="processName">�i�{�W�١]���t .exe�^</param>
        public static bool SetProcessToForeground(string processName)
        {
            // ����Ҧ��P�W���i�{
            Process[] processes = Process.GetProcessesByName(processName);
            // �ˬd�O�_����L�i�{�]�ư���e�i�{�^
            foreach (Process process in processes)
            {
                IntPtr hWnd = process.MainWindowHandle;
                if (process.Id != Process.GetCurrentProcess().Id && hWnd != IntPtr.Zero)
                {
                    try
                    {
                        // �̤j�Ƶ���
                        SetForegroundWindow(process.MainWindowHandle);
                        // �٭�̤p�ƪ�����
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
            return false; // ������L�i�{
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
                dict["����"] = "�L��ƱҰ�";
                dict["�d��"] = "���ո��0";
                dict["�d��0"] = "���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0���ո��0";
                dict["�d��1"] = "���ո��0";
                dict["�d��2"] = "���ո��0";
                dict["�d��3"] = "���ո��0";
                dict["�d��4"] = "���ո��0";
            }
            Application.Run(new DisplayForm(dict));
        }
    }
}