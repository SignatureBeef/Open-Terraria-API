COPY ..\\artifacts\\patcher\\OTAPI.dll package\\lib\\net451\\OTAPI.dll 
COPY ..\\artifacts\\patcher\\OTAPI.pdb package\\lib\\net451\\OTAPI.pdb
COPY ..\\artifacts\\patcher\\OTAPI.xml package\\lib\\net451\\OTAPI.xml

nuget pack OTAPI.dll.nuspec -BasePath package
PAUSE