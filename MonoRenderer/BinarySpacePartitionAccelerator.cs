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

		public BinarySpacePartitionAccelerator (List<RenderItem> items, int maxdepth, int maxsize = 2) : base(items,(int) Math.Ceiling(Math.Log(Math.Max(1.0d,items.Count),2.0d))) {
		}
		public BinarySpacePartitionAccelerator (List<RenderItem> items, int maxdepth, int maxsize = 2) {
			BoundingBox bb = new BoundingBox();
			Utils.CalculateBoundingBox(items, bb);
			root = Subdivide(maxdepth, maxsize, bb, 0x00, items);
		}

		private static BinarySpaceNode Subdivide<T> (int maxdepth, int maxsize, BoundingBox bb, int depth, List<T> items) where T : IRenderable {
			CalculateOptimalSplit(items, bb, 0x00);
			return null;
		}

		private static double CalculateOptimalSplit<T> (List<T> items, BoundingBox bb, int dim) where T : IRenderable {
			SortedSet<AddRemoveEvent> events = new SortedSet<AddRemoveEvent>(GenerateEvents(items, dim));
			return 0.0d;
		}

		private static IEnumerable<AddRemoveEvent> GenerateEvents<T> (IEnumerable<T> items, int dim) where T :IRenderable {
			double x0, x1;
			int i = 0x00;
			foreach(T t in items) {
				t.GetDimensionBounds(dim, out x0, out x1);
				yield return new AddRemoveEvent(i, x0, true);
				yield return new AddRemoveEvent(i, x1, false);
			}
		}

		private void Split (List<IRenderable> inp, int dim, double x, List<IRenderable> left, List<IRenderable> right) {
			double x0, x1;
			Tuple<ProxyRenderItem[],ProxyRenderItem[]> splitted;
			foreach(IRenderable ir in inp) {
				ir.GetDimensionBounds(dim, out x0, out x1);
				if(x1 < x) {
					left.Add(ir);
				}
				else if(x < x0) {
					right.Add(ir);
				}
				else {
					splitted = ir.SplitAt(x, dim);
					left.AddRange(splitted.Item1);
					right.AddRange(splitted.Item2);
				}
			}
		}

		public RenderItem CalculateHit (Ray ray, out double tHit, double maxT) {
			tHit = 0.0d;
			return null;
		}

		private sealed class BinarySpaceNode {

			public readonly double x;
			public readonly int dim;
			public readonly BinarySpaceNode left;
			public readonly BinarySpaceNode right;
			public readonly Triangle[] tri;

			public BinarySpaceNode (Triangle[] tris) {
				this.tri = tris;
				this.left = this.right = null;
				this.x = double.NaN;
			}
			public BinarySpaceNode (BinarySpaceNode left, BinarySpaceNode right, double x, int dim) {
				this.left = left;
				this.right = right;
				this.x = x;
				this.dim = dim;
			}

		}

	}
}

