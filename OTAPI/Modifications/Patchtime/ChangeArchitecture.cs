namespace OTAPI.Modifications.Patchtime
{
    [Modification("Changing architecture to AnyCPU (64-bit preferred)")]
    [MonoMod.MonoModIgnore]
    class ChangeArchitecture
    {
        public ChangeArchitecture(MonoMod.MonoModder modder)
        {
            modder.Module.Architecture = Mono.Cecil.TargetArchitecture.I386;
            modder.Module.Attributes = Mono.Cecil.ModuleAttributes.ILOnly;
        }
    }
}
