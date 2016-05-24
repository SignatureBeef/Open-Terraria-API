#if CUSTOM_SOCKETS
using System;
using System.Diagnostics;
using System.Net.Sockets;
using OTA.Plugin;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections.Concurrent;
using OTA.Misc;
using OTA.Logging;

#if Full_API
using Terraria;
using Terraria.Net.Sockets;
using Terraria.Net;

#else
using OTA.Callbacks;
#endif
namespace OTA.Sockets
{
#if !Full_API
    public interface ISocket
    {
        void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state);

        void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state);

        void Close();

        void Connect(RemoteAddress address);

        RemoteAddress GetRemoteAddress();

        bool StartListening(SocketConnectionAccepted callback);

        void StopListening();

        bool IsConnected();

        bool IsDataAvailable();
    }

    public class RemoteAddress
    {

    }

    public class TcpAddress : RemoteAddress
    {
        public IPAddress Address;

        public int Port;

        public TcpAddress(IPAddress addr, int port)
        {
        }
    }

    public delegate void SocketConnectionAccepted(ISocket client);
    public delegate void SocketReceiveCallback(object state,int size);
    public delegate void SocketSendCallback(object state);

#endif

    /// <summary>
    /// A tcp client implementation that directly bolts into the Terrarian socket implementation
    /// </summary>
    public class ClientConnection : Connection, ISocket
    {
        private TcpAddress _remoteAddress;
        private TcpListener _listener;

        private bool _isListening;
        private bool _isReceiving;

        //        public SlotState State { get; set; }

        public int SlotId;

        public bool IsOTAClient { get; internal set; }

        public void Set(int value)
        {
            this.SlotId = value;
        }

        private SocketConnectionAccepted _listenerCallback;

        /*static ClientConnection()
        {
            var thread = new ProgramThread("TmoL", TimeoutLoop);
            thread.IsBackground = true;
            thread.Start();
        }*/

        public ClientConnection()
        {

        }

        public ClientConnection(Socket sock)
            : base(sock)
        {
            if (SlotId == 0)
                SlotId = -1;

            var remoteEndPoint = (IPEndPoint)sock.RemoteEndPoint;
            _remoteAddress = new TcpAddress(remoteEndPoint.Address, remoteEndPoint.Port);

            sock.LingerState = new LingerOption(true, 10);
            sock.NoDelay = true;

            //_isReceiving = true; //The connection was established, so we can begin reading
        }

        class AsyncCallback //: IDisposable
        {
            public SocketReceiveCallback Callback;
            public int Offset;
            public int Size;
            public byte[] Buffer;
            public object State;

            public void Dispose()
            {
                Callback = null;
                Offset = 0;
                Size = 0;
                Buffer = null;
                State = null;
            }
        }

        AsyncCallback _callback;

        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
        {
            ProgramLog.Log("Data requested from ", this.SlotId);
            if (_callback == null)
            {
                _callback = new AsyncCallback()
                {
                    Callback = callback,
                    Buffer = data,
                    Offset = offset,
                    Size = size,
                    State = state
                };
            }
            else
            {
                _callback.Offset = offset;
            }

            if (recvBytes > 0)
                ProcessRead();
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
        {
#if Full_API
            //            Main.ignoreErrors = false;
            var ctx = new HookContext()
            {
                Connection = this
            };
            var args = new HookArgs.SendClientData()
            {
                Data = data,
                Offset = offset,
                Size = size,
                Callback = callback,
                State = state
            };

            HookPoints.SendClientData.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                var bt = new byte[size];
                Buffer.BlockCopy(data, offset, bt, 0, size);
                this.Send(bt);
                bt = null;

                if (callback != null)
                    callback(state);

                this.Flush();
            }
#endif
        }

        protected override void ProcessRead()
        {
            if (_callback != null)
            {
                ProgramLog.Log("Despatching data from " + this.SlotId);
                if (!_isReceiving) _isReceiving = true;
                byte[] local;
                lock (recvSyncRoot)
                {
                    local = new byte[recvBytes];
                    Buffer.BlockCopy(recvBuffer, 0, local, 0, recvBytes);

                    //Reset read position
                    recvBytes = 0;
                }

                DespatchData(local);
            }
            else
            {
                ProgramLog.Log("No data vailable for ", this.SlotId);
            }
        }

        void DespatchData(byte[] buff)
        {
            try
            {
                int processed = 0;
                while (processed < buff.Length)
                {
                    var len = buff.Length - processed;
                    if (len > _callback.Size)
                        len = _callback.Size;

                    if (len > 0)
                    {
                        //No point shifting the target buffer as they always read from index 0
                        Buffer.BlockCopy(buff, processed, _callback.Buffer, _callback.Offset, len);
                        _callback.Callback(_callback.State, len);

                        processed += len;
                    }
                }
            }
            catch (Exception e)
            {
                var buffLength = buff == null ? "<null>" : buff.Length.ToString();
                var callbackExists = _callback == null ? "<null>" : "yes";
                var cbBufferExists = _callback == null || _callback.Buffer == null ? "<null>" : "Nope";
                var cbOffset = _callback == null ? "<null>" : _callback.Offset.ToString();
                var cbSize = _callback == null ? "<null>" : _callback.Size.ToString();

                Logger.Error("Read exception caught.");
                Logger.Error("Debug Values: {0},{1},{2},{3},{4}", buffLength, callbackExists, cbBufferExists, cbOffset, cbSize);
                Logger.Error("Error: {0}", e);
            }
        }

        protected override void HandleClosure(SocketError err)
        {
            Logger.Log(ProgramLog.Categories.User, TraceLevel.Info, "{0}: connection closed ({1}).", RemoteAddress, err);
            _isReceiving = false;

            //Issue a 0 byte response, Terraria will close the connection :)
            if (_callback != null) _callback.Callback(null, 0);
        }

        public void StartReading()
        {
#if DEBUG && Full_API
            Main.ignoreErrors = false;
#endif
            StartReceiving(new byte[4192]);
            txBuffer = new byte[16384];
        }

        void ISocket.Close()
        {
            ProgramLog.Log("Socket closed at " + this.SlotId);
            //if (_isReceiving)
            Close();
            //_isReceiving = false;
        }

        void ISocket.Connect(RemoteAddress address)
        {
            _remoteAddress = address as TcpAddress;
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(_remoteAddress.Address, _remoteAddress.Port);
            SetSocket(sock);
        }

        RemoteAddress ISocket.GetRemoteAddress()
        {
            return _remoteAddress;
        }

        public void Kick(string reason)
        {
            var messageLength = (short)(1 /*PacketId*/ + reason.Length);

            using (var ms = new System.IO.MemoryStream())
            {
                using (var bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(messageLength);
                    bw.Write((byte)Packet.DISCONNECT);
                    bw.Write(reason);
                }

                KickAfter(ms.ToArray());
            }
        }

        bool ISocket.StartListening(SocketConnectionAccepted callback)
        {
#if Full_API
            IPAddress any = IPAddress.Any;
            string ipString;
            if (Program.LaunchParameters.TryGetValue("-ip", out ipString) && !IPAddress.TryParse(ipString, out any))
            {
                any = IPAddress.Any;
            }
            this._isListening = true;
            this._listenerCallback = callback;
            if (this._listener == null)
            {
                this._listener = new TcpListener(any, Netplay.ListenPort);
            }
            try
            {
                this._listener.Start();
            }
            catch (Exception)
            {
                return false;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenLoop));
#endif
            return true;
        }

        void ISocket.StopListening()
        {
            this._isListening = false;
        }

        bool ISocket.IsConnected()
        {
            return Active;
        }

        bool ISocket.IsDataAvailable()
        {
            return recvBytes > 0;
        }

        private void ListenLoop(object unused)
        {
#if Full_API
            while (this._isListening && !Terraria.Netplay.disconnect)
            {
                try
                {
                    var socket = this._listener.AcceptSocket();
                    var connection = new ClientConnection(socket);
                    Netplay.anyClients = true;
                    //                    ProgramLog.Users.Log(socket.GetRemoteAddress() + " is connecting..."); TODO remove IL in vanilla as it's a bare log (or change it...)
                    //                    this._listenerCallback(socket);
                    this._listenerCallback.BeginInvoke(connection, (ar) =>
                        {
                            _listenerCallback.EndInvoke(ar);
                        }, null);

                    var ctx = new HookContext
                    {
                        Connection = connection
                    };

                    var args = new HookArgs.NewConnection();

                    HookPoints.NewConnection.Invoke(ref ctx, ref args);

                    if (ctx.CheckForKick())
                        break;

                    connection.StartReading();
                }
                catch (Exception e)
                {
                    ProgramLog.Error.Log(e, "Exception in ClientConnection.ListenLoop");
                }
            }
            this._listener.Stop();
#endif
        }
    }
}
#else
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTA.Sockets
{
    public class TemporarySynchSock : ISocket /* Whoever done this, I love you. */
    {

        //
        // Fields
        //
        private TcpClient _connection;

        private TcpListener _listener;

        private SocketConnectionAccepted _listenerCallback;

        private RemoteAddress _remoteAddress;

        private bool _isListening;

        //
        // Constructors
        //
        public TemporarySynchSock(TcpClient tcpClient)
        {
            this._connection = tcpClient;
            this._connection.NoDelay = true;
            IPEndPoint iPEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            this._remoteAddress = new TcpAddress(iPEndPoint.Address, iPEndPoint.Port);
        }

        public TemporarySynchSock()
        {
            //            this._connection = new TcpClient ();
            //            this._connection.NoDelay = true;
        }

        private void CheckSocket()
        {
            if (this._connection == null)
            {
                this._connection = new TcpClient();
                this._connection.NoDelay = true;
            }
        }

        //
        // Methods
        //
        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
        {
            int len = this._connection.GetStream().Read(data, offset, size);
            callback(null, len);
            //            this._connection.GetStream ().BeginRead (data, offset, size, new AsyncCallback (this.ReadCallback), new Tuple<SocketReceiveCallback, object> (callback, state));
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
        {
            this._connection.GetStream().Write(data, offset, size);
            callback(null);
            //            this._connection.GetStream ().BeginWrite (data, 0, size, new AsyncCallback (this.SendCallback), new Tuple<SocketSendCallback, object> (callback, state));
        }

        void ISocket.Close()
        {
            this._remoteAddress = null;
            if (this._connection != null)
            {
                this._connection.Close();
            }
        }

        void ISocket.Connect(RemoteAddress address)
        {
            CheckSocket();
            TcpAddress tcpAddress = (TcpAddress)address;
            this._connection.Connect(tcpAddress.Address, tcpAddress.Port);
            this._remoteAddress = address;
        }

        RemoteAddress ISocket.GetRemoteAddress()
        {
            return this._remoteAddress;
        }

        bool ISocket.IsConnected()
        {
            return this._connection != null && this._connection.Client != null && this._connection.Connected;
        }

        bool ISocket.IsDataAvailable()
        {
            return this._connection.GetStream().DataAvailable;
        }

        private void ListenLoop(object unused)
        {
            while (this._isListening && !Terraria.Netplay.disconnect)
            {
                try
                {
                    ISocket socket = new TemporarySynchSock(this._listener.AcceptTcpClient());
                    Console.WriteLine(socket.GetRemoteAddress() + " is connecting...");
                    this._listenerCallback(socket);
                }
                catch (Exception)
                {
                }
            }
            this._listener.Stop();
        }

        private void ReadCallback(IAsyncResult result)
        {
            Tuple<SocketReceiveCallback, object> tuple = (Tuple<SocketReceiveCallback, object>)result.AsyncState;
            tuple.Item1(tuple.Item2, this._connection.GetStream().EndRead(result));
        }

        private void SendCallback(IAsyncResult result)
        {
            Tuple<SocketSendCallback, object> tuple = (Tuple<SocketSendCallback, object>)result.AsyncState;
            try
            {
                this._connection.GetStream().EndWrite(result);
                tuple.Item1(tuple.Item2);
            }
            catch (Exception)
            {
                ((ISocket)this).Close();
            }
        }

        bool ISocket.StartListening(SocketConnectionAccepted callback)
        {
            this._isListening = true;
            this._listenerCallback = callback;
            if (this._listener == null)
            {
                this._listener = new TcpListener(IPAddress.Any, Terraria.Netplay.ListenPort);
            }
            try
            {
                this._listener.Start();
            }
            catch (Exception)
            {
                return false;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenLoop));
            return true;
        }

        void ISocket.StopListening()
        {
            this._isListening = false;
        }
    }
}
#endif