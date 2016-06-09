#Development
		
####How it works
This version of OTAPI aims to make the project as simple and clean as possible. In order to try and acheive this the project is split into the following components:

- A core that handles data from callbacks that are injected into the terraria assembly.
- A xna shim library for allowing the server code to run on mono.
- A patcher api that provides:
  - an engine to run external modifications of a .net assembly using the Mono.Cecil library.
  - an implementation that defines the OTAPI specific callback modifications and patches.
- A test console application project for hook examples and testing

In addition to these components we have decided that we wish to try and give more control to projects that use our API. To do this we combine both the terraria assembly and the OTAPI assembly using using ILRepack to bundle them into one library, named OTAPI.dll. This dll is then bundled along side the pdb and xml files into a NuGet package for developers.

####Debugging OTAPI
**Setup**: Even though ILRepack can provide us a pdb we find that when we are debugging it is easier to reference the OTAPI.Core.dll and patched terraria assembly separately. When we are happy we switch back to the NuGet package to keep things clean for repo cloners.
