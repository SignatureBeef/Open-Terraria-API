using System;
using System.Linq;
using System.Reflection;

namespace OTA.Patcher
{
    public partial class Injector
    {
        /// <summary>
        /// Grabs all available hooks and executes them to hook into the target assembly
        /// </summary>
        public void InjectHooks(SupportType currentType)
        {
            /*
             * Gather all methods marked with the OTAPatchAttribute.
             * These must then be fitered with the current SupportType, and order as appropriate.
             */
            var hooks = typeof(Injector)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new
                {
                    Attribute = x
                                .GetCustomAttributes(typeof(OTAPatchAttribute), false)
                                .Select(a => a as OTAPatchAttribute)
                                .SingleOrDefault(y => (y.SupportedTypes & currentType) != 0),
                        
                    Method = x
                })
                .Where(z => z.Attribute != null)
                .OrderBy(o => o.Attribute.Order)
                .ToArray();

            //Run the patches
            Console.WriteLine("Running the patches...");
            for (var x = 0; x < hooks.Length; x++)
            {
                Console.Write("\t{0}/{1} - {2}", x + 1, hooks.Length, hooks[x].Attribute.Text);

                hooks[x].Method.Invoke(this, null);
                Console.WriteLine(" [OK]");
            }
            Console.WriteLine("All patches ran.");
        }
    }
}

