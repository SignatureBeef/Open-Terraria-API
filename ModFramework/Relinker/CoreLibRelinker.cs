using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ModFramework.Relinker
{
    [MonoMod.MonoModIgnore]
    public class CoreLibRelinker : RelinkTask
    {
        private AssemblyNameReference RuntimeRef;

        public CoreLibRelinker()
        {
            var systemRuntime = AppDomain.CurrentDomain.GetAssemblies().Single(mr => mr.GetName().Name == "netstandard");

            RuntimeRef = new AssemblyNameReference(systemRuntime.GetName().Name, systemRuntime.GetName().Version)
            {
                PublicKeyToken = systemRuntime.GetName().GetPublicKeyToken()
            };
        }

        public override void Registered()
        {
            base.Registered();

            foreach (var reference in Modder.Module.AssemblyReferences
                .Where(x => x.Name.StartsWith("mscorlib") || x.Name.StartsWith("System.Private.CoreLib"))
                .ToArray()
            )
            {
                reference.Name = RuntimeRef.Name;
                reference.Version = RuntimeRef.Version;
                reference.PublicKey = RuntimeRef.PublicKey;
                reference.PublicKeyToken = RuntimeRef.PublicKeyToken;
            }
        }

        //TypeReference FixType(TypeReference type)
        //{
        //    if (type.Scope.Name == "mscorlib")
        //    {
        //        type.Scope = RuntimeRef;
        //    }

        //    if (type.Scope.Name == "System.Private.CoreLib")
        //    {
        //        type.Scope = RuntimeRef;
        //    }

        //    return type;
        //}

        //public override void Relink(MethodBody body, Instruction instr)
        //{
        //    base.Relink(body, instr);

        //    if (instr.Operand is MethodReference mref)
        //    {
        //        FixType(mref.DeclaringType);
        //    }
        //}

        //public override void Relink(EventDefinition typeEvent)
        //{
        //    base.Relink(typeEvent);
        //}

        //public override void Relink(FieldDefinition field)
        //{
        //    base.Relink(field);
        //}

        //public override void Relink(MethodDefinition method)
        //{
        //    base.Relink(method);
        //}

        //public override void Relink(MethodDefinition method, ParameterDefinition parameter)
        //{
        //    base.Relink(method, parameter);
        //}

        //public override void Relink(MethodDefinition method, VariableDefinition variable)
        //{
        //    base.Relink(method, variable);
        //}

        //public override void Relink(PropertyDefinition property)
        //{
        //    base.Relink(property);
        //}
    }
}
