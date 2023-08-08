using EyesOnU.Service;
using EyesOnU.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Compoment
{
    public enum CounterType
    {
        [Description("CPU usage")]
        [Postfix("%")]
        CPU,
        [Description("RAM usage")]
        [Postfix("B")]
        RAMused,
        [Description("RAM useable")]
        [Postfix("B")]
        RAMspace,
        [Description("Network")]
        [Postfix("B")]
        NET
    }
    public class CounterMonitor //: Label
    {
        public event EventHandler<DataEventArgs> ValueUpdated;
        PerformanceCounter _performanceCounter;
        NetworkCounter _networkCounter;
        public string DisplayValue { get; set; }
        public CounterType CounterType { get; set; }
        public CounterMonitor()
        {
            Initialize();
        }
        public CounterMonitor(PerformanceCounter counter, CounterType counterType)
        {
            Initialize();
            _performanceCounter = counter;
            CounterType = counterType;
        }

        public CounterMonitor(NetworkCounter counter, CounterType counterType)
        {
            Initialize();
            _networkCounter = counter;
            CounterType = counterType;
        }

        void Initialize()
        {
        }

        protected virtual void OnValueUpdated(DataEventArgs e)
        {
            ValueUpdated?.Invoke(this, e);
        }

        public void StartNext(int refreshRate)
        {
            while (true)
            {
                DisplayValue = NextValue();
                OnValueUpdated(new DataEventArgs(DisplayValue));
                System.Threading.Thread.Sleep(refreshRate);
            }
        }
        public string NextValue()
        {
            if (_performanceCounter != null)
            {
                return $"{ParseValueString(_performanceCounter.NextValue(), CounterType.GetPostfix())}";
            }
            else if (_networkCounter != null)
            {
                return $"↑{ParseValueString(_networkCounter.GetSent())} ↓{ParseValueString(_networkCounter.GetReceived())}";
            }
            return "No counter...";
        }
        public string ParseValueString(float value, string postfix = "B")
        {
            string infix = " ";
            if (value > 1073741824)
            {
                return $"{(value / 1073741824).ToString("0.00")}{infix}{(postfix == "B" ? "GB" : postfix)}";
            }
            if (value > 1048576)
            {
                return $"{(value / 1048576).ToString("0.00")}{infix}{(postfix == "B" ? "MB" : postfix)}";
            }
            else if (value > 1024)
            {
                return $"{(value / 1024).ToString("0.00")}{infix}{(postfix == "B" ? "KB" : postfix)}";
            }
            else
            {
                return $"{value.ToString("0.00")}{infix}{(postfix == "B" ? "B" : postfix)}";
            }
        }
    }

    public class DataEventArgs : EventArgs
    {
        public readonly string _data;
        public DataEventArgs(string data)
        {
            _data = data;
        }

        public string Data
        {
            get
            {
                return _data;
            }
        }
    }
}
