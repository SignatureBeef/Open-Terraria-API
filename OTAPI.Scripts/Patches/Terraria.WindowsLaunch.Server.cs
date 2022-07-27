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
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.Platforms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

/// <summary>
/// @doc Improves cross platform launch checks and add extra launch features
/// </summary>
namespace Terraria
{
    class patch_WindowsLaunch
    {
        public static extern bool orig_SetConsoleCtrlHandler(WindowsLaunch.HandlerRoutine handler, bool add);
        public static bool SetConsoleCtrlHandler(WindowsLaunch.HandlerRoutine handler, bool add)
        {
            if (ReLogic.OS.Platform.IsWindows)
            {
                return orig_SetConsoleCtrlHandler(handler, add);
            }
            return false;
        }

        /// <summary>
        /// Allows consumers to override the clrjit resolution (if applied)
        /// </summary>
        public static Func<ProcessModule, string>? OnResolveClrJit;

        public static string ResolveClrJitResolution(ProcessModule module)
        {
            var name = OnResolveClrJit?.Invoke(module);
            var res = name ?? module?.FileName ?? "clrjit";

            if (VerboseJitInfo)
            {
                Console.WriteLine("OnResolveClrJit: " + (name ?? "<null>"));
                Console.WriteLine("ProcessModule.FileName: " + (module?.FileName ?? "<null>"));
                Console.WriteLine("Returned value: " + res);
            }

            return res;
        }

        /// <summary>
        /// If the MonoMod detour runtime is not .net6 then try and load clrjit and reattach
        /// </summary>
        public static bool TryResolveClrJit { get; set; }

        /// <summary>
        /// Enables messages for jit failures
        /// </summary>
        public static bool VerboseJitInfo { get; set; }

        public static void TryFixMonoMod()
        {
            // MonoMod can fail to find clrjit if Process.Modules does not list it, however NativeLibrary can sometimes find it by name in this scenario
            // so this code will rewrite MonoMods discovery to fallback to clrjit by name, allowing it to resolve.
            if (DetourHelper.Runtime is not DetourRuntimeNET60Platform)
            {
                var dn = typeof(DetourRuntimeNETCorePlatform);
                var GetJitObject = dn.GetMethod("GetJitObject", BindingFlags.Static | BindingFlags.NonPublic);

                if (GetJitObject is not null)
                {
                    var ilh = new ILHook(GetJitObject, il =>
                    {
                        var firstEx = il.Instrs
                            .First(x => x.OpCode == OpCodes.Newobj && x.Operand is MethodReference mref && mref.DeclaringType.Name == "PlatformNotSupportedException")
                            .Previous
                            .Previous;

                        if (firstEx.OpCode != OpCodes.Ldloc_0)
                            throw new Exception("Method already patched");

                        if (VerboseJitInfo)
                            Console.WriteLine("IL location: " + firstEx.OpCode);

                        while (firstEx.Next.OpCode != OpCodes.Callvirt)
                        {
                            if (VerboseJitInfo)
                                Console.WriteLine("Removing IL: " + firstEx.Next.OpCode);

                            il.IL.Remove(firstEx.Next);
                        }

                        if (VerboseJitInfo)
                            Console.WriteLine("Updating IL: " + firstEx.Next.OpCode);

                        firstEx.Next.OpCode = OpCodes.Call;
                        firstEx.Next.Operand = typeof(WindowsLaunch).GetMethod(nameof(ResolveClrJitResolution));
                    });

                    DetourHelper.Runtime = DetourRuntimeNETCorePlatform.Create();
                }

                if (VerboseJitInfo && DetourHelper.Runtime is not DetourRuntimeNET60Platform)
                    Console.WriteLine("Failed to resolve clrjit, you might experience hook failures.");
            }
        }

        public static extern void orig_Main(string[] args);
        public static void Main(string[] args)
        {
            if (TryResolveClrJit)
                TryFixMonoMod();

            if (VerboseJitInfo)
                Console.WriteLine("MonoMod Runtime: " + (DetourHelper.Runtime.GetType().FullName ?? "<null>"));

            orig_Main(args);
        }
    }
}
