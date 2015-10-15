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
        public static readonly HookPoint<HookArgs.RemoteClientReset> RemoteClientReset;
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
        public static readonly HookPoint<HookArgs.SendClientData> SendClientData;
        public static readonly HookPoint<HookArgs.SendNetMessage> SendNetMessage;
        public static readonly HookPoint<HookArgs.SignTextGet> SignTextGet;
        public static readonly HookPoint<HookArgs.SignTextSet> SignTextSet;
        public static readonly HookPoint<HookArgs.StartHardMode> StartHardMode;
        /// <summary>
        /// The callback to update the console with Terraria.Main.statusText
        /// </summary>
        public static readonly HookPoint<HookArgs.StatusTextChange> StatusTextChange;
        public static readonly HookPoint<HookArgs.TileSquareReceived> TileSquareReceived;
        public static readonly HookPoint<HookArgs.WorldSave> WorldSave;
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
            RemoteClientReset = new HookPoint<HookArgs.RemoteClientReset>("remote-client-reset");
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
            SendClientData = new HookPoint<HookArgs.SendClientData>("send-client-data");
            SendNetMessage = new HookPoint<HookArgs.SendNetMessage>("send-net-message");
            SignTextGet = new HookPoint<HookArgs.SignTextGet>("sign-text-get");
            SignTextSet = new HookPoint<HookArgs.SignTextSet>("sign-text-set");
            StartHardMode = new HookPoint<HookArgs.StartHardMode>("start-hard-mode");
            StatusTextChange = new HookPoint<HookArgs.StatusTextChange>("status-text-changed");
            TileSquareReceived = new HookPoint<HookArgs.TileSquareReceived>("tile-square-received");
            WorldSave = new HookPoint<HookArgs.WorldSave>("world-save");
#endif
        }
    }

    public enum MethodState : byte
    {
        Begin,
        End
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