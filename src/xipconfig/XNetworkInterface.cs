using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using X_ToolZ.WMI_Classes;

namespace xipconfig
{
    class XNetworkInterface
    {
        public readonly Win32_NetworkAdapter xWin32NetworkAdapter;

        public Win32_NetworkAdapterConfiguration xWin32NetworkAdapterConfiguration
        {
            get { return xWin32NetworkAdapter.Win32_NetworkAdapterConfiguration; }
        }

        public NetworkInterface xNetworkInterface
        {
            get { return xWin32NetworkAdapter.NetworkInterface; }
        }

        public MSFT_NetAdapter xMSFTNetAdapter
        {
            get { return xWin32NetworkAdapter.MSFT_NetAdapter; }
        }

        public XNetworkInterface(Win32_NetworkAdapter nic)
        {
            xWin32NetworkAdapter = nic;
        }

        public int ID { get; set; }

        public string ShortName
        {
            get { return InterfaceType.ToString() + ID; }
        }

        public string DeviceID
        {
            get { return xWin32NetworkAdapter.DeviceID; }
        }


        public string Name
        {
            get
            {
                if(xWin32NetworkAdapter != null && xWin32NetworkAdapter.Name !=null)
                return xWin32NetworkAdapter.Description;
                return string.Empty;
            }
        }

        public string DisplayName
        {
            get
            {
                if (xWin32NetworkAdapter != null && xWin32NetworkAdapter.NetConnectionID != null)
                    return xWin32NetworkAdapter.NetConnectionID;
                return string.Empty;
            }            
        }

        public bool IsEnabled
        {
            get
            {
                if (xWin32NetworkAdapter.NetEnabled.HasValue && xWin32NetworkAdapter.NetEnabled.Value)
                    return true;
                return false;
            }
        }



        public List<IPAddress> IPAddresses
        {
            get
            {
                var retval = new List<IPAddress>();
                if (xWin32NetworkAdapterConfiguration.IPAddress != null)
                    retval.AddRange(xWin32NetworkAdapterConfiguration.IPAddress.Select(IPAddress.Parse));
                return retval;
            }
        }

        public IPAddress SubnetMask
        {
            get
            {
                if (xWin32NetworkAdapterConfiguration.IPSubnet != null)
                    return IPAddress.Parse(xWin32NetworkAdapterConfiguration.IPSubnet[0]);
                return null;
            }
        }

        public IPAddress Gateway
        {
            get
            {
                if (xWin32NetworkAdapterConfiguration.DefaultIPGateway != null)
                    return IPAddress.Parse(xWin32NetworkAdapterConfiguration.DefaultIPGateway[0]);
                return null;
            }
        }

        public string MACAddress
        {
            get
            {
                return xWin32NetworkAdapter.MACAddress;
            }
        }

        public bool? DHCPEnabled
        {
            get { return xWin32NetworkAdapterConfiguration.DHCPEnabled; }
        }

        public IPAddress DHCPServer
        {
            get
            {
                if (xWin32NetworkAdapterConfiguration.DHCPServer != null)
                    return IPAddress.Parse(xWin32NetworkAdapterConfiguration.DHCPServer);
                return null;
            }
        }

        //public DateTime? DHCPLeaseObtained
        //{
        //    get { return xWin32NetworkAdapterConfiguration.DHCPLeaseObtained; }
        //}

        //public DateTime? DHCPLeaseExpires
        //{
        //    get { return xWin32NetworkAdapterConfiguration.DHCPLeaseExpires; }
        //}

        public List<IPAddress> DNSServers
        {
            get
            {
                var retval = new List<IPAddress>();
                if (xWin32NetworkAdapterConfiguration.DNSServerSearchOrder != null && xWin32NetworkAdapterConfiguration.DNSServerSearchOrder.Length > 0)
                    retval.AddRange(xWin32NetworkAdapterConfiguration.DNSServerSearchOrder.Select(IPAddress.Parse));
                return retval;
            }
        }

        public string DNSSuffix
        {
            get
            {
                if (xWin32NetworkAdapterConfiguration.DNSDomainSuffixSearchOrder != null && xWin32NetworkAdapterConfiguration.DNSDomainSuffixSearchOrder.Length > 0)
                    return xWin32NetworkAdapterConfiguration.DNSDomainSuffixSearchOrder.Aggregate((current, next) => current + ", " + next);
                return string.Empty;
            }
        }

        public bool? NetBIOSEnabled
        {
            get
            {
                if (xWin32NetworkAdapterConfiguration.TcpipNetbiosOptions.HasValue)
                {
                    if (xWin32NetworkAdapterConfiguration.TcpipNetbiosOptions == 0 || xWin32NetworkAdapterConfiguration.TcpipNetbiosOptions == 1)
                        return true;
                    if (xWin32NetworkAdapterConfiguration.TcpipNetbiosOptions == 2)
                        return false;
                }
                return null;
            }
        }

        public enum XInterfaceType
        {
            [Description("Ethernet")] eth = 1,
            [Description("Wireless80211")] wlan = 2,
            [Description("Virtual")] virt = 3,
            [Description("Loopback")] loop = 4,
            [Description("Tunnel")] tun = 5,
            [Description("Modem")] mod = 6,
            [Description("unknown")] unkn = 7
        }

        public XInterfaceType InterfaceType
        {
            get
            {
                XInterfaceType retval = XInterfaceType.unkn;

                if (xMSFTNetAdapter != null)
                {
                    if(!xMSFTNetAdapter.HardwareInterface)
                        return XInterfaceType.virt;

                    switch (xMSFTNetAdapter.NdisPhysicalMedium)
                    {
                        case 1:
                        case 9:
                        case 12:
                            return XInterfaceType.wlan;
                        case 14:
                        case 15:
                            return XInterfaceType.eth;
                    }   

                }

                if (xNetworkInterface != null)
                {
                    switch (xNetworkInterface.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.FastEthernetT:
                        case NetworkInterfaceType.FastEthernetFx:
                        case NetworkInterfaceType.Ethernet3Megabit:
                        case NetworkInterfaceType.GigabitEthernet:
                        case NetworkInterfaceType.Ethernet:
                            return XInterfaceType.eth;
                        case NetworkInterfaceType.Wireless80211:
                            return XInterfaceType.wlan;
                        case NetworkInterfaceType.Loopback:
                            return XInterfaceType.loop;
                        case NetworkInterfaceType.Tunnel:
                            return XInterfaceType.tun;
                        case NetworkInterfaceType.GenericModem:
                            return XInterfaceType.mod;
                    }
                }

                if (xWin32NetworkAdapter.AdapterTypeID != null)
                {
                    switch (xWin32NetworkAdapter.AdapterTypeID)
                    {
                        case 0:
                            return XInterfaceType.eth;
                        case 9:
                            return XInterfaceType.wlan;
                    }
                }

                if (!string.IsNullOrWhiteSpace(xWin32NetworkAdapterConfiguration.ServiceName))
                {
                    string serviceName = xWin32NetworkAdapterConfiguration.ServiceName.ToLower();
                    if (serviceName.Contains("tunnel"))
                        return XInterfaceType.tun;
                }
                
                return retval;
            }
        }

        public enum XConnectionStatus
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,
            Disconnecting = 3,
            //[Description("Hardware not present")] HardwareNotPresent = 4,
            [Description("Disabled")] HardwareNotPresent = 4,
            [Description("Hardware disabled")] HardwareDisabled = 5,
            [Description("Hardware malfunction")] HardwareMalfunction = 6,
            [Description("Media disconnected")] MediaDisconnected = 7,
            Authenticating = 8,
            [Description("Authentication succeeded")] AuthenticationSucceeded = 9,
            [Description("Authentication failed")] AuthenticationFailed = 10,
            [Description("Invalid address")] InvalidAddress = 11,
            [Description("Credentials required")] CredentialsRequired = 12,
            unknown = 13
        }

        public XConnectionStatus ConnectionStatus
        {
            get
            {
                //ToDo: look for alternatives in MSFTNetAdapter
                if (xWin32NetworkAdapter.NetConnectionStatus != null)
                    return (XConnectionStatus) xWin32NetworkAdapter.NetConnectionStatus;
                return  XConnectionStatus.unknown;
            }
        }

        public bool Enable()
        {
            return PowerDevice(true);
        }

        public bool Disable()
        {
            return PowerDevice(false);
        }

        private bool PowerDevice(bool up)
        {
            ManagementObject mo = this.xWin32NetworkAdapter.GetWMIObject();

            string todo;
            if (up)
                todo = "Enable";
            else
                todo = "Disable";
            uint exitCode = (uint)mo.InvokeMethod(todo, null);
            return exitCode == 0;
        }

        public WMIResult EnableDHCP()
        {
            ManagementBaseObject retval;
            using (ManagementObject mo = this.xWin32NetworkAdapterConfiguration.GetWMIObject())
            {
                retval = mo.InvokeMethod("EnableDHCP", null, null);
            }
            SetDNS(null);

            return HandleExitCode((uint) retval["returnValue"]);
        }

        public WMIResult EnableStatic(string IPAddress, string SubnetMask)
        {
            using (ManagementObject mo = xWin32NetworkAdapterConfiguration.GetWMIObject())
            {
                using (var parameters = mo.GetMethodParameters("EnableStatic"))
                {
                    parameters["IPAddress"] = new[] {IPAddress};
                    parameters["SubnetMask"] = new[] {SubnetMask};

                    ManagementBaseObject retval = mo.InvokeMethod("EnableStatic", parameters, null);
                    return HandleExitCode((uint) retval["returnValue"]);
                }
            }
        }

        public WMIResult SetGateway(string gateway)
        {
            using (ManagementObject mo = xWin32NetworkAdapterConfiguration.GetWMIObject())
            {
                using (var parameters = mo.GetMethodParameters("SetGateways"))
                {
                    parameters["DefaultIPGateway"] = new[] {gateway};
                    parameters["GatewayCostMetric"] = new[] {1};

                    ManagementBaseObject retval = mo.InvokeMethod("SetGateways", parameters, null);

                    return HandleExitCode((uint)retval["returnValue"]);
                }
            }
        }

        public WMIResult SetDNS(string[] servers)
        {
            using (ManagementObject mo = xWin32NetworkAdapterConfiguration.GetWMIObject())
            {
                using (var parameters = mo.GetMethodParameters("SetDNSServerSearchOrder"))
                {
                    parameters["DNSServerSearchOrder"] = servers;

                    ManagementBaseObject retval = mo.InvokeMethod("SetDNSServerSearchOrder", parameters, null);
                    return HandleExitCode((uint)retval["returnValue"]);
                }
            }
        }

        public WMIResult HandleExitCode(uint code)
        {
            WMIResult retval;
            if (code == 2147749891)
                return WMIResult.Access_denied;
            if (Enum.TryParse(code.ToString(), out retval))
                return retval;
            return WMIResult.Error;
        }

        public enum WMIResult
        {
            //Successful_completion_no_reboot_required = 0,
            OK = 0,
            [Description("Successful but reboot required")] Successful_completion_reboot_required = 1,
            Method_not_supported_on_this_platform = 64,
            Unknown_failure = 65,
            [Description("Invalid SubnetMask")] Invalid_subnet_mask = 66,
            An_error_occurred_while_processing_an_instance_that_was_returned = 67,
            [Description("Invalid Input Parameter")] Invalid_input_parameter = 68,
            More_than_five_gateways_specified = 69,
            Invalid_IP_address = 70,
            Invalid_gateway_IP_address = 71,
            An_error_occurred_while_accessing_the_registry_for_the_requested_information = 72,
            Invalid_domain_name = 73,
            Invalid_host_name = 74,
            No_primary_or_secondary_WINS_server_defined = 75,
            Invalid_file = 76,
            Invalid_system_path = 77,
            File_copy_failed = 78,
            Invalid_security_parameter = 79,
            Unable_to_configure_TCPIP_service = 80,
            Unable_to_configure_DHCP_service = 81,
            Unable_to_renew_DHCP_lease = 82,
            Unable_to_release_DHCP_lease = 83,
            IP_not_enabled_on_adapter = 84,
            IPX_not_enabled_on_adapter = 85,
            Frame_or_network_number_bounds_error = 86,
            Invalid_frame_type = 87,
            Invalid_network_number = 88,
            Duplicate_network_number = 89,
            Parameter_out_of_bounds = 90,
            [Description("Access Denied")]
            Access_denied = 91,
            Out_of_memory = 92,
            Already_exists = 93,
            Path_file_or_object_not_found = 94,
            Unable_to_notify_service = 95,
            Unable_to_notify_DNS_service = 96,
            Interface_not_configurable = 97,
            Not_all_DHCP_leases_could_be_released_or_renewed = 98,
            DHCP_not_enabled_on_the_adapter = 100,
            Error
        }
    }
}