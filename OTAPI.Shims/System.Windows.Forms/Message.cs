namespace System.Windows.Forms
{
    public struct Message
    {
        public IntPtr HWnd { get; set; }
        public int Msg { get; set; }
        public IntPtr WParam { get; set; }
        public IntPtr LParam { get; set; }
        public IntPtr Result { get; set; }
        public object GetLParam(Type cls) => null;
        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => default(Message);
        public override bool Equals(object o) => false;
        public static bool operator !=(Message a, Message b) => false;
        public static bool operator ==(Message a, Message b) => false;
        public override int GetHashCode() => 0;
        public override string ToString() => "";
    }
}
