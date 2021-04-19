## Modification development

Modifications are C# top level scripts, stored in Modifications
MonoMod patches are class based scripts, stored in Patches

### Debugging

The ModFramework.Modules plugin will compile these scripts using Roslyn, and will also produce the symbols at runtime as well.
You should be able to set a breakpoint and run the patcher and, assuming the .cs file copied to the output directory, will
then allow it to be hit and you can debug as normal.