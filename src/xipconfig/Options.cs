using System;
using System.Collections.Generic;
using X_ToolZ;

namespace xipconfig
{
    internal class Options
    {
        public string Version { get { return "1.0 (https://github.com/Memphizzz/xipconfig)" + Environment.NewLine + "(c) by MemphiZ (@X-ToolZ.com)"; } }

        [BoolOption("h", "help")]
        public bool ShowHelp { get; set; }

        [BoolOption("v", "version")]
        public bool ShowVersion { get; set; }

        [BoolOption("verbose")]
        public bool Verbose { get; set; }

        [BoolOption("all", 0)]
        public bool ListAll { get; set; }

        [Option("i", "ipaddress")]
        public string IPAddress { get; set; }

        [Option("s", "subnet", DefaultValue = "255.255.255.0")]
        public string SubnetMask { get; set; }

        [Option("g", "gateway")]
        public string Gateway { get; set; }

        [ListOption("d", "dns")]
        public List<string> DNSServers { get; set; }

        [BoolOption("DHCP", 1)]
        public bool DHCP { get; set; }

        [IndexOption(0)]
        public string InterfaceName { get; set; }

        [BoolOption("down", 1)]
        public bool down { get; set; }

        [BoolOption("up", 1)]
        public bool up { get; set; }

        public void PrintHelp()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("\txipconfig [interface] [all] | " + Environment.NewLine + "\txipconfig interface [up/down/DHCP]  [-i][ipaddress] [-s][subnet] [-g][gateway] [-d][dns]");
            Console.WriteLine("\t\t\t List active network interfaces");
            Console.WriteLine("\tinterface\t List this network interface only");
            Console.WriteLine("\t-all\t\t List all network interfaces");
            Console.WriteLine("\tup/down\t\t Enable or Disable interface");
            Console.WriteLine("\tDHCP\t\t Set the interface to use DHCP");
            Console.WriteLine("\t-i\t\t Set IP Address of interface");
            Console.WriteLine("\t-s\t\t Set Subnet Mask of interface");
            Console.WriteLine("\t-g\t\t Set Gateway of interface");
            Console.WriteLine("\t-d\t\t Set DNS Server(s) of interface");
            Console.WriteLine("\t-v\t\t Show Version");
        }

        public void PrintVersion()
        {
            Console.WriteLine("xipconfig version " + Version);
        }
    }
}