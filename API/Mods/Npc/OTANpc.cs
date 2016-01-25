using System;
using OTA.Plugin;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using OTA.Mod.Net;
using OTA.Extensions;

namespace OTA.Mod.Npc
{
    internal struct ServerTexture
    {
        public string FilePath { get; set; }

        public int FrameCount { get; set; }
    }

    public abstract class OTANpc : INativeMod
    {
        #region Privates

        private int _typeId;
        private Terraria.NPC _npc;

        //        private int _emulateNPCTypeId;

        #endregion

        public Terraria.NPC Npc
        {
            get { return _npc; }
            internal set
            {
                _npc = value;

                if (_npc != null)
                {
                    if (_npc.type != 0) this.TypeId = _npc.type;
                    if (_npc.type == 0) _npc.type = this.TypeId;
                }
            }
        }

        public int TypeId
        {
            get
            {
                if (Npc != null && Npc.type != 0) return Npc.type;
                return _typeId;
            }
            set
            {
                if (Npc != null) Npc.type = value;
                _typeId = value;
            }
        }

        internal void Initialise()
        {
            OnSetDefaults();
        }

        public OTANpc()
        {

        }

        public virtual bool OnAI()
        {
            return true;
        }

        public virtual void OnAfterAI()
        {
        }

        #if CLIENT
        public virtual bool OnDraw(bool behindTiles)
        {
            return true;
        }

        public virtual string OnChat()
        {
            return null;
        }

        public virtual string[] OnGetChatButtons()
        {
            return null;
        }

        public virtual bool OnChatButtonClick(OTA.Mod.Npc.NpcChatButton button)
        {
            return true;
        }

        public virtual void OnAfterDraw(bool behindTiles)
        {
        }
        #endif

        /// <summary>
        /// Determines how many chances the NPC has to spawn.
        /// </summary>
        /// <returns>
        ///     0 to never spawn
        ///     1 to have the same chance as a vanilla NPC
        ///     2 to have twice as many chances as a vanilla NPC
        ///     .5 to be half as common as a vanilla NPC
        /// </returns>
        public virtual double OnPreSpawn(HookArgs.NpcPreSpawn args)
        {
            return 0;
        }

        public virtual bool OnUpdate()
        {
//            if (_emulateNPCTypeId > 0)
//            {
//                try
//                {
//                    var t = Npc.type;
//                    Npc.type = _emulateNPCTypeId;
//                    Main.npc[Npc.whoAmI].UpdateNPCDirect(Npc.whoAmI);
//                    Npc.type = t;
//                }
//                catch (Exception e)
//                {
//                    Logging.ProgramLog.Log(e, $"Failed to update NPC {Npc.whoAmI}");
//                }
//                return false;
//            }
            return true;
        }

        public virtual void OnAfterUpdate()
        {
        }

        public virtual void OnSetDefaults()
        {
        }

        /// <summary>
        /// Called when your NPC is to load resources. This method willn not be called when your npc is active.
        /// </summary>
        public virtual void OnPrepareResources()
        {
        }

        public virtual bool OnDamage(double damage)
        {
            return true;
        }

        public virtual void OnDeath()
        {
        }

        #region Helpers

        public void EmulateNPC(int npcTypeId)
        {
            #if SERVER
            //Transition any old textures across
            if (NpcTextures.ContainsKey(this.TypeId) && !NpcTextures.ContainsKey(npcTypeId))
            {
                NpcTextures[npcTypeId] = NpcTextures[this.TypeId];
                //TODO: determine if the old ID's need removing. Mostly up to the implementation using this method...
                Logging.Logger.Debug("Moving old textures across");

                PacketRegister
                    .Write<SyncNpcTexture>(npcTypeId, NpcTextures[npcTypeId])
                    .Broadcast(x => x != null && x.IsOTAClient());
            }
            #endif

            this.TypeId = npcTypeId;

            #if CLIENT
            LoadTexture(npcTypeId);
            #endif

//            var initialType = Npc.type;
            Npc.SetDefaultsDirect(npcTypeId);
//            Npc.type = initialType;

            //Copy properties
            Main.npcCatchable[Npc.type] = Main.npcCatchable[npcTypeId];
            Main.npcFrameCount[Npc.type] = Main.npcFrameCount[npcTypeId];
            NPC.killCount[Npc.type] = NPC.killCount[npcTypeId];
            Main.npcName[Npc.type] = Main.npcName[npcTypeId];

            //Resize ID sets
            foreach (var field in typeof(Terraria.ID.NPCID.Sets).GetFields())
            {
                if (field.FieldType.IsArray)
                {
                    //Fetch the instance
                    var arr = field.GetValue(null) as Array;

                    //Set the custom npc to the vanilla npc value
                    arr.SetValue(arr.GetValue(npcTypeId), Npc.type);

                    //Reupdate the instance
                    field.SetValue(null, arr);
                }
            }
        }

        /// <summary>
        /// Sets the name of the npc.
        /// </summary>
        /// <param name="name">Name.</param>
        public void SetName(string name)
        {
            Npc.name = name;
            Npc.displayName = name;
            Main.npcName[TypeId] = name;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Mod.Npc.OTANpc"/> is catchable.
        /// </summary>
        /// <value><c>true</c> if catchable; otherwise, <c>false</c>.</value>
        public bool Catchable
        {
            get { return Main.npcCatchable[TypeId]; }
            set { Main.npcCatchable[TypeId] = value; }
        }

        /// <summary>
        /// Gets or sets the frame count.
        /// </summary>
        /// <value>The frame count.</value>
        public int FrameCount
        {
            get { return Main.npcFrameCount[TypeId]; }
            set { Main.npcFrameCount[TypeId] = value; }
        }

        /// <summary>
        /// Gets or sets the kill count.
        /// </summary>
        /// <value>The kill count.</value>
        public int KillCount
        {
            get { return NPC.killCount[TypeId]; }
            set { NPC.killCount[TypeId] = value; }
        }

        #endregion

        #region Textures

        #if CLIENT
        /// <summary>
        /// Load a NPC texture using an asset name for the current instance if it's not already been done.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(string assetName, bool force = false)
        {
            if (force || null == Main.npcTexture[TypeId])
            {
                Main.npcTexture[TypeId] = Main.instance.Content.Load<Texture2D>(assetName);
                Main.NPCLoaded[TypeId] = true;
            }
        }

        /// <summary>
        /// Loads an existing NPC's texture using its NPC Type Id.
        /// </summary>
        /// <param name="targetNPCType">The target NPC type you wish to clone.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(int targetNPCType, bool force = false)
        {
            if (force || null == Main.npcTexture[TypeId])
            {
                Main.npcTexture[TypeId] = Main.instance.Content.Load<Texture2D>($"Images{Path.DirectorySeparatorChar}NPC_{targetNPCType}");
                Main.NPCLoaded[TypeId] = true;
            }
        }

        //public void LoadTexture(string filePath, bool force = false)
        //{
        //    if (force || null == Main.npcTexture[TypeId])
        //    {
        //        filePath = Path.Combine("Content", filePath);
        //        if (!File.Exists(filePath) && File.Exists(filePath + ".png"))
        //        {
        //            filePath += ".png";
        //        }

        //        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        //        {
        //            Main.npcTexture[TypeId] = Texture2D.FromStream(Terraria.Main.graphics.GraphicsDevice, fileStream);
        //            Main.NPCLoaded[TypeId] = true;
        //        }
        //    }
        //}

        /// <summary>
        /// Load the NPC texture for the current instance if it's not already been done.
        /// </summary>
        /// <param name="texture">Texture to be assigned.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(Texture2D texture, bool force = false)
        {
            if (force || null == Main.npcTexture[TypeId])
            {
                Main.npcTexture[TypeId] = texture;
                Main.NPCLoaded[TypeId] = true;
            }
        }

#elif SERVER
        internal static readonly System.Collections.Concurrent.ConcurrentDictionary<Int32, ServerTexture> NpcTextures = new System.Collections.Concurrent.ConcurrentDictionary<Int32, ServerTexture>();

        public bool LoadTexture(string filePath, int frameCount)
        {
            Logging.Logger.Debug($"Adding npc texture {filePath}");
            if (!File.Exists(filePath)) throw new FileNotFoundException("Missing texture file", filePath);

            return NpcTextures.TryAdd(this.TypeId, new ServerTexture()
                {
                    FilePath = filePath,
                    FrameCount = frameCount
                });
        }
        #endif
        #endregion

        #region "Handlers"

        internal static void ResizeArrays()
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (NpcModRegister.MaxNpcId + 1 >= Main.npcTexture.Length)
            {
                var length = Main.npcTexture.Length + BlockIssueSize;

                Array.Resize(ref Main.npcCatchable, length);
                Array.Resize(ref Main.npcFrameCount, length);
                Array.Resize(ref NPC.killCount, length);
                Array.Resize(ref Main.NPCLoaded, length);
                Array.Resize(ref Main.npcName, length);
#if CLIENT
                Array.Resize(ref Main.npcTexture, length);
#endif

                Array.Resize(ref Terraria.GameContent.UI.EmoteBubble.CountNPCs, length);

                //Resize ID sets
                foreach (var field in typeof(Terraria.ID.NPCID.Sets).GetFields())
                {
                    if (field.FieldType.IsArray)
                    {
                        var arr = field.GetValue(null) as Array;
                        var t = field.FieldType.GetElementType();

                        var args = new object[] { arr, length };
                        typeof(Array).GetMethod("Resize").MakeGenericMethod(t).Invoke(null, args);

                        field.SetValue(null, args[0]);
                    }
                }
            }
        }

        #endregion
    }
}