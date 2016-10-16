$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";


Write-Host "Saving nuget.exe";
Try
{
    Invoke-WebRequest -Uri $nugetUrl -OutFile "nuget.exe"
}
Catch
{
    (New-Object System.Net.WebClient).DownloadFile($nugetUrl, "nuget.exe")
}

New-Item -ItemType directory -Path "package\lib\net451"