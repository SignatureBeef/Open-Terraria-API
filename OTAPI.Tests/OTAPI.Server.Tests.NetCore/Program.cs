using System;

namespace OTAPI.Server.Tests.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            OTAPI.Tests.Common.Server.Start(false, args); // false because .net core cannot load Steamworks
        }
    }
}
