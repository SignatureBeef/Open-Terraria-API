using OTA.Plugin;
using OTA.Command;
using OTA.Logging;

namespace BareBones
{
	[OTAVersion(1, 0)]
    public class YourPlugin : BasePlugin
    {
        public YourPlugin()
        {
            this.Version = "1";
            this.Author = "TDSM";
            this.Name = "Simple name";
            this.Description = "This plugin does these awesome things!";
        }

        protected override void Initialized(object state)
        {
			ProgramLog.Plugin.Log ("Your plugin is initialising");
        }

        [Hook(HookOrder.NORMAL)]
        void MyFunctionNameThatDoesntMatter(ref HookContext ctx, ref HookArgs.NpcKilled args)
        {
            //Your implementation
        }
    }
}

