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
		public const ulong Alpha8Mask = 0xff000000;
		public const ulong Red8Mask = 0xff0000;
		public const ulong Green8Mask = 0xff00;
		public const ulong Blue8Mask = 0xff;
		public const ulong MaxValue = BlueMask;
		public const ulong NonOverflowMask = 0x3ffffdffffefffff;
		public const ulong Color8Mask = 0xffffff;
		public const ulong OverflowMask = 0x4000020000100000;
		public const ulong IntensityMask = 0x3f0001f8000fc000;
		public static readonly Color Black = new Color(0x00);
		public static readonly Color White = new Color(NonOverflowMask);
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
				return (uint)(((Data>>0x0c)&Blue8Mask)|((Data>>0x19)&Green8Mask)|((Data>>0x26)&Red8Mask));
			}
		}
		public bool IntensityTreshold {
			get {
				return (Data&IntensityMask) > 0x00;
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

		public static Color FromFrac (double frac) {
			ulong data = (ulong)frac*MaxValue;
			ulong overflow = (data&OverflowMask);
			data |= overflow-(overflow>>0x14);
			data &= NonOverflowMask;
			data |= data<<0x15;
			data |= data<<0x15;
			return new Color(data);
		}
		public static Color LoseIntensity (Color c, double distanceUnit, double distance) {
			return c*Math.Min(distanceUnit/(distance*distance+1.0d), 1.0d);
		}

		public override bool Equals (object obj) {
			if(obj is Color) {
				return (this.Data == ((Color)obj).Data);
			}
			else {
				return false;
			}
		}
		public override string ToString () {
			return string.Format("[Color: Red={0}, Green={1}, Blue={2}]", RedInt.ToString("X"), GreenInt.ToString("X"), BlueInt.ToString("X"));
		}
		public override int GetHashCode () {
			return this.Data.GetHashCode();
		}

		public static Color operator + (Color c1, Color c2) {
			ulong datas = c1.Data+c2.Data;
			ulong overflow = (datas&OverflowMask);
			return new Color((datas|(overflow-(overflow>>0x14)))&NonOverflowMask);
		}
		public static Color operator * (Color c, double scal) {
			scal = Maths.Border(0.0d, scal, 1.0d);
			uint scale = (uint)Math.Round(scal*Color.MaxValue)+0x01;
			return new Color((((c.Data&BlueMask)*scale)>>0x14)|((((c.Data&GreenMask)*scale)>>0x14)&GreenMask)|((((c.Data&RedMask)>>0x14)*scale)&RedMask));
		}
		public static Color operator * (double scal, Color c) {
			scal = Maths.Border(0.0d, scal, 1.0d);
			uint scale = (uint)Math.Round(scal*Color.MaxValue)+0x01;
			return new Color((((c.Data&BlueMask)*scale)>>0x14)|((((c.Data&GreenMask)*scale)>>0x14)&GreenMask)|((((c.Data&RedMask)>>0x14)*scale)&RedMask));
		}
		/*public static Color operator * (Color c, uint scale) {
			scale++;
			return new Color((((c.Data&BlueMask)*scale)>>0x14)|((((c.Data&GreenMask)*scale)>>0x14)&GreenMask)|((((c.Data&RedMask)>>0x14)*scale)&RedMask));
		}*/
		public static Color operator * (Color c1, Color c2) {
			return new Color((((c1.Data&BlueMask)*((c2.Data&BlueMask)+0x01))>>0x14)|((((c1.Data&GreenMask)*(((c2.Data&GreenMask)>>0x15)+0x01))>>0x14)&GreenMask)|((((c1.Data&RedMask)>>0x14)*((c2.Data>>0x2a)+0x01))&RedMask));
		}
		public static bool operator == (Color c1, Color c2) {
			return c1.Data == c2.Data;
		}
		public static bool operator != (Color c1, Color c2) {
			return c1.Data != c2.Data;
		}
		public static Color operator ~ (Color c) {
			return new Color((~c.Data)&NonOverflowMask);
		}
		/*public static implicit operator Color (uint rgb) {
			return new Color(rgb);
		}*/

	}
}

