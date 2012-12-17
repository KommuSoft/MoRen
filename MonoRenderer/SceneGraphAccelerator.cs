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
using System.Linq;
using Renderer.SceneBuilding;

namespace Renderer {

	public sealed class SceneGraphAccelerator : IAccelerator {

		private static readonly Dictionary<Mesh,IAccelerator> cachedAccelerators = new Dictionary<Mesh, IAccelerator>();
		private readonly SceneGraphAcceleratorNode root;

		public SceneGraphAccelerator (SceneGraph sg, double time) {
			this.root = this.SplitData(sg.Root(time), sg.VersioningDictionary, sg.MaxDepth, time, 0x00);
		}

		private SceneGraphAcceleratorNode SplitData (SceneGraphNode sgn, VersioningDictionary<double,string,SceneGraphNode> versioning, int maxDepth, double version, int depth) {
			Mesh mesh = sgn.Mesh;
			AxisAlignedBoundingBox aabb = new AxisAlignedBoundingBox();
			IAccelerator acc = null;
			if(mesh != null) {
				if(!cachedAccelerators.TryGetValue(mesh, out acc)) {
					List<RenderItem> ris = new List<RenderItem>();
					aabb.Union(ris);
					mesh.Inject(ris);
					acc = new OctTreeAccelerator(ris);
					cachedAccelerators.Add(mesh, acc);
				}
			}
			List<SceneGraphAcceleratorNode> sgans = new List<SceneGraphAcceleratorNode>();
			SceneGraphAcceleratorNode sub;
			foreach(SceneGraphNode child in sgn.GetChildren(versioning,version,maxDepth,depth+0x01)) {
				sub = SplitData(child, versioning, maxDepth, version, depth+0x01);
				aabb.Union(sub.OuterBoundingBox);
				sgans.Add(sub);
			}
			return new SceneGraphAcceleratorNode(sgans, aabb, sgn.Transformer, acc);
		}

		#region IAccelerator implementation
		public RenderItem CalculateHit (Ray ray, out double t, double MaxT) {
			Ray clone = new Ray(ray);
			RenderItem ri = null;
			this.root.Hit(clone, ref ri, ref MaxT);
			t = MaxT;
			return ri;
		}
		#endregion

		private sealed class SceneGraphAcceleratorNode {

			private readonly SceneGraphAcceleratorNode[] children;
			private readonly AxisAlignedBoundingBox bb;
			private readonly Matrix4 toMatrix;
			private readonly Matrix4 backMatrix;
			private readonly IAccelerator accelerator;

			public AxisAlignedBoundingBox OuterBoundingBox {
				get {
					return new AxisAlignedBoundingBox(this.bb, this.backMatrix);
				}
			}

			public SceneGraphAcceleratorNode (IEnumerable<SceneGraphAcceleratorNode> children, AxisAlignedBoundingBox aabb, Matrix4 transformer, IAccelerator accelerator) {
				this.children = children.ToArray();
				this.bb = aabb;
				this.backMatrix = transformer;
				this.toMatrix = new Matrix4(backMatrix);
				this.toMatrix.Invert();
				this.accelerator = accelerator;
			}

			public void Hit (Ray transformedRay, ref RenderItem ri, ref double maxT) {
				double mul;
				transformedRay.TransformNormalize(toMatrix, out mul);
				double tt;
				RenderItem other = this.accelerator.CalculateHit(transformedRay, out tt, mul*maxT);
				if(other != null) {
					maxT = mul*tt;
					ri = other;
				}
				transformedRay.Transform(backMatrix);
			}

		}

	}
}

