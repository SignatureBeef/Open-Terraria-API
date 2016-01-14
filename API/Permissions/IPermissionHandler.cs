using System;

namespace OTA.Permissions
{
    public interface IPermissionHandler
    {
        Permission GetPlayerPermission(OTA.Command.ISender sender, string node);
    }
}

