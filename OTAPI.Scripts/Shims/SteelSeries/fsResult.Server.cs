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