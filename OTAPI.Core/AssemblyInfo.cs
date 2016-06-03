using System.Runtime.CompilerServices;

/*
 * These are here as OTAPI's callbacks (from vanilla to otapi) are internals.
 * This is not an issue when you run the merged OTAPI.dll, but when you wish to debug something 
 * you need to run the OTAPI.Core.dll,OTAPI.Xna.dll & TerrariaServer.exe beside each other.
 * This is where the problem lies, so we must allow the terraria assemblies.
 */

[assembly: InternalsVisibleTo("Terraria")]
[assembly: InternalsVisibleTo("TerrariaServer")]