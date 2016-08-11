namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Lighting
        {
            #region Handlers
            public delegate HookResult SwipeHandler(object lightingSwipeData);
            #endregion
			
            public static SwipeHandler Swipe;
        }
    }
}
