using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMOGS.Performance
{
    public class PerformanceAnalyzer
    {
        public delegate void HandleMemoryUsageInfo(string cpuInfo, string ramInfo);
        private static PerformanceAnalyzer _instance = null;
        private static readonly object _infoLock = new object();
        
        private HandleMemoryUsageInfo _getMemInfoDelegate = null;
        private bool _enabled = false;
        private int _infoIntervalMs = 1000;

        public static void BeginAnalyzing(HandleMemoryUsageInfo method = null)
        {
            if (PerformanceAnalyzer._instance == null)
                PerformanceAnalyzer._instance = new PerformanceAnalyzer();

            if (method != null)
                RegisterListener(method);

            PerformanceAnalyzer._instance._enabled = true;
            PerformanceAnalyzer._instance.ListenAsync();
        }
        
        public static void StopAnalyzing()
        {
            if (PerformanceAnalyzer._instance != null)
                PerformanceAnalyzer._instance._enabled = false;
        }

        public static void RegisterListener(HandleMemoryUsageInfo method)
        {
            lock (_infoLock)
            {
                if (PerformanceAnalyzer._instance != null)
                    PerformanceAnalyzer._instance._getMemInfoDelegate += method;
            }
        }

        private PerformanceAnalyzer()
        {
        }
        
        private async void ListenAsync()
        {
            try
            {
                if (_getMemInfoDelegate == null)
                    throw new Exception("listener delegate not registered!");

                PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                string cpuMsg = "";
                string ramMsg = "";

                while (_enabled)
                {
                    await Task.Factory.StartNew(() => Thread.Sleep(_infoIntervalMs));
                    await Task.Factory.StartNew
                    (
                        () =>
                        {
                            cpuMsg = $"{cpuCounter.NextValue()} %";
                            ramMsg = $"{ramCounter.NextValue()} MB";
                        }
                    );

                    lock (_infoLock)
                    {
                        _getMemInfoDelegate(cpuMsg, ramMsg);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Performance analyzer - error occured: {exception.Message}");
            }
            finally
            {
                _enabled = false;
            }
        }
    }
}
