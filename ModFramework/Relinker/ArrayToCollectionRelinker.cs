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
using ModFramework.Relinker;

namespace ModFramework.Relinker
{
    [MonoMod.MonoModIgnore]
    public class ArrayToCollectionRelinker : RelinkTask
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

        public ArrayToCollectionRelinker(TypeDefinition type, MonoModder modder)
        {
            this.Type = type;

            //ICollectionRef = modder.FindType($"ModFramework.{nameof(ModFramework.ICollection<object>)}`1");
            //CollectionRef = modder.FindType($"ModFramework.{nameof(ModFramework.DefaultCollection<object>)}`1");

            //var asdasd = modder.GetReference(() => ICollection<object>);
            ICollectionRef = modder.Module.ImportReference(typeof(ICollection<>));
            CollectionRef = modder.Module.ImportReference(typeof(DefaultCollection<>));


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

            CreateCollectionMethod = modder.GetReference(() => DefaultCollection<object>.CreateCollection(0, 0, ""));
            CreateCollectionMethod.ReturnType = ICollectionTItem;
            CreateCollectionMethod.DeclaringType = CollectionGen;

            System.Console.WriteLine($"[ModFw] Relinking to collection {type.FullName}=>{ICollectionDef.FullName}");
        }

        public override void Relink(FieldDefinition field)
        {
            if (field.FieldType is ArrayType array)
                if (array.ElementType.FullName == this.Type.FullName)
                    field.FieldType = ICollectionGen;
        }

        public override void Relink(PropertyDefinition property)
        {
            if (property.PropertyType is ArrayType array)
                if (array.ElementType.FullName == this.Type.FullName)
                    property.PropertyType = ICollectionGen;
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            if (body.Method.ReturnType is ArrayType arrayType && arrayType.ElementType.FullName == this.Type.FullName)
                body.Method.ReturnType = ICollectionGen;

            RelinkConstructors(body, instr);
            RemapFields(body, instr);
            RemapMethods(body, instr);
        }

        public void RelinkConstructors(MethodBody body, Instruction instr)
        {
            if (instr.OpCode == OpCodes.Newobj && instr.Operand is MethodReference ctorMethod)
            {
                if (ctorMethod.DeclaringType is ArrayType array)
                {
                    if (array.ElementType.FullName == this.Type.FullName)
                    {
                        body.GetILProcessor().InsertBefore(instr, Instruction.Create(OpCodes.Ldstr, body.Method.FullName));
                        instr.OpCode = OpCodes.Call;
                        instr.Operand = CreateCollectionMethod;
                    }
                }
            }
        }

        public void RemapFields(MethodBody body, Instruction instr)
        {
            if (instr.Operand is FieldReference field
                && field.FieldType is ArrayType fieldArray
                && fieldArray.ElementType.FullName == this.Type.FullName
            )
            {
                field.FieldType = this.ICollectionGen;
            }
        }

        public void RemapMethods(MethodBody body, Instruction instr)
        {
            if (instr.Operand is MethodReference methodRef)
            {
                if (methodRef.DeclaringType is ArrayType methodArray && methodArray.ElementType.FullName == this.Type.FullName
                )
                {
                    methodRef.DeclaringType = this.ICollectionGen;
                    methodRef.ReturnType = methodRef.Name == "Get" ? this.ICollectionDef.GenericParameters[0] : methodRef.Module.TypeSystem.Void;

                    if (methodRef.Name == "Set")
                    {
                        methodRef.Parameters[2].ParameterType = this.ICollectionDef.GenericParameters[0];
                    }
                    methodRef.Name = $"{methodRef.Name.ToLower()}_Item";

                    instr.OpCode = OpCodes.Callvirt;
                }

                if (methodRef.ReturnType is ArrayType arrayType && arrayType.ElementType.FullName == this.Type.FullName)
                {
                    methodRef.ReturnType = ICollectionGen;
                }
            }
        }
    }
}

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class ArrayToCollectionRelinkerMixin
    {
        public static void RelinkAsCollection(this TypeDefinition sourceType, IRelinkProvider relinkProvider, MonoModder modder)
        {
            relinkProvider.AddTask(new ArrayToCollectionRelinker(sourceType, modder));
        }
    }

    public interface ICollection<TItem>
    {
        TItem this[int x, int y] { get; set; }
        int Width { get; }
        int Height { get; }
    }

    public class DefaultCollection<TItem> : ICollection<TItem>
    {
        protected TItem[,] _items;

        public int Width { get; set; }
        public int Height { get; set; }

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

        public delegate ICollection<TItem> CreateCollectionHandler(int width, int height, string source);
        public static event CreateCollectionHandler OnCreateCollection;

        public static ICollection<TItem> CreateCollection(int width, int height, string source)
        {
            var collection = OnCreateCollection?.Invoke(width, height, source) ?? new DefaultCollection<TItem>(width, height);
            System.Console.WriteLine($"Created new {collection.Width}x{collection.Height} {collection.GetType().Name} for source: {source}");
            return collection;
        }
    }
}