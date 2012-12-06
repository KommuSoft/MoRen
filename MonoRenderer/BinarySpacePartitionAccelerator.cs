//
//  BinarySpacePartitionAccelerator.cs
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

	public sealed class BinarySpacePartitionAccelerator : Accelerator {

		private readonly BinarySpaceNode root;

		public BinarySpacePartitionAccelerator (List<RenderItem> items) {
		}

		public BinarySpaceNode Subdivide () {

		}

		public RenderItem CalculateHit (Ray ray, out double tHit, double maxT) {
			tHit = 0.0d;
			return null;
		}

		private class BinarySpaceNode {

		}

	}
}

