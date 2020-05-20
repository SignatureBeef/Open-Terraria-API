using System.Linq;

namespace OTAPI.Modifications.Patchtime
{
    [Modification("Shimming System.Windows.Forms")]
    [MonoMod.MonoModIgnore]
    class ShimWinForms
    {
        public ShimWinForms(MonoMod.MonoModder modder)
        {
            foreach (var reference in modder.Module.AssemblyReferences
                .Where(x => x.Name.StartsWith("System.Windows.Forms")).ToArray())
            {
                reference.Name = modder.Module.Assembly.Name.Name;
                reference.PublicKey = modder.Module.Assembly.Name.PublicKey;
                reference.PublicKeyToken = modder.Module.Assembly.Name.PublicKeyToken;
                reference.Version = modder.Module.Assembly.Name.Version;
            }
        }
    }
}