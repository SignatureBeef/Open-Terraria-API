namespace Microsoft.Xna.Framework.Input
{
    public struct GamePadState
    {
        public GamePadDPad DPad { get; set; }
        public GamePadTriggers Triggers { get; set; }
        public GamePadThumbSticks ThumbSticks { get; set; }
        public bool IsConnected { get; }

        public bool IsButtonDown(Buttons button) => false;
        public bool IsButtonUp(Buttons button) => false;
    }
}