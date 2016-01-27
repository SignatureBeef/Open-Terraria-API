using System;
using OTA.Mod.Net;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using OTA.Mod.Npc;

namespace OTA.Mod.Net
{
    /// <summary>
    /// These define the minimum requirements for OTAPI to function between a server and client.
    /// </summary>
    public enum OTAPackets : int
    {
        NotiyOTAClient = 1,

        ProbePlugins,
        SyncPackets
    }

    /// <summary>
    /// This is a custom networking message that can be used to communicate to the server, multiple clients or a single client.
    /// It is intended for mods to use as OTAPI takes care of Packet Id handling.
    /// </summary>
    public abstract class OTAPacket
    {
        /// <summary>
        /// Gets or sets the packet identifier.
        /// </summary>
        /// <value>The packet identifier.</value>
        internal int PacketId { get; set; }

        private string _packetName;

        /// <summary>
        /// Gets the name of the packet. This is used to sync packets between client & server.
        /// </summary>
        internal string PacketName
        {
            get
            {
                if (_packetName == null)
                {
                    _packetName = this.GetType().FullName;
                }
                return _packetName;
            }
        }

        /// <summary>
        /// Write your custom packet data. This will then be sent to the other end of the pipe, which then will have
        /// the <see cref="OTAPacket.Read"/> method fired.
        /// </summary>
        /// <param name="writer">Writer that is ready to accept data that is to be sent to the remote endpoint.</param>
        /// <param name="args">Arguments specified when you create the packet to write.</param>
        public virtual void Write(BinaryWriter writer, params object[] args)
        {
        }

        /// <summary>
        /// Read your custom packet data using this method. It will be called when you have successfully sent a message from the
        /// client or server using the <see cref="OTAPacket.Write"/> method.
        /// </summary>
        /// <param name="bufferId">Identifier of the client.</param>
        /// <param name="reader">Reader that contains the data.</param>
        public virtual void Read(int bufferId, BinaryReader reader)
        {
        }
    }

    public class PluginRequirement : OTAPacket
    {
        struct PluginDefinition
        {
            public string Name { get; set; }

            public string Version { get; set; }

            public string DownloadUrl { get; set; }
        }

        public override void Read(int bufferId, BinaryReader reader)
        {
            Logging.Logger.Debug("READ " + this.GetType().FullName);
        }

        public override void Write(BinaryWriter writer, params object[] args)
        {
            Logging.Logger.Debug("WRITE " + this.GetType().FullName);
            //If writing as the server
            //  Send plugins that require a client version, plus the download url via the OTAPI servers

        }
    }

    public class SyncPackets : OTAPacket
    {
        public override void Read(int bufferId, BinaryReader reader)
        {
            Logging.Logger.Debug("READ " + this.GetType().FullName);
            var packetMap = new Dictionary<String, Int16>();
            var count = reader.ReadInt16();

            Logging.Logger.Debug($"Packets to sync: {count}");

            for (var x = 0; x < count; x++)
            {
                packetMap.Add(reader.ReadString(), reader.ReadInt16());
            }

            PacketRegister.LoadFromPacketMap(packetMap);
        }

        public override void Write(BinaryWriter writer, params object[] args)
        {
            Logging.Logger.Debug("WRITE " + this.GetType().FullName);
            //Send the qualified packet name, and it's id
            var packets = PacketRegister.GetRegisteredPackets();
            writer.Write((short)packets.Count());
            foreach (var packet in packets)
            {
                Logging.Logger.Debug("Sending packet to sync {packet.PacketName}->{packet.PacketId}");
                writer.Write(packet.PacketName);
                writer.Write((short)packet.PacketId); //Short for consistency
            }
        }
    }

    public class SyncNpcTexture : OTAPacket
    {
        public override void Read(int bufferId, BinaryReader reader)
        {
            try
            {
                Logging.Logger.Debug("READ " + this.GetType().FullName);

                #if CLIENT
                var typeId = reader.ReadInt32();
                var filename = reader.ReadString();
                var frameCount = reader.ReadInt32();
                var fileLength = reader.ReadInt32();
                var contents = reader.ReadBytes(fileLength);

                Logging.Logger.Debug("Read NPC texture {0} with {1} bytes for type {2}", filename, fileLength, typeId);

                var folder = Path.Combine(Globals.DataPath, "ServerNPCs");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var savePath = Path.Combine(folder, filename);
                if (File.Exists(savePath)) File.Delete(savePath);

                File.WriteAllBytes(savePath, contents);

                Callbacks.MainCallback.ScheduleNpcTextureLoad(typeId, savePath, frameCount);
                #endif
            }
            catch (Exception e)
            {
                Logging.Logger.Error("SyncNpcTexture read error {0}", e);
            }
        }

        public override void Write(BinaryWriter writer, params object[] args)
        {
            #if SERVER
            Logging.Logger.Debug("WRITE " + this.GetType().FullName);

            var typeId = (int)args[0];
            var texture = (OTA.Mod.Npc.ServerTexture)args[1];

            //TODO implement caching
            var info = new FileInfo(texture.FilePath);
            if (info.Exists)
            {
                writer.Write(typeId);
                writer.Write(info.Name);
                writer.Write(texture.FrameCount);
                var content = File.ReadAllBytes(info.FullName);
                writer.Write(content.Length);
                writer.Write(content);

                Logging.Logger.Debug("Sent NPC texture {0} with {1} bytes for type {2}", info.Name, content.Length, typeId);
            }
            else
            {
                Logging.Logger.Error("Tried to send missing NPC texture {0}", args[0] as string);
            }
            #endif
        }
    }

    /// <summary>
    /// PacketRegister is a facility that mods can use to register custom packets, or to start sending them.
    /// </summary>
    public static class PacketRegister
    {
        internal const int BasePacket = (int)Packet.PACKET_67;

        private static Dictionary<Int32, OTAPacket> _packets = new Dictionary<Int32, OTAPacket>();
        private static int _assignedIds;
        private static readonly object _packetLock = new object();

        static PacketRegister()
        {
            lock (_packetLock)
                RegisterDefaults(_packets);
        }

        private static void RegisterDefaults(Dictionary<Int32, OTAPacket> packets)
        {
            int id = 1;
            packets.Add(id, new PluginRequirement() { PacketId = id });
            id++;
            packets.Add(id, new SyncPackets() { PacketId = id });
            id++;
            packets.Add(id, new SyncNpcTexture() { PacketId = id });
            id++;
        }

        /// <summary>
        /// Returns the registered list of CUSTOM packets.
        /// These are non OTAPI packets that should be synced to the client (as OTAPI has a defined preset in order to do this).
        /// </summary>
        /// <returns>The registered packets.</returns>
        internal static IEnumerable<OTAPacket> GetRegisteredPackets()
        {
            lock (_packetLock)
                return _packets
                    .Select(x => x.Value)
                    .Where(y => y.PacketId > (int)OTAPackets.SyncPackets);
        }

        /// <summary>
        /// Loads designated packets from a client net map
        /// </summary>
        /// <param name="map">Map.</param>
        internal static bool LoadFromPacketMap(Dictionary<String, Int16> map)
        {
            var packets = new Dictionary<Int32, OTAPacket>();
            RegisterDefaults(packets);

            var ota = typeof(OTAPacket);
            foreach (var item in map)
            {
                var type = Type.GetType(item.Key);
                if (type == null || !ota.IsAssignableFrom(type))
                {
                    Logging.Logger.Error($"Failed to find packet type sent from server {item.Key}");
                    return false;
                }

                var pkt = (OTAPacket)Activator.CreateInstance(type);
                packets.Add(item.Value, pkt);
            }

            lock (_packets)
            {
                _assignedIds = 0;
                foreach (var item in packets)
                {
                    item.Value.PacketId = System.Threading.Interlocked.Increment(ref _assignedIds);
                }
                Logging.Logger.Debug("Loaded {0} packets from map", packets.Count);
                _packets = packets;
            }

            return true;
        }

        /// <summary>
        /// Registers a custom OTAPacket. Typically OTAPI will scan your assembly and automatically import your custom packets.
        /// </summary>
        /// <typeparam name="T">Your custom packet class</typeparam>
        public static void Register<T>() where T : OTAPacket
        {
            lock (_packetLock)
            {
                var pkt = (OTAPacket)Activator.CreateInstance<T>();
                pkt.PacketId = System.Threading.Interlocked.Increment(ref _assignedIds);
                _packets.Add(pkt.PacketId, pkt);
            }
        }

        /// <summary>
        /// Start building a new packet to be sent to the remote endpoint.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The packet you wish to send to the remote endpoint.</typeparam>
        public static MessageBuilder Write<T>(params object[] args) where T : OTAPacket
        {
            return Write<T>(null, args);
        }

        /// <summary>
        /// Start building a new packet to be sent to the remote endpoint.
        /// </summary>
        /// <param name="builder">Allows you to append to an existing builder.</param>
        /// <param name="args">Arguments to be sent to the Write method of your custom packet</param>
        /// <typeparam name="T">The packet you wish to send to the remote endpoint.</typeparam>
        public static MessageBuilder Write<T>(MessageBuilder builder = null, params object[] args) where T : OTAPacket
        {
            lock (_packetLock)
            {
                var instance = _packets.SingleOrDefault(x => x.Value is T);
                if (instance.Value != null)
                {
                    var begin = builder == null;
                    builder = builder ?? MessageBuilder.PrepareThreadInstance();
                    if(begin) builder.Begin(BasePacket);

                    builder.Write(instance.Value, args);

//                    builder.End();

                    return builder;
                }
            }
            return null;
        }

        /// <summary>
        /// This method will attempt to read a custom packet sent from the client/server.
        /// </summary>
        /// <returns><c>true</c>, if packet was processed, <c>false</c> otherwise.</returns>
        /// <param name="bufferId">Identifier of the endpoint.</param>
        internal static bool ProcessPacket(int bufferId, int length)
        {
            var reader = NetMessage.buffer[bufferId].reader;

            bool processed = false;

            var max = reader.BaseStream.Position + length - 1;
            while (reader.BaseStream.Position < max)
            {
                var packetId = reader.ReadInt16();

                Logging.Logger.Debug($"Receiving sub packet {packetId}");

                OTAPacket instance = null;
                lock (_packets)
                    _packets.TryGetValue(packetId, out instance);

                if (instance != null)
                {
                    Logging.Logger.Debug($"Processing sub packet {packetId}");
                    instance.Read(bufferId, reader);
                    processed = true;
                }
                else
                {
                    Logging.Logger.Debug($"Invalid sub packet {packetId}");
                    break;
                }
            }

            return processed;
        }
    }
}

