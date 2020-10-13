using System;
using System.Collections.Generic;

namespace SteelSeries.GameSense
{
	// Token: 0x02000046 RID: 70
	public static class HIDCodes
	{
		// Token: 0x04000090 RID: 144
		public static readonly Dictionary<string, byte> XnaKeyNamesToSteelSeriesKeyIndex = new Dictionary<string, byte>
		{
			{
				"None",
				0
			},
			{
				"Tab",
				43
			},
			{
				"Enter",
				40
			},
			{
				"Pause",
				72
			},
			{
				"CapsLock",
				57
			},
			{
				"Space",
				44
			},
			{
				"PageUp",
				75
			},
			{
				"PageDown",
				78
			},
			{
				"End",
				77
			},
			{
				"Home",
				74
			},
			{
				"Left",
				80
			},
			{
				"Up",
				82
			},
			{
				"Right",
				79
			},
			{
				"Down",
				81
			},
			{
				"Insert",
				73
			},
			{
				"Delete",
				76
			},
			{
				"Help",
				117
			},
			{
				"D0",
				39
			},
			{
				"D1",
				30
			},
			{
				"D2",
				31
			},
			{
				"D3",
				32
			},
			{
				"D4",
				33
			},
			{
				"D5",
				34
			},
			{
				"D6",
				35
			},
			{
				"D7",
				36
			},
			{
				"D8",
				37
			},
			{
				"D9",
				38
			},
			{
				"A",
				4
			},
			{
				"B",
				5
			},
			{
				"C",
				6
			},
			{
				"D",
				7
			},
			{
				"E",
				8
			},
			{
				"F",
				9
			},
			{
				"G",
				10
			},
			{
				"H",
				11
			},
			{
				"I",
				12
			},
			{
				"J",
				13
			},
			{
				"K",
				14
			},
			{
				"L",
				15
			},
			{
				"M",
				16
			},
			{
				"N",
				17
			},
			{
				"O",
				18
			},
			{
				"P",
				19
			},
			{
				"Q",
				20
			},
			{
				"R",
				21
			},
			{
				"S",
				22
			},
			{
				"T",
				23
			},
			{
				"U",
				24
			},
			{
				"V",
				25
			},
			{
				"W",
				26
			},
			{
				"X",
				27
			},
			{
				"Y",
				28
			},
			{
				"Z",
				29
			},
			{
				"NumPad0",
				98
			},
			{
				"NumPad1",
				89
			},
			{
				"NumPad2",
				90
			},
			{
				"NumPad3",
				91
			},
			{
				"NumPad4",
				92
			},
			{
				"NumPad5",
				93
			},
			{
				"NumPad6",
				94
			},
			{
				"NumPad7",
				95
			},
			{
				"NumPad8",
				96
			},
			{
				"NumPad9",
				97
			},
			{
				"F1",
				58
			},
			{
				"F2",
				59
			},
			{
				"F3",
				60
			},
			{
				"F4",
				61
			},
			{
				"F5",
				62
			},
			{
				"F6",
				63
			},
			{
				"F7",
				64
			},
			{
				"F8",
				65
			},
			{
				"F9",
				66
			},
			{
				"F10",
				67
			},
			{
				"F11",
				68
			},
			{
				"F12",
				69
			},
			{
				"F13",
				104
			},
			{
				"F14",
				105
			},
			{
				"F15",
				106
			},
			{
				"F16",
				107
			},
			{
				"F17",
				108
			},
			{
				"F18",
				109
			},
			{
				"F19",
				110
			},
			{
				"F20",
				111
			},
			{
				"F21",
				112
			},
			{
				"F22",
				113
			},
			{
				"F23",
				114
			},
			{
				"F24",
				115
			},
			{
				"NumLock",
				83
			}
		};

		// Token: 0x04000091 RID: 145
		public const byte MOD_LCTRL = 1;

		// Token: 0x04000092 RID: 146
		public const byte MOD_LSHIFT = 2;

		// Token: 0x04000093 RID: 147
		public const byte MOD_LALT = 4;

		// Token: 0x04000094 RID: 148
		public const byte MOD_LMETA = 8;

		// Token: 0x04000095 RID: 149
		public const byte MOD_RCTRL = 16;

		// Token: 0x04000096 RID: 150
		public const byte MOD_RSHIFT = 32;

		// Token: 0x04000097 RID: 151
		public const byte MOD_RALT = 64;

		// Token: 0x04000098 RID: 152
		public const byte MOD_RMETA = 128;

		// Token: 0x04000099 RID: 153
		public const byte NONE = 0;

		// Token: 0x0400009A RID: 154
		public const byte ERR_OVF = 1;

		// Token: 0x0400009B RID: 155
		public const byte A = 4;

		// Token: 0x0400009C RID: 156
		public const byte B = 5;

		// Token: 0x0400009D RID: 157
		public const byte C = 6;

		// Token: 0x0400009E RID: 158
		public const byte D = 7;

		// Token: 0x0400009F RID: 159
		public const byte E = 8;

		// Token: 0x040000A0 RID: 160
		public const byte F = 9;

		// Token: 0x040000A1 RID: 161
		public const byte G = 10;

		// Token: 0x040000A2 RID: 162
		public const byte H = 11;

		// Token: 0x040000A3 RID: 163
		public const byte I = 12;

		// Token: 0x040000A4 RID: 164
		public const byte J = 13;

		// Token: 0x040000A5 RID: 165
		public const byte K = 14;

		// Token: 0x040000A6 RID: 166
		public const byte L = 15;

		// Token: 0x040000A7 RID: 167
		public const byte M = 16;

		// Token: 0x040000A8 RID: 168
		public const byte N = 17;

		// Token: 0x040000A9 RID: 169
		public const byte O = 18;

		// Token: 0x040000AA RID: 170
		public const byte P = 19;

		// Token: 0x040000AB RID: 171
		public const byte Q = 20;

		// Token: 0x040000AC RID: 172
		public const byte R = 21;

		// Token: 0x040000AD RID: 173
		public const byte S = 22;

		// Token: 0x040000AE RID: 174
		public const byte T = 23;

		// Token: 0x040000AF RID: 175
		public const byte U = 24;

		// Token: 0x040000B0 RID: 176
		public const byte V = 25;

		// Token: 0x040000B1 RID: 177
		public const byte W = 26;

		// Token: 0x040000B2 RID: 178
		public const byte X = 27;

		// Token: 0x040000B3 RID: 179
		public const byte Y = 28;

		// Token: 0x040000B4 RID: 180
		public const byte Z = 29;

		// Token: 0x040000B5 RID: 181
		public const byte ALPHA_1 = 30;

		// Token: 0x040000B6 RID: 182
		public const byte ALPHA_2 = 31;

		// Token: 0x040000B7 RID: 183
		public const byte ALPHA_3 = 32;

		// Token: 0x040000B8 RID: 184
		public const byte ALPHA_4 = 33;

		// Token: 0x040000B9 RID: 185
		public const byte ALPHA_5 = 34;

		// Token: 0x040000BA RID: 186
		public const byte ALPHA_6 = 35;

		// Token: 0x040000BB RID: 187
		public const byte ALPHA_7 = 36;

		// Token: 0x040000BC RID: 188
		public const byte ALPHA_8 = 37;

		// Token: 0x040000BD RID: 189
		public const byte ALPHA_9 = 38;

		// Token: 0x040000BE RID: 190
		public const byte ALPHA_0 = 39;

		// Token: 0x040000BF RID: 191
		public const byte ENTER = 40;

		// Token: 0x040000C0 RID: 192
		public const byte ESC = 41;

		// Token: 0x040000C1 RID: 193
		public const byte BACKSPACE = 42;

		// Token: 0x040000C2 RID: 194
		public const byte TAB = 43;

		// Token: 0x040000C3 RID: 195
		public const byte SPACE = 44;

		// Token: 0x040000C4 RID: 196
		public const byte MINUS = 45;

		// Token: 0x040000C5 RID: 197
		public const byte EQUAL = 46;

		// Token: 0x040000C6 RID: 198
		public const byte LEFTBRACE = 47;

		// Token: 0x040000C7 RID: 199
		public const byte RIGHTBRACE = 48;

		// Token: 0x040000C8 RID: 200
		public const byte BACKSLASH = 49;

		// Token: 0x040000C9 RID: 201
		public const byte HASHTILDE = 50;

		// Token: 0x040000CA RID: 202
		public const byte SEMICOLON = 51;

		// Token: 0x040000CB RID: 203
		public const byte APOSTROPHE = 52;

		// Token: 0x040000CC RID: 204
		public const byte GRAVE = 53;

		// Token: 0x040000CD RID: 205
		public const byte COMMA = 54;

		// Token: 0x040000CE RID: 206
		public const byte DOT = 55;

		// Token: 0x040000CF RID: 207
		public const byte SLASH = 56;

		// Token: 0x040000D0 RID: 208
		public const byte CAPSLOCK = 57;

		// Token: 0x040000D1 RID: 209
		public const byte F1 = 58;

		// Token: 0x040000D2 RID: 210
		public const byte F2 = 59;

		// Token: 0x040000D3 RID: 211
		public const byte F3 = 60;

		// Token: 0x040000D4 RID: 212
		public const byte F4 = 61;

		// Token: 0x040000D5 RID: 213
		public const byte F5 = 62;

		// Token: 0x040000D6 RID: 214
		public const byte F6 = 63;

		// Token: 0x040000D7 RID: 215
		public const byte F7 = 64;

		// Token: 0x040000D8 RID: 216
		public const byte F8 = 65;

		// Token: 0x040000D9 RID: 217
		public const byte F9 = 66;

		// Token: 0x040000DA RID: 218
		public const byte F10 = 67;

		// Token: 0x040000DB RID: 219
		public const byte F11 = 68;

		// Token: 0x040000DC RID: 220
		public const byte F12 = 69;

		// Token: 0x040000DD RID: 221
		public const byte SYSRQ = 70;

		// Token: 0x040000DE RID: 222
		public const byte SCROLLLOCK = 71;

		// Token: 0x040000DF RID: 223
		public const byte PAUSE = 72;

		// Token: 0x040000E0 RID: 224
		public const byte INSERT = 73;

		// Token: 0x040000E1 RID: 225
		public const byte HOME = 74;

		// Token: 0x040000E2 RID: 226
		public const byte PAGEUP = 75;

		// Token: 0x040000E3 RID: 227
		public const byte DELETE = 76;

		// Token: 0x040000E4 RID: 228
		public const byte END = 77;

		// Token: 0x040000E5 RID: 229
		public const byte PAGEDOWN = 78;

		// Token: 0x040000E6 RID: 230
		public const byte RIGHT = 79;

		// Token: 0x040000E7 RID: 231
		public const byte LEFT = 80;

		// Token: 0x040000E8 RID: 232
		public const byte DOWN = 81;

		// Token: 0x040000E9 RID: 233
		public const byte UP = 82;

		// Token: 0x040000EA RID: 234
		public const byte NUMLOCK = 83;

		// Token: 0x040000EB RID: 235
		public const byte KPSLASH = 84;

		// Token: 0x040000EC RID: 236
		public const byte KPASTERISK = 85;

		// Token: 0x040000ED RID: 237
		public const byte KPMINUS = 86;

		// Token: 0x040000EE RID: 238
		public const byte KPPLUS = 87;

		// Token: 0x040000EF RID: 239
		public const byte KPENTER = 88;

		// Token: 0x040000F0 RID: 240
		public const byte KP1 = 89;

		// Token: 0x040000F1 RID: 241
		public const byte KP2 = 90;

		// Token: 0x040000F2 RID: 242
		public const byte KP3 = 91;

		// Token: 0x040000F3 RID: 243
		public const byte KP4 = 92;

		// Token: 0x040000F4 RID: 244
		public const byte KP5 = 93;

		// Token: 0x040000F5 RID: 245
		public const byte KP6 = 94;

		// Token: 0x040000F6 RID: 246
		public const byte KP7 = 95;

		// Token: 0x040000F7 RID: 247
		public const byte KP8 = 96;

		// Token: 0x040000F8 RID: 248
		public const byte KP9 = 97;

		// Token: 0x040000F9 RID: 249
		public const byte KP0 = 98;

		// Token: 0x040000FA RID: 250
		public const byte KPDOT = 99;

		// Token: 0x040000FB RID: 251
		public const byte KEY_102ND = 100;

		// Token: 0x040000FC RID: 252
		public const byte COMPOSE = 101;

		// Token: 0x040000FD RID: 253
		public const byte POWER = 102;

		// Token: 0x040000FE RID: 254
		public const byte KPEQUAL = 103;

		// Token: 0x040000FF RID: 255
		public const byte F13 = 104;

		// Token: 0x04000100 RID: 256
		public const byte F14 = 105;

		// Token: 0x04000101 RID: 257
		public const byte F15 = 106;

		// Token: 0x04000102 RID: 258
		public const byte F16 = 107;

		// Token: 0x04000103 RID: 259
		public const byte F17 = 108;

		// Token: 0x04000104 RID: 260
		public const byte F18 = 109;

		// Token: 0x04000105 RID: 261
		public const byte F19 = 110;

		// Token: 0x04000106 RID: 262
		public const byte F20 = 111;

		// Token: 0x04000107 RID: 263
		public const byte F21 = 112;

		// Token: 0x04000108 RID: 264
		public const byte F22 = 113;

		// Token: 0x04000109 RID: 265
		public const byte F23 = 114;

		// Token: 0x0400010A RID: 266
		public const byte F24 = 115;

		// Token: 0x0400010B RID: 267
		public const byte OPEN = 116;

		// Token: 0x0400010C RID: 268
		public const byte HELP = 117;

		// Token: 0x0400010D RID: 269
		public const byte PROPS = 118;

		// Token: 0x0400010E RID: 270
		public const byte FRONT = 119;

		// Token: 0x0400010F RID: 271
		public const byte STOP = 120;

		// Token: 0x04000110 RID: 272
		public const byte AGAIN = 121;

		// Token: 0x04000111 RID: 273
		public const byte UNDO = 122;

		// Token: 0x04000112 RID: 274
		public const byte CUT = 123;

		// Token: 0x04000113 RID: 275
		public const byte COPY = 124;

		// Token: 0x04000114 RID: 276
		public const byte PASTE = 125;

		// Token: 0x04000115 RID: 277
		public const byte FIND = 126;

		// Token: 0x04000116 RID: 278
		public const byte MUTE = 127;

		// Token: 0x04000117 RID: 279
		public const byte VOLUMEUP = 128;

		// Token: 0x04000118 RID: 280
		public const byte VOLUMEDOWN = 129;

		// Token: 0x04000119 RID: 281
		public const byte KPCOMMA = 133;

		// Token: 0x0400011A RID: 282
		public const byte RO = 135;

		// Token: 0x0400011B RID: 283
		public const byte KATAKANAHIRAGANA = 136;

		// Token: 0x0400011C RID: 284
		public const byte YEN = 137;

		// Token: 0x0400011D RID: 285
		public const byte HENKAN = 138;

		// Token: 0x0400011E RID: 286
		public const byte MUHENKAN = 139;

		// Token: 0x0400011F RID: 287
		public const byte KPJPCOMMA = 140;

		// Token: 0x04000120 RID: 288
		public const byte HANGEUL = 144;

		// Token: 0x04000121 RID: 289
		public const byte HANJA = 145;

		// Token: 0x04000122 RID: 290
		public const byte KATAKANA = 146;

		// Token: 0x04000123 RID: 291
		public const byte HIRAGANA = 147;

		// Token: 0x04000124 RID: 292
		public const byte ZENKAKUHANKAKU = 148;

		// Token: 0x04000125 RID: 293
		public const byte KPLEFTPAREN = 182;

		// Token: 0x04000126 RID: 294
		public const byte KPRIGHTPAREN = 183;

		// Token: 0x04000127 RID: 295
		public const byte LEFTCTRL = 224;

		// Token: 0x04000128 RID: 296
		public const byte LEFTSHIFT = 225;

		// Token: 0x04000129 RID: 297
		public const byte LEFTALT = 226;

		// Token: 0x0400012A RID: 298
		public const byte LEFTMETA = 227;

		// Token: 0x0400012B RID: 299
		public const byte RIGHTCTRL = 228;

		// Token: 0x0400012C RID: 300
		public const byte RIGHTSHIFT = 229;

		// Token: 0x0400012D RID: 301
		public const byte RIGHTALT = 230;

		// Token: 0x0400012E RID: 302
		public const byte RIGHTMETA = 231;

		// Token: 0x0400012F RID: 303
		public const byte MEDIA_PLAYPAUSE = 232;

		// Token: 0x04000130 RID: 304
		public const byte MEDIA_STOPCD = 233;

		// Token: 0x04000131 RID: 305
		public const byte MEDIA_PREVIOUSSONG = 234;

		// Token: 0x04000132 RID: 306
		public const byte MEDIA_NEXTSONG = 235;

		// Token: 0x04000133 RID: 307
		public const byte MEDIA_EJECTCD = 236;

		// Token: 0x04000134 RID: 308
		public const byte MEDIA_VOLUMEUP = 237;

		// Token: 0x04000135 RID: 309
		public const byte MEDIA_VOLUMEDOWN = 238;

		// Token: 0x04000136 RID: 310
		public const byte MEDIA_MUTE = 239;

		// Token: 0x04000137 RID: 311
		public const byte MEDIA_WWW = 240;

		// Token: 0x04000138 RID: 312
		public const byte MEDIA_BACK = 241;

		// Token: 0x04000139 RID: 313
		public const byte MEDIA_FORWARD = 242;

		// Token: 0x0400013A RID: 314
		public const byte MEDIA_STOP = 243;

		// Token: 0x0400013B RID: 315
		public const byte MEDIA_FIND = 244;

		// Token: 0x0400013C RID: 316
		public const byte MEDIA_SCROLLUP = 245;

		// Token: 0x0400013D RID: 317
		public const byte MEDIA_SCROLLDOWN = 246;

		// Token: 0x0400013E RID: 318
		public const byte MEDIA_EDIT = 247;

		// Token: 0x0400013F RID: 319
		public const byte MEDIA_SLEEP = 248;

		// Token: 0x04000140 RID: 320
		public const byte MEDIA_COFFEE = 249;

		// Token: 0x04000141 RID: 321
		public const byte MEDIA_REFRESH = 250;

		// Token: 0x04000142 RID: 322
		public const byte MEDIA_CALC = 251;
	}
}
