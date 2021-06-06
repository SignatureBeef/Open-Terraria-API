/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
namespace System.Windows.Forms
{
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public enum MessageBoxButtons
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }
    public enum MessageBoxIcon
    {
        None = 0,
        Hand = 0x10,
        Question = 0x20,
        Exclamation = 48,
        Asterisk = 0x40,
        Stop = 0x10,
        Error = 0x10,
        Warning = 48,
        Information = 0x40
    }
    public enum MessageBoxDefaultButton
    {
        Button1 = 0,
        Button2 = 0x100,
        Button3 = 0x200
    }
    [Flags]
    public enum MessageBoxOptions
    {
        ServiceNotification = 0x200000,
        DefaultDesktopOnly = 0x20000,
        RightAlign = 0x80000,
        RtlReading = 0x100000
    }
    public enum HelpNavigator
    {
        Topic = -2147483647,
        TableOfContents,
        Index,
        Find,
        AssociateIndex,
        KeywordIndex,
        TopicId
    }

    public interface IWin32Window
    {
        IntPtr Handle
        {
            get;
        }
    }


    public class MessageBox
    {
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
         => DialogResult.Ignore;
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
         => DialogResult.Ignore;
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
         => DialogResult.Ignore;
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
         => DialogResult.Ignore;

        public static DialogResult Show(string text, string caption)
         => DialogResult.Ignore;

        public static DialogResult Show(string text)
         => DialogResult.Ignore;
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
         => DialogResult.Ignore;


        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
         => DialogResult.Ignore;


        public static DialogResult Show(IWin32Window owner, string text, string caption)
         => DialogResult.Ignore;

        public static DialogResult Show(IWin32Window owner, string text)
         => DialogResult.Ignore;
    }
}
