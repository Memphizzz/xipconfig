using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using xipconfig;

namespace X_ToolZ.WMI_Classes
{
    internal class Win32_NetworkAdapter
    {
        public string AdapterType { get; set; }
        public UInt16? AdapterTypeID { get; set; }
        public bool? AutoSense { get; set; }
        public UInt16? Availability { get; set; }
        public string Caption { get; set; }
        public UInt32? ConfigManagerErrorCode { get; set; }
        public bool? ConfigManagerUserConfig { get; set; }
        public string CreationClassName { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }
        public bool? ErrorCleared { get; set; }
        public string ErrorDescription { get; set; }
        public string GUID { get; set; }
        public UInt32? Index { get; set; }
        public DateTime? InstallDate { get; set; }
        public bool? Installed { get; set; }
        public UInt32? InterfaceIndex { get; set; }
        public UInt32? LastErrorCode { get; set; }
        public string MACAddress { get; set; }
        public string Manufacturer { get; set; }
        public UInt32? MaxNumberControlled { get; set; }
        public UInt64? MaxSpeed { get; set; }
        public string Name { get; set; }
        public string NetConnectionID { get; set; }
        public UInt16? NetConnectionStatus { get; set; }
        public bool? NetEnabled { get; set; }
        public string[] NetworkAddresses { get; set; }
        public string PermanentAddress { get; set; }
        public bool? PhysicalAdapter { get; set; }
        public string PNPDeviceID { get; set; }
        public UInt16?[] PowerManagementCapabilities { get; set; }
        public bool? PowerManagementSupported { get; set; }
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
        public UInt64? Speed { get; set; }
        public string Status { get; set; }
        public UInt16? StatusInfo { get; set; }
        public string SystemCreationClassName { get; set; }
        public string SystemName { get; set; }
        public DateTime? TimeOfLastReset { get; set; }
        public Win32_NetworkAdapterConfiguration Win32_NetworkAdapterConfiguration { get; set; }
        public NetworkInterface NetworkInterface { get; set; }
        public MSFT_NetAdapter MSFT_NetAdapter { get; set; }

        public ManagementObject GetWMIObject(int? deviceID = null)
        {
            string query;
            if (deviceID == null)
                query = "SELECT * FROM Win32_NetworkAdapter WHERE DeviceID = " + this.DeviceID;
            else
                query = "SELECT * FROM Win32_NetworkAdapter WHERE DeviceID = " + deviceID;

            var oQuery = new ObjectQuery(query);
            using (var searcher = new ManagementObjectSearcher(oQuery))
            {
                return searcher.Get().Cast<ManagementObject>().FirstOrDefault();
            }
        }
    }
}
