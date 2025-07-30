using System.Security.Cryptography;
using System.Text;
using System.Management;

public static class CryptoHelper
{
    /// <summary>
    /// 取得指定類型的硬體序號
    /// idType: "disk"=硬碟, "board"=主機板, "cpu"=CPU, "machine"=機器名稱
    /// </summary>
    public static string GetMachineKey(string idType = "")
    {
        switch (idType?.ToLowerInvariant())
        {
            case "disk":
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia");
                    foreach (ManagementObject wmi_HD in searcher.Get())
                    {
                        var serial = wmi_HD["SerialNumber"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(serial))
                            return serial;
                    }
                }
                catch { }
                break;
            case "board":
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                    foreach (ManagementObject board in searcher.Get())
                    {
                        var serial = board["SerialNumber"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(serial))
                            return serial;
                    }
                }
                catch { }
                break;
            case "cpu":
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                    foreach (ManagementObject cpu in searcher.Get())
                    {
                        var id = cpu["ProcessorId"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(id))
                            return id;
                    }
                }
                catch { }
                break;
            case "machine":
                return Environment.MachineName;
            default:
                return Environment.MachineName;
        }
        return Environment.MachineName;
    }

    public static byte[] EncryptString(string plainText)
    {
        var data = Encoding.UTF8.GetBytes(plainText);
        return ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
    }

    public static string DecryptToString(byte[] cipherData)
    {
        var data = ProtectedData.Unprotect(cipherData, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(data);
    }
}
