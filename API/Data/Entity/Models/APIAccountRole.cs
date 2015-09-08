using System;

namespace OTA.Data.Entity.Models
{
    public class APIAccountRole
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}

