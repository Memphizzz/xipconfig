using System;
using System.Linq;
using System.Management;

namespace X_ToolZ.WMI_Classes
{
    public class Win32_NetworkAdapterConfiguration
    {
        public bool? ArpAlwaysSourceRoute { get; set; }
        public bool? ArpUseEtherSNAP { get; set; }
        public string Caption { get; set; }
        public string DatabasePath { get; set; }
        public bool? DeadGWDetectEnabled { get; set; }
        public string[] DefaultIPGateway { get; set; }
        public byte? DefaultTOS { get; set; }
        public byte? DefaultTTL { get; set; }
        public string Description { get; set; }
        public bool? DHCPEnabled { get; set; }
        public DateTime? DHCPLeaseExpires { get; set; }
        public DateTime? DHCPLeaseObtained { get; set; }
        public string DHCPServer { get; set; }
        public string DNSDomain { get; set; }
        public string[] DNSDomainSuffixSearchOrder { get; set; }
        public bool? DNSEnabledForWINSResolution { get; set; }
        public string DNSHostName { get; set; }
        public string[] DNSServerSearchOrder { get; set; }
        public bool? DomainDNSRegistrationEnabled { get; set; }
        public UInt32? ForwardBufferMemory { get; set; }
        public bool? FullDNSRegistrationEnabled { get; set; }
        public UInt16?[] GatewayCostMetric { get; set; }
        public byte? IGMPLevel { get; set; }
        public UInt32? Index { get; set; }
        public UInt32? InterfaceIndex { get; set; }
        public string[] IPAddress { get; set; }
        public UInt32? IPConnectionMetric { get; set; }
        public bool? IPEnabled { get; set; }
        public bool? IPFilterSecurityEnabled { get; set; }
        public bool? IPPortSecurityEnabled { get; set; }
        public string[] IPSecPermitIPProtocols { get; set; }
        public string[] IPSecPermitTCPPorts { get; set; }
        public string[] IPSecPermitUDPPorts { get; set; }
        public string[] IPSubnet { get; set; }
        public bool? IPUseZeroBroadcast { get; set; }
        public string IPXAddress { get; set; }
        public bool? IPXEnabled { get; set; }
        public UInt32?[] IPXFrameType { get; set; }
        public UInt32? IPXMediaType { get; set; }
        public string[] IPXNetworkNumber { get; set; }
        public string IPXVirtualNetNumber { get; set; }
        public UInt32? KeepAliveInterval { get; set; }
        public UInt32? KeepAliveTime { get; set; }
        public string MACAddress { get; set; }
        public UInt32? MTU { get; set; }
        public UInt32? NumForwardPackets { get; set; }
        public bool? PMTUBHDetectEnabled { get; set; }
        public bool? PMTUDiscoveryEnabled { get; set; }
        public string ServiceName { get; set; }
        public string SettingID { get; set; }
        public UInt32? TcpipNetbiosOptions { get; set; }
        public UInt32? TcpMaxConnectRetransmissions { get; set; }
        public UInt32? TcpMaxDataRetransmissions { get; set; }
        public UInt32? TcpNumConnections { get; set; }
        public bool? TcpUseRFC1122UrgentPointer { get; set; }
        public UInt16? TcpWindowSize { get; set; }
        public bool? WINSEnableLMHostsLookup { get; set; }
        public string WINSHostLookupFile { get; set; }
        public string WINSPrimaryServer { get; set; }
        public string WINSScopeID { get; set; }
        public string WINSSecondaryServer { get; set; }

        public ManagementObject GetWMIObject(int? interfaceIndex = null)
        {
            string query;
            if (interfaceIndex == null)
                query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE InterfaceIndex = " + this.InterfaceIndex;
            else
                query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE InterfaceIndex = " + interfaceIndex;

            var oQuery = new ObjectQuery(query);
            using (var searcher = new ManagementObjectSearcher(oQuery))
            {
                return searcher.Get().Cast<ManagementObject>().FirstOrDefault();
            }
        }
    }
}
