using EyesOnU.Compoment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Service
{
    public sealed class SystemMonitorService
    {
        private static readonly Lazy<SystemMonitorService> lazy = new Lazy<SystemMonitorService>(() => new SystemMonitorService());

        public static SystemMonitorService Instance { get { return lazy.Value; } }

        List<NetworkCounter> networkCounters = new List<NetworkCounter>();
        PerformanceCounter performanceCounterCpu;
        PerformanceCounter performanceCounterRamused;
        PerformanceCounter performanceCounterRamspace;

        private SystemMonitorService()
        {
        }

        public List<CounterMonitor> GetCounterMonitors()
        {
            List<CounterMonitor> monitors = new List<CounterMonitor>();

            performanceCounterCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            monitors.Add(new CounterMonitor(performanceCounterCpu, CounterType.CPU));
            performanceCounterRamused = new PerformanceCounter("Process", "Working Set", "_Total");
            monitors.Add(new CounterMonitor(performanceCounterRamused, CounterType.RAMused));
            performanceCounterRamspace = new PerformanceCounter("Memory", "Available Bytes");
            monitors.Add(new CounterMonitor(performanceCounterRamspace, CounterType.RAMspace));

            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            networkCounters = performanceCounterCategory.GetInstanceNames().Select(instance => new NetworkCounter(instance)).ToList();
            monitors.AddRange(networkCounters.Select(counter => new CounterMonitor(counter, CounterType.NET)));
            return monitors;
        }
    }
}
