//
//  SceneGraphAccelerator.cs
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
using Renderer.SceneBuilding;

namespace Renderer {

	public class SceneGraphAccelerator : IAccelerator {

		private static readonly Dictionary<Mesh,IAccelerator> cachedAccelerators = new Dictionary<Mesh, IAccelerator>();

		public SceneGraphAccelerator (SceneGraph sg, double time) {

		}

		#region IAccelerator implementation
		public RenderItem CalculateHit (Ray ray, out double t, double MaxT) {
			throw new System.NotImplementedException();
		}
		#endregion

		private sealed class SceneGraphAcceleratorNode {

			private readonly SceneGraphAcceleratorNode[] children;
			private readonly AxisAlignedBoundingBox bb;
			private readonly Matrix4 toMatrix;
			private readonly Matrix4 backMatrix;
			private readonly IAccelerator accelerator;

			public SceneGraphAcceleratorNode (SceneGraphAcceleratorNode[] children) {
				this.children = children;

			}

			public void Hit () {

			}

		}

	}
}

