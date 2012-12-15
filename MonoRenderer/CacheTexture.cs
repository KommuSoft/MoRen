//
//  CacheTexture.cs
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

	public sealed class CacheTexture : ITexture {

		public readonly int Width, Height;
		public readonly ColorCache[] Pixel;

		public int TextureWidth {
			get {
				return this.Width;
			}
		}
		public int TextureHeight {
			get {
				return this.Height;
			}
		}

		public CacheTexture (int w, int h) {
			this.Width = w;
			this.Height = h;
			this.Pixel = new ColorCache[w*h];
		}

		public void AddTexture (Texture txt) {
			uint[] data = txt.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i].AddColor(data[i]);
		}
		//assumption: the texture was once added and not yet removed, otherwise underflow is possible
		public void RemoveTexture (Texture txt) {
			uint[] data = txt.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i].RemoveColor(data[i]);
		}
		public Texture Mix (int n) {
			Texture tex = new Texture(this.Width, this.Height);
			return tex;
		}

	}
}

