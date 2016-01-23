using System;
using Terraria;
using OTA.Sockets;
using OTA.Plugin;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text;
using OTA.Mods.Net;

namespace OTA.Mod.Net
{
    public partial class MessageBuilder
    {
        const int DefaultSize = 65535;
        const int PacketIdLength = 2;

        private readonly System.IO.MemoryStream _ms;
        private readonly System.IO.BinaryWriter _bin;
        private readonly byte[] _buffer;
        private int _start;
        private bool _ended;

        [ThreadStatic]
        private static MessageBuilder _threadInstance;

        public static MessageBuilder PrepareThreadInstance(int size = DefaultSize)
        {
            if (_threadInstance == null || _threadInstance._buffer.Length < size)
                _threadInstance = new MessageBuilder(size);
            else
            {
                _threadInstance._ms.Position = 0;
                _threadInstance._start = 0;
            }
            return _threadInstance;
        }

        public byte[] Output
        {
            get
            {
                var copy = new byte[_ms.Position];
                Array.Copy(_buffer, copy, _ms.Position);
                return copy;
            }
        }

        public ArraySegment<byte> Segment
        {
            get
            {
                return new ArraySegment<byte>(_buffer, 0, (int)_ms.Position);
            }
        }

        public int Written
        {
            get
            {
                return (int)_ms.Position;
            }
        }

        public MessageBuilder(int bufSize = 65535)
        {
            _buffer = new byte[bufSize];
            _ms = new System.IO.MemoryStream(_buffer);
            _bin = new System.IO.BinaryWriter(_ms);
        }

        public void Clear()
        {
            _start = 0;
            _ms.Position = 0;
        }

        public void Begin()
        {
            _start = (int)_ms.Position;
            _ms.Position += PacketIdLength;
            _ended = false;
        }

        public void Begin(int id)
        {
            Logging.Logger.Debug($"Create custom packet id of {id}");
            _start = (int)_ms.Position;
            _ms.Position += PacketIdLength;
            _ms.WriteByte((byte)id);
            _ended = false;
        }

        public void Begin(Packet id)
        {
            Begin((int)id);
        }

        public void End()
        {
            var pos = _ms.Position;
            _ms.Position = _start;
            _bin.Write((short)(pos - _start));
            Logging.Logger.Debug($"Ending custom packet of size {pos - _start}");
            _ms.Position = pos;
            _ended = true;
        }

        public void Write(OTAPacket packet, params object[] args)
        {
            _bin.Write((short)packet.PacketId);
            packet.Write(_bin, args);
        }

        /// <summary>
        /// Append another packet to be built, and sent at the same time.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public MessageBuilder Append<T>(params object[] args) where T : OTAPacket
        {
            return PacketRegister.Write<T>(this, args);
        }

        #if SERVER
        /// <summary>
        /// Broadcast to all remote clients that are connected to the server.
        /// </summary>
        /// <param name="except">Except.</param>
        public void Broadcast(int except = -1)
        {
            if (!_ended) End();

            Logging.Logger.Debug($"Broadcasting custom packet");
            for (var x = 0; x < Terraria.Netplay.Clients.Length; x++)
            {
                if (x != except)
                {
                    var client = Terraria.Netplay.Clients[x];
                    if (client.IsConnected())
                    {
                        (client.Socket as ClientConnection).CopyAndSend(Segment);
                    }
                }
            }
        }
        #endif

        /// <summary>
        /// Send the built message to the endpoint
        /// </summary>
        /// <param name="target">Target buffer identifier that is to receive the data.</param>
        public void SendTo(int target)
        {
            if (!_ended) End();

            Logging.Logger.Debug($"Sending custom packet to {target}");
            var client = Terraria.Netplay.Clients[target];
            if (client.IsConnected())
            {
                (client.Socket as ClientConnection).CopyAndSend(Segment);
            }
        }
    }
}