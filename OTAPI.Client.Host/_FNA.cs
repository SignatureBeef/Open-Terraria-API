using System;
using System.IO;
using System.Runtime.InteropServices;

#if FNA
public static class Bootstrap
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    public static void Initialize_FNA()
    {
        // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            SetDllDirectory(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.Is64BitProcess ? "x64" : "x86"
            ));
        }

        // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
        // NOTE: from documentation: 
        //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
        //           <key>NSHighResolutionCapable</key>
        //           <string>True</string>
        Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");
    }
}
#endif
