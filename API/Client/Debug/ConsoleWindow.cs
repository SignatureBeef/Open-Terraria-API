using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OTA.Client.Debug
{
    public partial class ConsoleWindow : Form, OTA.Logging.ILogger
    {
        class RichTextboxWriter : TextWriter
        {
            private RichTextBox _textbox;

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            public RichTextboxWriter(RichTextBox textbox)
            {
                _textbox = textbox;

                _textbox.ReadOnly = true;
            }

            public override void Write(char value)
            {
                Append(value.ToString());
            }

            public override void Write(char[] buffer)
            {
                Append(new string(buffer));
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Append(new string(buffer, index, count));
            }

            public override void Write(string value)
            {
                Append(value);
            }

            protected void Append(string line)
            {
                _textbox.AppendText(line);
            }
        }

        class ConsoleReceiver : OTA.Logging.InteractiveLogTarget
        {
            public ConsoleReceiver(RichTextboxWriter writer) : base("Debug Window", writer)
            {
                OTA.Logging.ProgramLog.AddTarget(this);
            }
        }
        private ConsoleReceiver _receiver;

        public ConsoleWindow()
        {
            InitializeComponent();
            //OTA.Logging.Logger.AddLogger(this);
        }

        public void Log(string category, TraceLevel level, string message, ConsoleColor? colour = default(ConsoleColor?))
        {
            RtbConsole.Text += String.Format("{0} {2}: {1}", category, message, level);
        }

        private void DebugWindow_Load(object sender, EventArgs e)
        {
            _receiver = new ConsoleReceiver(new RichTextboxWriter(RtbConsole));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _receiver.Close();
            _receiver = null;
        }
    }
}
