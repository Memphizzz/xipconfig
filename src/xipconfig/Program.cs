using System;
using System.Linq;
using X_ToolZ.Helpers;

namespace xipconfig
{
    class Program
    {
        private static void Main(string[] args)
        {
            Options options = new Options();

            X_ToolZ.SimpleParser.Parse(args, options);

            if (options.ShowHelp)
            {
                options.PrintHelp();
                return;
            }
            if (options.ShowVersion)
            {
                options.PrintVersion();
                return;
            }

            InterfaceManager manager = new InterfaceManager();

            if (args.Length == 0 || options.ListAll)
            {
                if (options.ListAll)
                    manager.Print(manager.OrderBy(x => x.InterfaceType));
                else
                    manager.Print(manager.Where(x => x.xMSFTNetAdapter != null).OrderBy(x => x.InterfaceType));
                return;
            }

            //IsInterfaceCommand
            XNetworkInterface nic = manager[options.InterfaceName];

            if (nic == null)
            {
                Console.WriteLine("xipconfig: interface not found");
                return;
            }

            if (args.Length == 1)
            {
                manager.Print(nic);
                return;
            }

            if (options.up)
            {
                Console.Write("Bringing UP " + nic.ShortName + "...");
                Console.WriteLine(nic.Enable() ? "OK!" : "failed (are you root?)");
                return;
            }

            if (options.down)
            {
                Console.Write("Bringing DOWN " + nic.ShortName + "...");
                Console.WriteLine(nic.Disable() ? "OK!" : "failed (are you root?)");
                return;
            }

            if (options.DHCP)
            {
                Console.Write("Setting " + nic.ShortName + " to use DHCP...");
                HandleWMIResult(nic.EnableDHCP());
            }
            
            if (options.IPAddress != null && options.SubnetMask != null)
            {
                Console.Write("Setting IP of " + nic.ShortName + "...");
                HandleWMIResult(nic.EnableStatic(options.IPAddress, options.SubnetMask));
            }

            if (options.Gateway != null)
            {
                Console.Write("Setting Gateway of " + nic.ShortName + "...");
                HandleWMIResult(nic.SetGateway(options.Gateway));
            }
            
            if (options.DNSServers != null && options.DNSServers.Count > 0)
            {
                Console.Write("Setting DNS Server(s) for " + nic.ShortName + "...");
                HandleWMIResult(nic.SetDNS(options.DNSServers.ToArray()));
            }
        }

        private static void HandleWMIResult(XNetworkInterface.WMIResult result)
        {
            if (result == XNetworkInterface.WMIResult.OK)
                Console.WriteLine("OK!");
            else
                Console.WriteLine("failed: " + result.GetDescription() + " (are you root?)");
        }
    }
}