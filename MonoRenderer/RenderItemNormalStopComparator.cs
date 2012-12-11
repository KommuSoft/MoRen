//
//  RenderItemNormalStopComparator.cs
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

	//
//  RenderItemNormalStartComparator.cs
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

		public class RenderItemNormalStopComparator : IComparer<RenderItem> {

			private readonly Point3 faceNormal;

			public RenderItemNormalStopComparator (Point3 faceNormal) {
				this.faceNormal = faceNormal;
			}

			public int Compare (RenderItem ria, RenderItem rib) {
				double x1a, x1b, dummy;
				ria.GetFaceNormalBounds(this.faceNormal, out dummy, out x1a);
				rib.GetFaceNormalBounds(this.faceNormal, out dummy, out x1b);
				int result = x1a.CompareTo(x1b);
				if(result != 0x00) {
					return result;
				}
				else {
					return ria.Id.CompareTo(rib.Id);
				}
			}
		}

	}
}

