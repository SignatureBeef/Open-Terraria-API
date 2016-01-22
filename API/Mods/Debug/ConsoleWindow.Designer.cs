namespace OTA.Mod.Debug
{
    partial class ConsoleWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RtbConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // RtbConsole
            // 
            this.RtbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RtbConsole.Location = new System.Drawing.Point(0, 0);
            this.RtbConsole.Name = "RtbConsole";
            this.RtbConsole.Size = new System.Drawing.Size(662, 248);
            this.RtbConsole.TabIndex = 0;
            this.RtbConsole.Text = "";
            // 
            // ConsoleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 248);
            this.Controls.Add(this.RtbConsole);
            this.Name = "ConsoleWindow";
            this.Text = "OTAPI Console Window";
            this.Load += new System.EventHandler(this.DebugWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RtbConsole;
    }
}