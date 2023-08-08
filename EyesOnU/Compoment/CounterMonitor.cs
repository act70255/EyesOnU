﻿using EyesOnU.Service;
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
        CPU,
        [Description("RAM usage")]
        RAMused,
        [Description("RAM useable")]
        RAMspace,
        [Description("Network")]
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

        public void StartNext()
        {
            while (true)
            {
                DisplayValue = NextValue();
                OnValueUpdated(new DataEventArgs(DisplayValue));
                System.Threading.Thread.Sleep(1000);
            }
        }
        public string NextValue()
        {
            if (_performanceCounter != null)
            {
                return $"{ParseValueString(_performanceCounter.NextValue(), CounterType == CounterType.CPU ? "%" : "B")}";
            }
            else if (_networkCounter != null)
            {
                return $"↑{ParseValueString(_networkCounter.GetSent())} ↓{ParseValueString(_networkCounter.GetReceived())}";
            }
            return "No counter...";
        }
        public string ParseValueString(float value, string postfix = "B")
        {
            if (value > 1073741824)
            {
                return (value / 1073741824).ToString("0.00") + (postfix == "B" ? "GB" : postfix);
            }
            if (value > 1048576)
            {
                return (value / 1048576).ToString("0.00") + (postfix == "B" ? "MB" : postfix);
            }
            else if (value > 1024)
            {
                return (value / 1024).ToString("0.00") + (postfix == "B" ? "KB" : postfix);
            }
            else
            {
                return value.ToString("0.00") + (postfix == "B" ? "B" : postfix);
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
