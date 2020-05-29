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
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

namespace System.Windows.Forms
{
	public interface IDataObject
	{
		object GetData(string format, bool autoConvert);
		object GetData(string format);
		object GetData(Type format);
		void SetData(string format, bool autoConvert, object data);
		void SetData(string format, object data);
		void SetData(Type format, object data);
		void SetData(object data);
		bool GetDataPresent(string format, bool autoConvert);
		bool GetDataPresent(string format);
		bool GetDataPresent(Type format);
		string[] GetFormats(bool autoConvert);
		string[] GetFormats();
	}


	public enum TextDataFormat
    {
        Text,
        UnicodeText,
        Rtf,
        Html,
        CommaSeparatedValue
    }

    public sealed class Clipboard
    {
        public static void SetDataObject(object data) { }
        public static void SetDataObject(object data, bool copy) { }
        public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay) { }
        public static System.Windows.Forms.IDataObject GetDataObject() => null;
        public static void Clear() { }
        public static bool ContainsAudio() => false;
        public static bool ContainsData(string format) => false;
        public static bool ContainsFileDropList() => false;
        public static bool ContainsImage() => false;
        public static bool ContainsText() => false;
        public static bool ContainsText(TextDataFormat format) => false;
        public static Stream GetAudioStream() => null;
        public static object GetData(string format) => null;
        public static StringCollection GetFileDropList() => null;
        //public static Image GetImage() => null;
        public static string GetText() => "";
        public static string GetText(TextDataFormat format) => "";
        public static void SetAudio(byte[] audioBytes) { }
        public static void SetAudio(Stream audioStream) { }
        public static void SetData(string format, object data) { }
        public static void SetFileDropList(StringCollection filePaths) { }
        //public static void SetImage(Image image) { }
        public static void SetText(string text) { }
        public static void SetText(string text, TextDataFormat format) { }
    }
}