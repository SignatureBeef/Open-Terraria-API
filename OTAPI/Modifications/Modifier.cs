using OTAPI.Modifications;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace OTAPI.Modifications
{
    //[MonoModTargetModule("")]
    public class Modifier
    {
        public void Discover()
        {

        }

        public void Apply(string modificationNamespace, MonoMod.MonoModder modder = null)
        {
            var category = modificationNamespace.Replace("OTAPI.Modifications.", String.Empty);

            var attr = typeof(ModificationAttribute);
            var asm = Assembly.GetExecutingAssembly();

            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            var modifications = types.Where(x => x != null && !x.IsAbstract && x.Namespace == modificationNamespace);

            foreach (var type in modifications)
            {
                var modificationAttr = type.CustomAttributes.SingleOrDefault(a => a.AttributeType == attr);
                if (modificationAttr != null)
                {
                    Console.WriteLine($"[OTAPI] [{category}] {(string)modificationAttr.ConstructorArguments[0].Value}");

                    if (type.GetConstructors().Single().GetParameters().Count() == 1)
                    {
                        Activator.CreateInstance(type, modder);
                    }
                    else Activator.CreateInstance(type);
                }
                //modificationAttr
            }
        }
    }
}
