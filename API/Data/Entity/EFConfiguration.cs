using System;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using OTA.Data.Entity.SQLite;
using System.Data.Entity;

namespace OTA.Data.Entity
{
//    class EFConfiguration : DbConfiguration
//    {
//        public static string ProviderName { get; set; }
//
//        public static DbProviderServices ProviderService { get; set; }
//
//        public static DbProviderFactory ProviderFactory { get; set; }
//
//                public static DbConfiguration ProviderConfiguration { get; set; }
//
//        public EFConfiguration()
//        {
//            if (ProviderFactory != null && !String.IsNullOrEmpty(ProviderName))
//            {
//                Console.WriteLine("Setting {0} as {1}", ProviderName, ProviderFactory.GetType().FullName);
//                this.SetProviderFactory(ProviderName, ProviderFactory);
//            }
//            if (ProviderService != null && !String.IsNullOrEmpty(ProviderName))
//            {
//                Console.WriteLine("Setting {0} as {1}", ProviderName, ProviderService.GetType().FullName);
//                this.SetProviderServices(ProviderName, ProviderService);
//            }
//            if (ProviderFactory != null)
//            {
//                this.SetDefaultConnectionFactory(OTAConnectionFactory.Instance);
//            }
//            this.SetProviderFactoryResolver(new SQLiteProviderFactoryResolver());
//        }
//    }
}

