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

		public BinarySpacePartitionAccelerator (List<RenderItem> items) : this(items,(int) Math.Ceiling(1.25d*Math.Log(Math.Max(1.0d,items.Count),2))) {
		}
		public BinarySpacePartitionAccelerator (List<RenderItem> items, int maxdepth, int maxsize = 6) {
			BoundingBox bb = new BoundingBox();
			Utils.CalculateBoundingBox(items, bb);
			root = Subdivide(maxdepth, maxsize, bb, 0x00, items);
			bb.OutParam(out x0, out x1, out y0, out y1, out z0, out z1);
		}

		private static BinarySpaceNode Subdivide (int maxdepth, int maxsize, BoundingBox bb, int depth, List<RenderItem> items) {
			if(depth >= maxdepth || items.Count <= maxsize) {
				return new BinarySpaceNode(items.ToArray());
			}
			int dim;
			double sweep = CalculateOptimalSplit(items, bb, out dim);
			BoundingBox bbleft, bbright;
			bb.SplitAt(sweep, dim, out bbleft, out bbright);
			List<RenderItem> left = new List<RenderItem>(), right = new List<RenderItem>();
			Split(items, dim, sweep, left, right, bbleft, bbright);
			return new BinarySpaceNode(Subdivide(maxdepth, maxsize, bbleft, depth+0x01, left), Subdivide(maxdepth, maxsize, bbright, depth+0x01, right), sweep, dim);
		}

		private static double CalculateOptimalSplit (List<RenderItem> items, BoundingBox bb, out int maxDim) {
			double totalSurface = items.Sum(ri => ri.Surface());
			double heu, maxHeu = double.NegativeInfinity, x, maxx;
			maxDim = 0x00;
			maxx = CalculateOptimalSplit(items, bb, totalSurface, out maxHeu, 0x00);
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, 0x01);
			if(heu < maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = 0x01;
			}
			x = CalculateOptimalSplit(items, bb, totalSurface, out heu, 0x02);
			if(heu < maxHeu) {
				maxx = x;
				maxHeu = heu;
				maxDim = 0x02;
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
					if(!activ.Remove(index)) {
						torem.Add(index);
					}
				}
				if(x0 < x && x < x1) {
					//Console.WriteLine("Inside");
					heu = Math.Max((nleft+activ.Count), (ntotal-nleft));
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

		private static void Split (List<RenderItem> inp, int dim, double x, List<RenderItem> left, List<RenderItem> right, BoundingBox bbl, BoundingBox bbr) {
			double x0, x1;
			foreach(RenderItem ir in inp) {
				ir.GetDimensionBounds(dim, out x0, out x1);
				if(x0 < x && ir.InBox(bbl)) {
					left.Add(ir);
				}
				if(x1 > x && ir.InBox(bbr)) {
					right.Add(ir);
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
			public readonly BinarySpaceNode left;
			public readonly BinarySpaceNode right;
			public readonly RenderItem[] tri;

			public BinarySpaceNode (RenderItem[] tris) {
				this.tri = tris;
				this.left = this.right = null;
				this.x = double.NaN;
			}
			public BinarySpaceNode (BinarySpaceNode left, BinarySpaceNode right, double x, int dim) {
				this.tri = null;
				this.left = left;
				this.right = right;
				this.x = x;
				this.dim = dim;
			}

			private void  HitChild (int c, Ray ray, Point3 inter, ref double t, ref double tHit, ref RenderItem ri) {
				if(c < 0x00) {
					this.left.Hit(ray, inter, ref t, ref tHit, ref ri);
				}
				else {
					this.right.Hit(ray, inter, ref t, ref tHit, ref ri);
				}
			}
			public void Hit (Ray ray, Point3 inter, ref double t, ref double tHit, ref RenderItem ri) {
				double tt, tt2;
				if(this.tri != null) {
					foreach(RenderItem rit in tri) {
						tt = rit.HitAt(ray);
						if(tt < tHit) {
							tHit = tt;
							ri = rit;
						}
					}
				}
				else if(t < tHit) {
					int cur = Math.Sign(inter[dim]-x);
					if(cur*Maths.SoftSign(ray.Direction[dim]) < 0.0d) {//with migration
						tt2 = tHit;
						tHit = t+(x-inter[dim])/ray.Direction[dim];
						HitChild(cur, ray, inter, ref t, ref tHit, ref ri);
						tt2 = tHit;
						t = tt;
						if(t < tHit) {
							ray.PointAt(t, inter);
							HitChild(-cur, ray, inter, ref t, ref tHit, ref ri);
						}
					}
				}
			}

		}

	}
}

