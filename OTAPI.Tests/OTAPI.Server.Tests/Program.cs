using Microsoft.Xna.Framework;
using OTAPI.Tests.Common;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Terraria;

namespace OTAPI.Tests
{
	class Program
	{

        public static void ForceLoadThread(object ThreadContext)
        {
            Program.ForceLoadAssembly(typeof(Terraria.Program).Assembly, true);
            Terraria.Program.LoadedEverything = true;
        }

        public static void ForceJITOnAssembly(Assembly assembly)
        {
            try
            {
                Type[] types = assembly.GetTypes();
                Type[] array = types;
                for (int i = 0; i < array.Length; i++)
                {
                    Type type = array[i];
                    MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    MethodInfo[] array2 = methods;
                    for (int j = 0; j < array2.Length; j++)
                    {
                        MethodInfo methodInfo = array2[j];
                        if (!methodInfo.IsAbstract && !methodInfo.ContainsGenericParameters && methodInfo.GetMethodBody() != null)
                        {
                            RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
                        }
                    }
                    Terraria.Program.ThingsLoaded++;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public static void ForceStaticInitializers(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            Type[] array = types;
            for (int i = 0; i < array.Length; i++)
            {
                Type type = array[i];
                if (!type.IsGenericType)
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }

        public static void ForceLoadAssembly(Assembly assembly, bool initializeStaticMembers)
        {
            Terraria.Program.ThingsToLoad = assembly.GetTypes().Length;
            Program.ForceJITOnAssembly(assembly);
            if (initializeStaticMembers)
            {
                Program.ForceStaticInitializers(assembly);
            }
        }

        public static void ForceLoadAssembly(string name, bool initializeStaticMembers)
        {
            Assembly assembly = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].GetName().Name.Equals(name))
                {
                    assembly = assemblies[i];
                    break;
                }
            }
            if (assembly == null)
            {
                assembly = Assembly.Load(name);
            }
            Program.ForceLoadAssembly(assembly, initializeStaticMembers);
        }
        static void Main(string[] args)
		{
			try
			{
                //Program.ForceLoadThread(null);

                var runner = new GameRunner();
				runner.PreStart += AttachHooks;
				runner.Main(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void AttachHooks(object sender, EventArgs args)
		{
			Hooks.Tile.CreateCollection = () =>
			{
				LogHook(nameof(Hooks.Tile.CreateCollection));
				return null;
			};
			#region Game Hooks
			OTAPI.Hooks.Game.PreUpdate = (ref GameTime gameTime) =>
			{
				LogHook(nameof(Hooks.Game.PreUpdate));
			};
			OTAPI.Hooks.Game.PostUpdate = (ref GameTime gameTime) =>
			{
				LogHook(nameof(Hooks.Game.PostUpdate));
			};
			OTAPI.Hooks.World.HardmodeTileUpdate = (int x, int y, ref ushort type) =>
			{
				LogHook(nameof(Hooks.World.HardmodeTileUpdate));
				return HardmodeTileUpdateResult.Continue;
			};
			OTAPI.Hooks.Game.PreInitialize = () =>
			{
				LogHook(nameof(Hooks.Game.PreInitialize));
			};
			OTAPI.Hooks.Game.Started = () =>
			{
				LogHook(nameof(Hooks.Game.Started));
			};
			OTAPI.Hooks.World.Statue = (StatueType caller, float x, float y, int type, ref int num, ref int num2, ref int num3) =>
			{
				LogHook(nameof(Hooks.World.Statue));
				return OTAPI.HookResult.Continue;
			};
			#endregion
			#region Item Hooks
			OTAPI.Hooks.Item.PreSetDefaultsById = (Item item, ref int type, ref bool noMatCheck) =>
			{
				LogHook(nameof(Hooks.Item.PreSetDefaultsById));
				return HookResult.Continue;
			};
			Hooks.Item.PreNetDefaults = (Item item, ref int type) =>
			{
				LogHook(nameof(Hooks.Item.PreNetDefaults));
				return HookResult.Continue;
			};
			Hooks.Chest.QuickStack = (int playerId, Item item, int chestIndex) =>
			{
				LogHook(nameof(Hooks.Chest.QuickStack));
				return HookResult.Continue;
			};
			#endregion
			#region Net Hooks
			Hooks.Net.SendData = (ref int bufferId, ref int msgType, ref int remoteClient, ref int ignoreClient, ref Terraria.Localization.NetworkText text,
				  ref int number, ref float number2, ref float number3, ref float number4, ref int number5, ref int number6,
				  ref int number7) =>
			{
				LogHook(nameof(Hooks.Net.SendData));
				return HookResult.Continue;
			};
			Hooks.Net.ReceiveData = (MessageBuffer buffer, ref byte packetId, ref int readOffset, ref int start, ref int length) =>
			{
				LogHook(nameof(Hooks.Net.ReceiveData));
				return HookResult.Continue;
			};
			Hooks.Player.PreGreet = (ref int playerId) =>
			{
				LogHook(nameof(Hooks.Player.PreGreet));
				return HookResult.Continue;
			};
			Hooks.Net.SendBytes = (ref int remoteClient, ref byte[] data, ref int offset, ref int size,
				  ref Terraria.Net.Sockets.SocketSendCallback callback, ref object state) =>
			{
				LogHook(nameof(Hooks.Net.SendBytes));
				return HookResult.Continue;
			};
			Hooks.Player.NameCollision = (Player player) =>
			{
				LogHook(nameof(Hooks));
				return HookResult.Continue;
			};
			#endregion
			#region Npc Hooks
			Hooks.Npc.PreSetDefaultsById = (NPC npc, ref int type, ref float scaleOverride) =>
			{
				LogHook(nameof(Hooks.Npc.PreSetDefaultsById));
				return HookResult.Continue;
			};
			Hooks.Npc.PreNetDefaults = (NPC npc, ref int type) =>
			{
				LogHook(nameof(Hooks.Npc.PreNetDefaults));
				return HookResult.Continue;
			};
			Hooks.Npc.Strike = (NPC npc, ref double cancelResult, ref int damage, ref float knockBack,
			   ref int hitDirection, ref bool critical, ref bool noEffect, ref bool fromNet, Entity entity) =>
			{
				LogHook(nameof(Hooks.Npc.Strike));
				return HookResult.Continue;
			};
			Hooks.Npc.PreTransform = (NPC npc, ref int newType) =>
			{
				LogHook(nameof(Hooks.Npc.PreTransform));
				return HookResult.Continue;
			};
			Hooks.Npc.Spawn = (ref int index) =>
			{
				LogHook(nameof(Hooks.Npc.Spawn));
				return HookResult.Continue;
			};
			Hooks.Npc.PreDropLoot = (
				 global::Terraria.NPC npc,
				 ref int itemId,
				 ref int x,
				 ref int y,
				 ref int width,
				 ref int height,
				 ref int type,
				 ref int stack,
				 ref bool noBroadcast,
				 ref int prefix,
				 ref bool noGrabDelay,
				 ref bool reverseLookup) =>
			{
				LogHook(nameof(Hooks.Npc.PreDropLoot));
				return HookResult.Continue;
			};

			Hooks.Npc.BossBagItem = (
				global::Terraria.NPC npc,
			ref int X,
			ref int Y,
			ref int Width,
			 ref int Height,
			 ref int Type,
			 ref int Stack,
			 ref bool noBroadcast,
			 ref int pfix,
			ref bool noGrabDelay,
			 ref bool reverseLookup) =>
			{
				LogHook(nameof(Hooks.Npc.BossBagItem));
				return HookResult.Continue;
			};
			Hooks.Npc.PreAI = (NPC npc) =>
			{
				LogHook(nameof(Hooks.Npc.PreAI));
				return HookResult.Continue;
			};
			#endregion
			Hooks.Collision.PressurePlate = (ref int x, ref int y, ref IEntity entity) =>
			{
				LogHook(nameof(Hooks.Collision.PressurePlate) + " @" + entity.GetType().Name);
				return HookResult.Continue;
			};
			#region Projectile Hooks
			Hooks.Projectile.PostSetDefaultsById = (Projectile projectile, int type) =>
			{
				LogHook(nameof(Hooks.Projectile.PostSetDefaultsById));
			};
			Hooks.Projectile.PreAI = (Projectile projectile) =>
			{
				LogHook(nameof(Hooks));
				return HookResult.Continue;
			};
			#endregion
			#region Server Hooks
			Hooks.Command.Process = (string lowered, string raw) =>
			{
				LogHook(nameof(Hooks.Command.Process));
				return HookResult.Continue;
			};
			Hooks.Net.RemoteClient.PreReset = (RemoteClient remoteClient) =>
			{
				LogHook(nameof(Hooks.Net.RemoteClient.PreReset));
				return HookResult.Continue;
			};
			#endregion
			#region World Hooks
			Hooks.World.IO.PreSaveWorld = (ref bool useCloudSaving, ref bool resetTime) =>
			{
				LogHook(nameof(Hooks.World.IO.PreSaveWorld));
				return HookResult.Continue;
			};
			Hooks.World.PreHardmode = () =>
			{
				LogHook(nameof(Hooks.World.PreHardmode));
				return HookResult.Continue;
			};
			Hooks.World.DropMeteor = (ref int x, ref int y) =>
			{
				LogHook(nameof(Hooks.World.DropMeteor));
				return HookResult.Continue;
			};
			Hooks.Game.Christmas = () =>
			{
				LogHook(nameof(Hooks.Game.Christmas));
				return HookResult.Continue;
			};
			Hooks.Game.Halloween = () =>
			{
				LogHook(nameof(Hooks.Game.Halloween));
				return HookResult.Continue;
			};
			#endregion
			#region Wiring Hooks
			Hooks.Wiring.AnnouncementBox = (int x, int y, int signId) =>
			{
				LogHook(nameof(Hooks.Wiring.AnnouncementBox));
				return HookResult.Continue;
			};
			#endregion
		}

		static System.Collections.Generic.List<string> _hooks = new System.Collections.Generic.List<string>();
		static void LogHook(string name)
		{
			lock (_hooks)
			{
				if (!_hooks.Contains(name))
				{
					_hooks.Add(name);
					Console.WriteLine($"[Hook] {name}");
				}
			}
		}
	}
}
