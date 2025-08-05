using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace TOTP
{
    public class TOTPRecordModel
    {
        public string Description { get; set; } = "";
        public string Secret { get; set; } = "";
    }

    public static class RecordProvider
    {
        public static Action RecordsChangedEventHandler { get; set; }
        public static List<TOTPRecordModel> Records { get; private set; } = [];
        private static string GetRecordsFilePath()
        {
            var key = CryptoHelper.GetMachineKey("disk");
            var safeKey = string.Concat(key.Where(char.IsLetterOrDigit)); // Á×§K¯S®í¦r¤¸
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TOTP", $"{safeKey}.dat");
        }

        public static List<TOTPRecordModel> LoadRecords()
        {
            try
            {
                var filePath = GetRecordsFilePath();
                Debug.WriteLine("LoadRecords: " + filePath);
                if (File.Exists(filePath))
                {
                    var encrypted = File.ReadAllBytes(filePath);
                    var json = CryptoHelper.DecryptToString(encrypted);
                    Records = JsonSerializer.Deserialize<List<TOTPRecordModel>>(json) ?? new();
                }
                else
                {
                    Records = new();
                }
            }
            catch (Exception ex)
            {
                Records = new();
                Debug.WriteLine(ex);
            }
            return Records;
        }

        public static void SaveRecords()
        {
            try
            {
                var filePath = GetRecordsFilePath();
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir!);

                var json = JsonSerializer.Serialize(Records, new JsonSerializerOptions { WriteIndented = true });
                var encrypted = CryptoHelper.EncryptString(json);
                File.WriteAllBytes(filePath, encrypted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static void AddRecord(string description, string secret)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = "New Record " + (Records.Count + 1);
            }
            if (Records.FirstOrDefault(r => r.Description == description) is TOTPRecordModel recordData)
            {
                recordData.Secret = secret;
                SaveRecords();
            }
            else
            {
                Records.Add(new TOTPRecordModel { Description = description, Secret = secret });
                SaveRecords();
            }
        }

        public static void DeleteRecord(string secret)
        {
            var record = Records.Find(r => r.Secret == secret);
            if (record != null)
            {
                Records.Remove(record);
                SaveRecords();
                RecordsChangedEventHandler?.Invoke();
            }
        }
    }
}
