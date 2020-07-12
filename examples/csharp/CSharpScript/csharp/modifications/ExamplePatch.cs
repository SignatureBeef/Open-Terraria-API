// below is an OTAPI patch. you can 
[Modification(ModType.PostPatch, "Testing a patch", ModPriority.Last)]
void Patch(MonoModder modder, IRelinkProvider relinkProvider)
{
    Console.WriteLine("Modder? " + (modder != null ? "yes" : "no"));
    Console.WriteLine("RelinkProvider? " + (relinkProvider != null ? "yes" : "no"));
}

// below is a monomod patch, it will be merged into the final assembly.
namespace Test
{
    public static class Tester
    {
        public const string abc = "123";
    }
}