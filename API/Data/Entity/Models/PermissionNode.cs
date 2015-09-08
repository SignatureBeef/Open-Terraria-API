using System;

namespace OTA.Data.Entity.Models
{
    public class PermissionNode
    {
        public int Id { get; set; }

        public string Node { get; set; }

        public Permission Permission { get; set; }
    }
}

