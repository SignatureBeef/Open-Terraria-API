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
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;

namespace OTAPI
{
    public interface ICollection<TItem>
    {
        TItem this[int x, int y] { get; set; }
    }

    public class DefaultCollection<TItem> : ICollection<TItem>
    {
        protected TItem[,] _items;

        protected int Width { get; set; }
        protected int Height { get; set; }

        public DefaultCollection(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public virtual TItem this[int x, int y]
        {
            get
            {
                if (_items == null)
                    _items = new TItem[this.Width, this.Height];

                return _items[x, y];
            }
            set => _items[x, y] = value;
        }

        public static ICollection<TItem> CreateCollection(int width, int height)
        {
            return new DefaultCollection<TItem>(width, height);
        }
    }

    [MonoMod.MonoModIgnore]
    public class ArrayToCollectionRemapper
    {
        public TypeDefinition Type { get; set; }

        public TypeReference ICollectionRef { get; set; }
        public TypeDefinition ICollectionDef { get; set; }
        public GenericInstanceType ICollectionGen { get; set; }
        public GenericInstanceType ICollectionTItem { get; set; }
        public TypeReference CollectionRef { get; set; }
        public TypeDefinition CollectionDef { get; set; }
        public GenericInstanceType CollectionGen { get; set; }

        public MethodReference CreateCollectionMethod { get; set; }

        public ArrayToCollectionRemapper(TypeDefinition type, MonoModder modder)
        {
            this.Type = type;

            ICollectionRef = modder.FindType($"OTAPI.{nameof(OTAPI.ICollection<object>)}`1");
            CollectionRef = modder.FindType($"OTAPI.{nameof(OTAPI.DefaultCollection<object>)}`1");

            ICollectionDef = ICollectionRef.Resolve();
            CollectionDef = CollectionRef.Resolve();

            ICollectionGen = new GenericInstanceType(ICollectionRef);
            ICollectionTItem = new GenericInstanceType(ICollectionRef);
            CollectionGen = new GenericInstanceType(CollectionRef);

            ICollectionGen.GenericArguments.Clear();
            ICollectionGen.GenericArguments.Add(type);
            CollectionGen.GenericArguments.Clear();
            CollectionGen.GenericArguments.Add(type);
            ICollectionTItem.GenericArguments.Clear();
            ICollectionTItem.GenericArguments.Add(ICollectionDef.GenericParameters[0]);

            CreateCollectionMethod = type.Module.GetReference(() => DefaultCollection<Terraria.Tile>.CreateCollection(1, 1));
            CreateCollectionMethod.DeclaringType = CollectionGen;
            CreateCollectionMethod.ReturnType = ICollectionTItem;
        }

        [RemapHook]
        public void Remap(FieldDefinition field)
        {
            if (field.FieldType is ArrayType array)
            {
                if (array.ElementType.FullName == this.Type.FullName)
                {
                    field.FieldType = ICollectionGen;
                }
            }
        }

        [RemapHook]
        public void Remap(PropertyDefinition property)
        {
            if (property.PropertyType is ArrayType array)
            {
                if (array.ElementType.FullName == this.Type.FullName)
                {
                    property.PropertyType = ICollectionGen;
                }
            }
        }

        [RemapHook]
        public void RemapConstructors(Instruction instruction, MethodDefinition method)
        {
            if (instruction.OpCode == OpCodes.Newobj
                && instruction.Operand is MethodReference ctorMethod
            )
            {
                if (ctorMethod.DeclaringType is ArrayType array)
                {
                    if (array.ElementType.FullName == this.Type.FullName)
                    {
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = CreateCollectionMethod;
                    }
                }
            }
        }

        [RemapHook]
        public void RemapFields(Instruction instruction, MethodDefinition method)
        {
            if (instruction.Operand is FieldReference field
                && field.FieldType is ArrayType fieldArray
                && fieldArray.ElementType.FullName == this.Type.FullName
            )
            {
                if (field.DeclaringType is TypeDefinition declaringType)
                {
                    field.FieldType = this.ICollectionGen;
                }
            }
        }

        [RemapHook]
        public void RemapMethods(Instruction instruction, MethodDefinition method)
        {
            if (instruction.Operand is MethodReference methodRef
                && methodRef.DeclaringType is ArrayType methodArray
                && methodArray.ElementType.FullName == this.Type.FullName
            )
            {
                methodRef.DeclaringType = this.ICollectionGen;
                methodRef.ReturnType = methodRef.Name == "Get" ? this.ICollectionDef.GenericParameters[0] : methodRef.Module.TypeSystem.Void;

                if (methodRef.Name == "Set")
                {
                    methodRef.Parameters[2].ParameterType = this.ICollectionDef.GenericParameters[0];
                }
                methodRef.Name = $"{methodRef.Name.ToLower()}_Item";

                instruction.OpCode = OpCodes.Callvirt;
            }
        }
    }
}