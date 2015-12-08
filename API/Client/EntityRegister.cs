using System;
using System.Collections.Concurrent;
using OTA.Client.Npc;
using System.Linq;
using System.Reflection;

namespace OTA.Client
{
    public static class EntityRegistrar
    {
        public static NpcModRegister Npcs { get; } = new NpcModRegister();

        internal static void ScanAssembly(Assembly asm)
        {
            DebugFramework.Assert.Expression(() => asm == null);

            Logging.ProgramLog.Debug.Log($"Scanning assemby {asm.FullName}");

            //Look for INativeMod
            var nm = typeof(NativeModAttribute);
            var npc = typeof(OTANpc);

            var mth = typeof(NpcModRegister).GetMethod("Register");

            if (null != asm.ExportedTypes)
                foreach (var nativeMod in asm.ExportedTypes.Where(x => Attribute.IsDefined(x, nm)))
                {
                    var attr = (NativeModAttribute)Attribute.GetCustomAttribute(nativeMod, nm);

                    Logging.ProgramLog.Debug.Log($"Flagged class {nativeMod.Name}");
                    if (npc.IsAssignableFrom(nativeMod))
                    {
                        Logging.ProgramLog.Debug.Log($"Detected custom NPC {nativeMod.Name}");
                        mth.MakeGenericMethod(nativeMod).Invoke(Npcs, new object[] { attr.EntityName });
                    }
                }
        }
    }
}