#if ENABLE_NAT
using Open.Nat;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using OTA.Logging;
using System;
#endif

namespace OTA.Callbacks
{
    public static class NAT
    {
#if ENABLE_NAT
        //        static Mono.Nat.Mapping _map;
        //        static System.Collections.Generic.List<Mono.Nat.INatDevice> _devices = new System.Collections.Generic.List<Mono.Nat.INatDevice>();
        static NatDevice _device;
        const String NatMapName = "Terraria Server";

        private static CancellationTokenSource _cancel;
#endif

        /// <summary>
        /// Open the NAT port for the current Terraria ip:port
        /// </summary>
        public static void OpenPort()
        {
#if ENABLE_NAT && Full_API
            if (Terraria.Netplay.UseUPNP)
            {
                Terraria.Netplay.portForwardIP = Terraria.Netplay.GetLocalIPAddress();
                Terraria.Netplay.portForwardPort = Terraria.Netplay.ListenPort;

//                Mono.Nat.NatUtility.DeviceFound += NatUtility_DeviceFound;
//                Mono.Nat.NatUtility.StartDiscovery();

                var th = new System.Threading.Thread(async () =>
                    {
                        System.Threading.Thread.CurrentThread.Name = "NAT";
                        try
                        {
                            var discoverer = new NatDiscoverer();
                            _cancel = new CancellationTokenSource(10000);
                            _device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, _cancel);

                            if (_device != null)
                            {
                                var ip = await _device.GetExternalIPAsync();

                                var existing = await _device.GetSpecificMappingAsync(Protocol.Tcp, Terraria.Netplay.portForwardPort);
                                if (existing != null && existing.PublicPort == Terraria.Netplay.portForwardPort)
                                {
                                    ProgramLog.Admin.Log("Detected an existing NAT map record for {0} on IP {1}", NatMapName, ip);
                                }
                                else
                                {
                                    await _device.CreatePortMapAsync(new Mapping(Protocol.Tcp, Terraria.Netplay.portForwardPort, Terraria.Netplay.portForwardPort, NatMapName));
                                    ProgramLog.Admin.Log("Created a new NAT map record for {0} on IP {1}", NatMapName, ip);
                                }
                                Terraria.Netplay.portForwardOpen = true;
                            }
                            else ProgramLog.Admin.Log("Failed to find a NAT device");
                        }
                        catch (Exception e)
                        {
                            ProgramLog.Log(e, "Failed to create NAT device mapping");
                        }
                    });
                th.Start();
            }
#endif

            //if (Netplay.mappings != null)
            //{
            //    foreach (IStaticPortMapping staticPortMapping in Netplay.mappings)
            //    {
            //        if (staticPortMapping.InternalPort == Netplay.portForwardPort && staticPortMapping.InternalClient == Netplay.portForwardIP && staticPortMapping.Protocol == "TCP")
            //        {
            //            Netplay.portForwardOpen = true;
            //        }
            //    }
            //    if (!Netplay.portForwardOpen)
            //    {
            //        Netplay.mappings.Add(Netplay.portForwardPort, "TCP", Netplay.portForwardPort, Netplay.portForwardIP, true, "Terraria Server");
            //        Netplay.portForwardOpen = true;
            //    }
            //}
        }

        //        static void NatUtility_DeviceFound(object sender, Mono.Nat.DeviceEventArgs e)
        //        {
        //#if ENABLE_NAT && Full_API
        //            try
        //            {
        //                if (e.Device is Mono.Nat.Upnp.UpnpNatDevice) //TODO, see if Pmp should work as well
        //                {
        //                    try
        //                    {
        //                        var current = e.Device.GetAllMappings();
        //                        if (current != null)
        //                        {
        //                            foreach (var map in current)
        //                            {
        //                                if (map.Protocol == Mono.Nat.Protocol.Tcp && map.PrivatePort == Terraria.Netplay.portForwardPort && map.PublicPort == Terraria.Netplay.portForwardPort)
        //                                {
        //                                    Terraria.Netplay.portForwardOpen = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ProgramLog.Log(ex, "Failed to detect existing NAT device mapping(s)");
        //                    }
        //
        //                    if (!Terraria.Netplay.portForwardOpen)
        //                    {
        //                        _devices.Add(e.Device);
        //                        _map = new Mono.Nat.Mapping(Mono.Nat.Protocol.Tcp, Terraria.Netplay.portForwardPort, Terraria.Netplay.portForwardPort)
        //                        {
        //                            Description = "Terraria Server"
        //                        };
        //
        //                        e.Device.CreatePortMap(_map);
        //                        ProgramLog.Admin.Log("Created a new NAT map record for Terraria Server");
        //                        Terraria.Netplay.portForwardOpen = true;
        //                    }
        //                    else
        //                    {
        //                        ProgramLog.Admin.Log("Detected an existing NAT map record for Terraria Server");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ProgramLog.Log(ex, "Failed to create NAT device mapping");
        //            }
        //#endif
        //        }

#if ENABLE_NAT
        public static bool ShuttingDown { get; set; }
#endif

        /// <summary>
        /// Closes the open NAT port.
        /// </summary>
        public static void ClosePort()
        {
#if ENABLE_NAT && Full_API
            ShuttingDown = true;
            if (Terraria.Netplay.portForwardOpen)
            {
                if (_device != null)
                {
                    try
                    {
                        var task = Task.Run(async () =>
                        {
                            Mapping map = await _device.GetSpecificMappingAsync(Protocol.Tcp, Terraria.Netplay.portForwardPort);
                            while (map != null && map.PublicPort == Terraria.Netplay.portForwardPort)
                            {
                                System.Threading.Thread.Sleep(5);
                                await _device.DeletePortMapAsync(new Mapping(Protocol.Tcp, Terraria.Netplay.portForwardPort, Terraria.Netplay.portForwardPort, NatMapName));
                            }
                        });
                        task.Wait();

                        if (task.IsCompleted)
                        {
                            ProgramLog.Admin.Log("Successfully removed NAT device mapping");
                        }
                        else
                        {
                            ProgramLog.Admin.Log("Failed to remove NAT device mapping");
                        }
                    }
                    catch (Exception e)
                    {
                        ProgramLog.Log(e, "Failed to delete NAT device mapping");
                    }
                }
//                else ProgramLog.Admin.Log("Device is null");
            }
            else if (_cancel != null)
            {
                if (!_cancel.IsCancellationRequested) _cancel.Cancel();
            }

//            if (Terraria.Netplay.portForwardOpen && _map != null && _devices.Count > 0)
//            {
//                //Netplay.mappings.Remove(Netplay.portForwardPort, "TCP");
//                foreach (var device in _devices)
//                {
//                    try
//                    {
//                        device.DeletePortMap(_map);
//                    }
//                    catch (Exception e)
//                    {
//                        ProgramLog.Log(e);
//                    }
//                }
//                ProgramLog.Admin.Log("Removed NAT map record for Terraria Server");
//            }
//            Mono.Nat.NatUtility.StopDiscovery();
            ShuttingDown = false;
#endif
        }
        
        //        static class NatUtility
        //        {
        //            private static bool _discovering;
        //            private static Socket _discoverer;
        //
        //            public delegate void DiscoveryError(DiscoverError err);
        //            public static DiscoveryError OnDiscoveryError;
        //
        //            public enum DiscoverResult
        //            {
        //                Started,
        //                InProcess
        //            }
        //            public enum DiscoverError
        //            {
        //                NoLocalIP
        //            }
        //            public static DiscoverResult Discover(int broadcastPort)
        //            {
        //                if (null == _discoverer && !_discovering)
        //                {
        //                    _discovering = true;
        //                    System.Threading.ThreadPool.QueueUserWorkItem(Discovery, broadcastPort);
        //                    return DiscoverResult.Started;
        //                } else
        //                    return DiscoverResult.InProcess;
        //            }
        //
        //            private static string GetLocalIP()
        //            {
        //
        //                var itfs = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        //                foreach (var itf in itfs)
        //                {
        //                    if (itf.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
        //                    {
        //                        var props = itf.GetIPProperties();
        //                        //                        var old = props.GetIPv4Properties();
        //                        //                        var nw = props.GetIPv6Properties();
        //                        foreach (var add in props.UnicastAddresses)
        //                        {
        //                            if (add.DuplicateAddressDetectionState == System.Net.NetworkInformation.DuplicateAddressDetectionState.Preferred)
        //                            {
        //                                if (add.AddressPreferredLifetime != UInt32.MaxValue)
        //                                {
        //
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //
        //                var entry = Dns.GetHostEntry(Dns.GetHostName());
        //                foreach (var ip in entry.AddressList)
        //                {
        //                    if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
        //                    {
        //                        return ip.ToString();
        //                    }
        //                }
        //
        //                return null;
        //            }
        //
        //            private static void ClearSock()
        //            {
        //                if (_discoverer != null)
        //                {
        //                    if (_discoverer.Connected)
        //                        _discoverer.Close();
        //                    _discoverer.Dispose();
        //                    _discoverer = null;
        //                }
        //            }
        //
        //            private static void Discovery(object broadcastPort)
        //            {
        //                _discoverer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //                _discoverer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //
        //                var ip = GetLocalIP();
        //                if (String.IsNullOrEmpty(ip))
        //                {
        //                    if(OnDiscoveryError != null) OnDiscoveryError.Invoke(DiscoverError.NoLocalIP);
        //                    ClearSock();
        //                    return;
        //                }
        //
        //                var request = new StringBuilder();
        //                request.Append("M-SEARCH * HTTP/1.1\r\n");
        //                request.Append("HOST: ");
        //                request.Append(ip);
        //                request.Append(':');
        //                request.Append(broadcastPort.ToString());
        //                request.Append("\r\n");
        //                request.Append("ST:upnp:rootdevice\r\n");
        //                request.Append("MAN:\"ssdp:discover\"\r\n");
        //                request.Append("MX:3\r\n\r\n");
        //            }
        //        }
    }
}