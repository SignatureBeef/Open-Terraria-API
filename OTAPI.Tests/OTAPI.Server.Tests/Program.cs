using OTAPI.Tests.Common;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OTAPI.Tests
{
	class Program
	{

		public static void StartForceLoad()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(Program.ForceLoadThread));
		}

		public static void ForceLoadThread(object ThreadContext)
		{
			Console.WriteLine("Jitting");
			Program.ForceLoadAssembly(Assembly.GetExecutingAssembly(), true);
			Program.ForceLoadAssembly(typeof(Terraria.Chest).Assembly, true);
			Console.WriteLine("JIT loaded, apparently");
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
					try
					{
						MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						MethodInfo[] array2 = methods;
						for (int j = 0; j < array2.Length; j++)
						{
							MethodInfo methodInfo = array2[j];
							try
							{
								if (!methodInfo.IsAbstract && !methodInfo.ContainsGenericParameters && methodInfo.GetMethodBody() != null)
								{
									RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
								}
							}
							catch (Exception ex2)
							{
								Console.WriteLine($"ex2:{type.FullName}.{methodInfo.Name},{ex2}");
							}
						}
					}
					catch (Exception ex1)
					{
						Console.WriteLine($"ex1:{type.FullName},{ex1}");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ex:{ex}");
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
			var runner = new GameRunner();
			runner.PreStart += AttachHooks;
			runner.Main(args);
		}

		static void AttachHooks(object sender, EventArgs args)
		{
			StartForceLoad();

			Hooks.Net.Socket.Create = () =>
			{
				return new OTAPI.Sockets.PoolSocket();
				//return new Terraria.Net.Sockets.TcpSocket();
			};
			//Hooks.Chest.QuickStack = (int playerId, Terraria.Item item, int chestIndex) =>
			//{
			//	Console.WriteLine($"playerId:{playerId},item.type:{item.type},chestIndex:{chestIndex}");
			//	return HookResult.Continue;
			//};
			//Hooks.Command.Process = (lowered, raw) =>
			//{
			//	Console.WriteLine($"Processing: `{raw}`");
			//	if (lowered == "test")
			//	{
			//		return HookResult.Cancel;
			//	}
			//	return HookResult.Continue;
			//};
			//Hooks.Command.StartCommandThread = () =>
			//{
			//	Console.WriteLine("[Hook] Command thread.");

			//	Console.WriteLine($"[Hook] Tile type at {Terraria.Main.tile[0, 0].GetType().FullName}/{Terraria.Main.tile[0, 0].GetType().GetInterfaces()[0].FullName}");
			//	return HookResult.Continue;
			//};
			//Hooks.Net.SendData =
			//(
			//	ref int bufferIndex,
			//	ref int msgType,
			//	ref int remoteClient,
			//	ref int ignoreClient,
			//	ref string text,
			//	ref int number,
			//	ref float number2,
			//	ref float number3,
			//	ref float number4,
			//	ref int number5,
			//	ref int number6,
			//	ref int number7
			//) =>
			//{
			//	//Console.WriteLine($"Sending {msgType} to {bufferIndex}");
			//	return HookResult.Continue;
			//};
			//Hooks.Net.ReceiveData =
			//(
			//	global::Terraria.MessageBuffer buffer,
			//	ref byte packetId,
			//	ref int readOffset,
			//	ref int start,
			//	ref int length,
			//	ref int messageType
			//) =>
			//{
			//	//Console.WriteLine($"Receiving {packetId} from {buffer.whoAmI}");
			//	return HookResult.Continue;
			//};
			//Hooks.Player.PreGreet = (ref int playerId) =>
			//{
			//	Console.WriteLine(nameof(Hooks.Player.PreGreet) + " " + playerId);
			//	return HookResult.Continue;
			//};
			//Hooks.Player.PostGreet = (int playerId) =>
			//{
			//	Console.WriteLine(nameof(Hooks.Player.PostGreet) + " " + playerId);
			//};
			//Hooks.Net.SendBytes = (
			//	ref int remoteClient,
			//	ref byte[] data,
			//	ref int offset,
			//	ref int size,
			//	ref global::Terraria.Net.Sockets.SocketSendCallback callback,
			//	ref object state) =>
			//{
			//	//Console.WriteLine($"Sending {size} bytes of data to {remoteClient}");
			//	return HookResult.Continue;
			//};
			//Hooks.Player.NameCollision = (player) =>
			//{
			//	Console.WriteLine($"Booting {player.name} as their name conflicts with an existing player.");
			//	return HookResult.Continue;
			//};
			//Hooks.Npc.Strike = (
			//	global::Terraria.NPC npc,
			//	ref int cancelResult,
			//	ref int Damage,
			//	ref float knockBack,
			//	ref int hitDirection,
			//	ref bool crit,
			//	ref bool noEffect,
			//	ref bool fromNet) =>
			//{
			//	cancelResult = 0;
			//	Console.WriteLine($"Hitting npc at {npc.whoAmI} with: {Damage},{knockBack},{hitDirection},{crit},{noEffect},{fromNet}");
			//	return HookResult.Continue;
			//};
			//Hooks.Npc.Create = (
			//	ref int index,
			//	ref int x,
			//	ref int y,
			//	ref int type,
			//	ref int start,
			//	ref float ai0,
			//	ref float ai1,
			//	ref float ai2,
			//	ref float ai3,
			//	ref int target) =>
			//{
			//	Console.WriteLine($"Creating npc for index {index} at {x},{y}");
			//	return null;
			//};
			//Hooks.Npc.PreDropLoot = (
			//	NPC npc,
			//	ref int itemId,
			//	ref int x,
			//	ref int y,
			//	ref int width,
			//	ref int height,
			//	ref int type,
			//	ref int stack,
			//	ref bool noBroadcast,
			//	ref int prefix,
			//	ref bool noGrabDelay,
			//	ref bool reverseLookup) =>
			//{
			//	Console.WriteLine($"[Pre] Dropping loot {type} at {x},{y}");
			//	return HookResult.Continue;
			//};
			//Hooks.Npc.PostDropLoot = (
			//	NPC npc,
			//	int x,
			//	int y,
			//	int width,
			//	int height,
			//	int type,
			//	int stack,
			//	bool noBroadcast,
			//	int prefix,
			//	bool noGrabDelay,
			//	bool reverseLookup) =>
			//{
			//	Console.WriteLine($"[Post] Dropping loot {type} at {x},{y}");
			//};
			//Hooks.Collision.PressurePlate = (
			//	ref int x,
			//	ref int y,
			//	ref global::Terraria.Entity entity) =>
			//{
			//	Console.WriteLine($"Pressure plate triggered at {x},{y} by {entity}");
			//	return HookResult.Continue;
			//};
			//Hooks.Npc.BossBagItem = (
			//	global::Terraria.NPC npc,
			//	ref int X,
			//	ref int Y,
			//	ref int Width,
			//	ref int Height,
			//	ref int Type,
			//	ref int Stack,
			//	ref bool noBroadcast,
			//	ref int pfix,
			//	ref bool noGrabDelay,
			//	ref bool reverseLookup) =>
			//{
			//	Console.WriteLine($"Drop boss bag at {X},{Y}");
			//	return HookResult.Continue;
			//};
			//Hooks.Projectile.PreSetDefaultsById = (
			//	global::Terraria.Projectile projectile,
			//	ref int type) =>
			//{
			//	//Console.WriteLine($"Creating new projectile using type: {type}");
			//	return HookResult.Continue;
			//};
			//Hooks.Projectile.PostSetDefaultsById = (
			//	global::Terraria.Projectile projectile,
			//	int type) =>
			//{
			//	//Console.WriteLine($"Created new projectile using type: {type}");
			//};
			//Hooks.Projectile.PreUpdate = (
			//	global::Terraria.Projectile projectile,
			//	ref int index) =>
			//{
			//	//Console.WriteLine($"Begin Update at index: {index}");
			//	return HookResult.Continue;
			//};
			//Hooks.Projectile.PostUpdate = (
			//	global::Terraria.Projectile projectile,
			//	int index) =>
			//{
			//	//Console.WriteLine($"End Update at index: {index}");
			//};
			//Hooks.World.IO.PreSaveWorld = (ref bool useCloudSaving, ref bool resetTime) =>
			//{
			//	Console.WriteLine($"Saving world useCloudSaving:{useCloudSaving}, resetTime:{resetTime}");
			//	return HookResult.Continue;
			//};
			//Hooks.World.IO.PostSaveWorld = (bool useCloudSaving, bool resetTime) =>
			//{
			//	Console.WriteLine($"Saved world useCloudSaving:{useCloudSaving}, resetTime:{resetTime}");
			//};
			//Hooks.World.PreHardmode = () =>
			//{
			//	Console.WriteLine($"Starting hardmode. Extra: netMode:{Terraria.Main.netMode},hardMode:{Terraria.Main.hardMode}");
			//	return HookResult.Continue;
			//};
			//Hooks.World.PostHardmode = () =>
			//{
			//	Console.WriteLine($"Hardmode is now in processed. Extra: netMode:{Terraria.Main.netMode},hardMode:{Terraria.Main.hardMode}");
			//};
			//Hooks.World.HardmodeTileUpdate = (int x, int y, ref ushort type) =>
			//{
			//	return HardmodeTileUpdateResult.Continue;
			//};
			//Hooks.Projectile.PreAI = (projectile) =>
			//{
			//	//Console.WriteLine($"[Prj] AI for {projectile.whoAmI}");
			//	return HookResult.Continue;
			//};
			//Hooks.Projectile.PostAI = (projectile) =>
			//{
			//	//Console.WriteLine($"[Prj] Post AI for {projectile.whoAmI}");
			//};
			//Hooks.World.Statue = (StatueType caller, float x, float y, int type, ref int num, ref int num2, ref int num3) =>
			//{
			//	Console.WriteLine($"Mech spawn for {caller} at {x},{y}");
			//	return HookResult.Continue;
			//};

			//Hooks.Game.StatusTextUpdate = () =>
			//{
			//	return HookResult.Continue;
			//};
			//Hooks.Game.StatusTextWrite = (ref string text) =>
			//{
			//	text = "[Hook] " + text;
			//	return HookResult.Continue;
			//};
			//Hooks.Net.RemoteClient.PreReset = (remoteClient) =>
			//{
			//	Console.WriteLine($"[Hook] Resetting client at {remoteClient.Id}");

			//	if (remoteClient.PendingTermination)
			//	{
			//		Console.WriteLine($"[Hook] {remoteClient.Name} disconnected at {remoteClient.Id}");
			//	}

			//	return HookResult.Continue;
			//};

			//Hooks.Net.RemoteClient.PostReset = (remoteClient) =>
			//{
			//	var socket = remoteClient.Socket == null ? "<null>" : "value";
			//	var connected = remoteClient.Socket != null && remoteClient.Socket.IsConnected();
			//	Console.WriteLine($"[Hook] Reset client at {remoteClient.Id}, socket: {socket}, connected: {connected}");
			//};

			//Hooks.Npc.Spawn = (ref int index) =>
			//{
			//	Console.WriteLine($"Spawning npc type {Terraria.Main.npc[index].type} at index {index}");
			//	return HookResult.Continue;
			//};
			//Hooks.Tile.CreateTile = () =>
			//{
			//	return new TestTile();
			//};
		}
	}

	//public class TestTile : Terraria.Tile
	//{
	//	public TestTile()
	//	{

	//	}

	//	public override void Initialise()
	//	{
	//		base.Initialise();
	//	}

	//	public override byte bTileHeader
	//	{
	//		get
	//		{
	//			return base.bTileHeader;
	//		}

	//		set
	//		{
	//			base.bTileHeader = value;
	//		}
	//	}

	//	public override int collisionType
	//	{
	//		get
	//		{
	//			return base.collisionType;
	//		}
	//	}

	//	public override bool active()
	//	{
	//		return base.active();
	//	}
	//}
}
