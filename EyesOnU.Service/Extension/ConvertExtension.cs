using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Service.Extension
{
    public static class ConvertExtension
    {
        public static int GetInt(this string key, int defaultValue = 0)
        {
            if (int.TryParse(key, out int result))
            {
                return result;
            }
            else
            {
                Debug.WriteLine($"[Int Parse failed] {key}");
                return defaultValue;
            }
        }
        public static float GetFloat(this string key, float defaultValue = 0)
        {
            if (float.TryParse(key, out float result))
            {
                return result;
            }
            else
            {
                Debug.WriteLine($"[Int Parse failed] {key}");
                return defaultValue;
            }
        }
    }
}
