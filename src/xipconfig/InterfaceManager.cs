using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using X_ToolZ.Helpers;
using X_ToolZ.WMI_Classes;

namespace xipconfig
{
    class InterfaceManager : IEnumerable<XNetworkInterface>
    {
        private readonly List<XNetworkInterface> interfaces = new List<XNetworkInterface>();
        private int ethID, wlanID, virtID, loopID, tunID, modID, unknID;

        public InterfaceManager()
        {
            Initialize();
        }

        #region Accessibility
        public XNetworkInterface this[string shortName]
        {
            get { return (from a in interfaces where a.ShortName == shortName select a).FirstOrDefault(); }
        }

        public IEnumerator<XNetworkInterface> GetEnumerator()
        {
            return interfaces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        
        public void Add(XNetworkInterface nic)
        {
            switch (nic.InterfaceType)
            {
                case XNetworkInterface.XInterfaceType.eth:
                    nic.ID = ethID++;
                    break;
                case XNetworkInterface.XInterfaceType.wlan:
                    nic.ID = wlanID++;
                    break;
                case XNetworkInterface.XInterfaceType.virt:
                    nic.ID = virtID++;
                    break;
                case XNetworkInterface.XInterfaceType.loop:
                    nic.ID = loopID++;
                    break;
                case XNetworkInterface.XInterfaceType.tun:
                    nic.ID = tunID++;
                    break;
                case XNetworkInterface.XInterfaceType.mod:
                    nic.ID = modID++;
                    break;
                case XNetworkInterface.XInterfaceType.unkn:
                    nic.ID = unknID++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            interfaces.Add(nic);
        }

        private void Initialize()
        {
            List<Win32_NetworkAdapter> networkAdapters = new List<Win32_NetworkAdapter>();

            CopyWMIData("SELECT * FROM Win32_NetworkAdapter", typeof(Win32_NetworkAdapter).FullName, networkAdapters, null);
            CopyWMIData("SELECT * FROM Win32_NetworkAdapterConfiguration", typeof(Win32_NetworkAdapterConfiguration).FullName, networkAdapters, null);
            CopyWMIData("SELECT * FROM MSFT_NetAdapter", typeof(MSFT_NetAdapter).FullName, networkAdapters, "root\\StandardCimv2");
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Win32_NetworkAdapter win32NetworkAdapter = (from a in networkAdapters where a.GUID == nic.Id select a).FirstOrDefault();
                if (win32NetworkAdapter != null)
                    win32NetworkAdapter.NetworkInterface = nic;
            }

            foreach (Win32_NetworkAdapter nic in networkAdapters.OrderByDescending(x => x.MSFT_NetAdapter != null))
                this.Add(new XNetworkInterface(nic));

            int len = Math.Max(this.OrderByDescending(x => x.Name.Length).First().Name.Length, this.OrderByDescending(x => x.DisplayName.Length).First().DisplayName.Length);
            
            try
            {
                if (Console.WindowWidth < (len + OutputHelper.LeftLen))
                    Console.WindowWidth = len + OutputHelper.LeftLen;
            }
            catch { }
        }

        private void CopyWMIData(string query, string typeName, List<Win32_NetworkAdapter> networkAdapters, string customScope)
        {
            ManagementObjectSearcher searcher;
            if (!string.IsNullOrWhiteSpace(customScope))
            {
                ManagementScope scope = new ManagementScope(customScope);
                scope.Options.Impersonation = ImpersonationLevel.Impersonate;
                searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query));
            }
            else
                searcher = new ManagementObjectSearcher(query);

            using (ManagementObjectCollection queryCollection = searcher.Get())
            {
                foreach (var o in queryCollection)
                {
                    var mo = (ManagementObject) o;
                    Type t = Type.GetType(typeName);
                    dynamic instance = Activator.CreateInstance(t);

                    foreach (PropertyData pd in mo.Properties)
                    {
                        try
                        {
                            PropertyInfo pInfo = t.GetProperty(pd.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            pInfo.SetValue(instance, pd.Value);
                        }
                        catch (Exception) { }
                    }


                    if (instance is Win32_NetworkAdapter)
                        networkAdapters.Add(instance);
                    else
                    {
                        Win32_NetworkAdapter win32NetworkAdapter = (from a in networkAdapters where a.InterfaceIndex == instance.InterfaceIndex select a).FirstOrDefault();
                        if (win32NetworkAdapter != null)
                        {
                            if (instance is Win32_NetworkAdapterConfiguration)
                                win32NetworkAdapter.Win32_NetworkAdapterConfiguration = instance;
                            else if (instance is MSFT_NetAdapter)
                                win32NetworkAdapter.MSFT_NetAdapter = instance;
                        }
                    }
                }
            }
            searcher.Dispose();
        }

        public void Print(XNetworkInterface nic)
        {
            Print(new[] {nic});
        }

        public void Print(IEnumerable<XNetworkInterface> interfaces)
        {
            foreach (XNetworkInterface nic in interfaces)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(nic.InterfaceType.ToString() + nic.ID + "\t" + "(" + nic.InterfaceType.GetDescription() + ")");
                sb.AppendLine();
                sb.AppendLine(OutputHelper.GetString("Adapter", nic.Name));
                sb.AppendLine(OutputHelper.GetString("Name", nic.DisplayName));
                sb.AppendLine(OutputHelper.GetString("Connection Status", nic.ConnectionStatus.GetDescription()));
                sb.AppendLine(OutputHelper.GetString("MAC Address", nic.MACAddress));
                sb.AppendLine(OutputHelper.GetString("IP Address(es)", nic.IPAddresses.Aggregate(string.Empty, (current, ip) => current + (ip.ToString() + Environment.NewLine))));
                sb.AppendLine(OutputHelper.GetString("Subnet Mask", nic.SubnetMask.GetIPOrEmpty()));
                sb.AppendLine(OutputHelper.GetString("Default Gateway", nic.Gateway.GetIPOrEmpty()));
                sb.AppendLine(OutputHelper.GetString("DHCP Enabled", nic.DHCPEnabled.GetText()));
                if (nic.DHCPEnabled.HasValue && nic.DHCPEnabled.Value)
                {
                    sb.AppendLine(OutputHelper.GetString("DHCP Server", nic.DHCPServer.GetIPOrEmpty()));
                    //sb.AppendLine(OutputHelper.GetString("DHCP Lease Obtained", nic.DHCPLeaseObtained.GetDateTimeOrEmpty()));
                    //sb.AppendLine(OutputHelper.GetString("DHCP Lease Expires", nic.DHCPLeaseExpires.GetDateTimeOrEmpty()));
                }
                sb.AppendLine(OutputHelper.GetString("DNS Server(s)", nic.DNSServers.Aggregate(string.Empty, (current, ip) => current + (ip.ToString() + Environment.NewLine))));
                sb.AppendLine(OutputHelper.GetString("DNS Suffix", nic.DNSSuffix));
                sb.AppendLine(OutputHelper.GetString("NetBIOS Enabled", nic.NetBIOSEnabled.GetText()));

                sb.AppendLine(OutputHelper.GetString("ServiceName", nic.xWin32NetworkAdapterConfiguration.ServiceName));

                Console.WriteLine(sb);
            }
        }
    }
}