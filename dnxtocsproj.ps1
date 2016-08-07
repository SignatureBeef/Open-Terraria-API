$lines = @();

[IO.File]::ReadAllLines("dnxtocsproj.ps1") | Where {$_.StartsWith("#@")} | ForEach-Object{$lines += $_.Remove(0, 2)}
$template = [String]::Join([Environment]::NewLine, $lines);

#$saveTo = [IO.Path]::Combine([Environment]::CurrentDirectory, "Test.csproj");
#Write-Host "Saving to $saveTo, $template"
#[IO.File]::WriteAllLines($saveTo, $lines);

$lookIn = [IO.Path]::Combine([Environment]::CurrentDirectory, "OTAPI.Modifications");
Write-Host "Creating projects in $lookIn"
$directory = [IO.Directory]::GetDirectories($lookIn);

ForEach($directory in $directory)
{
    $name = [IO.Path]::GetFileName($directory);
    $csproj = [IO.Path]::Combine($directory, $name + ".csproj");
    $packages = [IO.Path]::Combine($directory, "packages.config");
    $compile = "";
    $templateClone = $template;

	ForEach($removeItem in @(($name + ".xproj"), ($name + ".xproj.user"), ("project.json"), ("project.lock.json")))
	{
		$lookFor = [IO.Path]::Combine($directory, $removeItem);
		If(Test-Path $lookFor)
		{
			Remove-Item $lookFor
		}
	}

    If(!($name -eq "OTAPI.Modifications.Core"))
    {
        #Write-Host $csproj

        $obj = [IO.Path]::Combine($directory, "obj");
        $bin = [IO.Path]::Combine($directory, "bin");

        ForEach($file in [IO.Directory]::EnumerateFiles($directory, "*.cs", [System.IO.SearchOption]::AllDirectories))
        {
            If(!$file.StartsWith($obj))
            {
                If(!$file.StartsWith($bin))
                {
                    If([IO.Path]::GetExtension($file) -eq ".cs")
                    {
                        $relative = $file.Remove(0, $directory.Length + 1);
                        #Write-Host $relative
                        $compile = $compile + "<Compile Include=`"$relative`" />" + [Environment]::NewLine + "    ";
                    }
                }
            }
        }

        $templateClone = $templateClone.Replace("~NAMESPACE~", $name);
        $templateClone = $templateClone.Replace("~COMPILE_SECTION~", $compile.Trim());
        $templateClone = $templateClone.Replace("~PROJECT_GUID~", [Guid]::NewGuid().ToString().ToUpper());
        [IO.File]::WriteAllText($csproj, $templateClone);

        $packagesConfig = "<?xml version=`"1.0`" encoding=`"utf-8`"?>" + [Environment]::NewLine + "<packages>" + [Environment]::NewLine + "    <package id=`"Mono.Cecil`" version=`"0.9.6.4`" targetFramework=`"net451`" />" + [Environment]::NewLine + "</packages>";
        [IO.File]::WriteAllText($packages, $packagesConfig);
    }
}

####CSPROJ TEMPLATE FOR MODIFICATIONS
#@<?xml version="1.0" encoding="utf-8"?>
#@<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
#@	<PropertyGroup>
#@		<Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
#@		<Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
#@		<ProjectGuid>{~PROJECT_GUID~}</ProjectGuid>
#@    <OutputType>Library</OutputType>
#@		<RootNamespace>~NAMESPACE~</RootNamespace>
#@		<AssemblyName>~NAMESPACE~</AssemblyName>
#@		<TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
#@	</PropertyGroup>
#@	<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
#@		<DebugSymbols>true</DebugSymbols>
#@		<DebugType>full</DebugType>
#@		<Optimize>false</Optimize>
#@		<OutputPath>bin\Debug</OutputPath>
#@		<DefineConstants>DEBUG;</DefineConstants>
#@		<ErrorReport>prompt</ErrorReport>
#@		<WarningLevel>4</WarningLevel>
#@		<DocumentationFile>bin\Debug\~NAMESPACE~.xml</DocumentationFile>
#@	</PropertyGroup>
#@	<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
#@		<Optimize>true</Optimize>
#@		<OutputPath>bin\Release</OutputPath>
#@		<ErrorReport>prompt</ErrorReport>
#@		<WarningLevel>4</WarningLevel>
#@		<ConsolePause>false</ConsolePause>
#@		<DocumentationFile>bin\Release\~NAMESPACE~.xml</DocumentationFile>
#@	</PropertyGroup>
#@  <ItemGroup>
#@    <Reference Include="Mono.Cecil, Version=0.9.6.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756">
#@      <HintPath>..\..\packages\Mono.Cecil.0.9.6.4\lib\net45\Mono.Cecil.dll</HintPath>
#@    </Reference>
#@    <Reference Include="Mono.Cecil.Mdb, Version=0.9.6.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756">
#@      <HintPath>..\..\packages\Mono.Cecil.0.9.6.4\lib\net45\Mono.Cecil.Mdb.dll</HintPath>
#@    </Reference>
#@    <Reference Include="Mono.Cecil.Pdb, Version=0.9.6.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756">
#@      <HintPath>..\..\packages\Mono.Cecil.0.9.6.4\lib\net45\Mono.Cecil.Pdb.dll</HintPath>
#@    </Reference>
#@    <Reference Include="Mono.Cecil.Rocks, Version=0.9.6.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756">
#@      <HintPath>..\..\packages\Mono.Cecil.0.9.6.4\lib\net45\Mono.Cecil.Rocks.dll</HintPath>
#@    </Reference>
#@    <Reference Include="System" />
#@    <Reference Include="TerrariaServer">
#@      <HintPath>..\..\wrap\TerrariaServer\TerrariaServer.exe</HintPath>
#@    </Reference>
#@  </ItemGroup>
#@  <ItemGroup>
#@    ~COMPILE_SECTION~
#@  </ItemGroup>
#@  <ItemGroup>
#@    <ProjectReference Include="..\..\OTAPI.Patcher.Engine\OTAPI.Patcher.Engine.csproj">
#@      <Project>{A1F792B2-5D80-4DE4-B5DB-7A05DBEABD60}</Project>
#@      <Name>OTAPI.Patcher.Engine</Name>
#@    </ProjectReference>
#@    <ProjectReference Include="..\OTAPI.Modifications.Core\OTAPI.Modifications.Core.csproj">
#@      <Project>{D9439E01-19C1-4E89-9B33-2C19C804DDCF}</Project>
#@      <Name>OTAPI.Modifications.Core</Name>
#@    </ProjectReference>
#@  </ItemGroup>
#@  <ItemGroup>
#@    <Content Include="packages.config" />
#@  </ItemGroup>
#@  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
#@  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.
#@       Other similar extension points exist, see Microsoft.Common.targets.
#@  <Target Name="BeforeBuild">
#@  </Target>
#@  <Target Name="AfterBuild">
#@  </Target>
#@  -->
#@</Project>