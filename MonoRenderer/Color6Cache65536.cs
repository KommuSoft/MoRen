//
//  Color6Cache65536.cs
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

	public struct Color6Cache65536 {

		public ulong High;
		public ulong Mid;
		public ulong Low;

		public Color6Cache65536 (uint offset) {
			High = offset|((ulong)offset<<0x20);
			Mid = High;
			Low = High;
		}

		public void AddColor6 (Color6 color) {
			Low += (color.Low&0xffff)|((color.Low&0x1fffe00000)<<0x0b);
			Mid += ((color.Low&0x3fffc0000000000)>>0x2a)|((color.High&0xffff)<<0x20);
			High += ((color.High&0x1fffe00000)>>0x15)|((color.High&0x3fffc0000000000)>>0x0a);
		}

		public Color6 Mix (uint n) {
			return new Color6((ushort)((High>>0x20)/n), (ushort)((High&0xffffffff)/n), (ushort)((Mid>>0x20)/n), (ushort)((Mid&0xffffffff)/n), (ushort)((Low>>0x20)/n), (ushort)((Low&0xffffffff)/n));
		}

	}
}

