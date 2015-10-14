using System;
using System.Web.Http.Metadata.Providers;
using OTA.Sockets;

#if Full_API
using Terraria;
#endif
using Microsoft.Xna.Framework;
using OTA.Command;
using OTA.ID;

namespace OTA.Plugin
{
    /// <summary>
    /// Contains definitions of all OTA hooks
    /// </summary>
    // TODO remove unused hooks
    public static class HookPoints
    {
#if CLIENT
        public static readonly HookPoint<HookArgs.Draw> Draw;
        public static readonly HookPoint<HookArgs.Update> Update;
        public static readonly HookPoint<HookArgs.UpdateClient> UpdateClient;
#endif

#if SERVER
        public static readonly HookPoint<HookArgs.AddBan> AddBan;
        public static readonly HookPoint<HookArgs.Command> Command;
        public static readonly HookPoint<HookArgs.ConfigurationLine> ConfigurationLine;
        public static readonly HookPoint<HookArgs.ConsoleMessageReceived> ConsoleMessageReceived;
        public static readonly HookPoint<HookArgs.NameConflict> NameConflict;
        public static readonly HookPoint<HookArgs.NewConnection> NewConnection;
        public static readonly HookPoint<HookArgs.ParseCommandLineArguments> ParseCommandLineArguments;
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanged> PlayerAuthenticationChanged;
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanging> PlayerAuthenticationChanging;
        public static readonly HookPoint<HookArgs.PlayerEnteredGame> PlayerEnteredGame;
        public static readonly HookPoint<HookArgs.PlayerEnteringGame> PlayerEnteringGame;
        public static readonly HookPoint<HookArgs.PlayerLeftGame> PlayerLeftGame;
        public static readonly HookPoint<HookArgs.PlayerPassReceived> PlayerPassReceived;
        public static readonly HookPoint<HookArgs.PlayerPreGreeting> PlayerPreGreeting;
        public static readonly HookPoint<HookArgs.ServerStateChange> ServerStateChange;
        public static readonly HookPoint<HookArgs.ServerPassReceived> ServerPassReceived;
        /// <summary>
        /// Server tick event. Occurs without players.
        /// </summary>
        public static readonly HookPoint<HookArgs.ServerTick> ServerTick;
        /// <summary>
        /// Game UpdateServer event. Does not occur without players.
        /// </summary>
        public static readonly HookPoint<HookArgs.ServerUpdate> ServerUpdate;
        public static readonly HookPoint<HookArgs.StartCommandProcessing> StartCommandProcessing;
        public static readonly HookPoint<HookArgs.WorldAutoSave> WorldAutoSave;
#endif

#if CLIENT || SERVER
        public static readonly HookPoint<HookArgs.ChestBreakReceived> ChestBreakReceived;
        public static readonly HookPoint<HookArgs.ChestOpenReceived> ChestOpenReceived;
        public static readonly HookPoint<HookArgs.PressurePlateTriggered> PressurePlateTriggered;
        public static readonly HookPoint<HookArgs.DeathMessage> DeathMessage;
        public static readonly HookPoint<HookArgs.DoorStateChanged> DoorStateChanged;
        public static readonly HookPoint<HookArgs.GameUpdate> GameUpdate;
        public static readonly HookPoint<HookArgs.InvasionNpcSpawn> InvasionNpcSpawn;
        public static readonly HookPoint<HookArgs.InvasionWarning> InvasionWarning;
        public static readonly HookPoint<HookArgs.ItemNetDefaults> ItemNetDefaults;
        public static readonly HookPoint<HookArgs.ItemSetDefaultsByName> ItemSetDefaultsByName;
        public static readonly HookPoint<HookArgs.ItemSetDefaultsByType> ItemSetDefaultsByType;
        public static readonly HookPoint<HookArgs.LiquidFlowReceived> LiquidFlowReceived;
        public static readonly HookPoint<HookArgs.MechSpawn> MechSpawn;
        public static readonly HookPoint<HookArgs.NpcDropBossBag> NpcDropBossBag;
        public static readonly HookPoint<HookArgs.NpcDropLoot> NpcDropLoot;
        public static readonly HookPoint<HookArgs.NpcHurt> NpcHurt;
        public static readonly HookPoint<HookArgs.NpcKilled> NpcKilled;
        public static readonly HookPoint<HookArgs.NpcNetDefaults> NpcNetDefaults;
        public static readonly HookPoint<HookArgs.NpcSetDefaultsByName> NpcSetDefaultsByName;
        public static readonly HookPoint<HookArgs.NpcSetDefaultsByType> NpcSetDefaultsByType;
        public static readonly HookPoint<HookArgs.NpcSpawn> NpcSpawn;
        public static readonly HookPoint<HookArgs.NpcStrike> NpcStrike;
        public static readonly HookPoint<HookArgs.NpcTransform> NpcTransform;
        public static readonly HookPoint<HookArgs.PlayerChat> PlayerChat;
        public static readonly HookPoint<HookArgs.PlayerDataReceived> PlayerDataReceived;
        public static readonly HookPoint<HookArgs.PlayerHurt> PlayerHurt;
        public static readonly HookPoint<HookArgs.PlayerKilled> PlayerKilled;
        public static readonly HookPoint<HookArgs.PlayerWorldAlteration> PlayerWorldAlteration;
        public static readonly HookPoint<HookArgs.PluginLoadRequest> PluginLoadRequest;
        public static readonly HookPoint<HookArgs.PluginsLoaded> PluginsLoaded;
        public static readonly HookPoint<HookArgs.ProgramStart> ProgramStart;
        public static readonly HookPoint<HookArgs.ProjectileAI> ProjectileAI;
        public static readonly HookPoint<HookArgs.ProjectileKill> ProjectileKill;
        public static readonly HookPoint<HookArgs.ProjectileReceived> ProjectileReceived;
        public static readonly HookPoint<HookArgs.ProjectileSetDefaults> ProjectileSetDefaults;
        public static readonly HookPoint<HookArgs.ReceiveNetMessage> ReceiveNetMessage;
        public static readonly HookPoint<HookArgs.SendNetMessage> SendNetMessage;
        public static readonly HookPoint<HookArgs.SignTextGet> SignTextGet;
        public static readonly HookPoint<HookArgs.SignTextSet> SignTextSet;
        /// <summary>
        /// The callback to update the console with Terraria.Main.statusText
        /// </summary>
        public static readonly HookPoint<HookArgs.StatusTextChange> StatusTextChange;
        public static readonly HookPoint<HookArgs.TileSquareReceived> TileSquareReceived;
#endif

        //        public static readonly HookPoint<HookArgs.DatabaseInitialise> DatabaseInitialise;
        //        public static readonly HookPoint<HookArgs.StartDefaultServer> StartDefaultServer;
        //        public static readonly HookPoint<HookArgs.StatusTextChanged> StatusTextChanged;
        //        public static readonly HookPoint<HookArgs.UpdateServer> UpdateServer;
        //        public static readonly HookPoint<HookArgs.ConnectionRequestReceived> ConnectionRequestReceived;
        //        public static readonly HookPoint<HookArgs.DisconnectReceived> DisconnectReceived;
        //        public static readonly HookPoint<HookArgs.StateUpdateReceived> StateUpdateReceived;
        //        public static readonly HookPoint<HookArgs.InventoryItemReceived> InventoryItemReceived;
        //        public static readonly HookPoint<HookArgs.ObituaryReceived> ObituaryReceived;
        //        public static readonly HookPoint<HookArgs.PlayerTeleport> PlayerTeleport;
        //        public static readonly HookPoint<HookArgs.Explosion> Explosion;
        //        public static readonly HookPoint<HookArgs.PvpSettingReceived> PvpSettingReceived;
        //        public static readonly HookPoint<HookArgs.PartySettingReceived> PartySettingReceived;
        //        public static readonly HookPoint<HookArgs.WorldLoaded> WorldLoaded;
        //        public static readonly HookPoint<HookArgs.NpcCreation> NpcCreation;
        //        public static readonly HookPoint<HookArgs.PlayerTriggeredEvent> PlayerTriggeredEvent;
        //        public static readonly HookPoint<HookArgs.WorldGeneration> WorldGeneration;
        //        public static readonly HookPoint<HookArgs.WorldRequestMessage> WorldRequestMessage;


        static HookPoints()
        {
#if CLIENT
            Draw = new HookPoint<HookArgs.Draw>("draw");
            Update = new HookPoint<HookArgs.Update>("update");
            UpdateClient = new HookPoint<HookArgs.UpdateClient>("update-client");
#endif

#if SERVER
            AddBan = new HookPoint<HookArgs.AddBan>("add-ban");
            Command = new HookPoint<HookArgs.Command>("command");
            ConfigurationLine = new HookPoint<HookArgs.ConfigurationLine>("config-line");
            ConsoleMessageReceived = new HookPoint<HookArgs.ConsoleMessageReceived>("console-message-received");
            NewConnection = new HookPoint<HookArgs.NewConnection>("new-connection");
            NameConflict = new HookPoint<HookArgs.NameConflict>("name-conflict");
            ParseCommandLineArguments = new HookPoint<HookArgs.ParseCommandLineArguments>("parse-cmd-args");
            PlayerAuthenticationChanged = new HookPoint<HookArgs.PlayerAuthenticationChanged>("player-auth-change");
            PlayerAuthenticationChanging = new HookPoint<HookArgs.PlayerAuthenticationChanging>("player-auth-changing");
            PlayerEnteredGame = new HookPoint<HookArgs.PlayerEnteredGame>("player-entered-game");
            PlayerEnteringGame = new HookPoint<HookArgs.PlayerEnteringGame>("player-entering-game");
            PlayerLeftGame = new HookPoint<HookArgs.PlayerLeftGame>("player-left-game");
            PlayerPassReceived = new HookPoint<HookArgs.PlayerPassReceived>("player-pass-received");
            PlayerPreGreeting = new HookPoint<HookArgs.PlayerPreGreeting>("player-pre-greeting");
            ServerStateChange = new HookPoint<HookArgs.ServerStateChange>("server-state-change");
            ServerPassReceived = new HookPoint<HookArgs.ServerPassReceived>("server-pass-received");
            ServerTick = new HookPoint<HookArgs.ServerTick>("server-tick");
            ServerUpdate = new HookPoint<HookArgs.ServerUpdate>("server-update");
            StartCommandProcessing = new HookPoint<HookArgs.StartCommandProcessing>("start-command-processing");
            WorldAutoSave = new HookPoint<HookArgs.WorldAutoSave>("world-auto-save");
#endif

#if CLIENT || SERVER
            ChestBreakReceived = new HookPoint<HookArgs.ChestBreakReceived>("chest-break-received");
            ChestOpenReceived = new HookPoint<HookArgs.ChestOpenReceived>("chest-open-received");
            PressurePlateTriggered = new HookPoint<HookArgs.PressurePlateTriggered>("pressure-plate-triggered");
            DeathMessage = new HookPoint<HookArgs.DeathMessage>("death-message");
            DoorStateChanged = new HookPoint<HookArgs.DoorStateChanged>("door-state-changed");
            GameUpdate = new HookPoint<HookArgs.GameUpdate>("game-update");
            InvasionNpcSpawn = new HookPoint<HookArgs.InvasionNpcSpawn>("invasion-npc-spawn");
            InvasionWarning = new HookPoint<HookArgs.InvasionWarning>("invasion-warning");
            ItemNetDefaults = new HookPoint<HookArgs.ItemNetDefaults>("item-net-defaults");
            ItemSetDefaultsByName = new HookPoint<HookArgs.ItemSetDefaultsByName>("item-set-defaults-by-name");
            ItemSetDefaultsByType = new HookPoint<HookArgs.ItemSetDefaultsByType>("item-set-defaults-by-type");
            LiquidFlowReceived = new HookPoint<HookArgs.LiquidFlowReceived>("liquid-flow-received");
            MechSpawn = new HookPoint<HookArgs.MechSpawn>("mech-spawn");
            NpcDropBossBag = new HookPoint<HookArgs.NpcDropBossBag>("npc-drop-boss-bag");
            NpcDropLoot = new HookPoint<HookArgs.NpcDropLoot>("npc-drop-loot");
            NpcHurt = new HookPoint<HookArgs.NpcHurt>("npc-hurt");
            NpcKilled = new HookPoint<HookArgs.NpcKilled>("npc-killed");
            NpcNetDefaults = new HookPoint<HookArgs.NpcNetDefaults>("npc-net-defaults");
            NpcSetDefaultsByName = new HookPoint<HookArgs.NpcSetDefaultsByName>("npc-set-defaults-by-name");
            NpcSetDefaultsByType = new HookPoint<HookArgs.NpcSetDefaultsByType>("npc-set-defaults-by-type");
            NpcSpawn = new HookPoint<HookArgs.NpcSpawn>("npc-spawn");
            NpcStrike = new HookPoint<HookArgs.NpcStrike>("npc-strike");
            NpcTransform = new HookPoint<HookArgs.NpcTransform>("npc-transform");
            PlayerChat = new HookPoint<HookArgs.PlayerChat>("player-chat");
            PlayerDataReceived = new HookPoint<HookArgs.PlayerDataReceived>("player-data-received");
            PlayerHurt = new HookPoint<HookArgs.PlayerHurt>("player-hurt");
            PlayerKilled = new HookPoint<HookArgs.PlayerKilled>("player-killed");
            PlayerWorldAlteration = new HookPoint<HookArgs.PlayerWorldAlteration>("player-world-alteration");
            PluginLoadRequest = new HookPoint<HookArgs.PluginLoadRequest>("plugin-load-request");
            PluginsLoaded = new HookPoint<HookArgs.PluginsLoaded>("plugins-loaded");
            ProgramStart = new HookPoint<HookArgs.ProgramStart>("program-start");
            ProjectileAI = new HookPoint<HookArgs.ProjectileAI>("projectile-ai");
            ProjectileKill = new HookPoint<HookArgs.ProjectileKill>("projectile-kill");
            ProjectileReceived = new HookPoint<HookArgs.ProjectileReceived>("projectile-received");
            ProjectileSetDefaults = new HookPoint<HookArgs.ProjectileSetDefaults>("projectile-set-defaults");
            ReceiveNetMessage = new HookPoint<HookArgs.ReceiveNetMessage>("receive-net-message");
            SendNetMessage = new HookPoint<HookArgs.SendNetMessage>("send-net-message");
            SignTextGet = new HookPoint<HookArgs.SignTextGet>("sign-text-get");
            SignTextSet = new HookPoint<HookArgs.SignTextSet>("sign-text-set");
            StatusTextChange = new HookPoint<HookArgs.StatusTextChange>("status-text-changed");
            TileSquareReceived = new HookPoint<HookArgs.TileSquareReceived>("tile-square-received");
#endif
        }
    }

    public enum MethodState : byte
    {
        Begin,
        End
    }

    public static class HookArgs
    {
#if CLIENT
        public struct Draw { public MethodState State { get; set; } }
        public struct Update { public MethodState State { get; set; } }
        public struct UpdateClient { public MethodState State { get; set; } }
#endif

#if SERVER
        public struct AddBan
        {
            public string RemoteAddress { get; set; }
        }

        public struct ConfigurationLine
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public struct ConsoleMessageReceived
        {
            public string Message { get; set; }
            public OTA.Logging.SendingLogger Logger { get; set; }
        }

        public struct Command
        {
            public string Prefix { get; internal set; }
            public ArgumentList Arguments { get; set; }
            public string ArgumentString { get; set; }
        }

        public struct NameConflict
        {
            public Terraria.Player Connectee { get; set; }
            public int BufferId { get; set; }
        }

        public struct NewConnection
        {
        }

        public struct ParseCommandLineArguments
        {
        }

        public struct PlayerAuthenticationChanged
        {
            public string AuthenticatedAs { get; set; }
            public string AuthenticatedBy { get; set; }
        }

        public struct PlayerAuthenticationChanging
        {
            public string AuthenticatedAs { get; set; }
            public string AuthenticatedBy { get; set; }
        }

        public struct PlayerEnteredGame
        {
            public int Slot { get; set; }
        }

        public struct PlayerEnteringGame
        {
            public int Slot { get; set; }
        }

        public struct PlayerLeftGame
        {
            public int Slot { get; set; }
        }

        public struct PlayerPassReceived
        {
            public string Password { get; set; }
        }

        public struct PlayerPreGreeting
        {
            public int Slot { get; set; }
            public string Motd { get; set; }
            public Color MotdColour { get; set; }
        }

        public struct ServerStateChange
        {
            public ServerState ServerChangeState { get; set; }
        }

        public struct ServerPassReceived
        {
            public string Password { get; set; }
        }

        public struct ServerTick
        {
            public static readonly ServerTick Empty = new ServerTick();
        }

        public struct ServerUpdate
        {
            public static readonly ServerUpdate Begin = new ServerUpdate() { State = MethodState.Begin };
            public static readonly ServerUpdate End = new ServerUpdate() { State = MethodState.End };

            public MethodState State { get; set; }
        }

        public struct StartCommandProcessing
        {
        }

        public struct WorldAutoSave { }
#endif

#if CLIENT || SERVER
        public struct ChestBreakReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct ChestOpenReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int ChestIndex { get; set; }
        }

        public struct PressurePlateTriggered
        {
            public Sender Sender { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct DoorStateChanged
        {
            /// <summary>
            /// Location of the player when the state was changed
            /// </summary>
            /// <value>The position.</value>
            public Vector2 Position { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

            public int Direction { get; set; }

            /// <summary>
            /// Type of object being opened
            /// </summary>
            /// <value>The kind.</value>
            public int Kind { get; set; }
        }

        public struct DeathMessage
        {
            public int Player { get; set; }
            public int NPC { get; set; }
            public int Projectile { get; set; }
            public int Other { get; set; }
        }

        public struct GameUpdate
        {
            public static readonly GameUpdate Begin = new GameUpdate() { State = MethodState.Begin };
            public static readonly GameUpdate End = new GameUpdate() { State = MethodState.End };

            public MethodState State { get; set; }
        }

        public struct InvasionNpcSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct InvasionWarning
        {
        }

        public struct ItemNetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }
            public int Type { get; set; }
        }

        public struct ItemSetDefaultsByName
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }
            public string Name { get; set; }
        }

        public struct ItemSetDefaultsByType
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }
            public int Type { get; set; }
            public bool NoMatCheck { get; set; }
        }

        public struct LiquidFlowReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public byte Amount { get; set; }
            public bool Lava { get; set; }

            public bool Water
            {
                get { return !Lava; }
                set { Lava = !value; }
            }
        }

        public struct MechSpawn
        {
            public float X { get; set; }
            public float Y { get; set; }
            public int Type { get; set; }
            public int Num { get; set; }
            public int Num2 { get; set; }
            public int Num3 { get; set; }
            public OTA.Callbacks.MechSpawnType Sender { get; set; }
        }

        public struct NpcDropBossBag
        {
            public MethodState State { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Type { get; set; }
            public int Stack { get; set; }
            public bool NoBroadcast { get; set; }
            public int Prefix { get; set; }
            public bool NoGrabDelay { get; set; }
            public bool ReverseLookup { get; set; }
        }

        public struct NpcDropLoot
        {
            public MethodState State { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Type { get; set; }
            public int Stack { get; set; }
            public bool NoBroadcast { get; set; }
            public int Prefix { get; set; }
            public bool NoGrabDelay { get; set; }
            public bool ReverseLookup { get; set; }
        }

        public struct NpcHurt
        {
#if Full_API
            public NPC Victim { get; set; }
#endif
            public int Damage { get; set; }
            public int HitDirection { get; set; }
            public float Knockback { get; set; }
            public bool Critical { get; set; }
            public bool FromNet { get; set; }
            public bool NoEffect { get; set; }
        }

        public struct NpcKilled
        {
            public int Type { get; set; }
            public int NetId { get; set; }
        }

        public struct NpcNetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public int Type { get; set; }
        }

        public struct NpcSetDefaultsByName
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public string Name { get; set; }
        }

        public struct NpcSetDefaultsByType
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public int Type { get; set; }
            public float ScaleOverride { get; set; }
        }

        public struct NpcSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Type { get; set; }
            public int Start { get; set; }
        }

        public struct NpcTransform
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public int NewType { get; set; }
        }

        public struct NpcStrike
        {
            public Terraria.NPC Npc { get; set; }
            public double Damage { get; set; }
        }

        public struct PlayerChat
        {
            public string Message { get; set; }
            public Color Color { get; set; }
        }

        public struct PlayerDataReceived
        {
            public bool IsConnecting { get; set; }
            public string Name { get; set; }
            public int SkinVariant { get; set; }
            public int Hair { get; set; }
            public byte HairDye { get; set; }
            public bool[] HideVisual { get; set; }
            public byte HideMisc { get; set; }
            public byte Difficulty { get; set; }
            public bool ExtraAccessory { get; set; }

            public Color HairColor { get; set; }
            public Color SkinColor { get; set; }
            public Color EyeColor { get; set; }
            public Color ShirtColor { get; set; }
            public Color UnderShirtColor { get; set; }
            public Color PantsColor { get; set; }
            public Color ShoeColor { get; set; }

            public bool NameChecked { get; set; }
            public bool BansChecked { get; set; }
            public bool WhitelistChecked { get; set; }

            public int Parse(System.IO.BinaryReader reader, int at, int length)
            {
#if Full_API
                reader.ReadByte(); //Ignore player id

                SkinVariant = (int)MathHelper.Clamp((float)(int)reader.ReadByte(), 0, 7);
                Hair = (int)reader.ReadByte();
                if (Hair >= 134)
                    Hair = 0;

                Name = reader.ReadString().Trim();
                HairDye = reader.ReadByte();

                BitsByte bb;

                HideVisual = new bool[10];
                bb = reader.ReadByte();
                for (int i = 0; i < 8; i++)
                    HideVisual[i] = bb[i];

                bb = reader.ReadByte();
                for (int i = 0; i < 2; i++)
                    HideVisual[i + 8] = bb[i];

                HideMisc = reader.ReadByte();
                HairColor = reader.ReadRGB();
                SkinColor = reader.ReadRGB();
                EyeColor = reader.ReadRGB();
                ShirtColor = reader.ReadRGB();
                UnderShirtColor = reader.ReadRGB();
                PantsColor = reader.ReadRGB();
                ShoeColor = reader.ReadRGB();

                bb = reader.ReadByte();
                Difficulty = 0;
                if (bb[0]) Difficulty += 1;
                if (bb[1]) Difficulty += 2;
                if (Difficulty > 2) Difficulty = 2;

                ExtraAccessory = bb[2];

                return 0;
#else
                        return 0;
#endif
            }

#if Full_API
            public void Apply(Player player)
            {
                player.difficulty = Difficulty;
                player.name = Name;

                player.skinVariant = SkinVariant;
                player.hair = Hair;
                player.hairDye = HairDye;
                player.hideVisual = HideVisual;
                player.hideMisc = HideMisc;

                player.hairColor = HairColor;
                player.skinColor = SkinColor;
                player.eyeColor = EyeColor;
                player.shirtColor = ShirtColor;
                player.underShirtColor = UnderShirtColor;
                player.pantsColor = PantsColor;
                player.shoeColor = ShoeColor;
                player.extraAccessory = ExtraAccessory;

                Netplay.Clients[player.whoAmI].Name = Name;
            }
#endif

            public static Color ParseColor(byte[] buf, ref int at)
            {
                return new Color(buf[at++], buf[at++], buf[at++]);
            }

            public bool CheckName(out string error)
            {
                error = null;
                NameChecked = true;

                if (Name.Length > 20)
                {
                    error = "Invalid name: longer than 20 characters.";
                    return false;
                }

                if (Name == "")
                {
                    error = "Invalid name: whitespace or empty.";
                    return false;
                }

                foreach (char c in Name)
                {
                    if (c < 32 || c > 126)
                    {
                        //Console.Write ((byte) c);
                        error = "Invalid name: contains non-printable characters.";
                        return false;
                    }
                    //Console.Write (c);
                }

                if (Name.Contains(" " + " "))
                {
                    error = "Invalid name: contains double spaces.";
                    return false;
                }

                return true;
            }
        }

        public struct PlayerHurt
        {
#if Full_API
            public Player Victim { get; internal set; }
#endif
            public int Damage { get; set; }
            public int HitDirection { get; set; }
            public bool Pvp { get; set; }
            public bool Quiet { get; set; }
            public string Obituary { get; set; }
            public bool Critical { get; set; }
            public int CooldownCounter { get; set; }
        }

        public struct PlayerKilled
        {
            public double Damage { get; set; }
            public int HitDirection { get; set; }
            public bool PvP { get; set; }
            public string DeathText { get; set; }
        }

        public struct PlayerWorldAlteration
        {
            public int X { get; set; }

            public int Y { get; set; }
            public ActionType Action { get; set; }

            public short Type { get; set; }

            public int Style { get; set; }

            public bool TypeChecked { get; set; }

            //            public WorldMod.PlayerSandbox Sandbox { get; internal set; }

            public bool TileWasRemoved => Action == ActionType.KillTile || Action == ActionType.KillTile1 || Action == ActionType.UNKNOWN_1;

            public bool NoItem
            {
                get { return Action == ActionType.KillTile1 || Action == ActionType.UNKNOWN_2; }
                set
                {
                    if (value)
                    {
                        if (Action == ActionType.KillTile)
                            Action = ActionType.KillTile1;
                        else if (Action == ActionType.UNKNOWN_1)
                            Action = ActionType.UNKNOWN_2;
                    }
                    else
                    {
                        if (Action == ActionType.KillTile1)
                            Action = ActionType.KillTile;
                        else if (Action == ActionType.UNKNOWN_2)
                            Action = ActionType.UNKNOWN_1;
                    }
                }
            }

            public bool TileWasPlaced => Action == ActionType.PlaceTile;

            public bool WallWasRemoved => Action == ActionType.KillWall || Action == ActionType.UNKNOWN_1 || Action == ActionType.UNKNOWN_2;

            public bool WallWasPlaced => Action == ActionType.PlaceWall;

            public bool RemovalFailed
            {
                get { return Type == 1 && (Action == ActionType.KillTile || Action == ActionType.PlaceTile || Action == ActionType.KillTile1); }
                set
                {
                    if (Action == (int)ActionType.KillTile || Action == ActionType.KillWall || Action == ActionType.KillTile1)
                        Type = value ? (byte)1 : (byte)0;
                }
            }

#if Full_API && !MemTile && !VANILLACOMPAT
                    public Terraria.Tile Tile => Main.tile[X, Y];
#elif Full_API && (MemTile || VANILLACOMPAT) && TileReady
            public OTA.Memory.MemTile Tile => Main.tile[X, Y];
#endif
        }

        public struct PluginLoadRequest
        {
            public string Path { get; set; }
            public BasePlugin LoadedPlugin { get; set; }
        }

        public struct PluginsLoaded
        {
        }

        public struct ProgramStart
        {
            public string[] Arguments { get; set; }
        }

        public struct ProjectileAI
        {
            public MethodState State { get; set; }

            public Terraria.Projectile Projectile { get; set; }
        }

        public struct ProjectileKill
        {
            public int Index { get; set; }
            public int Id { get; set; }
            public int Owner { get; set; }
        }

        public struct ProjectileReceived
        {
            public int Id { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            //public float X { get; set; }
            //public float Y { get; set; }
            //public float VX { get; set; }
            //public float VY { get; set; }
            public float Knockback { get; set; }
            public int Damage { get; set; }
            public int Owner { get; set; }
            public int Type { get; set; }
            public float[] AI { get; set; }

            public float AI_0
            {
                get { return AI[0]; }
                set { AI[0] = value; }
            }

            public float AI_1
            {
                get { return AI[01]; }
                set { AI[01] = value; }
            }

            public int ExistingIndex { get; set; }

#if Full_API
            internal Projectile projectile;

            public Projectile CreateProjectile()
            {
                if (projectile != null)
                    return projectile;

                //                var index = Projectile.ReserveSlot(Id, Owner);
                //
                //                if (index == 1000) return null;
                //
                //                projectile = Registries.Projectile.Create(TypeByte);

                //                projectile.Id = index;
                //                Apply(projectile);

                return projectile;
            }

            public void Apply(Projectile projectile)
            {
                //                if (Owner < 255)
                //                    projectile.Creator = Main.player[Owner];
                projectile.identity = Id;
                projectile.owner = Owner;
                projectile.damage = Damage;
                projectile.knockBack = Knockback;
                //                projectile.position = new Vector2(X, Y);
                //                projectile.velocity = new Vector2(VX, VY);
                projectile.ai = AI;
            }

            internal void CleanupProjectile()
            {
                if (projectile != null)
                {
                    //                    Projectile.FreeSlot(projectile.identity, projectile.Owner, projectile.Id);
                    projectile = null;
                }
            }

            //            public ProjectileType Type
            //            {
            //                get { return (ProjectileType)TypeByte; }
            //                set { TypeByte = (byte)value; }
            //            }

            public Projectile Current
            {
                get { return Main.projectile[Id]; }
            }
#endif
        }

        public struct ProjectileSetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.Projectile Projectile { get; set; }
            public int Type { get; set; }
        }

        public struct ReceiveNetMessage
        {
            public int BufferId { get; set; }
            public byte PacketId { get; set; }
            public int Start { get; set; }
            public int Length { get; set; }
        }

        public struct SendNetMessage
        {
            public int MsgType { get; set; }
            public int BufferId { get; set; }
            public int RemoteClient { get; set; }
            public int IgnoreClient { get; set; }
            public string Text { get; set; }
            public int Number { get; set; }
            public float Number2 { get; set; }
            public float Number3 { get; set; }
            public float Number4 { get; set; }
            public int Number5 { get; set; }
        }

        public struct SignTextGet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int SignIndex { get; set; }
            public string Text { get; set; }
        }

        public struct SignTextSet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int SignIndex { get; set; }
            public string Text { get; set; }
#if Full_API
            public Sign OldSign { get; set; }
#endif
        }

        public struct StatusTextChange
        {
            public static readonly StatusTextChange Empty = new StatusTextChange();
        }

        public struct TileSquareReceived
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Size { get; set; }

            public byte[] ReadBuffer;
            public int Start;
            public int Applied;

            //            public void ForEach(object state, TileSquareForEachFunc func)
            //            {
            //                int num = start;
            //
            //                for (int x = X; x < X + Size; x++)
            //                {
            //                    for (int y = Y; y < Y + Size; y++)
            //                    {
            //                        TileData tile = Main.tile.At(x, y).Data;
            //
            //                        byte b9 = readBuffer[num++];
            //
            //                        bool wasActive = tile.Active;
            //
            //                        tile.Active = ((b9 & 1) == 1);
            //
            //                        if ((b9 & 2) == 2)
            //                        {
            //                            tile.Lighted = true;
            //                        }
            //
            //                        if (tile.Active)
            //                        {
            //                            int wasType = (int)tile.Type;
            //                            tile.Type = readBuffer[num++];
            //
            //                            if (tile.Type < Main.MAX_TILE_SETS && Main.tileFrameImportant[(int)tile.Type])
            //                            {
            //                                tile.FrameX = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                                tile.FrameY = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                            }
            //                            else if (!wasActive || (int)tile.Type != wasType)
            //                            {
            //                                tile.FrameX = -1;
            //                                tile.FrameY = -1;
            //                            }
            //                        }
            //
            //                        if ((b9 & 4) == 4)
            //                            tile.Wall = readBuffer[num++];
            //                        else
            //                            tile.Wall = 0;
            //
            //                        if ((b9 & 8) == 8)
            //                        {
            //                            tile.Liquid = readBuffer[num++];
            //                            byte b10 = readBuffer[num++];
            //                            tile.Lava = (b10 == 1);
            //                        }
            //                        else
            //                            tile.Liquid = 0;
            //
            //                        var result = func(x, y, ref tile, state);
            //                        if (result == TileSquareForEachResult.ACCEPT)
            //                        {
            //                            applied += 1;
            //                            Main.tile.At(x, y).SetData(tile);
            //                        }
            //                        else if (result == TileSquareForEachResult.BREAK)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
        }
#endif
    }

    public enum TileSquareForEachResult
    {
        ACCEPT,
        IGNORE,
        BREAK,
    }

    //    public delegate TileSquareForEachResult TileSquareForEachFunc(int x, int y, ref TileData tile, object state);

    public enum WorldEventType
    {
        BOSS,
        INVASION,
        SHADOW_ORB,
    }
}