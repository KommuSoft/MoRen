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
using System.Linq;
using System.Collections.Generic;

namespace Renderer {

	public sealed class BinarySpacePartitionAccelerator : Accelerator {

		private readonly BinarySpaceNode root;

		public BinarySpacePartitionAccelerator (List<RenderItem> items) : this(items,(int) Math.Ceiling(Math.Log(Math.Max(1.0d,items.Count),2))) {
		}
		public BinarySpacePartitionAccelerator (List<RenderItem> items, int maxdepth, int maxsize = 2) {
			BoundingBox bb = new BoundingBox();
			Utils.CalculateBoundingBox(items, bb);
			root = Subdivide(maxdepth, maxsize, bb, 0x00, items);
		}

		private static BinarySpaceNode Subdivide<T> (int maxdepth, int maxsize, BoundingBox bb, int depth, List<T> items) where T : IRenderable {
			//Console.WriteLine("enter {0}", String.Join(",", items.Select(r => r.Root)));
			//HashSet<RenderItem> hs = new HashSet<RenderItem>(items.Select(x => x.Root));
			HashSet<RenderItem> hri = new HashSet<RenderItem>(items.Select(x => x.Root));
			Console.WriteLine("Subdiv({0}, {1}, {2}, {3}, {4} ({5}u{6}))", maxdepth, maxsize, bb, depth, items, items.Count, hri.Count);
			if(depth >= maxdepth || hri.Count <= maxsize) {
				return new BinarySpaceNode(hri.ToArray());
			}
			int dim;
			double sweep = CalculateOptimalSplit(items, bb, out dim);
			BoundingBox bbleft, bbright;
			List<IRenderable> left = new List<IRenderable>(), right = new List<IRenderable>();
			bb.SplitAt(sweep, dim, out bbleft, out bbright);
			Split(items, dim, sweep, left, right);
			return new BinarySpaceNode(Subdivide(maxdepth, maxsize, bbleft, depth+0x01, left), Subdivide(maxdepth, maxsize, bbright, depth+0x01, right), sweep, dim);
		}

		private static double CalculateOptimalSplit<T> (List<T> items, BoundingBox bb, out int maxDim) where T : IRenderable {
			double totalSurface = items.Sum(ri => ri.Surface());
			double heu, maxHeu = double.NegativeInfinity, x, maxx;
			maxDim = 0x00;
			maxx = CalculateOptimalSplit(items, bb, totalSurface, out maxHeu, 0x00);
			//Console.WriteLine("X split {0}/{1}", maxx, maxHeu);
			maxHeu /= bb.DimensionSurface(0x00);
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, 0x01);
			//Console.WriteLine("Y split {0}/{1}", x, heu);
			heu /= bb.DimensionSurface(0x01);
			if(heu > maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = 0x01;
			}
			heu /= bb.DimensionSurface(0x02);
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, 0x02);
			//Console.WriteLine("Z split {0}/{1}", x, heu);
			if(heu > maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = 0x02;
			}
			//Console.WriteLine("Optimal split {0}/{1}", maxDim, maxx);
			return maxx;
		}

		private static double CalculateOptimalSplit<T> (List<T> items, BoundingBox bb, double totalSurface, out double maxHeur, int dim) where T : IRenderable {
			SortedSet<AddRemoveEvent> events = new SortedSet<AddRemoveEvent>(GenerateEvents(items, dim));
			IEnumerator<AddRemoveEvent> aree = events.GetEnumerator();
			HashSet<int> activ = new HashSet<int>();
			HashSet<int> torem = new HashSet<int>();
			AddRemoveEvent are;
			double x0, x1;
			bb.GetDimensionBounds(dim, out x0, out x1);
			double leftSurface = 0.0d;
			double activeSurface;
			maxHeur = double.NegativeInfinity;
			double xheu = double.NaN, heu;
			while(aree.MoveNext()) {
				are = aree.Current;
				double x = are.X;
				//Console.WriteLine("try {1}<{0}<{2}", x, x0, x1);
				int index = are.Index;
				if(are.Add) {
					if(!torem.Remove(index)) {
						activ.Add(index);
					}
				}
				else {
					if(!activ.Remove(index)) {
						torem.Add(index);
					}
					leftSurface += items[index].Surface();
				}
				if(x0 < x && x < x1) {
					//Console.WriteLine("check {0}", x);
					activeSurface = 0.0d;
					foreach(int ai in activ) {
						activeSurface += items[ai].SplitSurface(x, dim);
					}
					heu = (leftSurface+activeSurface)/(x-x0)+(totalSurface-leftSurface-activeSurface)/(x1-x);
					if(heu > maxHeur) {
						maxHeur = heu;
						xheu = x;
					}
				}
			}
			if(xheu == double.NaN) {
				return 0.5d*(x0+x1);
			}
			return xheu;
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

		private static void Split<T> (List<T> inp, int dim, double x, List<IRenderable> left, List<IRenderable> right) where T : IRenderable {
			double x0, x1;
			int s = 0x00;
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
					s++;
					splitted = ir.SplitAt(x, dim);
					left.AddRange(splitted.Item1);
					right.AddRange(splitted.Item2);
				}
			}
			Console.WriteLine("{0}/{1}/{2}", left.Count, s, right.Count);
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
			public readonly RenderItem[] tri;

			public BinarySpaceNode (RenderItem[] tris) {
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

