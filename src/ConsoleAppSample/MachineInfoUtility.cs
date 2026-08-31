using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSample
{
    public static class MachineInfoUtility
    {
        /// <summary>
        /// Gets the human-readable name of the operating system.
        /// Example: "Microsoft Windows 11 Pro" or "Ubuntu 22.04.4 LTS"
        /// </summary>
        public static string OsName => RuntimeInformation.OSDescription;

        /// <summary>
        /// Gets the system architecture (e.g., X64, Arm64).
        /// </summary>
        public static string OsArchitecture => RuntimeInformation.OSArchitecture.ToString();

        /// <summary>
        /// Gets the architecture of the running application process.
        /// </summary>
        public static string AppArchitecture => RuntimeInformation.ProcessArchitecture.ToString();

        /// <summary>
        /// Gets the exact .NET runtime version running this code.
        /// </summary>
        public static string FrameworkVersion => RuntimeInformation.FrameworkDescription;

        /// <summary>
        /// Gets the NetBIOS or configured host name of the machine.
        /// </summary>
        public static string MachineName => Environment.MachineName;

        /// <summary>
        /// Gets the current logged-in user name.
        /// </summary>
        public static string UserName => Environment.UserName;

        /// <summary>
        /// Gets the domain or machine name associated with the current user.
        /// </summary>
        public static string UserDomain => Environment.UserDomainName;

        /// <summary>
        /// Gets the friendly display name of the system's current geographic region.
        /// </summary>
        public static string RegionName => RegionInfo.CurrentRegion.DisplayName;

        /// <summary>
        /// Gets the two-letter ISO region code (e.g., "US", "AU", "GB").
        /// </summary>
        public static string RegionCode => RegionInfo.CurrentRegion.Name;

        public static bool IsRunningAsAdmin => Environment.IsPrivilegedProcess;

        /// <summary>
        /// Gets the physical memory (Working Set) currently allocated for this application process.
        /// </summary>
        public static long AppMemoryBytes => Process.GetCurrentProcess().WorkingSet64;

        /// <summary>
        /// Gets the application memory usage formatted as a human-readable string (MB).
        /// </summary>
        public static string AppMemoryFormatted => $"{AppMemoryBytes / (1024 * 1024):N0} MB";

        /// <summary>
        /// Gets the total amount of memory currently managed by the .NET Garbage Collector.
        /// </summary>
        public static string ManagedMemoryFormatted => $"{GC.GetTotalMemory(false) / (1024 * 1024):N0} MB";


        /// <summary>
        /// Retrieves a comma-separated list of active local IPv4 addresses.
        /// Returns "Unknown" if an exception occurs or no network interface is found.
        /// </summary>
        public static string GetLocalIpAddresses()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ipList = new System.Collections.Generic.List<string>();
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(ip.ToString());
                    }
                }
                return ipList.Count > 0 ? string.Join(", ", ipList) : "No IPv4 Found";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static void PrintDiskSpaceSummary(StringBuilder sb)
        {
            sb.AppendLine("\n--- Disk Space Information ---");
            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.IsReady)
                    {
                        double totalSizeGb = d.TotalSize / (1024.0 * 1024.0 * 1024.0);
                        double freeSpaceGb = d.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0);
                        double usedSpaceGb = totalSizeGb - freeSpaceGb;
                        double percentUsed = (usedSpaceGb / totalSizeGb) * 100;

                        sb.AppendLine($"Drive {d.Name} [{d.DriveFormat}]:");
                        sb.AppendLine($"  Total Space: {totalSizeGb:F2} GB");
                        sb.AppendLine($"  Free Space:  {freeSpaceGb:F2} GB");
                        sb.AppendLine($"  Used Space:  {usedSpaceGb:F2} GB ({percentUsed:F1}% used)");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error retrieving disk info: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the recent CPU usage percentage of this specific application process.
        /// Takes two samples over a short interval (e.g., 500ms) to deliver a reliable cross-platform reading.
        /// </summary>
        public static double GetAppCpuUsagePercentage(int sampleTimeMs = 500)
        {
            try
            {
                var process = Process.GetCurrentProcess();

                // Sample 1
                var startTime = DateTime.UtcNow;
                var startCpuTime = process.TotalProcessorTime;

                // Synchronous block
                Thread.Sleep(sampleTimeMs);

                // Sample 2
                var endTime = DateTime.UtcNow;
                var endCpuTime = process.TotalProcessorTime;

                // Calculate metrics
                double samplePeriodSec = (endTime - startTime).TotalSeconds;
                double totalCpuTimeUsedSec = (endCpuTime - startCpuTime).TotalSeconds;

                if (samplePeriodSec == 0) return 0.0;

                // Divide by total logical CPU cores to scale appropriately (0% to 100%)
                double cpuUsage = (totalCpuTimeUsedSec / samplePeriodSec) / Environment.ProcessorCount;

                return Math.Round(cpuUsage * 100, 1);
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Prints a comprehensive, formatted summary of all system information to the console.
        /// </summary>
        public static string PrintSystemSummary()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("====================================");
            sb.AppendLine("=== PORTABLE MACHINE INFORMATION ===");
            sb.AppendLine("====================================");
            sb.AppendLine($"Machine Name:      {MachineName}");
            sb.AppendLine($"User Account:      {UserDomain}\\{UserName}");
            sb.AppendLine($"Privilege Level:   {(IsRunningAsAdmin ? "Elevated (Admin/Root)" : "Standard User")}");
            sb.AppendLine($"Operating System:  {OsName} ({OsArchitecture})");
            sb.AppendLine($"App Architecture:  {AppArchitecture}");
            sb.AppendLine($"Runtime Version:   {FrameworkVersion}");
            sb.AppendLine($"Cores Available:   {Environment.ProcessorCount} Cores");
            sb.AppendLine($"Region/Culture:    {RegionName} ({RegionCode})");
            sb.AppendLine($"Local IP(s):       {GetLocalIpAddresses()}");

            // Fetch the async CPU calculation
            double cpuUsage = GetAppCpuUsagePercentage();
            sb.AppendLine($"App CPU Usage:     {cpuUsage}%");
            sb.AppendLine($"App RAM Usage:     {AppMemoryFormatted} (Managed GC Heap: {ManagedMemoryFormatted})");

            PrintDiskSpaceSummary(sb);
            sb.AppendLine("====================================");

            return sb.ToString();
        }
    }
}
