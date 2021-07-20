#Use this script to prepare your OTAPI solution for building when you have a fresh clone
#To execute, run the following line in the Package Manager Console window in Visual Studio (View->Other Windows->Package Manager Console)
#	Invoke-Expression .\prebuild.ps1

#OTAPI expected paths
$serverSaveFile = [IO.Path]::Combine("wrap", "TerrariaServer", "TerrariaServer.exe");
$zipSavePath = [IO.Path]::Combine("wrap", "TerrariaServer", "TerrariaServer.zip");

$clientSaveFile = "wrap\Terraria\Terraria.exe"
$clientSourcePath = "$((Get-ItemProperty HKLM:\SOFTWARE\WOW6432Node\re-logic\terraria\).exe_path)".Replace('/','\')

#Gets the working path for the current script being executed
Try
{
    $workingDirectory = Split-Path -parent $PSCommandPath;
}
Catch
{
    #Pash doesnt like here apparently, so since i'm using terminal for pash anyway
    #i don't actually need the above.
    $workingDirectory = [Environment]::CurrentDirectory;
}

#Generate the full paths
$serverSaveFile = [System.IO.Path]::Combine($workingDirectory, $serverSaveFile)
$zipSavePath = [System.IO.Path]::Combine($workingDirectory, $zipSavePath)
$clientSaveFile = [System.IO.Path]::Combine($workingDirectory, $clientSaveFile)
$clientSourcePath = [System.IO.Path]::Combine([Environment]::GetFolderPath([Environment+SpecialFolder]::ProgramFilesX86), $clientSourcePath);

#Remove any existing TerrariaServer.exe
If(Test-Path $serverSaveFile)
{
	#Remove-Item $serverSaveFile
}
#Remove any existing client Terraria.exe
If(Test-Path $clientSaveFile)
{
	Remove-Item $clientSaveFile
}

#Remove any existing temp zips
If(Test-Path $zipSavePath)
{
	Remove-Item $zipSavePath
}

$downloadUrl = "https://terraria.org/api/download/pc-dedicated-server/terraria-server-1423.zip";

#Download the latest zip
Write-Host "Downloading $downloadUrl";
Try
{
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipSavePath
}
Catch
{
    (New-Object System.Net.WebClient).DownloadFile($downloadUrl, $zipSavePath)
}

#Import zip namespaces
[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression");
[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem");

Write-Host "Extracting TerrariaServer.exe from $zipSavePath";
#Open the zip archive
$zip = [System.IO.Compression.ZipFile]::OpenRead($zipSavePath); #not working on PASH yet :c

#Get the particular .exe we are after
$entry = $zip.GetEntry("1423/Windows/TerrariaServer.exe");

#Write the new zip to disk
$output = New-Object IO.StreamWriter $serverSaveFile;
$input = $entry.Open();
$input.CopyTo($output.BaseStream);
$input.Dispose();
$output.Dispose();

#Cleanup zips
Write-Host "Cleaning up"
$zip.Dispose();
Remove-Item $zipSavePath;

#Process the client. No fancy web update here as people must have paid for it anyway.
Write-Host "Copying client exe"
[System.IO.File]::Copy($clientSourcePath, $clientSaveFile);

Write-Host "Setup complete"
