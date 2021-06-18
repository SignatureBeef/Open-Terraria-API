using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModFramework.Modules.ClearScript.Typings
{
    public class TypeTraverser : IDisposable
    {
        public List<Type> Types = new List<Type>();

        private bool disposedValue;

        public TypeTraverser()
        {
        }

        public void AddAssembly(System.Reflection.Assembly assembly)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                types = rtle.Types;
            }

            var publicTypes = types.Where(t => t.IsPublic);

            foreach (var type in publicTypes)
                AddType(type);
        }

        static string GetCommonName(Type type)
        {
            var name = type.FullName;
            var ix = name.LastIndexOf('`');

            if(ix > -1)
            {
                name = name.Substring(0, ix);
                name += type.GetGenericArguments().Length.ToString();
            }

            return name;
        }

        public void AddType(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (type.FullName != null
                && !this.Types.Any(t => GetCommonName(t) == GetCommonName(type))
                && !type.IsByRef
                && !type.IsArray
                && !type.IsPointer
            )
            {
                this.Types.Add(type);

                foreach (var method in type.GetMethods()) //BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (method.Name.Contains("CSharpRail"))
                    {

                    }
                    if (method.ReturnType != type)
                    {
                        AddType(method.ReturnType);
                    }

                    foreach (var prm in method.GetParameters())
                    {
                        if (prm.ParameterType != type)
                        {
                            AddType(prm.ParameterType);
                        }
                    }
                }

                //foreach(var mem in type.GetMembers())
                //{
                //    AddType(mem.ReflectedType);
                //}

                foreach (var evt in type.GetEvents()) //BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (evt.EventHandlerType != type)
                    {
                        AddType(evt.EventHandlerType);

                        var invoke = evt.EventHandlerType.GetMethod("Invoke");
                        if (invoke != null)
                        {
                            foreach (var prm in invoke.GetParameters())
                            {
                                if (prm.ParameterType != type)
                                {
                                    AddType(prm.ParameterType);
                                }
                            }
                        }
                    }
                }

                if (type.BaseType != null)
                {
                    AddType(type.BaseType);
                }
            }
        }

        public void AddType<TType>() => AddType(typeof(TType));

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.Types.Clear();
                    this.Types = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TypingsGenerator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
