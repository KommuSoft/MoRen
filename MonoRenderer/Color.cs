//
//  Color.cs
//
//  Author:
//       Willem Van Onsem <vanonsem.willem@gmail.com>
//
//  Copyright (c) 2012 Willem Van Onsem
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace Renderer {

	public struct Color {

		public const ulong RedMask = 0x3ffffc0000000000;
		public const ulong GreenMask = 0x1ffffe00000;
		public const ulong BlueMask = 0xfffff;
		public const ulong Red8Mask = 0xff0000;
		public const ulong Green8Mask = 0xff00;
		public const ulong Blue8Mask = 0xff;
		public const ulong MaxValue = BlueMask;
		public const ulong NonOverflowMask = 0x3ffffdffffefffff;
		public const ulong OverflowMask = 0x4000020000100000;
		public ulong Data;

		public uint RedInt {
			get {
				return (uint)(this.Data>>0x2a);
			}
		}
		public uint GreenInt {
			get {
				return (uint)((this.Data>>0x15)&BlueMask);
			}
		}
		public uint BlueInt {
			get {
				return (uint)(this.Data&BlueMask);
			}
		}
		public uint RGB8 {
			get {
				return (uint)(((Data>>0x0c)&Blue8Mask)|((Data>>0x21)&Green8Mask)|((Data>>0x36)&Red8Mask));
			}
		}

		public Color (uint rgb) {
			ulong rgbl = (ulong)rgb;
			ulong tmp = rgbl&0xff;
			this.Data = (tmp>>0x04)|(tmp<<0x04)|(tmp<<0x0c);
			tmp = (rgbl&0xff00);
			this.Data |= ((tmp<<0x09)|(tmp<<0x11)|(tmp<<0x19))&GreenMask;
			tmp = (rgbl&0xff0000);
			this.Data |= ((tmp<<0x16)|(tmp<<0x1e)|(tmp<<0x26))&RedMask;
		}
		public Color (uint red, uint green, uint blue) {
			this.Data = ((ulong)red<<0x2a)|((ulong)green<<0x15)|blue;
		}
		public Color (ulong data) {
			this.Data = data;
		}

		public static Color operator + (Color c1, Color c2) {
			ulong datas = c1.Data+c2.Data;
			ulong overflow = (datas&OverflowMask);
			return new Color((datas|(overflow-(overflow>>0x14)))&NonOverflowMask);
		}

	}
}

