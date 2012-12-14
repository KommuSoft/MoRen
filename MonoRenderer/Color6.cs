//
//  Color6.cs
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

	public struct Color6 {

		public ulong High;
		public ulong Low;

		public ushort Seg0 {
			get {
				return (ushort)(this.High>>0x2a);
			}
		}
		public ushort Seg1 {
			get {
				return (ushort)((this.High>>0x15)&0xffff);
			}
		}
		public ushort Seg2 {
			get {
				return (ushort)(this.High&0xffff);
			}
		}
		public ushort Seg3 {
			get {
				return (ushort)(this.Low>>0x2a);
			}
		}
		public ushort Seg4 {
			get {
				return (ushort)((this.Low>>0x15)&0xffff);
			}
		}
		public ushort Seg5 {
			get {
				return (ushort)(this.Low&0xffff);
			}
		}

		public Color6 (ulong high, ulong low) {
			High = high;
			Low = low;
		}
		public Color6 (ushort seg0, ushort seg1, ushort seg2, ushort seg3, ushort seg4, ushort seg5) {
			High = ((ulong)seg0<<0x2a)|((ulong)seg1<<0x15)|seg2;
			Low = ((ulong)seg3<<0x2a)|((ulong)seg4<<0x15)|seg5;
		}
		public Color6 (uint rgb) {
			High = 0x00;
			Low = 0x00;
		}

		public uint ToRGB () {
			return 0x00;
		}

		public override bool Equals (object obj) {
			if(obj is Color6) {
				Color6 c = (Color6)obj;
				return (High == c.High) && (Low == c.Low);
			}
			else {
				return false;
			}
		}
		public override int GetHashCode () {
			return this.High.GetHashCode()^this.Low.GetHashCode();
		}

		public override string ToString () {
			return string.Format("[Color6: Seg0={0}, Seg1={1}, Seg2={2}, Seg3={3}, Seg4={4}, Seg5={5}]", Seg0.ToString("X"), Seg1.ToString("X"), Seg2.ToString("X"), Seg3.ToString("X"), Seg4.ToString("X"), Seg5.ToString("X"));
		}

	}
}

