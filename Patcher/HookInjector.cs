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
             * These must then be fitered with the current SupportType and ordered as appropriate.
             */
            var hooks = typeof(Injector)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Select(mth => new
                {
                    Attribute = mth.GetCustomAttribute<OTAPatchAttribute>(false),
                    Method = mth
                })
                .Where(patch => patch.Attribute != null && (patch.Attribute.SupportedTypes & currentType) != 0)
                .OrderBy(o => o.Attribute.Order)
                .ToArray();

            //Run the patches
            var dbg = new System.Diagnostics.Stopwatch();
            var col = Console.ForegroundColor;
            for (var x = 0; x < hooks.Length; x++)
            {
                dbg.Reset();
                Console.Write("\t{0}/{1} - {2}", x + 1, hooks.Length, hooks[x].Attribute.Text);

                dbg.Start();
                try
                {
                    hooks[x].Method.Invoke(this, null); //TODO get a boolean from this to determine the OK/FAIL

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" [OK] ");
                    Console.ForegroundColor = col;
                    dbg.Stop();

                    Console.WriteLine("Took {0}ms", dbg.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    dbg.Stop();

                    if (e is TargetInvocationException) e = (e as TargetInvocationException).InnerException;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" [FAIL]\n");
                    Console.WriteLine("\t\t{0}", e.Message);
                    Console.ForegroundColor = col;
                }
            }
            Console.ResetColor();
        }
    }
}

