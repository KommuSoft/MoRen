//
//  IRenderable.cs
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

	public interface IRenderable {

		RenderItem Root {
			get;
		}
		double Surface ();
		double SplitSurface (double sweep, int dimension);
		Tuple<ProxyRenderItem[],ProxyRenderItem[]> SplitAt (double sweep, int dimension);
		void GetBounds (out double x0, out double x1, out double y0, out double y1, out double z0, out double z1);
		void GetDimensionBounds (int dim, out double x0, out double x1);


	}
}

