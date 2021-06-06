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

namespace Microsoft.Xna.Framework.Input
{
    [System.Flags]
    public enum Buttons
    {
        A = 4096,
        B = 8192,
        X = 16384,
        Y = 32768,
        Back = 32,
        Start = 16,
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        LeftShoulder = 256,
        RightShoulder = 512,
        LeftStick = 64,
        RightStick = 128,
        BigButton = 2048,
        LeftThumbstickLeft = 2097152,
        LeftThumbstickRight = 1073741824,
        LeftThumbstickDown = 536870912,
        LeftThumbstickUp = 268435456,
        RightThumbstickLeft = 134217728,
        RightThumbstickRight = 67108864,
        RightThumbstickDown = 33554432,
        RightThumbstickUp = 16777216,
        LeftTrigger = 8388608,
        RightTrigger = 4194304
    }
}
