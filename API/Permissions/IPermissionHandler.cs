using System;

namespace OTA.Permissions
{
    public interface IPermissionHandler
    {
        Permission GetPlayerPermission(ISender sender, string node);
    }
}

