## Modification development

For the most part scripts that are 'completed' are stored as single files in the OTAPI.Scripts directory.
<br/>
However this structure is a real pain when developing new modifications, so this directory is designed specifically
so that your modification debug symbols are picked up, and you can then develop the modification with
the debugger so you can inspect and check variables, as one would normally do.

## Dev Modifications vs Completed Modifications

Completed modifications are C# top level scripts, stored in OTAPI.Scripts.
<br/>
Dev modifications on the other hand need to be a normal class with a static method.
When the modification is completed, move it to OTAPI.Scripts, remove the wrapper class and remove the static accessor
as a top level script is automatically static.