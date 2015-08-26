//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using Terraria;
//
//namespace tdsm.api.Callbacks
//{
//    class Tests
//    {
//        public static IAPISocket[] serverSock = new Terraria.ServerSock[255];
//
//        //        An unhandled exception of type 'System.ArrayTypeMismatchException' occurred in tdsm.exe
//
//        //Additional information: Attempted to access an element as a type incompatible with the array.
//
//        // Terraria.Netplay
//        public static void ServerLoop(object threadContext)
//        {
//            Netplay.ResetNetDiag();
//            if (Main.rand == null)
//            {
//                Main.rand = new Random((int)DateTime.Now.Ticks);
//            }
//            if (WorldGen.genRand == null)
//            {
//                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
//            }
//            Main.myPlayer = 255;
//            Netplay.serverIP = IPAddress.Any;
//            Netplay.serverListenIP = Netplay.serverIP;
//            Main.menuMode = 14;
//            Main.statusText = "Starting server...";
//            Main.netMode = 2;
//            Netplay.disconnect = false;
//            for (int i = 0; i < 256; i++)
//            {
//                Netplay.serverSock[i] = new ServerSock();
//                Tools.WriteLine("TEST");
//                Netplay.serverSock[i].Reset();
//                Netplay.serverSock[i].whoAmI = i;
//                Netplay.serverSock[i].tcpClient = new TcpClient();
//                Netplay.serverSock[i].tcpClient.NoDelay = true;
//                Netplay.serverSock[i].readBuffer = new byte[1024];
//                Netplay.serverSock[i].writeBuffer = new byte[1024];
//            }
//            Netplay.tcpListener = new TcpListener(Netplay.serverListenIP, Netplay.serverPort);
//            try
//            {
//                Netplay.tcpListener.Start();
//            }
//            catch (Exception ex)
//            {
//                Main.menuMode = 15;
//                Main.statusText = ex.ToString();
//                Netplay.disconnect = true;
//            }
//            if (!Netplay.disconnect)
//            {
//                ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
//                Main.statusText = "Server started";
//            }
//            if (Netplay.uPNP)
//            {
//                try
//                {
//                    tdsm.api.Callbacks.NAT.OpenPort();
//                }
//                catch
//                {
//                }
//            }
//            int num = 0;
//            while (!Netplay.disconnect)
//            {
//                if (Netplay.stopListen)
//                {
//                    int num2 = -1;
//                    for (int j = 0; j < Main.maxNetPlayers; j++)
//                    {
//                        if (!Netplay.serverSock[j].tcpClient.Connected)
//                        {
//                            num2 = j;
//                            break;
//                        }
//                    }
//                    if (num2 >= 0)
//                    {
//                        if (Main.ignoreErrors)
//                        {
//                            try
//                            {
//                                Netplay.tcpListener.Start();
//                                Netplay.stopListen = false;
//                                ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
//                                goto IL_219;
//                            }
//                            catch
//                            {
//                                goto IL_219;
//                            }
//                        }
//                        Netplay.tcpListener.Start();
//                        Netplay.stopListen = false;
//                        ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
//                    }
//                }
//            IL_219:
//                int num3 = 0;
//                for (int k = 0; k < 256; k++)
//                {
//                    if (NetMessage.buffer[k].checkBytes)
//                    {
//                        NetMessage.CheckBytes(k);
//                    }
//                    if (Netplay.serverSock[k].kill)
//                    {
//                        Netplay.serverSock[k].Reset();
//                        NetMessage.syncPlayers();
//                    }
//                    else if (Netplay.serverSock[k].tcpClient.Connected)
//                    {
//                        if (!Netplay.serverSock[k].active)
//                        {
//                            Netplay.serverSock[k].state = 0;
//                        }
//                        Netplay.serverSock[k].active = true;
//                        num3++;
//                        if (!Netplay.serverSock[k].locked)
//                        {
//                            try
//                            {
//                                Netplay.serverSock[k].networkStream = Netplay.serverSock[k].tcpClient.GetStream();
//                                if (Netplay.serverSock[k].networkStream.DataAvailable)
//                                {
//                                    Netplay.serverSock[k].locked = true;
//                                    Netplay.serverSock[k].networkStream.BeginRead(Netplay.serverSock[k].readBuffer, 0, Netplay.serverSock[k].readBuffer.Length, new AsyncCallback(Netplay.serverSock[k].ServerReadCallBack), Netplay.serverSock[k].networkStream);
//                                }
//                            }
//                            catch
//                            {
//                                Netplay.serverSock[k].kill = true;
//                            }
//                        }
//                        if (Netplay.serverSock[k].statusMax > 0 && Netplay.serverSock[k].statusText2 != "")
//                        {
//                            if (Netplay.serverSock[k].statusCount >= Netplay.serverSock[k].statusMax)
//                            {
//                                Netplay.serverSock[k].statusText = string.Concat(new object[]
//						{
//							"(",
//							Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//							") ",
//							Netplay.serverSock[k].name,
//							" ",
//							Netplay.serverSock[k].statusText2,
//							": Complete!"
//						});
//                                Netplay.serverSock[k].statusText2 = "";
//                                Netplay.serverSock[k].statusMax = 0;
//                                Netplay.serverSock[k].statusCount = 0;
//                            }
//                            else
//                            {
//                                Netplay.serverSock[k].statusText = string.Concat(new object[]
//						{
//							"(",
//							Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//							") ",
//							Netplay.serverSock[k].name,
//							" ",
//							Netplay.serverSock[k].statusText2,
//							": ",
//							(int)((float)Netplay.serverSock[k].statusCount / (float)Netplay.serverSock[k].statusMax * 100f),
//							"%"
//						});
//                            }
//                        }
//                        else if (Netplay.serverSock[k].state == 0)
//                        {
//                            Netplay.serverSock[k].statusText = string.Concat(new object[]
//					{
//						"(",
//						Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//						") ",
//						Netplay.serverSock[k].name,
//						" is connecting..."
//					});
//                        }
//                        else if (Netplay.serverSock[k].state == 1)
//                        {
//                            Netplay.serverSock[k].statusText = string.Concat(new object[]
//					{
//						"(",
//						Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//						") ",
//						Netplay.serverSock[k].name,
//						" is sending player data..."
//					});
//                        }
//                        else if (Netplay.serverSock[k].state == 2)
//                        {
//                            Netplay.serverSock[k].statusText = string.Concat(new object[]
//					{
//						"(",
//						Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//						") ",
//						Netplay.serverSock[k].name,
//						" requested world information"
//					});
//                        }
//                        else if (Netplay.serverSock[k].state != 3 && Netplay.serverSock[k].state == 10)
//                        {
//                            Netplay.serverSock[k].statusText = string.Concat(new object[]
//					{
//						"(",
//						Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint,
//						") ",
//						Netplay.serverSock[k].name,
//						" is playing"
//					});
//                        }
//                    }
//                    else if (Netplay.serverSock[k].active)
//                    {
//                        Netplay.serverSock[k].kill = true;
//                    }
//                    else
//                    {
//                        Netplay.serverSock[k].statusText2 = "";
//                        if (k < 255)
//                        {
//                            Main.player[k].active = false;
//                        }
//                    }
//                }
//                num++;
//                if (num > 10)
//                {
//                    Thread.Sleep(1);
//                    num = 0;
//                }
//                else
//                {
//                    Thread.Sleep(0);
//                }
//                if (!WorldGen.saveLock && !Main.dedServ)
//                {
//                    if (num3 == 0)
//                    {
//                        Main.statusText = "Waiting for clients...";
//                    }
//                    else
//                    {
//                        Main.statusText = num3 + " clients connected";
//                    }
//                }
//                if (num3 == 0)
//                {
//                    Netplay.anyClients = false;
//                }
//                else
//                {
//                    Netplay.anyClients = true;
//                }
//                Netplay.ServerUp = true;
//            }
//            Netplay.tcpListener.Stop();
//            try
//            {
//                tdsm.api.Callbacks.NAT.ClosePort();
//            }
//            catch
//            {
//            }
//            for (int l = 0; l < 256; l++)
//            {
//                Netplay.serverSock[l].Reset();
//            }
//            if (Main.menuMode != 15)
//            {
//                Main.netMode = 0;
//                Main.menuMode = 10;
//                WorldFile.saveWorld(false);
//                while (WorldGen.saveLock)
//                {
//                }
//                Main.menuMode = 0;
//            }
//            else
//            {
//                Main.netMode = 0;
//            }
//            Main.myPlayer = 0;
//        }
//
//    }
//
//    public class ServerSock2 : tdsm.api.Callbacks.IAPISocket
//    {
//        public Socket clientSocket;
//
//        public override void SpamUpdate()
//        {
//            if (!Netplay.spamCheck)
//            {
//                this.spamProjectile = 0f;
//                this.spamDelBlock = 0f;
//                this.spamAddBlock = 0f;
//                this.spamWater = 0f;
//                return;
//            }
//            if (this.spamProjectile > this.spamProjectileMax)
//            {
//                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Projectile spam");
//            }
//            if (this.spamAddBlock > this.spamAddBlockMax)
//            {
//                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Add tile spam");
//            }
//            if (this.spamDelBlock > this.spamDelBlockMax)
//            {
//                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Remove tile spam");
//            }
//            if (this.spamWater > this.spamWaterMax)
//            {
//                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
//            }
//            this.spamProjectile -= 0.4f;
//            if (this.spamProjectile < 0f)
//            {
//                this.spamProjectile = 0f;
//            }
//            this.spamAddBlock -= 0.3f;
//            if (this.spamAddBlock < 0f)
//            {
//                this.spamAddBlock = 0f;
//            }
//            this.spamDelBlock -= 5f;
//            if (this.spamDelBlock < 0f)
//            {
//                this.spamDelBlock = 0f;
//            }
//            this.spamWater -= 0.2f;
//            if (this.spamWater < 0f)
//            {
//                this.spamWater = 0f;
//            }
//        }
//
//        public override void SpamClear()
//        {
//            this.spamProjectile = 0f;
//            this.spamAddBlock = 0f;
//            this.spamDelBlock = 0f;
//            this.spamWater = 0f;
//        }
//
//        public static void CheckSection(int who, Microsoft.Xna.Framework.Vector2 position)
//        {
//            int sectionX = Netplay.GetSectionX((int)(position.X / 16f));
//            int sectionY = Netplay.GetSectionY((int)(position.Y / 16f));
//            int num = 0;
//            for (int i = sectionX - 1; i < sectionX + 2; i++)
//            {
//                for (int j = sectionY - 1; j < sectionY + 2; j++)
//                {
//                    if (i >= 0 && i < Main.maxSectionsX && j >= 0 && j < Main.maxSectionsY && !Netplay.serverSock[who].tileSection[i, j])
//                    {
//                        num++;
//                    }
//                }
//            }
//            if (num > 0)
//            {
//                int num2 = num;
//                NetMessage.SendData(9, who, -1, Lang.inter[44], num2, 0f, 0f, 0f, 0);
//                Netplay.serverSock[who].statusText2 = "is receiving tile data";
//                Netplay.serverSock[who].statusMax += num2;
//                for (int k = sectionX - 1; k < sectionX + 2; k++)
//                {
//                    for (int l = sectionY - 1; l < sectionY + 2; l++)
//                    {
//                        if (k >= 0 && k < Main.maxSectionsX && l >= 0 && l < Main.maxSectionsY && !Netplay.serverSock[who].tileSection[k, l])
//                        {
//                            NetMessage.SendSection(who, k, l, false);
//                            NetMessage.SendData(11, who, -1, "", k, (float)l, (float)k, (float)l, 0);
//                        }
//                    }
//                }
//            }
//        }
//
//        public override bool SectionRange(int size, int firstX, int firstY)
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                int num = firstX;
//                int num2 = firstY;
//                if (i == 1)
//                {
//                    num += size;
//                }
//                if (i == 2)
//                {
//                    num2 += size;
//                }
//                if (i == 3)
//                {
//                    num += size;
//                    num2 += size;
//                }
//                int sectionX = Netplay.GetSectionX(num);
//                int sectionY = Netplay.GetSectionY(num2);
//                if (this.tileSection[sectionX, sectionY])
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//
//        public override void Reset()
//        {
//            for (int i = 0; i < Main.maxSectionsX; i++)
//            {
//                for (int j = 0; j < Main.maxSectionsY; j++)
//                {
//                    this.tileSection[i, j] = false;
//                }
//            }
//            if (this.whoAmI < 255)
//            {
//                Main.player[this.whoAmI] = new Player();
//            }
//            this.timeOut = 0;
//            this.statusCount = 0;
//            this.statusMax = 0;
//            this.statusText2 = "";
//            this.statusText = "";
//            this.name = "Anonymous";
//            this.state = 0;
//            this.locked = false;
//            this.kill = false;
//            this.SpamClear();
//            this.active = false;
//            NetMessage.buffer[this.whoAmI].Reset();
//            if (this.networkStream != null)
//            {
//                this.networkStream.Close();
//            }
//            if (this.tcpClient != null)
//            {
//                this.tcpClient.Close();
//            }
//        }
//
//        public override void ServerWriteCallBack(IAsyncResult ar)
//        {
//            NetMessage.buffer[this.whoAmI].spamCount--;
//            if (this.statusMax > 0)
//            {
//                this.statusCount++;
//            }
//        }
//
//        public override void ServerReadCallBack(IAsyncResult ar)
//        {
//            int num = 0;
//            if (!Netplay.disconnect)
//            {
//                try
//                {
//                    num = this.networkStream.EndRead(ar);
//                }
//                catch
//                {
//                }
//                if (num == 0)
//                {
//                    this.kill = true;
//                }
//                else
//                {
//                    if (Main.ignoreErrors)
//                    {
//                        try
//                        {
//                            NetMessage.RecieveBytes(this.readBuffer, num, this.whoAmI);
//                            goto IL_57;
//                        }
//                        catch
//                        {
//                            goto IL_57;
//                        }
//                    }
//                    NetMessage.RecieveBytes(this.readBuffer, num, this.whoAmI);
//                }
//            }
//        IL_57:
//            this.locked = false;
//        }
//
//        public ServerSock2()
//        {
//            this.tcpClient = new TcpClient();
//            this.tileSection = new bool[Main.maxTilesX / 200 + 1, Main.maxTilesY / 150 + 1];
//            this.statusText = "";
//            this.name = "Anonymous";
//            this.oldName = "";
//            this.spamProjectileMax = 100f;
//            this.spamAddBlockMax = 100f;
//            this.spamDelBlockMax = 500f;
//            this.spamWaterMax = 50f;
//            //base..ctor();
//        }
//    }
//}
