//
//  RenderItemStartComparator.cs
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
using System.Collections.Generic;

namespace Renderer {

	public class RenderItemStartComparator : IComparer<RenderItem> {

		private int dim;

		public RenderItemStartComparator (int dim) {
			this.dim = dim;
		}

		public int Compare (RenderItem ria, RenderItem rib) {
			double x0a, x0b, dummy;
			ria.GetDimensionBounds(dim, out x0a, out dummy);
			rib.GetDimensionBounds(dim, out x0b, out dummy);
			int diff = x0a.CompareTo(x0b);
			if(diff != 0x00) {
				return diff;
			}
			else {
				return ria.Id.CompareTo(rib.Id);
			}
		}

	}
}

