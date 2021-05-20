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
    public class CoreLibRelinker : RelinkTask
    {
        class SystemType
        {
            public string FilePath { get; set; }
            public AssemblyDefinition Assembly { get; set; }
            public ExportedType Type { get; set; }
        }
        static IEnumerable<SystemType> SystemTypes { get; set; } = GetSystemType();

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

        public static void PostProcessCoreLib(params string[] inputs)
        {
            Plugins.PluginLoader.Clear();

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

                mm.AddTask(new CoreLibRelinker());

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

        void FixType(TypeReference type)
        {
            if (type is TypeSpecification ts)
            {
                FixType(ts.ElementType);
            }
            //else if (type is PointerType ptr)
            //{
            //    FixType(ptr.ElementType);
            //}
            //else if (type is GenericInstanceType git)
            //{
            //    FixType(git.ElementType);
            //}
            //else if (type is ByReferenceType brt)
            //{
            //    FixType(brt.ElementType);
            //}
            //else if (type is ArrayType at)
            //{
            //    FixType(at.ElementType);
            //}
            else if (type is GenericParameter gp)
            {
                FixAttributes(gp.CustomAttributes);

                foreach (var prm in gp.GenericParameters)
                    FixType(prm);
            }
            else if (type.Scope.Name == "mscorlib" || type.Scope.Name == "System.Private.CoreLib")
            {
                var searchType = type.FullName;

                var match = SystemTypes
                    .Where(x => x.Type.FullName == searchType)
                    // pick the assembly with the highest version.
                    // TODO: consider if this will ever need to target other fw's
                    .OrderByDescending(x => x.Assembly.Name.Version)
                    .FirstOrDefault();

                if (match != null)
                {
                    if (type.Scope is AssemblyNameReference anr)
                    {
                        var existing = type.Module.AssemblyReferences.SingleOrDefault(x => x.Name == match.Assembly.Name.Name);

                        if (existing != null)
                        {
                            type.Scope = existing;
                        }
                        else
                        {
                            var version = match.Assembly.Name.Version;
                            var newref = new AssemblyNameReference(match.Assembly.Name.Name, version)
                            {
                                PublicKey = match.Assembly.Name.PublicKey,
                                PublicKeyToken = match.Assembly.Name.PublicKeyToken,
                                Culture = match.Assembly.Name.Culture,
                                Hash = match.Assembly.Name.Hash,
                                HashAlgorithm = match.Assembly.Name.HashAlgorithm,
                                Attributes = match.Assembly.Name.Attributes
                            };
                            type.Scope = newref;
                            type.Module.AssemblyReferences.Add(newref);

                            // this is only needed for ilspy to pick up .net5 libs on osx
                            var filename = Path.GetFileName(match.FilePath);
                            if (!File.Exists(filename))
                                File.Copy(match.FilePath, filename);
                        }
                    }
                    else throw new NotImplementedException();
                }
                else throw new NotImplementedException();
            }
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            base.Relink(body, instr);

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

            if (instr.Operand is FieldReference fref)
            {
                FixType(fref.DeclaringType);
                FixType(fref.FieldType);
            }
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
