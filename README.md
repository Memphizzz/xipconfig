
xipconfig
=========

xipconfig - ipconfig reloaded<br>
xipconfig is a windows equivalent to ifconfig. It's syntax is close but not limited to ifconfig's.<br>
The class ```InterfaceManager``` is the core of the tool.<br>

It combines the following 3 WMI classes:<br>

```Win32_NetworkAdapter ```
```Win32_NetworkAdapterConfiguration ```
```MSFT_NetAdapter ```

Using these 3 classes its basically possible to get (and set) almost any information that is available when it comes to adapters and networking in Windows.

xipconfig uses the linux like naming for interfaces.<br>
```
eth      Ethernet adapters
wlan     Wireless 802.11 adapters
virt     Virtual adapters like Hyper-V or VMWare
tun      Tunnel adapters like Microsoft ISATAP or Teredo
unkn     Unknown or not yet correctly detected adapters
```
All interfaces are numbered like they are on linux, for example ethernet adapters would be: eth0, eth1, eth2, and so on...

# Usage:
```
USAGE:
        xipconfig [interface] [all] |
        xipconfig interface [up/down/DHCP]  [-i][ipaddress] [-s][subnet] [-g][gateway] [-d][dns]
                         List active network interfaces
        interface        List this network interface only
        -all             List all network interfaces
        up/down          Enable or Disable interface
        DHCP             Set the interface to use DHCP
        -i               Set IP Address of interface
        -s               Set Subnet Mask of interface
        -g               Set Gateway of interface
        -d               Set DNS Server(s) of interface
        -v               Show Version
```

As xipconfig uses <a href="https://github.com/Memphizzz/simpleparser/">simpleparser</a> for command line arguments processing you can use these 3 types of syntaxes:

"Words" Syntax:<br>
```xipconfig eth0 ipaddress 192.168.0.5 subnet 255.255.255.0 gateway 192.168.0.1 dns 192.168.0.1,192.168.0.2```<br>
"Dash" Syntax:<br>
```xipconfig eth0 -i 192.168.0.5 -s 255.255.255.0 -g 192.168.0.1 -d 192.168.0.1,192.168.0.2```<br>
"Lazy" Syntax:<br>
```xipconfig eth0 i 192.168.0.5 s 255.255.255.0 g 192.168.0.1 d 192.168.0.1,192.168.0.2```<br>

If your Subnet Mask is /24 you can leave it out as thats the default anyway.

You can even mix these 3 syntaxes if you like to.

If you would like to set ```eth0``` to use DHCP you would do this: ```xipconfig eth0 DHCP```<br>
To enable or disable an adapter use the ```up``` and ```down``` keywords like in ifconfig: ```xipconfig eth0 down```<br>

# Planned Features:
---

* xipconfigd - xipconfig Daemon running in the background as an elevated process for UAC enabled systems.
* edit manifest to require elevated process and remove "are you root" messages.
* Fix initial release bugs #1 - #3 :)

# Contact me:
---
MemphiZ AT X-ToolZ DOT com<br>
Web: <a href=http://www.X-ToolZ.com>X-ToolZ.com</a> (under construction)<br>
Twitter: <a href=https://twitter.com/xtoolz>@XToolZ</a><br>
