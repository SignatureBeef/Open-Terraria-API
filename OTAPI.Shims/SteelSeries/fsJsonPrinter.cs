

using System.Collections.Generic;

namespace FullSerializer
{
    public static class fsJsonPrinter
    {
        public static void PrettyJson(fsData data, System.IO.TextWriter outputStream) { }

        public static string PrettyJson(fsData data) => "";

        public static void CompressedJson(fsData data, System.IO.StreamWriter outputStream) { }

        public static string CompressedJson(fsData data) => "";
    }
}