using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQATEC.Analytics.Monitor;

namespace RT.Util
{
    /// <summary>Wraps AnalyticsMonitorSettings so that applications don't need to add a "using".</summary>
    public sealed class EqatecAnalyticsSettings
    {
        public AnalyticsMonitorSettings Settings { get; private set; }

        public EqatecAnalyticsSettings(string productId)
        {
            Settings = new AnalyticsMonitorSettings(productId);
        }

        public int DailyNetworkUtilizationInKB { get { return Settings.DailyNetworkUtilizationInKB; } set { Settings.DailyNetworkUtilizationInKB = value; } }
        public int MaxStorageSizeInKB { get { return Settings.MaxStorageSizeInKB; } set { Settings.MaxStorageSizeInKB = value; } }
        public TimeSpan StorageSaveInterval { get { return Settings.StorageSaveInterval; } set { Settings.StorageSaveInterval = value; } }
        public bool SynchronizeAutomatically { get { return Settings.SynchronizeAutomatically; } set { Settings.SynchronizeAutomatically = value; } }
        public bool TestMode { get { return Settings.TestMode; } set { Settings.TestMode = value; } }
        public Version Version { get { return Settings.Version; } set { Settings.Version = value; } }
        public ILogAnalyticsMonitor LoggingInterface { get { return Settings.LoggingInterface; } set { Settings.LoggingInterface = value; } }
        public ProxyConfiguration ProxyConfig { get { return Settings.ProxyConfig; } set { Settings.ProxyConfig = value; } }
        public Uri ServerUri { get { return Settings.ServerUri; } set { Settings.ServerUri = value; } }
        public IStorage Storage { get { return Settings.Storage; } set { Settings.Storage = value; } }
    }
}
