using System;
using Terraria;

namespace OTAPI.Core.Tests
{
	class Program
	{
		static class Config
		{
			public static bool AutoStart { get; set; }
		}

		static void Main(string[] args)
		{
			try
			{
				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Clear();

				Console.WriteLine("OTAPI Test Launcher.");

				var options = new NDesk.Options.OptionSet()
						.Add("as:|auto-start:", x => Config.AutoStart = true);
				options.Parse(args);

				AttachHooks();

				if (Config.AutoStart)
					StartServer(args);
				else Menu(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey(true);
			}
		}

		static void StartServer(string[] args)
		{
			Console.WriteLine("Starting...");
			Terraria.WindowsLaunch.Main(args);
		}

		static void Menu(string[] args)
		{
			Console.WriteLine("Main menu:");

			var offset = 0;

			var wait = true;

			var startX = Console.CursorLeft;
			var startY = Console.CursorTop;

			var menus = new[]
			{
				new
				{
					Text = "Start server",
					Execute = new Action(() =>
					{
						StartServer(args);
						wait =false;
					})
				},
				new
				{
					Text = "Exit",
					Execute = new Action(() => { wait=false; })
				}
			};

			for (var x = 0; x < menus.Length; x++)
			{
				Console.Write(x == offset ? "\t> " : "\t  ");
				Console.WriteLine(menus[x].Text);
			}

			while (wait)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.DownArrow)
				{
					offset++;

					if (offset >= menus.Length) offset = 0;
				}
				else if (key.Key == ConsoleKey.UpArrow)
				{
					offset--;

					if (offset < 0) offset = menus.Length - 1;
				}
				else if (key.Key == ConsoleKey.Enter)
				{
					menus[offset].Execute();
				}

				Console.CursorLeft = startX;
				Console.CursorTop = startY;
				for (var x = 0; x < menus.Length; x++)
				{
					Console.Write(x == offset ? "\t> " : "\t  ");
					Console.WriteLine(menus[x].Text);
				}
			}
		}

		static void AttachHooks()
		{
			Hooks.Net.Socket.Create = () =>
			{
				return new OTAPI.Sockets.PoolSocket();
				//return new Terraria.Net.Sockets.TcpSocket();
			};
			Hooks.Command.Process = (lowered, raw) =>
			{
				Console.WriteLine($"Processing: `{raw}`");
				if (lowered == "test")
				{
					return HookResult.Cancel;
				}
				return HookResult.Continue;
			};
			Hooks.Command.StartCommandThread = () =>
			{
				Console.WriteLine("[Hook] Command thread.");

				Console.WriteLine($"[Hook] Tile type at {Terraria.Main.tile[0, 0].GetType().FullName}/{Terraria.Main.tile[0, 0].GetType().GetInterfaces()[0].FullName}");
				return HookResult.Continue;
			};
			Hooks.Net.SendData =
			(
				ref int bufferIndex,
				ref int msgType,
				ref int remoteClient,
				ref int ignoreClient,
				ref string text,
				ref int number,
				ref float number2,
				ref float number3,
				ref float number4,
				ref int number5,
				ref int number6,
				ref int number7
			) =>
			{
				//Console.WriteLine($"Sending {msgType} to {bufferIndex}");
				return HookResult.Continue;
			};
			Hooks.Net.ReceiveData =
			(
				global::Terraria.MessageBuffer buffer,
				ref byte packetId,
				ref int readOffset,
				ref int start,
				ref int length,
				ref int messageType
			) =>
			{
				//Console.WriteLine($"Receiving {packetId} from {buffer.whoAmI}");
				return HookResult.Continue;
			};
			Hooks.Player.PreGreet = (ref int playerId) =>
			{
				Console.WriteLine(nameof(Hooks.Player.PreGreet) + " " + playerId);
				return HookResult.Continue;
			};
			Hooks.Player.PostGreet = (int playerId) =>
			{
				Console.WriteLine(nameof(Hooks.Player.PostGreet) + " " + playerId);
			};
			Hooks.Net.SendBytes = (
				ref int remoteClient,
				ref byte[] data,
				ref int offset,
				ref int size,
				ref global::Terraria.Net.Sockets.SocketSendCallback callback,
				ref object state) =>
			{
				//Console.WriteLine($"Sending {size} bytes of data to {remoteClient}");
				return HookResult.Continue;
			};
			Hooks.Player.NameCollision = (player) =>
			{
				Console.WriteLine($"Booting {player.name} as their name conflicts with an existing player.");
				return HookResult.Continue;
			};
			Hooks.Npc.Strike = (
				global::Terraria.NPC npc,
				ref int cancelResult,
				ref int Damage,
				ref float knockBack,
				ref int hitDirection,
				ref bool crit,
				ref bool noEffect,
				ref bool fromNet) =>
			{
				cancelResult = 0;
				Console.WriteLine($"Hitting npc at {npc.whoAmI} with: {Damage},{knockBack},{hitDirection},{crit},{noEffect},{fromNet}");
				return HookResult.Continue;
			};
			Hooks.Npc.Create = (
				ref int index,
				ref int x,
				ref int y,
				ref int type,
				ref int start,
				ref float ai0,
				ref float ai1,
				ref float ai2,
				ref float ai3,
				ref int target) =>
			{
				Console.WriteLine($"Creating npc for index {index} at {x},{y}");
				return null;
			};
			Hooks.Npc.PreDropLoot = (
				NPC npc,
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
				Console.WriteLine($"[Pre] Dropping loot {type} at {x},{y}");
				return HookResult.Continue;
			};
			Hooks.Npc.PostDropLoot = (
				NPC npc,
				int x,
				int y,
				int width,
				int height,
				int type,
				int stack,
				bool noBroadcast,
				int prefix,
				bool noGrabDelay,
				bool reverseLookup) =>
			{
				Console.WriteLine($"[Post] Dropping loot {type} at {x},{y}");
			};
			Hooks.Collision.PressurePlate = (
				ref int x,
				ref int y,
				ref global::Terraria.Entity entity) =>
			{
				Console.WriteLine($"Pressure plate triggered at {x},{y} by {entity}");
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
				Console.WriteLine($"Drop boss bag at {X},{Y}");
				return HookResult.Continue;
			};
			Hooks.Projectile.PreSetDefaultsById = (
				global::Terraria.Projectile projectile,
				ref int type) =>
			{
				//Console.WriteLine($"Creating new projectile using type: {type}");
				return HookResult.Continue;
			};
			Hooks.Projectile.PostSetDefaultsById = (
				global::Terraria.Projectile projectile,
				int type) =>
			{
				//Console.WriteLine($"Created new projectile using type: {type}");
			};
			Hooks.Projectile.PreUpdate = (
				global::Terraria.Projectile projectile,
				ref int index) =>
			{
				//Console.WriteLine($"Begin Update at index: {index}");
				return HookResult.Continue;
			};
			Hooks.Projectile.PostUpdate = (
				global::Terraria.Projectile projectile,
				int index) =>
			{
				//Console.WriteLine($"End Update at index: {index}");
			};
			Hooks.World.IO.PreSaveWorld = (ref bool useCloudSaving, ref bool resetTime) =>
			{
				Console.WriteLine($"Saving world useCloudSaving:{useCloudSaving}, resetTime:{resetTime}");
				return HookResult.Continue;
			};
			Hooks.World.IO.PostSaveWorld = (bool useCloudSaving, bool resetTime) =>
			{
				Console.WriteLine($"Saved world useCloudSaving:{useCloudSaving}, resetTime:{resetTime}");
			};
			Hooks.World.PreHardmode = () =>
			{
				Console.WriteLine($"Starting hardmode. Extra: netMode:{Terraria.Main.netMode},hardMode:{Terraria.Main.hardMode}");
				return HookResult.Continue;
			};
			Hooks.World.PostHardmode = () =>
			{
				Console.WriteLine($"Hardmode is now in processed. Extra: netMode:{Terraria.Main.netMode},hardMode:{Terraria.Main.hardMode}");
			};
			Hooks.World.HardmodeTileUpdate = (int x, int y, ref ushort type) =>
			{
				return HardmodeTileUpdateResult.Continue;
			};
			Hooks.Projectile.PreAI = (projectile) =>
			{
				//Console.WriteLine($"[Prj] AI for {projectile.whoAmI}");
				return HookResult.Continue;
			};
			Hooks.Projectile.PostAI = (projectile) =>
			{
				//Console.WriteLine($"[Prj] Post AI for {projectile.whoAmI}");
			};
			Hooks.World.Statue = (StatueType caller, float x, float y, int type, ref int num, ref int num2, ref int num3) =>
			{
				Console.WriteLine($"Mech spawn for {caller} at {x},{y}");
				return HookResult.Continue;
			};

			Hooks.Game.StatusTextUpdate = () =>
			{
				return HookResult.Continue;
			};
			Hooks.Game.StatusTextWrite = (ref string text) =>
			{
				text = "[Hook] " + text;
				return HookResult.Continue;
			};
			Hooks.Net.RemoteClient.PreReset = (remoteClient) =>
			{
				Console.WriteLine($"[Hook] Resetting client at {remoteClient.Id}");

				if (remoteClient.PendingTermination)
				{
					Console.WriteLine($"[Hook] {remoteClient.Name} disconnected at {remoteClient.Id}");
				}

				return HookResult.Continue;
			};

			Hooks.Net.RemoteClient.PostReset = (remoteClient) =>
			{
				var socket = remoteClient.Socket == null ? "<null>" : "value";
				var connected = remoteClient.Socket != null && remoteClient.Socket.IsConnected();
				Console.WriteLine($"[Hook] Reset client at {remoteClient.Id}, socket: {socket}, connected: {connected}");
			};

			Hooks.Npc.Spawn = (ref int index) =>
			{
				Console.WriteLine($"Spawning npc type {Terraria.Main.npc[index].type} at index {index}");
				return HookResult.Continue;
			};
			Hooks.Tile.CreateTile = () =>
			{
				return new TestTile();
			};
		}
	}

	public class TestTile : Terraria.Tile
	{
		public TestTile()
		{

		}

		public override void Initialise()
		{
			base.Initialise();
		}

		public override byte bTileHeader
		{
			get
			{
				return base.bTileHeader;
			}

			set
			{
				base.bTileHeader = value;
			}
		}

		public override int collisionType
		{
			get
			{
				return base.collisionType;
			}
		}

		public override bool active()
		{
			return base.active();
		}
	}
}
