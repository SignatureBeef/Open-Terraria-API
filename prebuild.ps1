#Use this script to prepare your OTAPI solution for building when you have a fresh clone
#To execute, run the following line in the Package Manager Console window in Visual Studio
#	Invoke-Expression .\prebuild.ps1

#OTAPI expected paths
$saveFile = "wrap\TerrariaServer\TerrariaServer.exe"
$zipSavePath = "wrap\TerrariaServer\TerrariaServer.zip"

#Gets the working path for the current script being executed
$workingDirectory = Split-Path -parent $PSCommandPath;

#Generate the full paths
$saveFile = [System.IO.Path]::Combine($workingDirectory, $saveFile)
$zipSavePath = [System.IO.Path]::Combine($workingDirectory, $zipSavePath)

#Remove the Initial TerrariaServer.exe
If(Test-Path $saveFile)
{
	Remove-Item $saveFile
}

#Remove any existing temp zips
If(Test-Path $zipSavePath)
{
	Remove-Item $zipSavePath
}

#Fetch the download url from the terraria.org website
$terrariaHtml = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($saveFile), "terraria_org.html");
If(Test-Path $terrariaHtml)
{
	Remove-Item $terrariaHtml
}
Write-Host "Saving terraria.org html to $terrariaHtml";
Invoke-WebRequest -Uri "https://terraria.org" -OutFile $terrariaHtml

#Find the zip url in the html content
$html = [System.IO.File]::ReadAllText($terrariaHtml);
$indexor = $html.LastIndexOf("'>Dedicated Server</a>")
$html = $html.Substring(0, $indexor);
$indexor = $html.LastIndexOf("'")
$html = $html.Remove(0, $indexor + 1);
#Found it, now prepare the variable so the download method can use it
$downloadUrl = "https://terraria.org/" + $html;

#Download the latest zip
Write-Host "Downloading $downloadUrl";
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipSavePath

#Import zip namespaces
[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression");
[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem");

Write-Host "Extracting TerrariaServer.exe from $zipFullPath"
#Open the zip archive
$zip = [System.IO.Compression.ZipFile]::OpenRead($zipSavePath);

#Get the particular .exe we are after
$entry = $zip.GetEntry("Dedicated Server/Windows/TerrariaServer.exe");

#Write the new zip to disk
$output = New-Object IO.StreamWriter $saveFile;
$input = $entry.Open();
$input.CopyTo($output.BaseStream);
$input.Dispose();
$output.Dispose();

#Cleanup zips
Write-Host "Cleaning up"
$zip.Dispose();
Remove-Item $terrariaHtml;
Remove-Item $zipSavePath;

Write-Host "Setup complete"