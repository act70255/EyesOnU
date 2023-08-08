using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Service
{
    public class NetworkCounter
    {
        PerformanceCounter performanceCounterSent;
        PerformanceCounter performanceCounterReceived;

        string instance;
        public NetworkCounter(string instance)
        {
            this.instance = instance;
            performanceCounterSent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
            performanceCounterReceived = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
        }
        public string Instance { get { return instance; } }
        public float GetSent()
        {
            return performanceCounterSent.NextValue();
        }
        public string GetSentString()
        {
            var value = GetSent();
            if(value > 1048576)
            {
                return (value / 1048576).ToString("0.00") + " MB";
            }
            else if(value > 1024)
            {
                return (value / 1024).ToString("0.00") + " KB";
            }
            else
            {
                return value.ToString("0.00") + " B";
            }
        }
        public float GetReceived()
        {
            return performanceCounterReceived.NextValue();
        }
        public string GetReceivedString()
        {
            var value = GetReceived();
            if (value > 1048576)
            {
                return (value / 1048576).ToString("0.00") + " MB";
            }
            else if (value > 1024)
            {
                return (value / 1024).ToString("0.00") + " KB";
            }
            else
            {
                return value.ToString("0.00") + " B";
            }
        }
    }
}
