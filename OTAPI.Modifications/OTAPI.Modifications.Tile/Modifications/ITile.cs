using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Modification.Tile.Modifications
{
    [Ordered(7)] //After all time modifications as we want to alter the new Terraria.Tile properties (and not fields)
    public class ITileModification : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Swapping all Terraria.Tile references to ITile...";

        public override void Run()
        {
            //Get the type definition of Terraria.Tile
            var terrariaTile = this.Type<Terraria.Tile>();

            var iTile = this.Type<OTAPI.Tile.ITile>();
            var importedITile = this.SourceDefinition.MainModule.Import(iTile);

            if (!iTile.SignatureMatches(terrariaTile))
            {
                throw new Exception("ITile does not match Terraria.Tile signatures!");
            }

            //Make Terraria.Tile implement ITile
            terrariaTile.Interfaces.Add(importedITile);

            #region Tile constructor
            //Swap all tile constructors to the OTAPI callback
            //var createTileCallback = this.SourceDefinition.MainModule.Import(
            //	this.Method(() => OTAPI.Callbacks.Terraria.Collection.CreateTile())
            //);
            this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
            {
                if (instruction.OpCode == OpCodes.Newobj)
                {
                    var operandMethod = instruction.Operand as MethodReference;
                    if (operandMethod.DeclaringType.FullName == "Terraria.Tile")
                    {
                        instruction.OpCode = OpCodes.Call;

                        //Find the appropriate create tile call, depending on the constructor parameters
                        var callback = this.ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Collection").Methods.SingleOrDefault(
                            x => x.Name == "CreateTile"
                            && x.Parameters.Count == operandMethod.Parameters.Count
                        );
                        instruction.Operand = this.SourceDefinition.MainModule.Import(callback);
                    }
                }
            });
            #endregion

            #region Terraria.Tile ITile implementations
            //Since we introduced ITile, some properties need to be marked to indicate there is an implementation.
            //However, with tiles we want to be able to override anything, and marking everything as virtual will
            //automatically correct the requirement for us anyway.
            terrariaTile.MakeVirtual();
            #endregion

            #region Tile methods
            this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
            {
                var operandMethod = instruction.Operand as MethodDefinition;

                var selfDeclared = method.DeclaringType.FullName == "Terraria.Tile";
                var ctorGetter = selfDeclared && method.IsConstructor && instruction.Previous != null && instruction.Previous.OpCode == OpCodes.Ldarg_1;
                var selfMethod = selfDeclared && !method.IsConstructor && !method.IsStatic && instruction.Previous != null && instruction.Previous.OpCode == OpCodes.Ldarg_1;

                if (operandMethod != null && (selfMethod || ctorGetter || method.IsStatic || method.DeclaringType.FullName != "Terraria.Tile"))
                {
                    if (operandMethod.IsConstructor)
                        return;

                    if (operandMethod.DeclaringType.FullName == "Terraria.Tile" && !operandMethod.IsStatic)
                    {
                        var methods = iTile.Methods.Where(mth =>
                            mth.Name == operandMethod.Name
                            && mth.Parameters.Count == operandMethod.Parameters.Count
                        );

                        if (methods.Count() == 0)
                            throw new Exception($"Method `{operandMethod.Name}` is not found on {iTile.FullName}");
                        else if (methods.Count() > 1)
                        {
                            // quick fix, compare parameter types/refs

                            foreach (var mth in methods.ToArray())
                            {
                                for (var i = 0; i < operandMethod.Parameters.Count; i++)
                                {
                                    var target = operandMethod.Parameters[i];
                                    var matched = mth.Parameters[i];

                                    if (target.ParameterType.FullName != matched.ParameterType.FullName)
                                    {
                                        methods = methods.Where(m => m != mth);
                                    }
                                }
                            }

                            if (methods.Count() > 1)
                                throw new Exception($"Too many methods named `{operandMethod.Name}` found in {iTile.FullName}");
                        }

                        instruction.Operand = this.SourceDefinition.MainModule.Import(methods.Single());
                    }
                }
            });
            #endregion

            #region Tile locals
            this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
            {
                if (method.HasBody && method.Body.HasVariables)
                {
                    foreach (var local in method.Body.Variables)
                    {
                        if (local.VariableType.FullName == "Terraria.Tile")
                        {
                            local.VariableType = importedITile;
                        }
                    }
                }
            });
            #endregion

            #region Method returns
            this.SourceDefinition.MainModule.ForEachMethod(method =>
            {
                if (method.ReturnType.FullName == "Terraria.Tile")
                {
                    method.ReturnType = importedITile;
                }
            });
            #endregion

            #region Method parameters
            this.SourceDefinition.MainModule.ForEachMethod(method =>
            {
                if (method.HasParameters)
                {
                    foreach (var parameter in method.Parameters)
                    {
                        var refType = parameter.ParameterType as ByReferenceType;

                        if (parameter.ParameterType.FullName == "Terraria.Tile")
                        {
                            parameter.ParameterType = importedITile;
                        }
                        else if(refType != null)
                        {
                            if (refType.ElementType.FullName == "Terraria.Tile")
                            {
                                parameter.ParameterType = new ByReferenceType(importedITile);
                            }
                        }
                    }
                }
            });
            #endregion

            #region Type fields
            this.SourceDefinition.MainModule.ForEachType(type =>
            {
                if (type.HasFields)
                {
                    foreach (var field in type.Fields)
                    {
                        if (field.FieldType.FullName == "Terraria.Tile")
                        {
                            field.FieldType = importedITile;
                        }
                    }
                }
            });
            #endregion
        }
    }
}
