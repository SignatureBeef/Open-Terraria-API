namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Game
		{
			#region Handlers
			public delegate HookResult PreLoadContentHandler(global::Terraria.Main game);
			public delegate void PostPreLoadContent(global::Terraria.Main game);
			#endregion

			public static PreLoadContentHandler PreLoadContent;

			public static PostPreLoadContent PostLoadContent;
        }
    }
}
