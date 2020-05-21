Copy-Item ..\LICENSE.txt .\package
Copy-Item ..\OTAPI.dll .\package\lib\net451
Copy-Item ..\OTAPI.pdb .\package\lib\net451
Copy-Item ..\OTAPI.xml .\package\lib\net451

Start-Process -FilePath "nuget" -ArgumentList "pack OTAPI.dll.nuspec -BasePath package"