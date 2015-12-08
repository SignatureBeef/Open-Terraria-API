using System;
using Mono.Cecil;

namespace OTA.Patcher
{
    public abstract class Organiser
    {
        protected AssemblyDefinition _asm;

        public Organiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        public TypeSystem TypeSystem
        {
            get
            { return _asm.MainModule.TypeSystem; }
        }

        public TypeReference Import(TypeReference typeReference)
        {
            return _asm.MainModule.Import(typeReference);
        }

        public MethodReference Import(MethodReference methodReference)
        {
            return _asm.MainModule.Import(methodReference);
        }

        public FieldReference Import(FieldReference fieldReference)
        {
            return _asm.MainModule.Import(fieldReference);
        }

        public void ForEachInstruction(Action<MethodDefinition, Mono.Cecil.Cil.Instruction> callback)
        {
            foreach (var type in _asm.MainModule.Types)
            {
                foreach (var mth in type.Methods)
                {
                    if (mth.HasBody)
                    {
                        foreach (var ins in mth.Body.Instructions.ToArray())
                            callback.Invoke(mth, ins);
                    }
                }
            }
        }
    }
}

