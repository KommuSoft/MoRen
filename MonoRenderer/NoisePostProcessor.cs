//
//  AddNoisePostProcessor.cs
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

	public class NoisePostProcessor : CameraPostProcessor {

		private readonly uint delta;

		public NoisePostProcessor (uint delta = 0x08) {
			this.delta = delta;
		}

		public override void Process (Camera cam, Texture output, IAccelerator acc) {
			uint[] pix = output.Pixel;
			for(int i = 0; i < pix.Length; i++) {
				pix[i] = ColorUtils.AlphaChannel|ColorUtils.Random(pix[i], delta);
			}
		}

	}
}

