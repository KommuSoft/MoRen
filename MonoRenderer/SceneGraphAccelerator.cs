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
					mesh.Inject(ris);
					aabb.Union(ris);
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
			SceneGraphAcceleratorNodeComparator comp = new SceneGraphAcceleratorNodeComparator(new AxisAlignedBoundingBox.BoundingBoxRayComparator(clone));
			RenderItem ri = null;
			this.root.Hit(clone, comp, ref ri, 1.0d, ref MaxT);
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
			public readonly AxisAlignedBoundingBox OuterBoundingBox;

			public SceneGraphAcceleratorNode (IEnumerable<SceneGraphAcceleratorNode> children, AxisAlignedBoundingBox aabb, Matrix4 transformer, IAccelerator accelerator) {
				this.children = children.ToArray();
				this.bb = aabb;
				this.backMatrix = transformer;
				this.toMatrix = new Matrix4(backMatrix);
				this.toMatrix.Invert();
				this.OuterBoundingBox = new AxisAlignedBoundingBox(this.bb, this.backMatrix);
				this.accelerator = accelerator;
			}

			public void Hit (Ray transformedRay, SceneGraphAcceleratorNodeComparator comp, ref RenderItem ri, double mul0, ref double maxT) {
				double mul1, dummy0, dummy1;
				transformedRay.TransformNormalize(this.toMatrix, out mul1);
				mul0 *= mul1;
				//Console.WriteLine("{0} -> {1}: {2}", transformedRay, bb, this.bb.IntersectingBox(transformedRay, out dummy0, out dummy1));
				//if(this.bb.IntersectingBox(transformedRay, out dummy0, out dummy1) && dummy0 < mul0*maxT) {
				double tt;
				if(this.accelerator != null) {
					RenderItem other = this.accelerator.CalculateHit(transformedRay, out tt, mul0*maxT);
					if(other != null) {
						maxT = mul0*tt;
						ri = other;
					}
				}
				SceneGraphAcceleratorNode[] children = (SceneGraphAcceleratorNode[])this.children.Clone();
				Array.Sort(children, comp);
				foreach(SceneGraphAcceleratorNode sgan in children) {
					sgan.Hit(transformedRay, comp, ref ri, mul0, ref maxT);
				}
				//}
				transformedRay.TransformNormalize(this.backMatrix);
			}

		}

		private sealed class SceneGraphAcceleratorNodeComparator : IComparer<SceneGraphAcceleratorNode> {

			private readonly AxisAlignedBoundingBox.BoundingBoxRayComparator comparator;

			public SceneGraphAcceleratorNodeComparator (AxisAlignedBoundingBox.BoundingBoxRayComparator comparator) {
				this.comparator = comparator;
			}

			#region IComparer implementation
			public int Compare (SceneGraphAcceleratorNode x, SceneGraphAcceleratorNode y) {
				return this.comparator.Compare(x.OuterBoundingBox, y.OuterBoundingBox);
			}
			#endregion


		}

	}
}

