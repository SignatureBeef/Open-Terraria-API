#if CLIENT
using System;
using System.IO;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace OTA.Client.Item
{
    public abstract class OTAItem : INativeMod
    {
        #region Privates

        private int _typeId;
        private Terraria.Item _item;

        #endregion

        public Terraria.Item Item
        { 
            get { return _item; }
            internal set
            {
                _item = value;

                if (_item != null)
                {
                    if (_item.type != 0) this.TypeId = _item.type;
                    if (_item.type == 0) _item.type = this.TypeId;
                }
            }
        }

        public int TypeId
        { 
            get
            { 
                if (Item != null && Item.type != 0) return Item.type;
                return _typeId;
            } 
            set
            {
                if (Item != null) Item.type = value;
                _typeId = value;
            }
        }

        internal void Initialise()
        {
            if (Main.netMode == 1 || Main.netMode == 2)
            {
                Item.owner = 255;
            }
            else
            {
                Item.owner = Main.myPlayer;
            }
            Item.ResetStats (this.TypeId);

            Item.netID = Item.type;
            Item.name = Lang.itemName (Item.netID, false);
            Item.CheckTip ();

            OnSetDefaults();
        }

        #region Textures

        /// <summary>
        /// Load a Item texture using an asset name for the current instance if it's not already been done.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(string assetName, bool force = false)
        {
            if (force || null == Main.itemTexture[Item.type])
            {
                Main.itemTexture[Item.type] = Main.instance.Content.Load<Texture2D>(assetName);
            }
        }

        /// <summary>
        /// Loads an existing Item's texture using its NPC Type Id.
        /// </summary>
        /// <param name="targetNPCType">The target Item type you wish to clone.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(int targetItemType, bool force = false)
        {
            LoadTexture(targetItemType, Item.type, force);
        }

        /// <summary>
        /// Loads an existing Item's texture using its NPC Type Id to the mods'.
        /// </summary>
        /// <param name="targetNPCType">The target Item type you wish to clone.</param>
        /// <param name="modTypeId">The type id assigned to the mod.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public static void LoadTexture(int targetItemType, int modTypeId, bool force = false)
        {
            if (force || null == Main.itemTexture[modTypeId])
            {
                Main.itemTexture[modTypeId] = Main.instance.Content.Load<Texture2D>($"Images{Path.DirectorySeparatorChar}Item_{targetItemType}");
                Main.itemAnimations[modTypeId] = Main.itemAnimations[targetItemType];
                Main.extraTexture[modTypeId] = Main.extraTexture[targetItemType];
                Main.itemAnimations[modTypeId] = Main.itemAnimations[targetItemType];
                Main.itemFlameTexture[modTypeId] = Main.itemFlameTexture[targetItemType];
                Main.itemName[modTypeId] = Main.itemName[targetItemType];
                Main.itemTexture[modTypeId] = Main.itemTexture[targetItemType];
                Main.extraTexture[modTypeId] = Main.extraTexture[targetItemType];
            }
        }

        /// <summary>
        /// Load the Item texture for the current instance if it's not already been done.
        /// </summary>
        /// <param name="texture">Texture to be assigned.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(Texture2D texture, bool force = false)
        {
            if (force || null == Main.itemTexture[Item.type])
            {
                Main.itemTexture[Item.type] = texture;
            }
        }

        #endregion

        public virtual void OnSetDefaults()
        {
        }

        internal static void ResizeArrays()
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (ItemModRegister.MaxItemId + 1 < Main.itemTexture.Length + BlockIssueSize)
            {
                var length = Main.itemTexture.Length + BlockIssueSize;

                Array.Resize(ref Main.itemAnimations, length);
                Array.Resize(ref Main.itemFlameLoaded, length);
                Array.Resize(ref Main.itemFlameTexture, length);
//                Array.Resize(ref Main.itemFrame, length);
//                Array.Resize(ref Main.itemFrameCounter, length);
//                Array.Resize(ref Main.itemLockoutTime, length);
                Array.Resize(ref Main.itemName, length);
//                Array.Resize(ref Main.itemText, length);
                Array.Resize(ref Main.itemTexture, length);
                Array.Resize(ref Main.extraTexture, length);

                //Resize ID sets
                foreach (var field in typeof(Terraria.ID.ItemID.Sets).GetFields())
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
    }
}
#endif