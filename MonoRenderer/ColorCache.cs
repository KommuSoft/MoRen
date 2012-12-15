//
//  ColorCache.cs
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

	public struct ColorCache {

		public ulong Red;
		public ulong Green;
		public ulong Blue;

		public ColorCache (ulong offset) {
			this.Red = offset;
			this.Green = offset;
			this.Blue = offset;
		}

		public void AddColor (Color c) {
			this.Red += c.RedInt;
			this.Green += c.GreenInt;
			this.Blue += c.BlueInt;
		}
		public void AddColor (uint rgb) {
			ulong dc = (rgb&Color.Red8Mask)>>0x04;
			dc |= dc>>0x08;
			dc |= dc>>0x08;
			this.Red += dc;
			dc = (rgb&Color.Green8Mask)<<0x04;
			dc |= dc>>0x08;
			dc |= dc>>0x08;
			this.Green += dc;
			dc = (rgb&Color.Blue8Mask)<<0x04;
			dc |= dc<<0x08;
			dc |= dc>>0x08;
			this.Blue += dc;
		}
		//assumption: the color was once added to the cache (there is no underflow checking)
		public void RemoveColor (uint rgb) {
			ulong dc = (rgb&Color.Red8Mask)>>0x04;
			dc |= dc>>0x08;
			dc |= dc>>0x08;
			this.Red -= dc;
			dc = (rgb&Color.Green8Mask)<<0x04;
			dc |= dc>>0x08;
			dc |= dc>>0x08;
			this.Green -= dc;
			dc = (rgb&Color.Blue8Mask)<<0x04;
			dc |= dc<<0x08;
			dc |= dc>>0x08;
			this.Blue -= dc;
		}

		public Color Mix (uint n) {
			return new Color((uint)(this.Red/n), (uint)(this.Green/n), (uint)(this.Blue/n));
		}
		public uint MixRGB (uint n) {
			return 0x00;
		}

	}
}

