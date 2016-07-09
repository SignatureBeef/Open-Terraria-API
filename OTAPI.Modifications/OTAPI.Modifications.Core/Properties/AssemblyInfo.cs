using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("OTAPI.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OTAPI.Core")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0822b1ac-42f2-4fcf-b0ab-f5e59a30548f")]


/*
 * These are here as OTAPI's callbacks (from vanilla to otapi) are internals.
 * This is not an issue when you run the merged OTAPI.dll, but when you wish to debug something 
 * you need to run the OTAPI.Core.dll,OTAPI.Xna.dll & TerrariaServer.exe beside each other.
 * This is where the problem lies, so we must allow the terraria assemblies.
 */
[assembly: InternalsVisibleTo("Terraria")]
[assembly: InternalsVisibleTo("TerrariaServer")]