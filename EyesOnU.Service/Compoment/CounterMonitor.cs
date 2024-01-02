using EyesOnU.Service;
using EyesOnU.Service.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Service.Compoment
{
    public enum CounterType
    {
        [Description("CPU usage")]
        [CategoryName("Processor")]
        [CounterName("% Processor Time")]
        [InstanceName("_Total")]
        [Postfix("%")]
        CPU,
        [Description("RAM usage")]
        [CategoryName("Process")]
        [CounterName("Working Set")]
        [InstanceName("_Total")]
        [Postfix("B")]
        RAMused,
        [Description("RAM useable")]
        [CategoryName("Memory")]
        [CounterName("Available Bytes")]
        [Postfix("B")]
        RAMspace,
        [Description("RAM usage")]
        [CategoryName("Memory")]
        [CounterName("Available Bytes")]
        [Postfix("%")]
        RAMusage,
        [Description("Disk busy")]
        [CategoryName("PhysicalDisk")]
        [CounterName("% Disk Time")]
        [InstanceName("_Total")]
        [Postfix("%")]
        DISK,
        [Description("Network")]
        [CategoryName("Processor")]
        [CounterName("% Processor Time")]
        [InstanceName("_Total")]
        [Postfix("B")]
        NET
    }
    public sealed class CounterMonitor
    {
        public event EventHandler<DataEventArgs>? ValueUpdated;
        readonly PerformanceCounter? _performanceCounter;
        readonly NetworkCounter? _networkCounter;
        private string? DisplayValue { get; set; }
        public CounterType CounterType { get; set; }
        public int RefreshRate { get => _refreshRate; set { _refreshRate = value; Debug.WriteLine($"[{CounterType}]_refreshRate <= {value}"); } }
        int _refreshRate;

        public CounterMonitor()
        {
        }
        public CounterMonitor(PerformanceCounter counter, CounterType counterType)
        {
            _performanceCounter = counter;
            CounterType = counterType;
        }

        public CounterMonitor(NetworkCounter counter, CounterType counterType)
        {
            _networkCounter = counter;
            CounterType = counterType;
        }

        private void OnValueUpdated(DataEventArgs e)
        {
            ValueUpdated?.Invoke(this, e);
        }

        public void StartNext(int refreshRate)
        {
            RefreshRate = refreshRate;
            while (true)
            {
                DisplayValue = NextValue();
                OnValueUpdated(new DataEventArgs(DisplayValue));
                System.Threading.Thread.Sleep(RefreshRate);
            }
        }

        private string NextValue()
        {
            switch (CounterType)
            {
                case CounterType.CPU:
                case CounterType.RAMused:
                case CounterType.RAMspace:
                case CounterType.DISK:
                case CounterType.RAMusage:
                    return $"{ParseValueString(_performanceCounter.NextValue(), CounterType.GetPostfix())}";

                case CounterType.NET:
                    return $"↑{ParseValueString(_networkCounter.GetSent())} ↓{ParseValueString(_networkCounter.GetReceived())}";
                default:
                    return "No counter...";
            }
        }

        private string ParseValueString(float value, string postfix = "B")
        {
            string infix = " ";
            var kb = 1024;
            var mb = Math.Pow(kb, 2);
            var gb = Math.Pow(kb, 3);
            if (value > gb)
            {
                return $"{(value / gb):0.00}{infix}{(postfix == "B" ? "GB" : postfix)}";
            }
            if (value > mb)
            {
                return $"{(value / mb):0.00}{infix}{(postfix == "B" ? "MB" : postfix)}";
            }
            else if (value > kb)
            {
                return $"{(value / kb):0.00}{infix}{(postfix == "B" ? "KB" : postfix)}";
            }
            else
            {
                return $"{value:0.00}{infix}{(postfix == "B" ? "B" : postfix)}";
            }
        }
    }

    public class DataEventArgs : EventArgs
    {
        public string Data { get; }
        public DataEventArgs(string data)
        {
            Data = data;
        }
    }
}
