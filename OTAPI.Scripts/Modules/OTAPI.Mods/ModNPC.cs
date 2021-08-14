/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS0436 // Type conflicts with imported type

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content.Sources;

namespace OTAPI.Mods
{
    /// <summary>
    /// A class to create custom NPC's for Terraria. It should always be able to be called from .Net, lua or javascript.
    /// </summary>
    public class ModNPC : IMod
    {
        /// <summary>
        /// The vanilla NPC instance used by the game engine
        /// </summary>
        public NPC? NPC { get; set; }

        /// <summary>
        /// The name of your NPC mod
        /// </summary>
        public virtual Terraria.Localization.LocalizedText? Name { get; set; }

        /// <summary>
        /// The texture for your NPC
        /// </summary>
        public virtual string? Texture { get; set; }

        /// <summary>
        /// The type used by the vanilla game engine. This is managed by OTAPI but usually needed for mods if NPC.NewNPC is called manually.
        /// </summary>
        public int TypeID { get; private set; }

        /// <summary>
        /// Fired when your mod is registered with OTAPI. Not when it is being instanced.
        /// </summary>
        public event EventHandler? OnRegistered;

        /// <summary>
        /// Fired when your NPC is being created.
        /// </summary>
        public event EventHandler? OnCreated;

        ///// <summary>
        ///// Fired when your NPC is being created by the rego instance.
        ///// </summary>
        //public event EventHandler? OnInstanceCreated;

        /// <summary>
        /// Fired when your NPC values need to be configured.
        /// </summary>
        public event EventHandler? OnSetDefaults;

        public ModNPC? Registration { get; set; }

        //public class SetDefaultsArgs : EventArgs
        //{
        //    public HookResult Result { get; set; }
        //    public int Type { get; set; }
        //    public NPCSpawnParams SpawnParams { get; set; }
        //}

        ///// <summary>
        ///// Fired when your NPC is being configured, after being created.
        ///// </summary>
        //public event EventHandler<SetDefaultsArgs>? OnSetDefaults;

        static ModNPC()
        {
            //OTAPI.Hooks.NPC.Create += NPC_Create;
            OTAPI.Hooks.NPC.Spawn += NPC_Spawn;
            On.Terraria.NPC.SetDefaults += NPC_SetDefaults;
            On.Terraria.NPC.OnSetDefaultType += NPC_OnSetDefaultType;
            On.Terraria.Lang.GetNPCName += Lang_GetNPCName;
        }

        private static void NPC_OnSetDefaultType(On.Terraria.NPC.orig_OnSetDefaultType orig, NPC self)
        {
            if (self.EntityMod is ModNPC mod)
            {
                if (mod.Registration is not null)
                {
                    self.type = mod.Registration.TypeID;
                    self.netID = mod.Registration.TypeID;

                    mod.TypeID = mod.Registration.TypeID;
                    mod.Name = mod.Registration.Name;
                }

                ConfigureTextureAssets();

                mod.OnSetDefaults?.Invoke(self, EventArgs.Empty);
            }
        }

        private static void NPC_Spawn(object? sender, Hooks.NPC.SpawnEventArgs e)
        {
            var npc = Main.npc[e.Index];

            if (npc.EntityMod is ModNPC mod)
            {
            }
        }

        private static Terraria.Localization.LocalizedText Lang_GetNPCName(On.Terraria.Lang.orig_GetNPCName orig, int netID)
        {
            if (netID > Terraria.Main.maxNPCTypes)
            {
                var rego = EntityDiscovery.Instance
                    .GetTypeModRegistrations<ModNPC>()
                    .SingleOrDefault(m => m.TypeID == netID);
                if (rego is not null && rego.Name is not null)
                    return rego.Name;
            }
            return orig(netID);
        }

        //private static void NPC_Create(object? sender, Hooks.NPC.CreateEventArgs e)
        //{
        //    var rego = EntityDiscovery.Instance
        //        .GetTypeModRegistrations<ModNPC>()
        //        .SingleOrDefault(m => m.TypeID == e.Type);
        //    if (rego is not null)
        //    {
        //        var instance = (ModNPC)Activator.CreateInstance(rego.GetType())!;
        //        e.Npc = new NPC();
        //        e.Npc.EntityMod = instance;
        //        instance.Registration = rego;
        //        instance.NPC = e.Npc;

        //        e.Npc.type = rego.TypeID;
        //        e.Npc.netID = rego.TypeID;

        //        instance.TypeID = rego.TypeID;
        //        instance.Name = rego.Name;

        //        Console.WriteLine($"[OTAPI] Creating NPC instance: {e.Type} {instance.Name!.Key}");

        //        instance.OnCreated?.Invoke(rego, EventArgs.Empty);
        //    }
        //}

        private static void NPC_SetDefaults(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, NPCSpawnParams spawnparams)
        {
            var rego = EntityDiscovery.Instance
                .GetTypeModRegistrations<ModNPC>()
                .SingleOrDefault(m => m.TypeID == Type);
            if (rego is not null)
            {
                var instance = (ModNPC)Activator.CreateInstance(rego.GetType())!;
                //e.Npc = new NPC();
                self.EntityMod = instance;
                instance.Registration = rego;
                instance.NPC = self;

                //self.type = rego.TypeID;
                //self.netID = rego.TypeID;

                instance.TypeID = rego.TypeID;
                instance.Name = rego.Name;
                instance.Texture = rego.Texture;

                Console.WriteLine($"[OTAPI] Creating NPC instance: {Type} {instance.Name!.Key}");

                instance.OnCreated?.Invoke(rego, EventArgs.Empty);
                //rego.OnInstanceCreated?.Invoke(instance, EventArgs.Empty);
            }

            //if (self.EntityMod is ModNPC mod)
            //{
            //        var args = new SetDefaultsArgs()
            //        {
            //            Result = HookResult.Cancel, // assume the mod sets their own defaults, otherwise they can change the type and return continue. by default vanilla won't know what to do with the custom NPC type
            //            Type = Type,
            //            SpawnParams = spawnparams,
            //        };

            //        mod.OnSetDefaults?.Invoke(mod, args);

            //        if (args.Result == HookResult.Cancel)
            //            return;

            //        Type = args.Type;
            //        spawnparams = args.SpawnParams;
            //}

            orig(self, Type, spawnparams);
        }

        private void LoadTexture()
        {
            if (!String.IsNullOrWhiteSpace(Texture))
            {
                if (TextureAssets.Npc[TypeID]?.IsLoaded != true && Main.Assets != null)
                {
                    Console.WriteLine($"[{GetType().FullName}] Loading texture {Texture}");
                    //var path = System.IO.Path.Combine(Environment.CurrentDirectory, "modifications", "OTAPI.Mods", "Resources", Texture);
                    TextureAssets.Npc[TypeID] = Main.Assets.Request<Texture2D>(Texture);
                }
            }
        }

        public string GetName()
        {
            if (String.IsNullOrWhiteSpace(this.Name?.Key))
                throw new Exception($"{nameof(Name)} has not been configured");

            return this.Name.Key;
        }

        public void Registered()
        {
            // setup this type for instancing later on when the NPC needs to be spawned.
            TypeID = IModRegistry.AllocateType<ModNPC>(Terraria.Main.maxNPCTypes);

            var name = this.Name?.Key;

            if (String.IsNullOrWhiteSpace(name))
                throw new Exception($"Mod name not specified ({this.GetType().FullName})");

            if (MaxNpcId < TypeID)
                MaxNpcId = TypeID;

            ResizeArrays();

            On.Terraria.Main.LoadContent += Main_LoadContent;
            On.Terraria.Initializers.AssetInitializer.LoadTextures += AssetInitializer_LoadTextures;
            On.Terraria.ID.ContentSamples.Initialize += ContentSamples_Initialize;
            On.Terraria.ID.ContentSamples.CreateBestiarySortingIds += ContentSamples_CreateBestiarySortingIds;

            Console.WriteLine($"[OTAPI] Assigned TypeID {TypeID} to {name}");

            OnRegistered?.Invoke(this, EventArgs.Empty);
        }

        private void ContentSamples_CreateBestiarySortingIds(On.Terraria.ID.ContentSamples.orig_CreateBestiarySortingIds orig, Terraria.GameContent.Bestiary.BestiaryDatabase database)
        {
            orig(database);
        }

        private void ContentSamples_Initialize(On.Terraria.ID.ContentSamples.orig_Initialize orig)
        {
            orig();

            ConfigureTextureAssets();

            //string name = NPCID.Search.GetName(nPC.netID);
            var name = this.Name?.Key;

            var nPC = new NPC();
            nPC.SetDefaults(TypeID);
            ContentSamples.NpcsByNetId[TypeID] = nPC;
            ContentSamples.NpcPersistentIdsByNetIds[TypeID] = name;
            ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[TypeID] = name;
            ContentSamples.NpcNetIdsByPersistentIds[name] = TypeID;
            ContentSamples.NpcBestiaryRarityStars[TypeID] = ContentSamples.GetNPCBestiaryRarityStarsCount(nPC);
            //ContentSamples.NpcBestiarySortingId[TypeID] = ContentSamples.NpcBestiarySortingId.Values.OrderByDescending(x => x).First() + 1;

            //ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[TypeID] = name;
            //ContentSamples.NpcsByNetId[TypeID] = ContentSamples.NpcsByNetId[mod.Registration.EmulateNpcID.Value];
            //ContentSamples.NpcPersistentIdsByNetIds[TypeID] = ContentSamples.NpcPersistentIdsByNetIds[mod.Registration.EmulateNpcID.Value];
            //ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[TypeID] = NpcBestiaryCreditIdsByNpcNetIds[mod.Registration.EmulateNpcID.Value];
        }

        private void AssetInitializer_LoadTextures(On.Terraria.Initializers.AssetInitializer.orig_LoadTextures orig, ReLogic.Content.AssetRequestMode mode)
        {
            orig(mode);

            //ConfigureTextureAssets();
            LoadTexture();
        }

        static int GetMaxNpcID() => EntityDiscovery.Instance
                    .GetTypeModRegistrations<ModNPC>()
                    .OrderByDescending(x => x.TypeID)
                    .First()
                    .TypeID;

        static void ConfigureTextureAssets()
        {
            var length = GetMaxNpcID() + 1; // +1 because of 0 based index, and terraria doesnt offset
            if (TextureAssets.Npc.Length < length)
                Array.Resize(ref TextureAssets.Npc, length);
        }

        private void Main_LoadContent(On.Terraria.Main.orig_LoadContent orig, Main self)
        {
            // wait and allow Terraria.Main.Assets to be assigned
            orig(self);

            // assign a new content source to the plugins resources folder
            var modName = this.GetType().Assembly.GetName().Name.Replace("CSharpScript_", "");
            if (!String.IsNullOrWhiteSpace(modName) && !ModSources.TryGetValue(modName, out var source))
            {
                var resources = Path.Combine("modifications", modName, "Resources");
                if (Directory.Exists(resources))
                {
                    Console.WriteLine($"[{this.Name.Key}] Using resources {resources}");
                    source = new FileSystemContentSource(resources);

                    var assets = (ReLogic.Content.AssetRepository)Terraria.Main.Assets;

                    assets._sources = assets._sources.Union(new[] { source });
                    //Terraria.Main.AssetSourceController._staticSources.Add(source);
                    ModSources[modName] = source;
                }
                else Console.WriteLine($"[{this.Name.Key}] No resources found at {resources ?? "<null>"}");
            }
            else Console.WriteLine($"[{this.Name.Key}] No resources found for assembly {modName}");
        }

        Dictionary<string, IContentSource> ModSources = new Dictionary<string, IContentSource>();

        internal static int MaxNpcId { get; set; }

        internal static void ResizeArrays()
        {
            var length = GetMaxNpcID() + 1; // +1 because of 0 based index, and terraria doesnt offset
            if (Terraria.GameContent.UI.EmoteBubble.CountNPCs.Length < length)
            {
                Array.Resize(ref Main.npcCatchable, length);
                Array.Resize(ref Main.npcFrameCount, length);
                Array.Resize(ref NPC.killCount, length);
                Array.Resize(ref Main.townNPCCanSpawn, length);
                Array.Resize(ref Main.slimeRainNPC, length);
                Array.Resize(ref Terraria.GameContent.UI.EmoteBubble.CountNPCs, length);
                //Array.Resize(ref Terraria.GameContent.TextureAssets.Npc, length);

                foreach (var field in typeof(NPCID.Sets).GetFields())
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

                Console.WriteLine($"[OTAPI] Npc block size set to {length}");
            }
        }
    }
}
