﻿using NLog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SystemModule.Base;

namespace GameSrv {
    /// <summary>
    /// 统计系统运行状态 
    /// 仅支持Windows系统
    /// </summary>
    //[SupportedOSPlatform("windows")]
    public class WordStatistics {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string processName;
        private readonly PerformanceCounter MemoryCounter;
        private readonly PerformanceCounter CpuCounter;

        public WordStatistics() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                processName = Process.GetCurrentProcess().ProcessName;
                MemoryCounter = new PerformanceCounter();
                CpuCounter = new PerformanceCounter();
            }
        }

        public void ShowServerStatus()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))//todo 待实现MACOS下的状态显示
            {
                return;
            }
            ServerEnvironment.GetCPULoad();
            ServerEnvironment.MemoryInfo memoryInfo = ServerEnvironment.GetMemoryStatus();
            _logger.Debug("CPU使用率:[{0}%]", ServerEnvironment.CpuLoad.ToString("F"));
            _logger.Debug($"物理内存:[{HUtil32.FormatBytesValue(memoryInfo.ullTotalPhys)}] 内存使用率:[{memoryInfo.dwMemoryLoad}%] 空闲内存:[{HUtil32.FormatBytesValue(memoryInfo.ullAvailPhys)}]");
            _logger.Debug($"虚拟内存:[{HUtil32.FormatBytesValue(memoryInfo.ullTotalVirtual)}] 虚拟内存使用率:[{ServerEnvironment.VirtualMemoryLoad}%] 空闲虚拟内存:[{HUtil32.FormatBytesValue(memoryInfo.ullAvailVirtual)}]");
            _logger.Debug($"使用内存:[{HUtil32.FormatBytesValue(ServerEnvironment.UsedPhysicalMemory)}] 工作内存:[{HUtil32.FormatBytesValue(ServerEnvironment.PrivateWorkingSet)}] GC内存:[{HUtil32.FormatBytesValue(GC.GetTotalMemory(false))}] ");
            _logger.Debug($"网络流入:[{HUtil32.FormatBytesValue(ServerEnvironment.PerSecondBytesReceived)}] 网络流出:[{HUtil32.FormatBytesValue(ServerEnvironment.PerSecondBytesSent)}]");
            ShowGCStatus();
            GetRunTime();
            FreeMemory();

            TimeSpan ts = DateTimeOffset.Now - DateTimeOffset.FromUnixTimeMilliseconds(M2Share.StartTime);
            _logger.Debug("{0}", "=".PadLeft(64, '='));
            _logger.Debug("{0}", $"Server Start Time: {DateTimeOffset.FromUnixTimeMilliseconds(M2Share.StartTime):G}");
            _logger.Debug("{0}", $"Total Online Time: {(int)ts.TotalDays} days, {ts.Hours} hours, {ts.Minutes} minutes, {ts.Seconds} seconds");
            //_logger.Debug("{0}", $"Online Players[{Kernel.RoleManager.OnlinePlayers}], Max Online Players[{Kernel.RoleManager.MaxOnlinePlayers}], Distinct Players[{Kernel.RoleManager.OnlineUniquePlayers}], Role Count[{Kernel.RoleManager.RolesCount}]");
            _logger.Debug("{0}", $"Total Bytes Sent: {M2Share.NetworkMonitor.TotalBytesSent:N0}, Total Packets Sent: {M2Share.NetworkMonitor.TotalPacketsSent:N0}");
            _logger.Debug("{0}", $"Total Bytes Recv: {M2Share.NetworkMonitor.TotalBytesRecv:N0}, Total Packets Recv: {M2Share.NetworkMonitor.TotalPacketsRecv:N0}");
            _logger.Debug("{0}", $"System Thread: {M2Share.SystemProcess.ElapsedMilliseconds:N0}ms");
            //_logger.Debug("{0}", $"Generator Thread: {M2Share.GeneratorManager.ElapsedMilliseconds}ms");
            _logger.Debug("{0}", $"User Thread: {M2Share.UserProcessor.ElapsedMilliseconds:N0}ms");
            _logger.Debug("{0}", $"Ai Thread: {M2Share.RobotProcessor.ElapsedMilliseconds:N0}ms ({M2Share.RobotProcessor.ProcessedMonsters} AI Agents)");
            _logger.Debug("{0}", $"Identities Remaining: ");
            //_logger.Debug("{0}", $"\tMonster: {IdentityGenerator.Monster.IdentitiesCount()}");
            //_logger.Debug("{0}", $"\tFurniture: {IdentityGenerator.Furniture.IdentitiesCount()}");
            //_logger.Debug("{0}", $"\tMapItem: {IdentityGenerator.MapItem.IdentitiesCount()}");
            //_logger.Debug("{0}", $"\tTraps: {IdentityGenerator.Traps.IdentitiesCount()}");
            _logger.Debug("{0}", "=".PadLeft(64, '='));
        }

        private void FreeMemory()
        {
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            if (gcMemoryInfo.TotalCommittedBytes > gcMemoryInfo.TotalAvailableMemoryBytes / 2)
            {
                _logger.Debug("释放内存...");
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
            }
        }

        private void GetRunTime() {
            TimeSpan ts = DateTimeOffset.Now - DateTimeOffset.FromUnixTimeMilliseconds(M2Share.StartTime);
            _logger.Debug($"服务器运行:[{ts.Days}天{ts.Hours}小时{ts.Minutes}分{ts.Seconds}秒]");
        }

        private void ShowGCStatus() {
            _logger.Debug($"GC回收:[{GC.CollectionCount(0)}]次 GC内存:[{HUtil32.FormatBytesValue(GC.GetTotalMemory(false))}] ");
            GC.Collect(0, GCCollectionMode.Forced, false);
        }

        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        private string GetProcessorData() {
            float d = GetCounterValue(CpuCounter, "Processor", "% Processor Time", processName);
            return d.ToString("F") + "%";
        }

        private static string GetCpuUsageForProcess() {
            DateTime startTime = DateTime.UtcNow;
            TimeSpan startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Thread.SpinWait(500);
            DateTime endTime = DateTime.UtcNow;
            TimeSpan endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            double totalMsPassed = (endTime - startTime).TotalMilliseconds;
            double cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return (cpuUsageTotal * 100).ToString("F") + "%";
        }

        /// <summary>
        /// 获取当前程序线程数
        /// </summary>
        /// <returns></returns>
        private float GetThreadCount() {
            return GetCounterValue(CpuCounter, "Process", "Thread Count", processName);
        }

        /// <summary>
        /// 获取工作集内存大小
        /// </summary>
        /// <returns></returns>
        private float GetWorkingSet() {
            return GetCounterValue(MemoryCounter, "Memory", "Working Set", processName);
        }

        /// <summary>
        /// 获取虚拟内存使用率详情
        /// </summary>
        /// <returns></returns>
        private string GetMemoryVData() {
            float d = GetCounterValue(MemoryCounter, "Memory", "% Committed Bytes In Use", null);
            string str = d.ToString("F") + "% (";
            d = GetCounterValue(MemoryCounter, "Memory", "Committed Bytes", null);
            str += HUtil32.FormatBytesValue(d) + " / ";
            d = GetCounterValue(MemoryCounter, "Memory", "Commit Limit", null);
            return str + HUtil32.FormatBytesValue(d) + ") ";
        }

        /// <summary>
        /// 获取虚拟内存使用率
        /// </summary>
        /// <returns></returns>
        private float GetUsageVirtualMemory() {
            return GetCounterValue(MemoryCounter, "Memory", "% Committed Bytes In Use", null);
        }

        /// <summary>
        /// 获取虚拟内存已用大小
        /// </summary>
        /// <returns></returns>
        private float GetUsedVirtualMemory() {
            return GetCounterValue(MemoryCounter, "Memory", "Committed Bytes", null);
        }

        /// <summary>
        /// 获取虚拟内存总大小
        /// </summary>
        /// <returns></returns>
        private float GetTotalVirtualMemory() {
            return GetCounterValue(MemoryCounter, "Memory", "Commit Limit", null);
        }

        /// <summary>
        /// 获取空闲的物理内存数，单位B
        /// </summary>
        /// <returns></returns>
        private float GetFreePhysicalMemory() {
            return GetCounterValue(MemoryCounter, "Memory", "Available Bytes", null);
        }

        private static float GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName) {
            pc.CategoryName = categoryName;
            pc.CounterName = counterName;
            pc.InstanceName = instanceName;
            return pc.NextValue();
        }
    }
}