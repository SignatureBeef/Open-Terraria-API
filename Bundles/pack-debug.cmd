COPY ..\\artifacts\\patcher\\OTAPI.Core.dll package-debug\\lib\\net451\\OTAPI.Core.dll 
COPY ..\\artifacts\\patcher\\OTAPI.Core.pdb package-debug\\lib\\net451\\OTAPI.Core.pdb
COPY ..\\artifacts\\patcher\\OTAPI.Core.xml package-debug\\lib\\net451\\OTAPI.Core.xml

COPY ..\\artifacts\\patcher\\OTAPI.Xna.dll package-debug\\lib\\net451\\OTAPI.Xna.dll 
COPY ..\\artifacts\\patcher\\OTAPI.Xna.pdb package-debug\\lib\\net451\\OTAPI.Xna.pdb
COPY ..\\artifacts\\patcher\\OTAPI.Xna.xml package-debug\\lib\\net451\\OTAPI.Xna.xml

COPY ..\\artifacts\\patcher\\TerrariaServer.dll package-debug\\lib\\net451\\TerrariaServer.dl

nuget pack OTAPI.debug.nuspec -BasePath package-debug
PAUSE