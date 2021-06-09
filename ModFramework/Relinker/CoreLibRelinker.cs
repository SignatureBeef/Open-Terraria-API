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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace ModFramework.Relinker
{
    [MonoMod.MonoModIgnore]
    public class SystemType
    {
        public string FilePath { get; set; }
        public AssemblyDefinition Assembly { get; set; }
        public ExportedType Type { get; set; }

        public AssemblyNameReference AsNameReference() => Assembly.AsNameReference();

        public override string ToString() => Type.ToString();


        public static IEnumerable<SystemType> SystemTypes { get; set; } = GetSystemType();

        static SystemType[] GetSystemType()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            var runtimeAssemblies = Directory.GetFiles(assemblyPath, "*.dll")
                .Select(x =>
                {
                    try
                    {
                        return new
                        {
                            asm = AssemblyDefinition.ReadAssembly(x),
                            path = x,
                        };
                    }
                    catch (Exception ex)
                    {
                        // discard assemblies that cecil cannot parse. e.g. api-ms**.dll on windows
                        return null;
                    }
                })
                .Where(x => x != null);

            var forwardTypes = runtimeAssemblies.SelectMany(ra =>
                ra.asm.MainModule.ExportedTypes
                    .Where(x => x.IsForwarder)
                    .Select(x => new SystemType()
                    {
                        Type = x,
                        Assembly = ra.asm,
                        FilePath = ra.path,
                    })
            );

            return forwardTypes.ToArray();
        }
    }

    [MonoMod.MonoModIgnore]
    public static partial class Extensions
    {
        public static AssemblyNameReference AsNameReference(this AssemblyDefinition assembly)
        {
            var name = assembly.Name;
            return new AssemblyNameReference(name.Name, name.Version)
            {
                PublicKey = name.PublicKey,
                PublicKeyToken = name.PublicKeyToken,
                Culture = name.Culture,
                Hash = name.Hash,
                HashAlgorithm = name.HashAlgorithm,
                Attributes = name.Attributes
            };
        }
    }

    [MonoMod.MonoModIgnore]
    public delegate AssemblyNameReference ResolveCoreLibHandler(TypeReference target);

    [MonoMod.MonoModIgnore]
    public class CoreLibRelinker : RelinkTask
    {
        public event ResolveCoreLibHandler Resolve;

        public static void PostProcessCoreLib(params string[] inputs)
        {
            PostProcessCoreLib(null, inputs);
        }

        public static void PostProcessCoreLib(CoreLibRelinker task, params string[] inputs)
        {
            foreach (var input in inputs)
            {
                using var mm = new ModFwModder()
                {
                    InputPath = input,
                    OutputPath = Path.GetFileName(input),
                    MissingDependencyThrow = false,
                    //LogVerboseEnabled = true,
                    // PublicEverything = true, // this is done in setup

                    GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
                };
                mm.Log($"[OTAPI] Processing corelibs to be net5: {Path.GetFileName(input)}");

                var extractor = new ResourceExtractor();
                var embeddedResourcesDir = extractor.Extract(input);

                (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);

                mm.Read();

                mm.AddTask(task ?? new CoreLibRelinker());

                mm.MapDependencies();
                mm.AutoPatch();

                mm.Write();
            }
        }

        void PatchTargetFramework()
        {
            var tfa = Modder.Module.Assembly.CustomAttributes.SingleOrDefault(ca =>
                ca.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");

            if (tfa != null)
            {
                tfa.ConstructorArguments[0] = new CustomAttributeArgument(
                    tfa.ConstructorArguments[0].Type,
                    ".NETCoreApp,Version=v5.0"
                );
                var fdm = tfa.Properties.Single();
                tfa.Properties[0] = new CustomAttributeNamedArgument(
                    fdm.Name,
                    new CustomAttributeArgument(fdm.Argument.Type, "")
                );
            }
        }

        public override void Registered()
        {
            base.Registered();

            PatchTargetFramework();

            FixAttributes(Modder.Module.Assembly.CustomAttributes);
            FixAttributes(Modder.Module.Assembly.MainModule.CustomAttributes);

            foreach (var sd in Modder.Module.Assembly.SecurityDeclarations)
            {
                foreach (var sa in sd.SecurityAttributes)
                {
                    FixType(sa.AttributeType);

                    foreach (var prop in sa.Properties)
                        FixType(prop.Argument.Type);

                    foreach (var fld in sa.Fields)
                        FixType(fld.Argument.Type);
                }
            }
        }

        AssemblyNameReference ResolveSystemType(TypeReference type)
        {
            var searchType = type.FullName;

            var matches = SystemType.SystemTypes
                .Where(x => x.Type.FullName == searchType
                    && x.Assembly.Name.Name != "mscorlib"
                    && x.Assembly.Name.Name != "System.Private.CoreLib"
                )
                // pick the assembly with the highest version.
                // TODO: consider if this will ever need to target other fw's
                .OrderByDescending(x => x.Assembly.Name.Version);
            var match = matches.FirstOrDefault();

            if (match is not null)
            {
                // this is only needed for ilspy to pick up .net5 libs on osx
                var filename = Path.GetFileName(match.FilePath);
                if (!File.Exists(filename))
                    File.Copy(match.FilePath, filename);

                return match.AsNameReference();
            }
            return null;
        }

        AssemblyNameReference ResolveDependency(TypeReference type)
        {
            var depds = Modder.DependencyCache.Values
                .Select(m => new
                {
                    Module = m,
                    Types = m.Types.Where(x => x.FullName == type.FullName
                        && m.Assembly.Name.Name != "mscorlib"
                        && m.Assembly.Name.Name != "System.Private.CoreLib"
                        && x.IsPublic
                    )
                })
                .Where(x => x.Types.Any())
                // pick the assembly with the highest version.
                // TODO: consider if this will ever need to target other fw's
                .OrderByDescending(x => x.Module.Assembly.Name.Version); ;

            var first = depds.FirstOrDefault();
            if (first is not null)
            {
                return first.Module.Assembly.AsNameReference();
            }
            return null;
        }

        AssemblyNameReference ResolveRedirection(TypeReference type)
        {
            foreach (var mod in Modder.Mods)
            {

            }

            return null;
        }

        AssemblyNameReference ResolveAssembly(TypeReference type)
        {
            var res = Resolve?.Invoke(type);
            if (res is null)
            {
                if (type.Scope is AssemblyNameReference anr)
                {
                    var redirected = ResolveRedirection(type);
                    if (redirected is not null)
                        return redirected;

                    var dependencyMatch = ResolveDependency(type);
                    if (dependencyMatch is not null)
                        return dependencyMatch;

                    var systemMatch = ResolveSystemType(type);
                    if (systemMatch is not null)
                        return systemMatch;

                    throw new MissingMemberException();
                }
                else throw new NotImplementedException();
            }

            if (res.Name == "mscorlib" || res.Name == "System.Private.CoreLib")
                throw new NotSupportedException();

            return res;
        }

        void FixType(TypeReference type)
        {
            if (type.IsNested)
            {
                FixType(type.DeclaringType);
            }
            else if (type is TypeSpecification ts)
            {
                FixType(ts.ElementType);
            }
            else if (type is GenericParameter gp)
            {
                FixAttributes(gp.CustomAttributes);

                foreach (var prm in gp.GenericParameters)
                    FixType(prm);
            }
            else if (type.Scope.Name == "mscorlib"
                || type.Scope.Name == "netstandard"
                || type.Scope.Name == "System.Private.CoreLib"
            )
            {
                var asm = ResolveAssembly(type);

                var existing = type.Module.AssemblyReferences.SingleOrDefault(x => x.Name == asm.Name);
                if (existing != null)
                {
                    type.Scope = existing;
                }
                else
                {
                    type.Scope = asm;
                    type.Module.AssemblyReferences.Add(asm);
                }
            }
        }

        public void Relink(Instruction instr)
        {
            if (instr.Operand is MethodReference mref)
            {
                if (mref is GenericInstanceMethod gim)
                    FixType(gim.ElementMethod.DeclaringType);
                else
                    FixType(mref.DeclaringType);

                FixType(mref.ReturnType);

                foreach (var prm in mref.Parameters)
                {
                    FixType(prm.ParameterType);

                    FixAttributes(prm.CustomAttributes);
                }
            }
            else if (instr.Operand is FieldReference fref)
            {
                FixType(fref.DeclaringType);
                FixType(fref.FieldType);
            }
            else if (instr.Operand is TypeSpecification ts)
            {
                FixType(ts.ElementType);
            }
            else if (instr.Operand is TypeReference tr)
            {
                FixType(tr);
            }
            else if (instr.Operand is VariableDefinition vd)
            {
                FixType(vd.VariableType);
            }
            else if (instr.Operand is ParameterDefinition pd)
            {
                FixType(pd.ParameterType);
            }
            else if (instr.Operand is Instruction[] instructions)
            {
                foreach (var ins in instructions)
                    Relink(ins);
            }
            else if (!(
                instr.Operand is null
                || instr.Operand is Instruction
                || instr.Operand is Int16
                || instr.Operand is Int32
                || instr.Operand is Int64
                || instr.Operand is UInt16
                || instr.Operand is UInt32
                || instr.Operand is UInt64
                || instr.Operand is string
                || instr.Operand is byte
                || instr.Operand is sbyte
                || instr.Operand is Single
                || instr.Operand is Double
            ))
            {
                throw new NotSupportedException();
            }
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            base.Relink(body, instr);

            Relink(instr);
        }

        public override void Relink(TypeDefinition type)
        {
            base.Relink(type);

            if (type.BaseType != null)
                FixType(type.BaseType);
        }

        public override void Relink(EventDefinition typeEvent)
        {
            base.Relink(typeEvent);
            FixType(typeEvent.EventType);
        }

        public override void Relink(FieldDefinition field)
        {
            base.Relink(field);
            FixType(field.FieldType);
        }

        void FixAttributes(Collection<CustomAttribute> attributes)
        {
            foreach (var attr in attributes)
            {
                FixType(attr.AttributeType);

                foreach (var ca in attr.ConstructorArguments)
                    FixType(ca.Type);

                foreach (var fld in attr.Fields)
                {
                    FixType(fld.Argument.Type);
                }

                foreach (var prop in attr.Properties)
                    FixType(prop.Argument.Type);
            }
        }

        public override void Relink(MethodDefinition method)
        {
            base.Relink(method);

            foreach (var prm in method.Parameters)
                FixType(prm.ParameterType);

            FixAttributes(method.CustomAttributes);
        }

        public override void Relink(MethodDefinition method, ParameterDefinition parameter)
        {
            base.Relink(method, parameter);
            FixType(parameter.ParameterType);
        }

        public override void Relink(MethodDefinition method, VariableDefinition variable)
        {
            base.Relink(method, variable);
            FixType(variable.VariableType);
        }

        public override void Relink(PropertyDefinition property)
        {
            base.Relink(property);
            FixType(property.PropertyType);
        }
    }
}
