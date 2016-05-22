//For debugging, When MemTile is used as a vanilla style it doesnt have properties, it has fields
//#define MEM_TILE_IS_VANILLA

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace OTA.Patcher
{
    /// <summary>
    /// Old collection of hooks, yet to be transitioned into Hooks.cs
    /// </summary>
    public partial class Injector : IDisposable
    {
        /// <summary>
        /// The current Terraria assembly
        /// </summary>
        private AssemblyDefinition _asm;

        /// <summary>
        /// The OTA assembly
        /// </summary>
        private AssemblyDefinition _self;

        /// <summary>
        /// Gets the terraria assembly.
        /// </summary>
        /// <value>The terraria assembly.</value>
        public AssemblyDefinition TerrariaAssembly
        {
            get
            { return _asm; }
        }

        /// <summary>
        /// Gets the API assembly.
        /// </summary>
        /// <value>The API assembly.</value>
        public AssemblyDefinition APIAssembly
        {
            get
            { return _self; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OTA.Patcher.Injector"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="patchFile">Patch file.</param>
        public Injector(string filePath, string patchFile)
        {
            Initalise(filePath, patchFile);
        }

        /// <summary>
        /// Initalise with the target files.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="patchFile">Patch file.</param>
        private void Initalise(string filePath, string patchFile)
        {
            //Load the Terraria assembly
            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(filePath))
                {
                    var buff = new byte[256];
                    while (fs.Position < fs.Length)
                    {
                        var task = fs.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, task);
                    }
                }

                ms.Seek(0L, SeekOrigin.Begin);
                _asm = AssemblyDefinition.ReadAssembly(ms);
            }
            //Load the assembly to patch to
            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(patchFile))
                {
                    var buff = new byte[256];
                    while (fs.Position < fs.Length)
                    {
                        var task = fs.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, task);
                    }
                    fs.Close();
                }

                ms.Seek(0L, SeekOrigin.Begin);
                _self = AssemblyDefinition.ReadAssembly(ms);
            }

            InitOrganisers();
        }

        /// <summary>
        /// Checks to see if the source (Terraria) binary is a supported version
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public string GetAssemblyVersion()
        {
            return _asm.CustomAttributes
                .Single(x => x.AttributeType.Name == "AssemblyFileVersionAttribute")
                .ConstructorArguments
                .First()
                .Value as string;
        }

        /// <summary>
        /// Switches the .NET framework
        /// </summary>
        /// <remarks>This is not indepth</remarks>
        /// <param name="version">Version.</param>
        public void SwitchFramework(string version)
        {
            //            _asm.MainModule.RuntimeVersion = "";

            for (var x = 0; x < _asm.CustomAttributes.Count; x++)
            {
                if (_asm.CustomAttributes[x].AttributeType.Name == "TargetFrameworkAttribute")
                {
                    _asm.CustomAttributes[x].ConstructorArguments[0] = new CustomAttributeArgument(_asm.MainModule.Import(typeof(String)), ".NETFramework,Version=v" + version);

                    var cs = new CustomAttributeArgument(_asm.MainModule.Import(typeof(String)), ".NET Framework " + version);
                    _asm.CustomAttributes[x].Properties[0] = new CustomAttributeNamedArgument("FrameworkDisplayName", cs);
                }
            }
        }

        #region "Memory"

        /// <summary>
        /// Swaps a Tile array type to an array of the replacement type
        /// </summary>
        /// <returns>The to vanilla reference.</returns>
        /// <param name="input">Input.</param>
        /// <param name="replacement">Replacement.</param>
        private TypeReference SwapToVanillaReference(TypeReference input, TypeReference replacement)
        {
            if (input.FullName == "Terraria.Tile")
            {
                return replacement;
            }
            else if (input is ArrayType)
            {
                var at = input as ArrayType;
                if (at.ElementType.FullName == "Terraria.Tile")
                {
                    var nt = new ArrayType(replacement);
                    nt.Dimensions.Clear();
                    //foreach (var dm in at.Dimensions.Reverse().ToArray())
                    //{
                    //    //nt.Dimensions.Add(dm);
                    //    nt.Dimensions.Insert(0, dm);
                    //}
                    foreach (var dm in at.Dimensions)
                    {
                        nt.Dimensions.Add(dm);
                    }

                    return _asm.MainModule.Import(nt);
                }
            }

            return input;
        }

        /// <summary>
        /// Finds the tile array.
        /// </summary>
        /// <returns>The tile array.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="mth">Mth.</param>
        private int FindTileArray(int x, MethodDefinition mth) //No used at this stage
        {
            if (x < 0 || x >= mth.Body.Instructions.Count)
                return -1;
            if (
                mth.Body.Instructions[x].OpCode == OpCodes.Ldsfld
                && mth.Body.Instructions[x].Operand is FieldDefinition
                && ((mth.Body.Instructions[x].Operand as FieldDefinition).FieldType is ArrayType))
            {

                var et = ((mth.Body.Instructions[x].Operand as FieldDefinition).FieldType as ArrayType).ElementType;
                if (et.Name == "MemTile" || et.Name == "Tile")
                {
                    return x;
                }
            }
            else if (
                mth.Body.Instructions[x].OpCode == OpCodes.Call
                && mth.Body.Instructions[x].Operand is MethodReference
                && ((mth.Body.Instructions[x].Operand as MethodReference).ReturnType is ArrayType))
            {
                var et = ((mth.Body.Instructions[x].Operand as MethodReference).ReturnType as ArrayType).ElementType;
                if (et.Name == "MemTile" || et.Name == "Tile")
                {
                    return x;
                }
            }

            return -1;
        }

        /// <summary>
        /// Swaps Terraria.Tile to OTA.Memory.MemTile
        /// </summary>
        /// <param name="ty">Ty.</param>
        private void SwapVanillaType(TypeDefinition ty)
        {
            var memTile = _self.MainModule.Types.Single(x => x.Name == "MemTile");
            var vt = _asm.MainModule.Import(memTile);

            //            MethodReference setCall = null;
            //#if !MEM_TILE_IS_VANILLA
            //            var memSetCall = memTile.Methods.SingleOrDefault(x => x.Name == "SetTile");
            //            if (memSetCall != null) setCall = _asm.MainModule.Import(memSetCall);
            //#endif
            //            var emptyTile = _asm.MainModule.Import(memTile.Fields.Single(x => x.Name == "Empty"));
            //var vt = _asm.MainModule.Import(_self.MainModule.Types.Single(x => x.Name == "VanillaTile"));

            var tileSet = _self.MainModule.Types.Single(x => x.Name == "TileCollection");
            var setCall = _asm.MainModule.Import(tileSet.Methods.SingleOrDefault(x => x.Name == "SetTile"));
            var getCall = _asm.MainModule.Import(tileSet.Methods.SingleOrDefault(x => x.Name == "GetTile"));

            if (ty.Name != "Tile")
            {
                if (ty.HasFields)
                    foreach (var fld in ty.Fields)
                    {
                        fld.FieldType = SwapToVanillaReference(fld.FieldType, vt);
                    }

                if (ty.HasProperties)
                    foreach (var prop in ty.Properties)
                    {
                        prop.PropertyType = SwapToVanillaReference(prop.PropertyType, vt);
                    }

                foreach (var mth in ty.Methods)
                {
                    //if (ty.Name != "Liquid" || mth.Name != "DelWater") continue;
                    if (mth.HasParameters)
                    {
                        foreach (var prm in mth.Parameters)
                        {
                            prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt);
                        }
                    }

                    if (mth.HasBody)
                    {
                        if (mth.Body.HasVariables)
                        {
                            foreach (var vrb in mth.Body.Variables)
                            {
                                vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt);
                            }
                        }

                        if (mth.Body.Instructions != null)
                        {
                            //foreach (var ins in mth.Body.Instructions)
                            for (var i = mth.Body.Instructions.Count - 1; i > 0; i--)
                            {
                                var ins = mth.Body.Instructions[i];


                                //                                if (mth.Name == "clearWorld")
                                {
                                    //                                    var sss = "";
                                    if (ins.OpCode == OpCodes.Call && ins.Operand is MemberReference)
                                    {
                                        var mr = ins.Operand as MemberReference;

                                        //if (setCall != null && mr.Name == "Set" && mr.DeclaringType is ArrayType)
                                        //{
                                        //    var asd = "";
                                        //}

                                        if (setCall != null && mr.Name == "Set" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.Name == "MemTile")
                                        {
                                            //Swap
                                            //                                            var il = mth.Body.GetILProcessor();

                                            //                                            if (ins.Previous != null && ins.Previous.OpCode == OpCodes.Ldnull)
                                            //                                            {
                                            //                                                il.Replace(ins.Previous, il.Create(OpCodes.Ldsfld, emptyTile));
                                            //                                            }

                                            //Remove previous instructions to remove the array instance

                                            //bool remove = true;
                                            //var iii = ins.Previous;
                                            //while (true)
                                            //{
                                            //    //Currently the LiquidRender tiles are not a TileCollection
                                            //    if ((iii.Operand is FieldReference
                                            //        && (iii.Operand as FieldReference).Name == "_tiles" ||
                                            //        (iii.Operand is MethodReference && (iii.Operand as MethodReference).Name == "get__tiles")))
                                            //    {
                                            //        remove = false;
                                            //        break;
                                            //    }

                                            //    if ((iii.Operand is FieldReference
                                            //        && (iii.Operand as FieldReference).Name == "tile"))
                                            //    {
                                            //        il.Remove(iii);
                                            //        i--;
                                            //        break;
                                            //    }
                                            //    else
                                            //    {
                                            //        //                                                    il.Remove(ins.Previous);
                                            //        iii = iii.Previous;
                                            //        //                                                    i--;
                                            //    }
                                            //}

                                            ins.Operand = _asm.MainModule.Import(setCall);
                                            //if (remove) il.Replace(ins, il.Create(OpCodes.Call, setCall));
                                        }

                                        if (getCall != null && mr.Name == "Get" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.Name == "MemTile")
                                        {
                                            //Swap
                                            //                                            var il = mth.Body.GetILProcessor();
                                            //Remove previous instructions to remove the array instance

                                            //bool remove = true;
                                            //var iii = ins.Previous;
                                            //while (true)
                                            //{
                                            //    if ((iii.Operand is FieldReference
                                            //        && (iii.Operand as FieldReference).Name == "_tiles") ||
                                            //        (iii.Operand is MethodReference && (iii.Operand as MethodReference).Name == "get__tiles"))
                                            //    {
                                            //        remove = false;
                                            //        break;
                                            //    }

                                            //    if (
                                            //        (iii.Operand is FieldReference
                                            //        && (iii.Operand as FieldReference).Name == "tile"))
                                            //    {
                                            //        //il.Remove(iii);
                                            //        i--;
                                            //        break;
                                            //    }
                                            //    else
                                            //    {
                                            //        //                                                    il.Remove(ins.Previous);
                                            //        iii = iii.Previous;
                                            //        //                                                    i--;
                                            //    }
                                            //}

                                            ins.Operand = _asm.MainModule.Import(getCall);
                                            //if (remove)
                                            //    il.Replace(iii, il.Create(OpCodes.Call, getCall));
                                        }
                                    }
                                }


                                if (ins.Operand is MethodReference)
                                {
                                    var meth = ins.Operand as MethodReference;
                                    if (meth.DeclaringType.Name == "Tile")
                                    {
                                        if (meth.Name == ".ctor")
                                        {
                                            //Find "ldsfld class Terraria.Tile[0..., 0...] Terraria.Main::tile"
                                            //Copy instructions afterwards/beforehand until "ins"

                                            /*const int MaxRange = 10; //Searching is probably not required. But I'm tired and this will do for now.

                                            int top = -1, bottom = -1;

                                            for (var x = i - 1; x > (i - MaxRange) - 1; x--)
                                            {
                                                top = FindTileArray(x, mth);
                                                if (top > 0)
                                                    break;
                                            }

                                            for (var x = i + 1; x < (i + MaxRange) + 1; x++)
                                            {
                                                bottom = FindTileArray(x, mth);
                                                if (bottom > 0)
                                                    break;
                                            }

                                            var topDiffToCurrent = top == -1 ? -1 : i - top;
                                            var btmDiffToCurrent = bottom == -1 ? -1 : bottom - i;

                                            int index = -1;
                                            if (topDiffToCurrent > 0 && btmDiffToCurrent > 0)
                                            {
                                                index = topDiffToCurrent < btmDiffToCurrent ? topDiffToCurrent : btmDiffToCurrent;
                                            }
                                            else if (topDiffToCurrent > 0)
                                            {
                                                index = topDiffToCurrent;
                                            }
                                            else if (btmDiffToCurrent > 0)
                                            {
                                                index = btmDiffToCurrent;
                                            }

                                            if (index != -1)
                                            {
                                                var total = i - index;
                                                if (total < 0) total *= -1;

                                                var c = 0;
                                            }
                                            else
                                            {
                                                if (ty.Name == "LiquidRenderer" && mth.Name == ".cctor")
                                                    continue;



                                                throw new InvalidDataException(String.Format("Failed to grab MemTile constructor in method {0}, instruction offset {1}", mth.FullName, i));
                                            }*/


                                            //                                            //Determine what initialisation methods are used.
                                            //
                                            //                                            //Assigning to an existing MemTile
                                            //                                            if (ins.OpCode == OpCodes.Newobj && ins.Next.OpCode == OpCodes.Stloc_S)
                                            //                                            {
                                            ////                                                var vbl = mth.Body.Variables[(int)ins.Next.Operand];
                                            //                                                if (ins.Next.Operand is VariableReference)
                                            //                                                {
                                            //                                                    var vr = ins.Next.Operand as VariableReference;
                                            //                                                    if (vr.VariableType.Name == "MemTile")
                                            //                                                    {
                                            //                                                        var asd = "";
                                            //                                                    }
                                            //                                                    else
                                            //                                                    {
                                            //                                                        var asd = "";
                                            //                                                    }
                                            //                                                }
                                            //                                                else
                                            //                                                {
                                            //                                                    var asd = "";
                                            //                                                }
                                            //                                            }


                                            //INSTEAD OF UPDATING CONSTRUCTORS, REPLACE Main.tile.Set(int, int, tile)! Why did it not do this earlier


                                            //                                            if (twoFrom > 0)
                                            //                                            {
                                            //                                                var first = mth.Body.Instructions[twoFrom];
                                            //                                                var second = first.Next;
                                            //
                                            //                                                var il = mth.Body.GetILProcessor();
                                            //                                                il.InsertBefore(ins, first);
                                            //                                                il.InsertBefore(ins, second);
                                            //
                                            //                                                i -= 2;
                                            //
                                            //                                                ins.Operand = _asm.MainModule.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == 2 && x.Parameters[0].ParameterType.Name == "Int32"));
                                            //                                                continue;
                                            //                                            }

                                            ins.Operand = _asm.MainModule.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
                                            continue;
                                        }

                                        ins.Operand = _asm.MainModule.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
                                        continue;
                                    }
                                    else if (meth.DeclaringType is ArrayType)
                                    {
                                        var at = meth.DeclaringType as ArrayType;
                                        if (at.ElementType.Name == "Tile")
                                        {
                                            meth.DeclaringType = SwapToVanillaReference(meth.DeclaringType, vt);
                                            //ins.Operand = _asm.MainModule.Import(tp.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));

                                        }
                                    }

                                    if (meth.HasParameters)
                                        foreach (var prm in meth.Parameters)
                                        {
                                            prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt);
                                        }

                                    meth.ReturnType = SwapToVanillaReference(meth.ReturnType, vt);
                                    meth.MethodReturnType.ReturnType = SwapToVanillaReference(meth.MethodReturnType.ReturnType, vt);
                                }
                                else if (ins.Operand is TypeReference)
                                {
                                    var typ = ins.Operand as TypeReference;
                                    if (typ.Name == "Tile")
                                    {

                                    }
                                    else if (typ is ArrayType)
                                    {
                                        var at = typ as ArrayType;
                                        if (at.ElementType.Name == "Tile")
                                        {

                                        }
                                    }
                                }
                                else if (ins.Operand is FieldReference)
                                {
                                    var fld = ins.Operand as FieldReference;
                                    if (fld.DeclaringType.Name == "Tile")
                                    {

                                        //Vanilla 
                                        //ins.Operand = _asm.MainModule.Import(vt.Resolve().Fields.Single(x => x.Name == fld.Name));

                                        //Now, instead map to our property methods

                                        var il = mth.Body.GetILProcessor();
                                        if (ins.OpCode == OpCodes.Ldfld)
                                        {
#if MEM_TILE_IS_VANILLA
                                            //Get
                                            var prop = _asm.MainModule.Import(vt.Resolve().Fields.Single(x => x.Name == fld.Name));
                                            ins.Operand = prop;
#else
                                            //Get
                                            var prop = _asm.MainModule.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).GetMethod);

                                            il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
#endif
                                        }
                                        else if (ins.OpCode == OpCodes.Stfld)
                                        {
#if MEM_TILE_IS_VANILLA
                                            //Set
                                            var prop = _asm.MainModule.Import(vt.Resolve().Fields.Single(x => x.Name == fld.Name));
                                            ins.Operand = prop;
#else
                                            //Set
                                            var prop = _asm.MainModule.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).SetMethod);

                                            il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
#endif
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else if (fld.DeclaringType is ArrayType)
                                    {
                                        var at = fld.DeclaringType as ArrayType;
                                        if (at.ElementType.Name == "Tile")
                                        {

                                        }
                                    }
                                }
                                else if (ins.Operand is PropertyReference)
                                {
                                    //var pro = ins.Operand as PropertyReference;
                                    //var np = new PropertyReference( vt.Resolve().Properties.Single(x => x.Name == pro.Name));
                                    //ins.Operand = _asm.MainModule.Import();
                                }
                                else if (ins.Operand is VariableReference)
                                {
                                    var vrb = ins.Operand as VariableReference;
                                    vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt);
                                }
                                else if (ins.Operand is MemberReference)
                                {
                                    //                                    var mem = ins.Operand as MemberReference;
                                    //if (mem..Name == "Tile")
                                    //{
                                    //    vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt);
                                    //}
                                }
                            }
                        }
                    }
                }
            }

            if (ty.HasNestedTypes)
                foreach (var nt in ty.NestedTypes)
                    SwapVanillaType(nt);
        }

        //        /// <summary>
        //        /// Swaps Terraria.Tile to OTA.Memory.MemTile
        //        /// </summary>
        //        public void SwapToVanillaTile()
        //        {
        //            foreach (var ty in _asm.MainModule.Types)
        //            {
        //                SwapVanillaType(ty);
        //            }

        //            //Swap the constructor
        //            var mainCctor = Terraria.Main.Methods.Single(x => x.Name == ".cctor");
        //            var constructor = _asm.MainModule.Import(_self.MainModule.Types.Single(x => x.Name == "TileCollection").Methods.Single(y => y.Name == ".ctor"));

        ////            var il = mainCctor.Body.GetILProcessor();
        //            var ins = mainCctor.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
        //                          && x.Operand is MethodReference
        //                          && (x.Operand as MethodReference).Name == ".ctor"
        //                          && (x.Operand as MethodReference).DeclaringType is ArrayType
        //                          && ((x.Operand as MethodReference).DeclaringType as ArrayType).ElementType.Name == "MemTile");
        //            ins.Operand = constructor;
        //        }

        #endregion

        //public void InjectTileSet()
        //{
        //    var ts = _self.MainModule.Types.Single(x => x.Name == "TileCollection");
        //    var tm = _asm.MainModule.Types.Single(x => x.Name == "Main").Fields.Single(x => x.Name == "tile");

        //    tm.FieldType = _asm.MainModule.Import(ts);
        //}

        /// <summary>
        /// Applies the patch so OTA can select what server states are valid
        /// </summary>
        public void HookValidPacketState()
        {
            var getData = Terraria.MessageBuffer.Methods.Single(x => x.Name == "GetData");
            var callback = _asm.MainModule.Import(API.MessageBufferCallback.Methods.Single(x => x.Name == "CheckForInvalidState"));

            var netMode = getData.Body.Instructions.Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldReference
                              && (x.Operand as FieldReference).Name == "netMode").ToArray()[2];

            var il = getData.Body.GetILProcessor();
            Instruction call;
            il.InsertAfter(netMode, (call = il.Create(OpCodes.Call, callback)));
            var target = il.Create(OpCodes.Brfalse_S, call.Next.Next.Operand as Instruction);

            il.Remove(netMode);

            var i = 30;
            while (i-- > 0)
            {
                il.Remove(call.Next);
            }

            il.InsertAfter(call, target);

            //Parameters
            var whoAmI = Terraria.MessageBuffer.Fields.Single(x => x.Name == "whoAmI");

            il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(call, il.Create(OpCodes.Ldfld, whoAmI));
            il.InsertBefore(call, il.Create(OpCodes.Ldloc_0));
        }

        /// <summary>
        /// Patches how Terraria handles removing files. At one stage this was incompatible with mono
        /// </summary>
        public void PathFileIO()
        {
            var erase = Terraria.Main.Methods.Single(x => x.Name == "EraseWorld");
            var repl = _asm.MainModule.Import(API.Utilities.Methods.Single(x => x.Name == "RemoveFile"));

            var il = erase.Body.GetILProcessor();
            foreach (var item in erase.Body.Instructions.Where(x =>
                x.OpCode == OpCodes.Call
                && x.Operand is MethodReference
                && (x.Operand as MethodReference).Name == "MoveToRecycleBin").ToArray())
            {
                il.Replace(item, il.Create(OpCodes.Call, repl));
            }
        }

        /// <summary>
        /// Hooks to the end of Terraria.Main.DedServ
        /// </summary>
        public void HookDedServEnd()
        {
            var method = Terraria.Main.Methods.Single(x => x.Name == "DedServ");
            var replacement = API.MainCallback.Methods.Single(x => x.Name == "OnProgramFinished" && x.IsStatic);

            var imported = _asm.MainModule.Import(replacement);
            var il = method.Body.GetILProcessor();

            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, imported));
        }

        /// <summary>
        /// Hooks Terraria.Main.LoadDedConfig to always use OTA.Callbacks.Configuration.Load
        /// </summary>
        public void HookConfig()
        {
            var main = Terraria.Main.Methods.Single(x => x.Name == "LoadDedConfig" && !x.IsStatic);
            var replacement = API.Configuration.Methods.Single(x => x.Name == "Load" && x.IsStatic);
            var first = main.Body.Instructions.First();

            var il = main.Body.GetILProcessor();
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(replacement)));
            il.InsertBefore(first, il.Create(OpCodes.Ret));

            //Grab all occurances of "LoadDedConfig" and route it to ours
            //            var toBeReplaced = main.Body.Instructions
            //                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt
            //                                   && x.Operand is MethodReference
            //                                   && (x.Operand as MethodReference).Name == "LoadDedConfig"
            //                               )
            //                .ToArray();

            //            for (var x = 0; x < toBeReplaced.Length; x++)
            //            {
            //                toBeReplaced[x].OpCode = OpCodes.Call;
            //                toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
            //            }
            //            var il = main.Body.GetILProcessor();
            //            for (var x = toBeReplaced.Length - 1; x > -1; x--)
            //            {
            //                il.Remove(toBeReplaced[x].Previous.Previous.Previous.Previous);
            //            }
        }

        /// <summary>
        /// Allows raining to be accessible.
        /// </summary>
        [Obsolete("This is now replaced with the make all types public function")]
        public void EnableRaining()
        {
            var mth = Terraria.Main.Methods.Single(x => x.Name == "StartRain" && x.IsStatic);
            mth.IsPrivate = false;
            mth.IsPublic = true;
            mth = Terraria.Main.Methods.Single(x => x.Name == "StopRain" && x.IsStatic);
            mth.IsPrivate = false;
            mth.IsPublic = true;
        }

        /// <summary>
        /// This was mean to add the update Terraria.Main.statusText so the saving of the world would clear its line
        /// </summary>
        public void FixStatusTexts()
        {
            var main = Terraria.WorldFile.Methods.Single(x => x.Name == "saveWorld" && x.IsStatic && x.Parameters.Count == 2);

            var il = main.Body.GetILProcessor();
            var statusText = Terraria.Main.Fields.Single(x => x.Name == "statusText");

            var ins = main.Body.Instructions.Where(x => x.OpCode == OpCodes.Leave_S).Last();

            il.InsertBefore(ins, il.Create(OpCodes.Ldstr, ""));
            il.InsertBefore(ins, il.Create(OpCodes.Stsfld, statusText));
        }

        ///// <summary>
        ///// Meant to inject OTA's loadWorld in order for world file debugging
        ///// </summary>
        //public void HookWorldFile_DEBUG()
        //{

        //    //            //Make public
        //    //            var fld = Terraria.WorldGen.Fields.Single(x => x.Name == "lastMaxTilesX");
        //    //            fld.IsPrivate = false;
        //    //            fld.IsFamily = false;
        //    //            fld.IsPublic = true;
        //    //
        //    //            fld = Terraria.WorldGen.Fields.Single(x => x.Name == "lastMaxTilesY");
        //    //            fld.IsPrivate = false;
        //    //            fld.IsFamily = false;
        //    //            fld.IsPublic = true;
        //    //            return;

        //    var mth = Terraria.WorldGen.Methods.Single(x => x.Name == "serverLoadWorldCallBack" && x.IsStatic);
        //    var replacement = API.WorldFileCallback.Methods.Single(x => x.Name == "loadWorld" && x.IsStatic);

        //    var toBeReplaced = mth.Body.Instructions
        //        .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
        //                           && x.Operand is MethodReference
        //                           && (x.Operand as MethodReference).Name == "loadWorld"
        //                       )
        //        .ToArray();

        //    for (var x = 0; x < toBeReplaced.Length; x++)
        //    {
        //        toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
        //    }
        //}

        /// <summary>
        /// Call OTA instead to print Terraria.Main.statusText to the console
        /// </summary>
        public void HookStatusText()
        {
            var dedServ = Terraria.Main.Methods.Single(x => x.Name == "DedServ");
            var callback = API.MainCallback.Methods.Single(x => x.Name == "OnStatusTextChange");

            var startInstructions = dedServ.Body.Instructions
                    .Where(x => x.OpCode == OpCodes.Ldsfld
                                        && x.Operand is FieldReference
                                        && (x.Operand as FieldReference).Name == "statusText"
                                        && x.Next.OpCode == OpCodes.Call
                                        && x.Next.Operand is MethodReference
                                        && (x.Next.Operand as MethodReference).Name == "WriteLine")
                    .Reverse() //Remove desc
                    .ToArray();

            var il = dedServ.Body.GetILProcessor();
            var insCallback = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
            foreach (var ins in startInstructions)
            {
                il.InsertBefore(ins.Previous.Previous, insCallback);

                il.Remove(ins.Previous);
                il.Remove(ins.Previous);

                il.Remove(ins.Next);
                il.Remove(ins);
            }

            return;
            //            var startInstructions = dedServ.Body.Instructions
            //                .Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "oldStatusText")
            //                .Reverse() //Remove desc
            //                .ToArray();
            //
            //            var il = dedServ.Body.GetILProcessor();
            //            foreach (var ins in startInstructions)
            //            {
            //                var end = ins.Operand as Instruction;
            //                var ix = il.Body.Instructions.IndexOf(ins);
            //
            //                var inLoop = il.Body.Instructions[ix].Previous.OpCode == OpCodes.Br_S;
            //
            //                while (!(il.Body.Instructions[ix].OpCode == OpCodes.Call && il.Body.Instructions[ix].Operand is MethodReference && ((MethodReference)il.Body.Instructions[ix].Operand).Name == "WriteLine"))
            //                {
            //                    il.Remove(il.Body.Instructions[ix]);
            //                }
            //                il.Remove(il.Body.Instructions[ix]); //Remove the Console.WriteLine
            //
            //                var insCallback = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
            //                il.InsertBefore(il.Body.Instructions[ix], insCallback);
            //
            //                //Fix the loop back to the start
            //                if (inLoop && il.Body.Instructions[ix + 2].OpCode == OpCodes.Brfalse_S)
            //                {
            //                    il.Body.Instructions[ix + 2].Operand = insCallback;
            //                }
            //            }
        }

        /// <summary>
        /// Hooks NetMessage event into OTA
        /// </summary>
        public void HookNetMessage()
        {
            var method = Terraria.NetMessage.Methods.Single(x => x.Name == "SendData");
            var callback = API.NetMessageCallback.Methods.First(m => m.Name == "SendData");

            var il = method.Body.GetILProcessor();

            var ret = il.Create(OpCodes.Ret);
            var call = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
            var first = method.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Nop));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_3));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[4]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[5]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[6]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[7]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[8]));
            il.InsertBefore(first, call);
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, ret);
        }

        //        /// <summary>
        //        /// Hooks the console title.
        //        /// </summary> TODO: this might actually be needed to avoid using the XNA shims
        //        public void HookConsoleTitle()
        //        {
        //            var method = Terraria.Main.Methods.Single(x => x.Name == "DedServ");
        //            var callback = API.GameWindow.Methods.First(m => m.Name == "SetTitle");
        //
        ////            var il = method.Body.GetILProcessor();
        //
        //            var replacement = _asm.MainModule.Import(callback);
        //            foreach (var ins in method.Body.Instructions
        //                .Where(x => x.OpCode == OpCodes.Call
        //                    && x.Operand is MethodReference
        //                    && (x.Operand as MethodReference).DeclaringType.FullName == "System.Console"
        //                    && (x.Operand as MethodReference).Name == "set_Title"))
        //            {
        //                ins.Operand = replacement;
        //            }
        //        }

        /// <summary>
        /// Hooks start of the application for plugins to be loaded
        /// </summary>
        /// <param name="mode">Mode.</param>
        public void HookProgramStart(SupportType mode)
        {
            if (mode == SupportType.Server)
            {
                var method = Terraria.WindowsLaunch.Methods.Single(x => x.Name == "Main");
                var callback = API.MainCallback.Methods.First(m => m.Name == "OnProgramStarted");

                var il = method.Body.GetILProcessor();

                var ret = il.Create(OpCodes.Ret);
                var call = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
                var first = method.Body.Instructions.First();

                il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
                il.InsertBefore(first, call);
                il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
                il.InsertBefore(first, ret);
            }
            else if (mode == SupportType.Client)
            {
                var method = Terraria.Program.Methods.Single(x => x.Name == "LaunchGame");
                var callback = API.MainCallback.Methods.First(m => m.Name == "OnProgramStarted");

                var il = method.Body.GetILProcessor();

                var ret = il.Create(OpCodes.Ret);
                var call = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
                var first = method.Body.Instructions.First();

                il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
                il.InsertBefore(first, call);
                il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
                il.InsertBefore(first, ret);
            }
        }

        /// <summary>
        /// Removes the console handler to prevent crashing on mono
        /// </summary>
        /// <remarks>>Server only</remarks>
        public void RemoveConsoleHandler()
        {
            var method = Terraria.WindowsLaunch.Methods.Single(x => x.Name == "Main");

            var il = method.Body.GetILProcessor();
            var target = il.Body.Instructions.Single(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "SetConsoleCtrlHandler");

            il.Remove(target.Previous);
            il.Remove(target);
        }

        //        public void RemoveProcess()
        //        {
        //            var method = Terraria.ProgramServer.Methods.Single(x => x.Name == "InnerStart");
        //
        //            var il = method.Body.GetILProcessor();
        //            var target = il.Body.Instructions.Single(x => x.OpCode == OpCodes.Callvirt
        //                             && x.Operand is MethodReference
        //                             && (x.Operand as MethodReference).Name == "set_PriorityClass");
        //
        //            il.Remove(target.Previous.Previous.Previous.Previous);
        //            il.Remove(target.Previous.Previous.Previous);
        //            il.Remove(target.Previous.Previous);
        //            il.Remove(target.Previous);
        //            il.Remove(target);
        //        }

        ///// <summary>
        ///// Hooks to the end of Terraria.Main.UpdateServer so we can call extra functionalities in OTA/plugins 
        ///// </summary>
        //public void HookUpdateServer()
        //{
        //    var method = Terraria.Main.Methods.Single(x => x.Name == "UpdateServer");
        //    var callback = API.MainCallback.Methods.First(m => m.Name == "UpdateServerEnd");

        //    var il = method.Body.GetILProcessor();
        //    il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        //}

        //        /// <summary>
        //        /// Hooks into the XNA Game.Initialize, so plugins can utilise
        //        /// </summary>
        //        public void HookInitialise()
        //        {
        //            var method = Terraria.Main.Methods.Single(x => x.Name == "Initialize");
        //            var callback = API.MainCallback.Methods.First(m => m.Name == "Initialise");
        //
        //            var il = method.Body.GetILProcessor();
        //            var first = method.Body.Instructions.First();
        //
        //            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        //        }

        /// <summary>
        /// Hooks into Terraria.Netplay.Initialize so we can fire the server starting event
        /// </summary>
        public void HookNetplayInitialise()
        {
            var method = Terraria.Netplay.Methods.Single(x => x.Name == "Initialize");
            var callback = API.NetplayCallback.Methods.First(m => m.Name == "Initialise");

            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));

            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        /// <summary>
        /// Hooks OTA world events into the vanilla code
        /// </summary>
        public void HookWorldEvents()
        {
            var method = Terraria.WorldGen.Methods.Single(x => x.Name == "generateWorld");
            var callbackBegin = API.MainCallback.Methods.First(m => m.Name == "WorldGenerateBegin");
            var callbackEnd = API.MainCallback.Methods.First(m => m.Name == "WorldGenerateEnd");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackBegin)));
            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackEnd)));

            method = Terraria.WorldFile.Methods.Single(x => x.Name == "loadWorld");

            callbackBegin = API.MainCallback.Methods.First(m => m.Name == "WorldLoadBegin");
            callbackEnd = API.MainCallback.Methods.First(m => m.Name == "WorldLoadEnd");

            il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackBegin)));

            //            var old = method.Body.Instructions.Last();
            var newI = il.Create(OpCodes.Call, _asm.MainModule.Import(callbackEnd));

            for (var x = 0; x < method.Body.Instructions.Count; x++)
            {
                var ins = method.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Call && ins.Operand is MethodReference)
                {
                    var mref = ins.Operand as MethodReference;
                    if (mref.Name == "setFireFlyChance")
                    {
                        il.InsertAfter(ins, newI);
                        break;
                    }
                }
            }
            //TODO work out why it crashes when you replace Ret with Ret
        }

        /// <summary>
        /// Fixes some crashes where I was getting some exceptions from the NPC.AI method.
        /// </summary>
        public void FixRandomErrors()
        {
            var ai = Terraria.NPC.Methods.Where(x => x.Name == "AI").First();

            var fld = _asm.MainModule.Import(API.NPCCallback.Fields.Where(x => x.Name == "CheckedRand").First());
            var sng = _asm.MainModule.Import(API.Rand.Methods.Where(x => x.Name == "Next" && x.Parameters.Count == 1).First());
            var dbl = _asm.MainModule.Import(API.Rand.Methods.Where(x => x.Name == "Next" && x.Parameters.Count == 2).First());

            //            var il = ai.Body.GetILProcessor();

            for (var i = ai.Body.Instructions.Count - 1; i > 0; i--)
            {
                var ins = ai.Body.Instructions[i];
                if (ins.OpCode == OpCodes.Ldsfld &&
                    ins.Operand is FieldReference &&
                    (ins.Operand as FieldReference).Name == "rand")
                {
                    if (ins.Next.Next.OpCode == OpCodes.Callvirt
                        && ins.Next.Next.Operand is MethodReference
                        && (ins.Next.Next.Operand as MethodReference).Name == "Next")
                    {
                        ins.Operand = fld;

                        var mth = ins.Next.Next.Operand as MethodReference;
                        if (mth.Parameters.Count == 1)
                        {
                            ins.Next.Next.Operand = sng;
                        }
                        else if (mth.Parameters.Count == 2)
                        {
                            ins.Next.Next.Operand = dbl;
                        }
                    }
                    else if (ins.Next.Next.Next.OpCode == OpCodes.Callvirt
                             && ins.Next.Next.Next.Operand is MethodReference
                             && (ins.Next.Next.Next.Operand as MethodReference).Name == "Next")
                    {
                        ins.Operand = fld;

                        var mth = ins.Next.Next.Next.Operand as MethodReference;
                        if (mth.Parameters.Count == 1)
                        {
                            ins.Next.Next.Next.Operand = sng;
                        }
                        else if (mth.Parameters.Count == 2)
                        {
                            ins.Next.Next.Next.Operand = dbl;
                        }
                    }
                }
            }
        }

        private void MakeAllAccessible(TypeDefinition type, bool nested)
        {
            if (!nested) type.IsPublic = true;

            foreach (var itm in type.Methods)
            {
                itm.IsPublic = true;
                if (itm.IsFamily) itm.IsFamily = false;
                if (itm.IsFamilyAndAssembly) itm.IsFamilyAndAssembly = false;
                if (itm.IsFamilyOrAssembly) itm.IsFamilyOrAssembly = false;
                if (itm.IsPrivate) itm.IsPrivate = false;
            }
            foreach (var itm in type.Fields)
            {
                if (itm.IsFamily) itm.IsFamily = false;
                if (itm.IsFamilyAndAssembly) itm.IsFamilyAndAssembly = false;
                if (itm.IsFamilyOrAssembly) itm.IsFamilyOrAssembly = false;
                if (itm.IsPrivate)
                {
                    if (type.Events.Where(x => x.Name == itm.Name).Count() == 0)
                        itm.IsPrivate = false;
                    else
                    {
                        continue;
                    }
                }

                itm.IsPublic = true;
            }
            foreach (var itm in type.Properties)
            {
                if (null != itm.GetMethod)
                {
                    itm.GetMethod.IsPublic = true;
                    if (itm.GetMethod.IsFamily) itm.GetMethod.IsFamily = false;
                    if (itm.GetMethod.IsFamilyAndAssembly) itm.GetMethod.IsFamilyAndAssembly = false;
                    if (itm.GetMethod.IsFamilyOrAssembly) itm.GetMethod.IsFamilyOrAssembly = false;
                    if (itm.GetMethod.IsPrivate) itm.GetMethod.IsPrivate = false;
                }
                if (null != itm.SetMethod)
                {
                    itm.SetMethod.IsPublic = true;
                    if (itm.SetMethod.IsFamily) itm.SetMethod.IsFamily = false;
                    if (itm.SetMethod.IsFamilyAndAssembly) itm.SetMethod.IsFamilyAndAssembly = false;
                    if (itm.SetMethod.IsFamilyOrAssembly) itm.SetMethod.IsFamilyOrAssembly = false;
                    if (itm.SetMethod.IsPrivate) itm.SetMethod.IsPrivate = false;
                }
            }

            foreach (var nt in type.NestedTypes)
                MakeAllAccessible(nt, true);
        }

        /// <summary>
        /// Makes all types in the Terraria assembly public
        /// </summary>
        public void MakeEverythingAccessible()
        {
            foreach (var type in _asm.MainModule.Types)
            {
                MakeAllAccessible(type, false);
            }
        }

        /// <summary>
        /// Changes entities to use a sender type
        /// </summary>
        public void HookSenders()
        {
            Terraria.Player.BaseType = _asm.MainModule.Import(API.BasePlayer);
            Terraria.Projectile.BaseType = _asm.MainModule.Import(API.WorldSender);
            Terraria.NPC.BaseType = _asm.MainModule.Import(API.WorldSender);

            ////By default the constructor calls Object.ctor. 
            //var ctor = Terraria.Player.Methods.Single(x => x.Name == ".ctor");
            //var baseCtor = API.BasePlayer.Methods.Single(x => x.Name == ".ctor");

            //var ctorIl = ctor.Body.GetILProcessor();
            //ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Call, _asm.MainModule.Import(baseCtor)));
            //ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Ldarg_0));
        }

        /// <summary>
        /// Hooks the OTA StartServer callback into Terraria.Netplay.StartServer
        /// </summary>
        public void PatchServer()
        {
            var method = Terraria.Netplay.Methods.Single(x => x.Name == "StartServer");
            var callback = API.NetplayCallback.Methods.First(m => m.Name == "StartServer");

            var ins = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldftn);
            ins.Operand = _asm.MainModule.Import(callback);

            //Make the Player inherit our defaults
            //var baseType = _self.MainModule.Types.Single(x => x.Name == "BasePlayer");
            //var interfaceType = _self.MainModule.Types.Single(x => x.Name == "ISender");

            //1.3
            //var rem = ctorIl.Body.Instructions.Single(x => x.OpCode == OpCodes.Call
            //    && x.Operand is MethodReference && (x.Operand as MethodReference).DeclaringType.Name == "Object");
            //ctorIl.Remove(rem.Previous);
            //ctorIl.Remove(rem);

            //Make the UpdateServer function public
            var us = Terraria.Main.Methods.Single(x => x.Name == "UpdateServer");
            us.IsPrivate = false;
            us.IsPublic = true;

            ////Map ServerSock.CheckSection to our own
            //var repl = _asm.MainModule.Types
            //    .SelectMany(x => x.Methods)
            //    .Where(x => x.HasBody)
            //    .SelectMany(x => x.Body.Instructions)
            //    .Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "CheckSection")
            //    .ToArray();
            //callback = userInputClass.Methods.First(m => m.Name == "CheckSection");
            //var mref = _asm.MainModule.Import(callback);
            //foreach (var inst in repl)
            //{
            //    inst.Operand = mref;
            //}
        }

        /// <summary>
        /// This removes the windows specific NAT implementation.
        /// </summary>
        public void FixNetplay()
        {
            const String NATGuid = "AE1E00AA-3FD5-403C-8A27-2BBDC30CD0E1";
            var staticConstructor = Terraria.Netplay.Methods.Single(x => x.Name == ".cctor");

            var il = staticConstructor.Body.GetILProcessor();
            var counting = 0;
            for (var x = 0; x < staticConstructor.Body.Instructions.Count; x++)
            {
                var ins = staticConstructor.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Ldstr && ins.Operand is String && ins.Operand as String == NATGuid)
                {
                    counting = 9;
                }

                if (counting-- > 0)
                {
                    il.Remove(ins);
                    x--;
                }
            }
            //            return;


            //Descrease the amount of file descriptors for mono.
            //We'll do this manually
            //            var fd = staticConstructor.Body.Instructions.Single(x => x.Operand is Int32 && (int)x.Operand == 256);
            //            fd.Operand = 0;
            var rcCtor = Terraria.RemoteServer.Methods.Single(x => x.Name == ".ctor");
            var rtsCtor = API.ClientConnection.Methods.Single(x => x.Name == ".ctor" && x.Parameters.Count == (rcCtor.Body.Instructions[1].Operand as MethodReference).Parameters.Count);

            rcCtor.Body.Instructions[1].Operand = _asm.MainModule.Import(rtsCtor);

            //1.3.0.7 doesnt have the TcpSocket constructor :)
            //            rcCtor = Terraria.RemoteClient.Methods.Single(x => x.Name == ".ctor");
            //            rtsCtor = API.ClientConnection.Methods.Single(x => x.Name == ".ctor" && x.Parameters.Count == (rcCtor.Body.Instructions[1].Operand as MethodReference).Parameters.Count);

            //            rcCtor.Body.Instructions[1].Operand = _asm.MainModule.Import(rtsCtor);


            var serverLoop = Terraria.Netplay.Methods.Single(x => x.Name == "ServerLoop");
            var target = serverLoop.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
                             && (x.Operand is MethodReference)
                             && (x.Operand as MethodReference).DeclaringType.Name == "TcpSocket");
            target.Operand = _asm.MainModule.Import(rtsCtor);


            ////            var tcpSocket = rcCtor.Body.Instructions.Single(x => x.Operand 
            //            rcCtor.Body.Instructions.RemoveAt(0);
            //            rcCtor.Body.Instructions.RemoveAt(0);
            //            rcCtor.Body.Instructions.RemoveAt(0);

            //            return;
            var fl = Terraria.Netplay.Fields.SingleOrDefault(x => x.Name == "upnpnat");
            if (fl != null)
                Terraria.Netplay.Fields.Remove(fl);

            //            return;
            //Clear open and close methods, add reference to the APIs
            var cb = Terraria.Netplay.Methods.Single(x => x.Name == "OpenPort");
            //    .Body;
            //cb.InitLocals = false;
            //cb.Variables.Clear();
            //cb.Instructions.Clear();
            Terraria.Netplay.Methods.Remove(cb);
            //cb.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Nop));
            //cb.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Ret));

            var close = Terraria.Netplay.Methods.Single(x => x.Name == "closePort");
            //    .Body;
            //close.InitLocals = false;
            //close.Variables.Clear();
            //close.Instructions.Clear();
            //close.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Nop));
            //close.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Ret));
            Terraria.Netplay.Methods.Remove(close);

            fl = Terraria.Netplay.Fields.SingleOrDefault(x => x.Name == "mappings");
            if (fl != null)
                Terraria.Netplay.Fields.Remove(fl);

            //use our uPNP (when using native terraria server)
            var openCallback = API.NAT.Methods.First(m => m.Name == "OpenPort");
            var closeCallback = API.NAT.Methods.First(m => m.Name == "ClosePort");

            //            var serverLoop = Terraria.Netplay.Methods.Single(x => x.Name == "ServerLoop");

            foreach (var ins in serverLoop.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Call
                    && x.Operand is MethodReference
                    && new string[] { "OpenPort", "closePort" }.Contains((x.Operand as MethodReference).Name)))
            {
                var mr = ins.Operand as MemberReference;
                if (mr.Name == "closePort")
                {
                    ins.Operand = _asm.MainModule.Import(closeCallback);
                }
                else
                {
                    ins.Operand = _asm.MainModule.Import(openCallback);
                }
            }

            for (var x = _asm.MainModule.Types.Count - 1; x > -1; x--)
            {
                var ty = _asm.MainModule.Types[x];
                if (ty.Namespace == "NATUPNPLib")
                {
                    _asm.MainModule.Types.RemoveAt(x);
                }
            }
        }

        /// <summary>
        /// Used for debugging to see if there are any missing XNA shims.
        /// </summary>
        public void DetectMissingXNA()
        {
            foreach (var t in _asm.MainModule.Types)
            {
                foreach (var m in t.Methods)
                {
                    if (m.Body != null)
                    {
                        foreach (var ins in m.Body.Instructions)
                        {
                            if (ins.Operand is FieldReference)
                            {
                                var fldref = ins.Operand as FieldReference;
                                var name = fldref.DeclaringType.Name;
                                if (fldref.DeclaringType.IsArray)
                                {
                                    name = (fldref.DeclaringType as ArrayType).ElementType.Name;
                                }
                                if (fldref.DeclaringType.Namespace.StartsWith("Microsoft.Xna"))
                                {
                                    if (_self.MainModule.Types.Single(x => x.Name == name).Fields.Where(x => x.Name == fldref.Name).Count() == 0)
                                    {
                                        Console.WriteLine(name + " " + fldref.Name);
                                        //                                        throw new NotImplementedException("Missing field for XNA: " + fldref.Name);
                                    }
                                }
                            }
                            else if (ins.Operand is MethodReference)
                            {
                                var mthref = ins.Operand as MethodReference;
                                var name = mthref.DeclaringType.Name;
                                if (mthref.DeclaringType.IsArray)
                                {
                                    name = (mthref.DeclaringType as ArrayType).ElementType.Name;
                                }
                                if (mthref.Name == "Get" || mthref.Name == "Set")
                                    continue;
                                if (mthref.DeclaringType.Namespace.StartsWith("Microsoft.Xna"))
                                {


                                    if (_self.MainModule.Types.Single(x => x.Name == name)
                                        .Methods.Where(x => x.Name == mthref.Name).Count() == 0)
                                    {
                                        Console.WriteLine(name + " " + mthref.Name);
                                        //                                        throw new NotImplementedException("Missing method for XNA: " + mthref.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //        public void FixEntryPoint()
        //        {
        //            var staticConstructor = Terraria.ProgramServer.Methods.Single(x => x.Name == "Main");
        //
        //            var il = staticConstructor.Body.GetILProcessor();
        //            var counting = 0;
        //            for (var x = 0; x < staticConstructor.Body.Instructions.Count; x++)
        //            {
        //                var ins = staticConstructor.Body.Instructions[x];
        //                if (ins.OpCode == OpCodes.Call && ins.Operand is MethodReference && (ins.Operand as MethodReference).Name == "GetCurrentProcess")
        //                {
        //                    counting = 5;
        //                }
        //
        //                if (counting-- > 0)
        //                {
        //                    il.Remove(ins);
        //                    x--;
        //                }
        //            }
        //        }

        /// <summary>
        /// This patches OTA's SavePath method to get the current directory. Previously Terraria would try to save in My Games
        /// </summary>
        public void FixSavePath()
        {
            var staticConstructor = Terraria.Main.Methods.Single(x => x.Name == ".cctor");
            var mth = staticConstructor.Body.Instructions.Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "GetStoragePath").FirstOrDefault();


            var dir = _asm.MainModule.Import(API.Patches.Methods.Single(k => k.Name == "GetCurrentDirectory"));
            mth.Operand = dir;
            /*1.3.0.7 var il = staticConstructor.Body.GetILProcessor();
            //            var ins = staticConstructor.Body.Instructions.First(x => x.OpCode == OpCodes.Stsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "SavePath");ActiveWorldFileData
            var ins = staticConstructor.Body.Instructions.First(x => x.OpCode == OpCodes.Stsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "ActiveWorldFileData");
            var ix = staticConstructor.Body.Instructions.IndexOf(ins);

            //            var dir = _asm.MainModule.Import(API.Patches.Methods.Single(k => k.Name == "ActiveWorldFileData"));

            //            il.Replace(ins, il.Create(OpCodes.Call, dir));



            //            var dir = _asm.MainModule.Import(API.Patches.Methods.Single(k => k.Name == "GetCurrentDirectory"));
            //
            //            il.InsertBefore(ins, il.Create(OpCodes.Call, dir));

            var i = 0;
            while (i < 28)
            {
                staticConstructor.Body.Instructions.RemoveAt(ix + 1);
                i++;
            }

            var dir = _asm.MainModule.Import(API.Patches.Methods.Single(k => k.Name == "GetCurrentDirectory"));

            il.InsertAfter(ins, il.Create(OpCodes.Call, dir));*/
        }

        /// <summary>
        /// Onward from this specific instruction is client code, and code afterwards returns for the server. (Odd i know)
        /// However, if we dont return and skipMenu is set, the server will crash
        /// </summary>
        public void SkipMenu(SupportType mode)
        {
            if (mode != SupportType.Server) throw new Exception("SkipMenu is a server-only fix");
            var initialise = Terraria.Main.Methods.Single(x => x.Name == "Initialize");
            var loc = initialise.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldDefinition)
                //.Select(x => x.Operand as FieldDefinition)
                .Single(x => (x.Operand as FieldDefinition).Name == "skipMenu");
            var il = initialise.Body.GetILProcessor();
            il.InsertBefore(loc, il.Create(OpCodes.Ret));
        }

        /// <summary>
        /// Adds our command line hook so we get input control from the admin
        /// </summary>
        public void PatchCommandLine()
        {
            //Simply switch to ours
            var serv = Terraria.Main.Methods.Single(x => x.Name == "DedServ");

            var callback = API.MainCallback.Methods.First(m => m.Name == "ListenForCommands");

            var ins = serv.Body.Instructions
                .Single(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "startDedInput");
            ins.Operand = _asm.MainModule.Import(callback);

            var ignore = new string[]
            {
                "Terraria.Main.DedServ",
                "Terraria.Main.startDedInputCallBack"
            };

            //Patch Console.WriteLines
            var cwi = _asm.MainModule.Types
                .SelectMany(x => x.Methods)
                .Where(x => x.HasBody && x.Body.Instructions.Count > 0 && !ignore.Contains(x.DeclaringType.FullName + "." + x.Name))
                .SelectMany(x => x.Body.Instructions)
                .Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference
                          && (x.Operand as MethodReference).Name == "WriteLine"
                          && (x.Operand as MethodReference).DeclaringType.FullName == "System.Console")
                .ToArray();

            foreach (var oci in cwi)
            {
                var mr = oci.Operand as MethodReference;

                var writeline = API.Logger.Methods.First(m => m.Name == "Vanilla"
                                    && CecilMethodExtensions.CompareParameters(m.Parameters, mr.Parameters));
                oci.Operand = _asm.MainModule.Import(writeline);
            }
        }

        /// <summary>
        /// Swaps the process priority.
        /// </summary>
        /// <remarks>This was due to it not working on mono</remarks>
        public void SwapProcessPriority()
        {
            var lsp = Terraria.LaunchInitializer.Methods.Single(x => x.Name == "LoadServerParameters");

            var il = lsp.Body.GetILProcessor();
            while (true)
            {
                il.Remove(lsp.Body.Instructions[0]);
                if (lsp.Body.Instructions[0].OpCode == OpCodes.Leave_S)
                {
                    il.Remove(lsp.Body.Instructions[0]);
                    il.Remove(lsp.Body.Instructions[0]);
                    il.Remove(lsp.Body.Instructions[0]);

                    break;
                }
            }

            lsp.Body.Variables.RemoveAt(0);
            lsp.Body.Variables.RemoveAt(0);
            lsp.Body.Variables.RemoveAt(0);

            lsp.Body.ExceptionHandlers.RemoveAt(0);

            var callback = _asm.MainModule.Import(API.Configuration.Methods.Single(x => x.Name == "StartupConfig"));
            il.InsertBefore(lsp.Body.Instructions[0], il.Create(OpCodes.Call, callback));
            il.InsertBefore(lsp.Body.Instructions[0], il.Create(OpCodes.Ldarg_0));
        }

        /// <summary>
        /// Makes the types public.
        /// </summary>
        /// <param name="server">If set to <c>true</c> server.</param>
        public void MakeTypesPublic(bool server)
        {
            var types = _asm.MainModule.Types
                .Where(x => x.IsPublic == false)
                .ToArray();

            for (var x = 0; x < types.Length; x++)
                types[x].IsPublic = true;

            var sd = Terraria.WorldGen.Fields
                .Where(x => x.Name == "stopDrops")
                .Select(x => x)
                .First();
            sd.IsPrivate = false;
            sd.IsPublic = true;

            if (server)
            {
                //                sd = Terraria.ProgramServer.Fields
                //                    .Where(x => x.Name == "Game")
                //                    .Select(x => x)
                //                    .First();
                //                sd.IsPrivate = false;
                //                sd.IsPublic = true;

                var main = Terraria.Main.Methods
                    .Where(x => x.Name == "Update")
                    .Select(x => x)
                    .First();
                main.IsFamily = false;
                main.IsPublic = true;

                var tp = Terraria.LaunchInitializer.Methods.Single(x => x.Name == "TryParameter");
                tp.IsFamily = false;
                tp.IsPublic = true;
            }
        }

        /// <summary>
        /// Ensures OTA references the current Terraria assemblyname
        /// </summary>
        private void SwapOTAReferences()
        {
            var terrariaReferences = _self.MainModule.AssemblyReferences
                .Where(x => x.Name.StartsWith("Terraria"))
                .ToArray();

            //            Console.WriteLine("Changing {0} references to {1}", terrariaReferences.Length, _asm.Name.Name);
            foreach (var item in terrariaReferences)
            {
                item.Name = _asm.Name.Name;
                item.PublicKey = _asm.Name.PublicKey;
                item.PublicKeyToken = _asm.Name.PublicKeyToken;
                item.Version = _asm.Name.Version;
            }

            //Annnnnnd save
            _self.Write(_self.Name.Name + ".dll");
        }

        /// <summary>
        /// Removes the references to the XNA binaries, and replaces them with dummies.
        /// </summary>
        public void PatchXNA(bool server)
        {
            var xnaFramework = _asm.MainModule.AssemblyReferences
                .Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
                .ToArray();

            if (server)
                for (var x = 0; x < xnaFramework.Length; x++)
                {
                    xnaFramework[x].Name = _self.Name.Name;
                    xnaFramework[x].PublicKey = _self.Name.PublicKey;
                    xnaFramework[x].PublicKeyToken = _self.Name.PublicKeyToken;
                    xnaFramework[x].Version = _self.Name.Version;
                }
            else
            {
                for (var x = 0; x < xnaFramework.Length; x++)
                {
                    xnaFramework[x].Name = "MonoGame.Framework";
                    xnaFramework[x].PublicKey = null;
                    xnaFramework[x].PublicKeyToken = null;
                    xnaFramework[x].Version = new Version("3.1.2.0");
                }

                //Use an NSApplication entry point for MAC
            }
        }

        /// <summary>
        /// Updates the Newtonsoft binaries to what Owin supports
        /// </summary>
        public void PatchJSON(string version)
        {
            var xnaFramework = _asm.MainModule.AssemblyReferences
                .Where(x => x.Name.StartsWith("Newtonsoft"))
                .ToArray();

            for (var x = 0; x < xnaFramework.Length; x++)
            {
                xnaFramework[x].Version = new Version(version);
            }
        }

        /// <summary>
        /// Removes the references to the Steam binaries, and replaces them with dummies.
        /// </summary>
        public void PatchSteam()
        {
            //Not finished
            //            return;
            //            var xnaFramework = _asm.MainModule.AssemblyReferences
            //                .Where(x => x.Name.StartsWith("Steamworks.NET"))
            //                .ToArray();
            //
            //            for (var x = 0; x < xnaFramework.Length; x++)
            //            {
            //                xnaFramework[x].Name = _self.Name.Name;
            //                xnaFramework[x].PublicKey = _self.Name.PublicKey;
            //                xnaFramework[x].PublicKeyToken = _self.Name.PublicKeyToken;
            //                xnaFramework[x].Version = _self.Name.Version;
            //            }
        }

        /// <summary>
        /// Hooks the processing of packets to OTA for extra functionalities and plugins to use
        /// </summary>
        public void HookMessageBuffer()
        {
            var getData = Terraria.MessageBuffer.Methods.Single(x => x.Name == "GetData");
            var whoAmI = Terraria.MessageBuffer.Fields.Single(x => x.Name == "whoAmI");

            var insertionPoint = getData.Body.Instructions
                .Single(x => x.OpCode == OpCodes.Callvirt
                                     && x.Operand is MethodReference
                                     && (x.Operand as MethodReference).Name == "set_Position");

            var callback = API.MessageBufferCallback.Methods.First(m => m.Name == "ProcessPacket");

            var il = getData.Body.GetILProcessor();
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Stloc_0));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldarg_2));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldloc_1));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldloc_0));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldfld, whoAmI));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldarg_0));

        }

        //        public void RemoveClientCode()
        //        {
        //            var methods = _asm.MainModule.Types
        //                .SelectMany(x => x.Methods)
        //                .ToArray();
        //            var offsets = new System.Collections.Generic.List<Instruction>();
        //
        //            foreach (var mth in methods)
        //            {
        //                var hasMatch = true;
        //                while (mth.HasBody && hasMatch)
        //                {
        //                    var match = mth.Body.Instructions
        //                        .SingleOrDefault(x => x.OpCode == OpCodes.Ldsfld
        //                                    && x.Operand is FieldReference
        //                                    && (x.Operand as FieldReference).Name == "netMode"
        //                                    && x.Next.OpCode == OpCodes.Ldc_I4_1
        //                                    && (x.Next.Next.OpCode == OpCodes.Bne_Un_S)// || x.Next.Next.OpCode == OpCodes.Bne_Un)
        //                                    && !offsets.Contains(x)
        //                                    && (x.Previous == null || x.Previous.OpCode != OpCodes.Bne_Un_S));
        //
        //                    hasMatch = match != null;
        //                    if (hasMatch)
        //                    {
        //                        var blockEnd = match.Next.Next.Operand as Instruction;
        //                        var il = mth.Body.GetILProcessor();
        //
        //                        var cur = il.Body.Instructions.IndexOf(match) + 3;
        //
        //                        while (il.Body.Instructions[cur] != blockEnd)
        //                        {
        //                            il.Remove(il.Body.Instructions[cur]);
        //                        }
        //                        offsets.Add(match);
        //                        //var newIns = il.Body.Instructions[cur];
        //                        //for (var x = 0; x < il.Body.Instructions.Count; x++)
        //                        //{
        //                        //    if (il.Body.Instructions[x].Operand == newIns)
        //                        //    {
        //                        //        il.Replace(il.Body.Instructions[x], il.Create(il.Body.Instructions[x].OpCode, newIns));
        //                        //    }
        //                        //}
        //                    }
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// Hooks the sockets.
        //        /// </summary>
        //        public void HookSockets()
        //        {
        //            //Remove the tcpClient initialisation
        //            //            var cst = Terraria.ServerSock.Methods.Single(x => x.Name == ".ctor");
        //            //            cst.Body.Instructions.RemoveAt(0);
        //            //            cst.Body.Instructions.RemoveAt(0);
        //            //            cst.Body.Instructions.RemoveAt(0);
        //
        //            //Temporary until i get more time
        //            foreach (var rep in new string[] { /*"SendAnglerQuest,"*/ /*"syncPlayers",*/ "AddBan" })
        //            {
        //                var toBeReplaced = _asm.MainModule.Types
        //                    .SelectMany(x => x.Methods
        //                                .Where(y => y.HasBody)
        //                                   )
        //                        .SelectMany(x => x.Body.Instructions)
        //                        .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
        //                                       && x.Operand is MethodReference
        //                                       && (x.Operand as MethodReference).Name == rep
        //                                   )
        //                        .ToArray();
        //
        //                var replacement = API.NetplayCallback.Methods.Single(x => x.Name == rep);
        //                for (var x = 0; x < toBeReplaced.Length; x++)
        //                {
        //                    toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
        //                }
        //            }
        //
        //            //Inherit
        //            //            Terraria.ServerSock.BaseType = _asm.MainModule.Import(API.IAPISocket);
        //
        //            //Now change Netplay.serverSock to the IAPISocket type
        //            //            var serverSockArr = Terraria.Netplay.Fields.Single(x => x.Name == "serverSock");
        //            //            var at = new ArrayType(API.IAPISocket);
        //            //            serverSockArr.FieldType = _asm.MainModule.Import(at);
        //            //
        //            //            //By default the constructor calls Object.ctor. This should also be changed to our socket since it now inherits that.
        //            //            var ctor = Terraria.ServerSock.Methods.Single(x => x.Name == ".ctor");
        //            //            var baseCtor = API.IAPISocket.Methods.Single(x => x.Name == ".ctor");
        //            //            ctor.Body.Instructions.RemoveAt(ctor.Body.Instructions.Count - 2);
        //            //            ctor.Body.Instructions.RemoveAt(ctor.Body.Instructions.Count - 2);
        //            //
        //            //            var ctorIl = ctor.Body.GetILProcessor();
        //            //            ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Call, _asm.MainModule.Import(baseCtor)));
        //            //            ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Ldarg_0));
        //
        //
        //            return;
        //            //            var targetField = API.IAPISocket.Fields.Single(x => x.Name == "tileSection");
        //            //            var targetArray = API.NetplayCallback.Fields.Single(x => x.Name == "slots");
        //            //
        //            //            //Replace Terraria.Netplay.serverSock references with TDSM.Core.Server.slots
        //            //            var instructions = _asm.MainModule.Types
        //            //                .SelectMany(x => x.Methods
        //            //                    .Where(y => y.HasBody && y.Body.Instructions != null)
        //            //                )
        //            //                .SelectMany(x => x.Body.Instructions)
        //            //                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
        //            //                    && x.Operand is FieldReference
        //            //                    && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
        //            //                    && x.Next.Next.Next.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
        //            //                    && x.Next.Next.Next.Operand is FieldReference
        //            //                    && (x.Next.Next.Next.Operand as FieldReference).Name == "tileSection"
        //            //                )
        //            //                .ToArray();
        //            //
        //            //            for (var x = 0; x < instructions.Length; x++)
        //            //            {
        //            //                //                instructions[x].Operand = _asm.MainModule.Import(targetArray);
        //            //                instructions[x].Next.Next.Next.Operand = _asm.MainModule.Import(targetField);
        //            //            }
        //            //
        //            //
        //            //            //TODO BELOW - update ServerSock::announce to IAPISocket::announce (etc)
        //            //#if VanillaSockets
        //            //#else
        //            //            //            Replace Terraria.Netplay.serverSock references with TDSM.Core.Server.slots
        //            //            instructions = _asm.MainModule.Types
        //            //               .SelectMany(x => x.Methods
        //            //                   .Where(y => y.HasBody && y.Body.Instructions != null)
        //            //               )
        //            //               .SelectMany(x => x.Body.Instructions)
        //            //               .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
        //            //                   && x.Operand is FieldReference
        //            //                   && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
        //            //               )
        //            //               .ToArray();
        //            //
        //            //            for (var x = 0; x < instructions.Length; x++)
        //            //            {
        //            //                instructions[x].Operand = _asm.MainModule.Import(targetArray);
        //            //
        //            //                //var var = instructions[x].Next.Next.Next;
        //            //                //if (var.OpCode == OpCodes.Ldfld && var.Operand is MemberReference)
        //            //                //{
        //            //                //    var mem = var.Operand as MemberReference;
        //            //                //    if (mem.DeclaringType.Name == "ServerSock")
        //            //                //    {
        //            //                //        var ourVar = sockClass.Fields.Where(j => j.Name == mem.Name).FirstOrDefault();
        //            //                //        if (ourVar != null)
        //            //                //        {
        //            //                //            var.Operand = _asm.MainModule.Import(ourVar);
        //            //                //        }
        //            //                //    }
        //            //                //}
        //            //            }
        //            //#endif
        //            //            //instructions = _asm.MainModule.Types
        //            //            //   .SelectMany(x => x.Methods
        //            //            //       .Where(y => y.HasBody && y.Body.Instructions != null)
        //            //            //   )
        //            //            //   .SelectMany(x => x.Body.Instructions)
        //            //            //   .Where(x => (x.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt)
        //            //            //       &&
        //            //            //       (
        //            //            //            (x.Operand is MemberReference && (x.Operand as MemberReference).DeclaringType.FullName == "Terraria.ServerSock")
        //            //            //            ||
        //            //            //            (x.Operand is MethodDefinition && (x.Operand as MethodDefinition).DeclaringType.FullName == "Terraria.ServerSock")
        //            //            //       )
        //            //            //   )
        //            //            //   .ToArray();
        //            //
        //            //#if VanillaSockets || true
        //            //            var bodies = _asm.MainModule.Types
        //            //               .SelectMany(x => x.Methods
        //            //                   .Where(y => y.HasBody && y.Body.Instructions != null)
        //            //               )
        //            //               .Where(x => x.Body.Instructions.Where(k => (k.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Stfld
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Ldftn)
        //            //                   &&
        //            //                   (
        //            //                        (k.Operand is MemberReference && (k.Operand as MemberReference).DeclaringType.FullName == "Terraria.ServerSock")
        //            //                        ||
        //            //                        (k.Operand is MethodDefinition && (k.Operand as MethodDefinition).DeclaringType.FullName == "Terraria.ServerSock")
        //            //                   )).Count() > 0
        //            //               )
        //            //               .ToArray();
        //            //            foreach (var targetMethod in bodies)
        //            //            {
        //            //                instructions = targetMethod.Body.Instructions.Where(k => (k.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Stfld
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt
        //            //                   || k.OpCode == Mono.Cecil.Cil.OpCodes.Ldftn)
        //            //                   &&
        //            //                   (
        //            //                        (k.Operand is MemberReference && (k.Operand as MemberReference).DeclaringType.FullName == "Terraria.ServerSock")
        //            //                        ||
        //            //                        (k.Operand is MethodDefinition && (k.Operand as MethodDefinition).DeclaringType.FullName == "Terraria.ServerSock")
        //            //                   )).ToArray();
        //            //
        //            //                for (var x = 0; x < instructions.Length; x++)
        //            //                {
        //            //                    var var = instructions[x];
        //            //                    if (var.Operand is MethodDefinition)
        //            //                    {
        //            //                        var mth = var.Operand as MethodDefinition;
        //            //                        var ourVar = API.IAPISocket.Methods.SingleOrDefault(j => j.Name == mth.Name);
        //            //                        if (ourVar != null)
        //            //                        {
        //            //                            var.Operand = _asm.MainModule.Import(ourVar);
        //            //
        //            //                            if (var.OpCode == OpCodes.Ldftn)
        //            //                            {
        //            //                                var.OpCode = OpCodes.Ldvirtftn;
        //            //
        //            //                                var ilp = targetMethod.Body.GetILProcessor();
        //            //                                ilp.InsertBefore(var, ilp.Create(OpCodes.Dup));
        //            //                            }
        //            //                        }
        //            //                    }
        //            //                    else if (var.Operand is MemberReference)
        //            //                    {
        //            //                        var mem = var.Operand as MemberReference;
        //            //                        var ourVar = API.IAPISocket.Fields.SingleOrDefault(j => j.Name == mem.Name);
        //            //                        if (ourVar != null)
        //            //                        {
        //            //                            var.Operand = _asm.MainModule.Import(ourVar);
        //            //                        }
        //            //                    }
        //            //                }
        //            //            }
        //            //#endif
        //            //
        //            //            foreach (var rep in new string[] { /*"SendAnglerQuest", "sendWater", "syncPlayers",*/ "AddBan" })
        //            //            {
        //            //                var toBeReplaced = _asm.MainModule.Types
        //            //                    .SelectMany(x => x.Methods
        //            //                        .Where(y => y.HasBody)
        //            //                    )
        //            //                    .SelectMany(x => x.Body.Instructions)
        //            //                    .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
        //            //                        && x.Operand is MethodReference
        //            //                        && (x.Operand as MethodReference).Name == rep
        //            //                    )
        //            //                    .ToArray();
        //            //
        //            //                var replacement = API.NetplayCallback.Methods.Single(x => x.Name == rep);
        //            //                for (var x = 0; x < toBeReplaced.Length; x++)
        //            //                {
        //            //                    toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
        //            //                }
        //            //            }
        //            //
        //            //            //Inherit
        //            //            Terraria.ServerSock.BaseType = _asm.MainModule.Import(API.IAPISocket);
        //            //
        //            //            //Change to override
        //            //            foreach (var rep in new string[] { "SpamUpdate", "SpamClear", "Reset", "SectionRange", "ServerReadCallBack", "ServerWriteCallBack" })
        //            //            {
        //            //                var mth = Terraria.ServerSock.Methods.Single(x => x.Name == rep);
        //            //                mth.IsVirtual = true;
        //            //
        //            //
        //            //            }
        //            //
        //            //            //Remove variables that are in the base class
        //            //            foreach (var fld in API.IAPISocket.Fields)
        //            //            {
        //            //                var rem = Terraria.ServerSock.Fields.SingleOrDefault(x => x.Name == fld.Name);
        //            //                if (rem != null)
        //            //                {
        //            //                    Terraria.ServerSock.Fields.Remove(rem);
        //            //                }
        //            //            }
        //            //
        //            //#if VanillaSockets
        //            //            //Now change Netplay.serverSock to the IAPISocket type
        //            //            var serverSockArr = Terraria.Netplay.Fields.Single(x => x.Name == "serverSock");
        //            //            var at = new ArrayType(API.IAPISocket);
        //            //            serverSockArr.FieldType = _asm.MainModule.Import(at);
        //            //
        //            //            //By default the constructor calls Object.ctor. This should also be changed to our socket since it now inherits that.
        //            //            var ctor = Terraria.ServerSock.Methods.Single(x => x.Name == ".ctor");
        //            //            var baseCtor = API.IAPISocket.Methods.Single(x => x.Name == ".ctor");
        //            //            ctor.Body.Instructions.RemoveAt(ctor.Body.Instructions.Count - 2);
        //            //            ctor.Body.Instructions.RemoveAt(ctor.Body.Instructions.Count - 2);
        //            //
        //            //            var ctorIl = ctor.Body.GetILProcessor();
        //            //            ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Call, _asm.MainModule.Import(baseCtor)));
        //            //            ctorIl.Body.Instructions.Insert(0, ctorIl.Create(OpCodes.Ldarg_0));
        //            //#endif
        //            //
        //            //            ////DEBUG
        //            //            ////ldstr "Starting server..."
        //            //            ////stsfld string Terraria.Main::statusText
        //            //            //var statusText = Terraria.Main.Fields.Single(x => x.Name == "statusText");
        //            //            //var sl = Terraria.Netplay.Methods.Single(x => x.Name == "ServerLoop");
        //            //            ////var writeLine = API.Tools.Methods.Single(x => x.Name == "WriteLine" && x.Parameters.Count == 1 && x.Parameters[0].ParameterType.Name == "String");
        //            //            ////var writeLine = API.IAPISocket.Methods.Single(x => x.Name == "Debug");
        //            //
        //            //
        //            //            ////sl.Body.Variables.Add(new VariableDefinition(_asm.MainModule.Import(typeof(String))));
        //            //
        //            //            //instructions = sl.Body.Instructions.Where(x => x.OpCode == OpCodes.Newobj).ToArray();
        //            //            //var slIl = sl.Body.GetILProcessor();
        //            //            //var debugKey = 0;
        //            //            //foreach (var ins in instructions)
        //            //            //{
        //            //            //    var insert = false;
        //            //            //    if (ins.Operand is MethodDefinition)
        //            //            //    {
        //            //            //        var md = ins.Operand as MethodDefinition;
        //            //            //        insert = md.DeclaringType.Name == "ServerSock";
        //            //            //    }
        //            //            //    //else if (ins.Operand is FieldReference)
        //            //            //    //{
        //            //            //    //    var fr = ins.Operand as FieldReference;
        //            //            //    //    insert = fr.FieldType.Name.Contains("ServerSock");
        //            //            //    //}
        //            //
        //            //            //    if (insert)
        //            //            //    {
        //            //            //        var target = ins;//.Next.Next.Next;
        //            //
        //            //            //        //target.Operand = _asm.MainModule.Import(ctor);
        //            //            //        //slIl.Replace(target, slIl.Create(OpCodes.Callvirt, _asm.MainModule.Import(writeLine)));
        //            //            //        //slIl.Replace(target, slIl.Create(OpCodes.Callvirt, _asm.MainModule.Import(writeLine)));
        //            //
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Pop));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Ret));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Nop));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Call, _asm.MainModule.Import(writeLine)));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Ldstr, (++debugKey).ToString()));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Stsfld, statusText));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Ldstr, (++debugKey).ToString()));
        //            //            //        //slIl.InsertAfter(target, slIl.Create(OpCodes.Nop));
        //            //
        //            //            //        break;
        //            //            //    }
        //            //            //}
        //            //
        //            //
        //            //            //var ss1 = Terraria.ServerSock;
        //            //            //var ss2 = _self.MainModule.Types.Single(x => x.Name == "ServerSock2");
        //            //
        //            //            var sendWater = Terraria.NetMessage.Methods.Single(x => x.Name == "sendWater");
        //            //            {
        //            //                var ix = 0;
        //            //                var removing = false;
        //            //                while (sendWater.Body.Instructions.Count > 0 && ix < sendWater.Body.Instructions.Count)
        //            //                {
        //            //                    var first = false;
        //            //                    var ins = sendWater.Body.Instructions[ix];
        //            //                    if (ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldReference && (ins.Operand as FieldReference).Name == "buffer")
        //            //                    {
        //            //                        removing = true;
        //            //                        first = true;
        //            //                    }
        //            //                    else first = false;
        //            //
        //            //                    if (ins.OpCode == OpCodes.Brfalse_S)
        //            //                    {
        //            //                        //Keep instruction, and replace the first (previous instruction)
        //            //                        var canSendWater = API.IAPISocket.Methods.Single(x => x.Name == "CanSendWater");
        //            //
        //            //                        var il = sendWater.Body.GetILProcessor();
        //            //                        var target = ins.Previous;
        //            //                        var newTarget = il.Create(OpCodes.Nop);
        //            //
        //            //                        il.Replace(target, newTarget);
        //            //
        //            //                        il.InsertAfter(newTarget, il.Create(OpCodes.Callvirt, _asm.MainModule.Import(canSendWater)));
        //            //                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldelem_Ref));
        //            //                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldloc_0));
        //            //#if VanillaSockets
        //            //                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldsfld, _asm.MainModule.Import(serverSockArr)));
        //            //#else
        //            //                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldsfld, _asm.MainModule.Import(targetArray)));
        //            //#endif
        //            //                        removing = false;
        //            //                        break;
        //            //                    }
        //            //
        //            //                    if (removing && !first)
        //            //                    {
        //            //                        sendWater.Body.Instructions.RemoveAt(ix);
        //            //                    }
        //            //
        //            //                    if (!removing || first) ix++;
        //            //                }
        //            //            }
        //            //
        //            //            var syncPlayers = Terraria.NetMessage.Methods.Single(x => x.Name == "syncPlayers");
        //            //            {
        //            //                var ix = 0;
        //            //                var removing = false;
        //            //                var isPlayingComplete = false;
        //            //                while (syncPlayers.Body.Instructions.Count > 0 && ix < syncPlayers.Body.Instructions.Count)
        //            //                {
        //            //                    var first = false;
        //            //                    var ins = syncPlayers.Body.Instructions[ix];
        //            //                    if (ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldDefinition && (ins.Operand as FieldDefinition).Name == "serverSock")
        //            //                    {
        //            //                        removing = true;
        //            //                        first = true;
        //            //
        //            //                        if (isPlayingComplete)
        //            //                        {
        //            //                            //We'll use the next two instructions because im cheap.
        //            //                            ix += 2;
        //            //                        }
        //            //                    }
        //            //                    else first = false;
        //            //
        //            //                    if (removing && ins.OpCode == OpCodes.Bne_Un)
        //            //                    {
        //            //                        //Keep instruction, and replace the first (previous instruction)
        //            //                        var isPlaying = API.IAPISocket.Methods.Single(x => x.Name == "IsPlaying");
        //            //
        //            //                        var il = syncPlayers.Body.GetILProcessor();
        //            //                        var target = ins.Previous;
        //            //
        //            //                        il.InsertAfter(target, il.Create(OpCodes.Callvirt, _asm.MainModule.Import(isPlaying)));
        //            //                        il.InsertAfter(target, il.Create(OpCodes.Ldelem_Ref));
        //            //                        il.InsertAfter(target, il.Create(OpCodes.Ldloc_1));
        //            //
        //            //                        il.Replace(ins, il.Create(OpCodes.Brfalse, ins.Operand as Instruction));
        //            //
        //            //                        isPlayingComplete = true;
        //            //                        removing = false;
        //            //                        //break;
        //            //
        //            //                        ix += 3;
        //            //                    }
        //            //                    else if (removing && ins.OpCode == OpCodes.Callvirt && isPlayingComplete)
        //            //                    {
        //            //                        if (ins.Operand is MethodReference)
        //            //                        {
        //            //                            var md = ins.Operand as MethodReference;
        //            //                            if (md.DeclaringType.Name == "Object" && md.Name == "ToString")
        //            //                            {
        //            //                                var remoteAddress = API.IAPISocket.Methods.Single(x => x.Name == "RemoteAddress");
        //            //                                ins.Operand = _asm.MainModule.Import(remoteAddress);
        //            //                                break;
        //            //                            }
        //            //                        }
        //            //                    }
        //            //
        //            //                    if (removing && !first)
        //            //                    {
        //            //                        syncPlayers.Body.Instructions.RemoveAt(ix);
        //            //                    }
        //            //
        //            //                    if (!removing || first) ix++;
        //            //                }
        //            //            }
        //            //
        //            //            var sendAngler = Terraria.NetMessage.Methods.Single(x => x.Name == "SendAnglerQuest");
        //            //            {
        //            //                var ix = 0;
        //            //                var removing = false;
        //            //                while (sendAngler.Body.Instructions.Count > 0 && ix < sendAngler.Body.Instructions.Count)
        //            //                {
        //            //                    var first = false;
        //            //                    var ins = sendAngler.Body.Instructions[ix];
        //            //                    if (ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldDefinition && (ins.Operand as FieldDefinition).Name == "serverSock")
        //            //                    {
        //            //                        removing = true;
        //            //                        first = true;
        //            //
        //            //                        //Reuse the next two as well
        //            //                        ix += 2;
        //            //                    }
        //            //                    else first = false;
        //            //
        //            //                    if (removing && ins.OpCode == OpCodes.Bne_Un_S)
        //            //                    {
        //            //                        //Keep instruction, and replace the first (previous instruction)
        //            //                        var isPlaying = API.IAPISocket.Methods.Single(x => x.Name == "IsPlaying");
        //            //
        //            //                        var il = sendAngler.Body.GetILProcessor();
        //            //
        //            //                        il.InsertBefore(ins, il.Create(OpCodes.Callvirt, _asm.MainModule.Import(isPlaying)));
        //            //                        il.Replace(ins, il.Create(OpCodes.Brfalse, ins.Operand as Instruction));
        //            //
        //            //                        removing = false;
        //            //                        break;
        //            //                    }
        //            //
        //            //                    if (removing && !first)
        //            //                    {
        //            //                        sendAngler.Body.Instructions.RemoveAt(ix);
        //            //                    }
        //            //
        //            //                    if (!removing || first) ix++;
        //            //                }
        //            //            }
        //        }

        //        /// <summary>
        //        /// Hooks if NPC's can spawn into OTA
        //        /// </summary>
        //        public void HookNPCSpawning()
        //        {
        //            var newNPC = Terraria.NPC.Methods.Single(x => x.Name == "NewNPC");
        //            var method = API.NPCCallback.Methods.Single(x => x.Name == "CanSpawnNPC");
        //
        //            var il = newNPC.Body.GetILProcessor();
        //            var first = newNPC.Body.Instructions.First();
        //
        //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
        //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
        //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
        //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_3));
        //            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(method)));
        //
        //            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
        //            il.InsertBefore(first, il.Create(OpCodes.Ldc_I4, 200));
        //            il.InsertBefore(first, il.Create(OpCodes.Ret));
        //        }

        /// <summary>
        /// Hooks the invasion warning call into OTA
        /// </summary>
        public void HookInvasionWarning()
        {
            var newNPC = Terraria.Main.Methods.Single(x => x.Name == "InvasionWarning");
            var method = API.MainCallback.Methods.Single(x => x.Name == "OnInvasionWarning");

            var il = newNPC.Body.GetILProcessor();
            var first = newNPC.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(method)));

            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        //        /// <summary>
        //        /// Allows eclipse to be started at any time
        //        /// </summary>
        //        //TODO mark as a feature, not a requirement for a functional OTA
        //        public void HookEclipse()
        //        {
        //            var mth = Terraria.Main.Methods.Single(x => x.Name == "UpdateTime");
        //            var field = API.MainCallback.Fields.Single(x => x.Name == "StartEclipse");
        //
        //            var il = mth.Body.GetILProcessor();
        //            var start = il.Body.Instructions.Single(x =>
        //                x.OpCode == OpCodes.Ldsfld
        //                            && x.Operand is FieldReference
        //                            && (x.Operand as FieldReference).Name == "hardMode"
        //                            && x.Previous.OpCode == OpCodes.Call
        //                            && x.Previous.Operand is MethodReference
        //                            && (x.Previous.Operand as MethodReference).Name == "StartInvasion"
        //                        );
        //
        //            //Preserve
        //            var old = start.Operand as FieldReference;
        //
        //            //Replace with ours to keep the IL inline
        //            start.Operand = _asm.MainModule.Import(field);
        //            //Readd the preserved
        //            il.InsertAfter(start, il.Create(OpCodes.Ldsfld, old));
        //
        //            //Now find the target instruction if the value is true
        //            var startEclipse = il.Body.Instructions.Single(x =>
        //                x.OpCode == OpCodes.Stsfld
        //                                   && x.Operand is FieldReference
        //                                   && (x.Operand as FieldReference).Name == "eclipse"
        //                                   && x.Next.OpCode == OpCodes.Ldsfld
        //                                   && x.Next.Operand is FieldReference
        //                                   && (x.Next.Operand as FieldReference).Name == "eclipse"
        //                               ).Previous;
        //            il.InsertAfter(start, il.Create(OpCodes.Brtrue, startEclipse));
        //
        //            //Since all we care about is setting the StartEclipse to TRUE; we need to be able to disable once done.
        //            il.InsertAfter(startEclipse.Next, il.Create(OpCodes.Stsfld, start.Operand as FieldReference));
        //            il.InsertAfter(startEclipse.Next, il.Create(OpCodes.Ldc_I4_0));
        //        }

        //        public void HookBloodMoon()
        //        {
        //            var mth = Terraria.Main.Methods.Single(x => x.Name == "UpdateTime");
        //            var field = API.MainCallback.Fields.Single(x => x.Name == "StartBloodMoon");
        //            //return;
        //            var il = mth.Body.GetILProcessor();
        //            var start = il.Body.Instructions.Single(x =>
        //                x.OpCode == OpCodes.Ldsfld
        //                            && x.Operand is FieldReference
        //                            && (x.Operand as FieldReference).Name == "spawnEye"
        //                            && x.Next.Next.OpCode == OpCodes.Ldsfld
        //                            && x.Next.Next.Operand is FieldReference
        //                            && (x.Next.Next.Operand as FieldReference).Name == "moonPhase"
        //                        );
        //
        //            //Preserve
        //            var old = start.Operand as FieldReference;
        //            var target = start.Next as Instruction;
        //
        //            //Replace with ours to keep the IL inline
        //            start.Operand = _asm.MainModule.Import(field);
        //            //Readd the preserved
        //            il.InsertAfter(start, il.Create(OpCodes.Ldsfld, old));
        //
        //            //Now find the target instruction if the value is true
        //            Instruction begin = start.Next;
        //            var i = 12;
        //            while (i > 0)
        //            {
        //                i--;
        //                begin = begin.Next;
        //            }
        //            il.InsertAfter(start, il.Create(OpCodes.Brtrue, begin));
        //
        //            //Since all we care about is setting the StartBloodMoon to TRUE; we need to be able to disable once done.
        //            var startBloodMoon = il.Body.Instructions.Single(x =>
        //                x.OpCode == OpCodes.Ldsfld
        //                                     && x.Operand is FieldReference
        //                                     && (x.Operand as FieldReference).Name == "bloodMoon"
        //                                     && x.Next.Next.OpCode == OpCodes.Ldsfld
        //                                     && x.Next.Next.Operand is FieldReference
        //                                     && (x.Next.Next.Operand as FieldReference).Name == "netMode"
        //                                 );
        //            il.InsertAfter(startBloodMoon.Next, il.Create(OpCodes.Stsfld, start.Operand as FieldReference));
        //            il.InsertAfter(startBloodMoon.Next, il.Create(OpCodes.Ldc_I4_0));
        //        }

        /// <summary>
        /// Saves the patched assembly
        /// </summary>
        /// <param name="mode">Mode.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="apiBuild">API build.</param>
        /// <param name="otapuUID">OTAPI UUID.</param>
        /// <param name="name">Name.</param>
        /// <param name="swapOTA">If set to <c>true</c> swap OT.</param>
        public void Save(SupportType mode, string fileName, int apiBuild, string otapuUID, string name, bool swapOTA = false)
        {
            if (mode == SupportType.Server)
            {
                //Ensure the name is updated to the new one
                _asm.Name = new AssemblyNameDefinition(name, new Version(0, 0, apiBuild, 0));
                _asm.MainModule.Name = fileName;

                //Change the uniqueness from what Terraria has, to something different (that way vanilla isn't picked up by assembly resolutions)
                //                var g = _asm.CustomAttributes.Single(x => x.AttributeType.Name == "GuidAttribute");

                for (var x = 0; x < _asm.CustomAttributes.Count; x++)
                {
                    if (_asm.CustomAttributes[x].AttributeType.Name == "GuidAttribute")
                    {
                        _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, otapuUID);
                    }
                    else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyTitleAttribute")
                    {
                        _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, name);
                    }
                    else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyProductAttribute")
                    {
                        _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, name);
                    }
                    //else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyFileVersionAttribute")
                    //{
                    //    _asm.CustomAttributes[x].ConstructorArguments[0] =
                    //        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, "1.0.0.0");
                    //}
                }
            }

            //            foreach (var mod in _asm.Modules)
            //            {
            //                for (var x = mod.Resources.Count - 1; x >= 0; x--)
            //                {
            //                    mod.Resources.RemoveAt(x);
            //                }
            //            }

            //_asm.Write(fileName);
            using (var fs = File.OpenWrite(fileName))
            {
                _asm.Write(fs);
                fs.Flush();
                fs.Close();
            }

            if (swapOTA) SwapOTAReferences();
        }

        public void Dispose()
        {
            _self = null;
            _asm = null;
        }
    }
}
