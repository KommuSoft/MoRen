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
		private readonly double x0, x1, y0, y1, z0, z1;

		public BinarySpacePartitionAccelerator (List<RenderItem> items) : this(items,(int) Math.Ceiling(3.0d*Math.Log(Math.Max(1.0d,items.Count),2))) {
		}
		public BinarySpacePartitionAccelerator (List<RenderItem> items, int maxdepth, int maxsize = 6) {
			BoundingBox bb = new BoundingBox();
			Utils.CalculateBoundingBox(items, bb);
			double totalSurface = items.Sum(ri => ri.Surface());
			root = Subdivide(maxdepth, maxsize, bb, 0x00, items, totalSurface);
			bb.OutParam(out x0, out x1, out y0, out y1, out z0, out z1);
		}

		private static BinarySpaceNode Subdivide (int maxdepth, int maxsize, BoundingBox bb, int depth, List<RenderItem> items, double total) {
			if(depth >= maxdepth || items.Count <= maxsize) {
				return new BinarySpaceNode(items.ToArray());
			}
			int dim;
			double sweep = CalculateOptimalSplit(items, depth%0x03, bb, out dim, total);
			BoundingBox bbleft, bbright;
			bb.SplitAt(sweep, dim, out bbleft, out bbright);
			List<RenderItem> left = new List<RenderItem>(), right = new List<RenderItem>();
			double leftsf, rightsf;
			Split(items, dim, sweep, left, right, bbleft, bbright, out leftsf, out rightsf);
			return new BinarySpaceNode(Subdivide(maxdepth, maxsize, bbleft, depth+0x01, left, leftsf), Subdivide(maxdepth, maxsize, bbright, depth+0x01, right, rightsf), sweep, dim);
		}

		private static double CalculateOptimalSplit (List<RenderItem> items, int depth3, BoundingBox bb, out int maxDim, double totalSurface) {
			double heu, maxHeu = double.NegativeInfinity, x, maxx;
			maxDim = 0x00;
			maxx = CalculateOptimalSplit(items, bb, totalSurface, out maxHeu, depth3);
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, (depth3+0x01)%0x03);
			if(heu < maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = (depth3+0x01)%0x03;
			}
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, (depth3+0x02)%0x03);
			if(heu < maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = (depth3+0x02)%0x03;
			}
			return maxx;
		}

		private static double CalculateOptimalSplit (List<RenderItem> items, BoundingBox bb, double totalSurface, out double maxHeur, int dim) {
			//Console.WriteLine("nr");
			SortedSet<AddRemoveEvent> events = new SortedSet<AddRemoveEvent>(GenerateEvents(items, dim));
			IEnumerator<AddRemoveEvent> aree = events.GetEnumerator();
			HashSet<int> activ = new HashSet<int>();
			HashSet<int> torem = new HashSet<int>();
			AddRemoveEvent are;
			double x0, x1;
			int nleft = 0x00;
			int ntotal = items.Count;
			double lsf = 0.0d;
			bb.GetDimensionBounds(dim, out x0, out x1);
			maxHeur = double.PositiveInfinity;
			double xheu = double.NaN, heu;
			while(aree.MoveNext()) {
				are = aree.Current;
				double x = are.X;
				int index = are.Index;
				if(are.Add) {
					if(!torem.Remove(index)) {
						activ.Add(index);
					}
				}
				else {
					nleft++;
					lsf += items[index].Surface();
					if(!activ.Remove(index)) {
						torem.Add(index);
					}
				}
				if(x0 < x && x < x1) {
					//Console.WriteLine("Inside");
					double lssf = 0.0d;
					foreach(int id in activ) {
						lssf += items[id].SplitSurface(x, dim);
					}
					heu = (nleft+activ.Count)*(lsf+lssf)+(ntotal-nleft)*(totalSurface-lsf-lssf);
					if(heu < maxHeur) {
						maxHeur = heu;
						xheu = x;
					}
				}
			}
			if(double.IsNaN(xheu)) {
				//Console.WriteLine("KERNEL PANIC {0} {2} {1}", bb, string.Join(",", items), dim);
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
				yield return new AddRemoveEvent(i++, x1, false);
			}
		}

		private static void Split (List<RenderItem> inp, int dim, double x, List<RenderItem> left, List<RenderItem> right, BoundingBox bbl, BoundingBox bbr, out double leftsf, out double rightsf) {
			double x0, x1;
			leftsf = 0.0d;
			rightsf = 0.0d;
			foreach(RenderItem ir in inp) {
				ir.GetDimensionBounds(dim, out x0, out x1);
				if(x0 < x && ir.InBox(bbl)) {
					left.Add(ir);
					leftsf += ir.Surface();
				}
				if(x1 > x && ir.InBox(bbr)) {
					right.Add(ir);
					rightsf += ir.Surface();
				}
			}
		}

		public RenderItem CalculateHit (Ray ray, out double tHit, double maxT) {
			Point3 inter = new Point3();
			tHit = maxT;
			double t;
			Utils.CalculateBoxHitpoint(ray, inter, out t, this.x0, this.x1, this.y0, this.y1, this.z0, this.z1);
			if(t > maxT) {
				return null;
			}
			RenderItem ri = null;
			this.root.Hit(ray, inter, ref t, ref tHit, ref ri);
			return ri;
		}

		private sealed class BinarySpaceNode {

			public readonly double x;
			public readonly int dim;
			public readonly BinarySpaceNode[] children;
			public readonly RenderItem[] tri;

			public BinarySpaceNode (RenderItem[] tris) {
				this.tri = tris;
				this.children = null;
				this.x = double.NaN;
			}
			public BinarySpaceNode (BinarySpaceNode left, BinarySpaceNode right, double x, int dim) {
				this.tri = null;
				this.children = new BinarySpaceNode[0x02] {left,right};
				this.x = x;
				this.dim = dim;
			}

			public void Hit (Ray ray, Point3 inter, ref double t, ref double tHit, ref RenderItem ri) {
				double tt;
				if(this.tri == null) {
					if(t < tHit) {
						int cur = Math.Sign(inter[dim]-x);
						if(cur*Maths.SoftSign(ray.Direction[dim]) < 0.0d) {//with migration
							tt = t+(x-inter[dim])/ray.Direction[dim];
							double tt2 = Math.Min(tt, tHit);
							this.children[(cur+0x01)>>0x01].Hit(ray, inter, ref t, ref tt2, ref ri);
							if(tt <= tt2) {
								t = tt;
								ray.PointAt(tt, inter);
								this.children[(0x01-cur)>>0x01].Hit(ray, inter, ref t, ref tHit, ref ri);
							}
							else {
								tHit = tt2;
							}
						}
						else {
							this.children[(cur+0x01)>>0x01].Hit(ray, inter, ref t, ref tHit, ref ri);
						}
					}
				}
				else {
					foreach(RenderItem rit in tri) {
						tt = rit.HitAt(ray);
						if(tt < tHit) {
							tHit = tt;
							ri = rit;
						}
					}
				}
			}

		}

	}
}

