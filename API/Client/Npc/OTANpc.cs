#if CLIENT
using System;
using OTA.Plugin;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace OTA.Client.Npc
{
    public abstract class OTANpc : Terraria.NPC, INativeMod
    {
        private int _emulateNPCTypeId;

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

        public virtual bool OnChatButtonClick(OTA.Callbacks.NpcChatButton button)
        {
            return true;
        }

        public virtual void OnAfterDraw(bool behindTiles)
        {
        }

        public virtual bool OnUpdate()
        {
            if (_emulateNPCTypeId > 0)
            {
                try
                {
                    var t = this.type;
                    this.type = _emulateNPCTypeId;
                    Main.npc[this.whoAmI].UpdateNPCDirect(this.whoAmI);
                    this.type = t;
                }
                catch (Exception e)
                {
                    Logging.ProgramLog.Log(e, $"Failed to update NPC {this.whoAmI}");
                }
                return false;
            }
            return true;
        }

        public virtual void OnAfterUpdate()
        {
        }

        public virtual void OnSetDefaults()
        {
        }

        public void StopEmulatingNPC()
        {
            _emulateNPCTypeId = 0;
        }

        public void EmulateNPC(int npcTypeId, bool onlySetInfo = false)
        {
            if (!onlySetInfo) _emulateNPCTypeId = npcTypeId;

            LoadTexture(npcTypeId);

            var initialType = this.type;
            base.SetDefaultsDirect(npcTypeId);
            this.type = initialType;

            //Copy properties
            Main.npcCatchable[this.type] = Main.npcCatchable[npcTypeId];
            Main.npcFrameCount[this.type] = Main.npcFrameCount[npcTypeId];
            NPC.killCount[this.type] = NPC.killCount[npcTypeId];
            Main.npcName[this.type] = Main.npcName[npcTypeId];

            //Resize ID sets
            foreach (var field in typeof(Terraria.ID.NPCID.Sets).GetFields())
            {
                if (field.FieldType.IsArray)
                {
                    //Fetch the instance
                    var arr = field.GetValue(null) as Array;

                    //Set the custom npc to the vanilla npc value
                    arr.SetValue(arr.GetValue(npcTypeId), this.type);

                    //Reupdate the instance
                    field.SetValue(null, arr);
                }
            }
        }

        #region Textures

        /// <summary>
        /// Load a NPC texture using an asset name for the current instance if it's not already been done.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(string assetName, bool force = false)
        {
            if (force || null == Main.npcTexture[this.type])
            {
                Main.npcTexture[this.type] = Main.instance.Content.Load<Texture2D>(assetName);
                Main.NPCLoaded[this.type] = true;
            }
        }

        /// <summary>
        /// Loads an existing NPC's texture using its NPC Type Id.
        /// </summary>
        /// <param name="targetNPCType">The target NPC type you wish to clone.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(int targetNPCType, bool force = false)
        {
            if (force || null == Main.npcTexture[this.type])
            {
                Main.npcTexture[this.type] = Main.instance.Content.Load<Texture2D>($"Images{Path.DirectorySeparatorChar}NPC_{targetNPCType}");
                Main.NPCLoaded[this.type] = true;
            }
        }

        /// <summary>
        /// Load the NPC texture for the current instance if it's not already been done.
        /// </summary>
        /// <param name="texture">Texture to be assigned.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(Texture2D texture, bool force = false)
        {
            if (force || null == Main.npcTexture[this.type])
            {
                Main.npcTexture[this.type] = texture;
                Main.NPCLoaded[this.type] = true;
            }
        }

        #endregion

        #region "Handlers"

        internal static void ResizeNPCArrays()
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (NpcModRegister.MaxNpcId + 1 < Main.npcTexture.Length + BlockIssueSize)
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
#endif