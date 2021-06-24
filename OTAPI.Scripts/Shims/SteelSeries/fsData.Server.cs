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
    public sealed class fsData
    {
        private object _value;

        public static readonly fsData True = new fsData(boolean: true);

        public static readonly fsData False = new fsData(boolean: false);

        public static readonly fsData Null = new fsData();

        public fsDataType Type => default(fsDataType);

        public bool IsNull => _value == null;

        public bool IsDouble => _value is double;

        public bool IsInt64 => _value is long;

        public bool IsBool => _value is bool;

        public bool IsString => _value is string;

        public bool IsDictionary => _value is Dictionary<string, fsData>;

        public bool IsList => _value is List<fsData>;

        public double AsDouble => 0;

        public long AsInt64 => 0;

        public bool AsBool => false;

        public string AsString => null;

        public Dictionary<string, fsData> AsDictionary => null;

        public List<fsData> AsList => null;

        public fsData() { }

        public fsData(bool boolean) { }

        public fsData(double f) { }

        public fsData(long i) { }

        public fsData(string str) { }

        public fsData(Dictionary<string, fsData> dict) { }

        public fsData(List<fsData> list) { }

        public static fsData CreateDictionary() => null;

        public static fsData CreateList() => null;

        public static fsData CreateList(int capacity) => null;

        public override string ToString() => "";

        public override bool Equals(object obj) => false;

        public bool Equals(fsData other) => false;

        public static bool operator ==(fsData a, fsData b) => false;

        public static bool operator !=(fsData a, fsData b) => false;

        public override int GetHashCode() => 0;
    }
}