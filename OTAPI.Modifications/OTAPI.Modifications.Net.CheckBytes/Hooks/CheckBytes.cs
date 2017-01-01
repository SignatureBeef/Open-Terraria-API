namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookResult CheckBytesHandler(ref int bufferIndex);
            #endregion
			
            public static CheckBytesHandler CheckBytes;
        }
    }
}
