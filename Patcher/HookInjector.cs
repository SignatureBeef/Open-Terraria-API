using System;
using System.Linq;

namespace OTA.Patcher
{
    public partial class Injector
    {
        /// <summary>
        /// Grabs all available hooks and executes them to hook into the target assembly
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void InjectHooks<T>()
        {
            var hooks = typeof(Injector)
                .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(T), false).Count() == 1)
                .ToArray();

            string line = null;

            for (var x = 0; x < hooks.Length; x++)
            {
                const String Fmt = "Patching in hooks - {0}/{1}";

                if (line != null)
                    ConsoleHelper.ClearLine();

                line = String.Format(Fmt, x + 1, hooks.Length);
                Console.Write(line);

                hooks[x].Invoke(this, null);
            }

            //Clear ready for the Ok\n
            if (line != null)
                ConsoleHelper.ClearLine();
            Console.Write("Patching in hooks - ");
        }
    }
}

