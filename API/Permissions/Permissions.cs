using System;
using OTA.Command;

namespace OTA.Permissions
{
    public static class Permissions
    {
        private static IPermissionHandler _handler;
        private static readonly object _sync = new object();

        public static void SetHandler(IPermissionHandler handler)
        {
            lock (_sync) _handler = handler;
        }

        public static Permission GetPermission(ISender sender, string node)
        {
            lock (_sync)
            {
                if (_handler != null) return _handler.GetPlayerPermission(sender, node);
            }

            return Permission.Permitted;
        }
    }

    public enum Permission : int
    {
        NoPermission = 1,
        Permitted = 2,
        Denied = 3
    }
}