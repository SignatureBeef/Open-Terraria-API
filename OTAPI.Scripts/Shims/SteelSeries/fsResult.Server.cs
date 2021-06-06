

using System.Collections.Generic;

namespace FullSerializer
{
    public struct fsResult
    {
        public static fsResult Success;

        public bool Failed => false;

        public bool Succeeded => false;

        public bool HasWarnings { get; }

        public System.Exception AsException { get; }

        public IEnumerable<string> RawMessages { get; }

        public string FormattedMessages => "";

        public void AddMessage(string message) { }

        public void AddMessages(fsResult result) { }

        public fsResult Merge(fsResult other) => default(fsResult);

        public static fsResult Warn(string warning) => default(fsResult);

        public static fsResult Fail(string warning) => default(fsResult);

        public static fsResult operator +(fsResult a, fsResult b) => default(fsResult);

        public fsResult AssertSuccess() => default(fsResult);

        public fsResult AssertSuccessWithoutWarnings() => default(fsResult);
    }
}