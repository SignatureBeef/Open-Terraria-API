using System;
using System.IO;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace OTA.Mod.Projectile
{
    public abstract class OTAProjectile : INativeMod
    {
        #region Privates

        private int _typeId;
        private Terraria.Projectile _projectile;

        #endregion

        public Terraria.Projectile Projectile
        { 
            get { return _projectile; }
            internal set
            {
                _projectile = value;

                if (_projectile != null)
                {
                    if (_projectile.type != 0) this.TypeId = _projectile.type;
                    if (_projectile.type == 0) _projectile.type = this.TypeId;
                }
            }
        }

        public int TypeId
        { 
            get
            { 
                if (Projectile != null && Projectile.type != 0) return Projectile.type;
                return _typeId;
            } 
            set
            {
                if (Projectile != null) Projectile.type = value;
                _typeId = value;
            }
        }

        internal void Initialise()
        {
            OnSetDefaults();
        }

        #if CLIENT
        #region Textures

        /// <summary>
        /// Load a Projectile texture using an asset name for the current instance if it's not already been done.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(string assetName, bool force = false)
        {
            if (force || null == Main.projectileTexture[Projectile.type])
            {
                Main.projectileTexture[Projectile.type] = Main.instance.Content.Load<Texture2D>(assetName);
                Main.projectileLoaded[Projectile.type] = true;
            }
        }

        /// <summary>
        /// Loads an existing Projectile's texture using its Projectile Type Id.
        /// </summary>
        /// <param name="targetProjectileType">The target projectile type you wish to clone.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(int targetProjectileType, bool force = false)
        {
            LoadTexture(targetProjectileType, Projectile.type, force);
        }

        /// <summary>
        /// Loads an existing Projectile's texture using its Projectile Type Id to the mods'.
        /// </summary>
        /// <param name="targetProjectileType">The target projectile type you wish to clone.</param>
        /// <param name="modTypeId">The type id assigned to the mod.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public static void LoadTexture(int targetProjectileType, int modTypeId, bool force = false)
        {
            if (force || null == Main.projectileTexture[modTypeId])
            {
                Main.projectileTexture[modTypeId] = Main.instance.Content.Load<Texture2D>($"Images{Path.DirectorySeparatorChar}Projectile_{targetProjectileType}");
                Main.projectileLoaded[modTypeId] = true;
            }
        }

        /// <summary>
        /// Load the Item texture for the current instance if it's not already been done.
        /// </summary>
        /// <param name="texture">Texture to be assigned.</param>
        /// <param name="force">Ignore an existing loaded texture and load the one specified.</param>
        public void LoadTexture(Texture2D texture, bool force = false)
        {
            if (force || null == Main.itemTexture[Projectile.type])
            {
                Main.itemTexture[Projectile.type] = texture;
            }
        }

        #endregion
        #endif

        public virtual void OnSetDefaults()
        {
        }

        internal static void ResizeArrays()
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (ProjectileModRegister.MaxId + 1 >= Main.projectileTexture.Length)
            {
                var length = Main.projectileTexture.Length + BlockIssueSize;

                Array.Resize(ref Main.projectileTexture, length);

                //Resize ID sets
                foreach (var field in typeof(Terraria.ID.ProjectileID.Sets).GetFields())
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

                Logging.ProgramLog.Debug.Log($"Projectile slots changed to: {length}");
            }
        }
    }
}