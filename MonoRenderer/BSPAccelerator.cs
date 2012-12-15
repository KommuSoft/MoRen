//
//  BSPAccelerator.cs
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

	public sealed class BSPAccelerator : IAccelerator {

		private readonly BSPNode root;
		private readonly NormalInterval[] intervals;

		public BSPAccelerator (IEnumerable<RenderItem> items) : this(items,ImplementedSplitHeuristics.SurfaceAreaHeuristic,Point3.UnitDummies) {
		}
		public BSPAccelerator (IEnumerable<RenderItem> items, SplitHeuristic sh) : this(items,sh,Point3.UnitDummies) {
		}
		public BSPAccelerator (IEnumerable<RenderItem> items, SplitHeuristic sh, IEnumerable<Point3> facenormals, int maxDepth = 10, int maxSize = 5) {
			double ta, tb;
			List<NormalInterval> fn = new List<NormalInterval>();
			foreach(Point3 normal in facenormals) {
				double tta = double.PositiveInfinity;
				double ttb = double.NegativeInfinity;
				foreach(RenderItem ri in items) {
					ri.GetFaceNormalBounds(normal, out ta, out tb);
					tta = Math.Min(tta, ta);
					ttb = Math.Max(ttb, tb);
				}
				fn.Add(new NormalInterval(normal, tta, ttb));
			}
			this.intervals = fn.ToArray();
			LinkedList<RenderItem> caches = new LinkedList<RenderItem>(items);
			this.root = Split(caches, sh, fn, maxDepth, maxSize, 0x00);
		}

		private BSPNode Split (LinkedList<RenderItem> current, SplitHeuristic sh, IEnumerable<NormalInterval> facenormals, int maxDepth, int maxSize, int depth) {
			if(depth < maxDepth && maxSize < current.Count) {
				Point3 nibest = null;
				double minHeu = double.PositiveInfinity;
				double ta = double.NaN, tb = ta, tta, ttb, theu;
				foreach(NormalInterval ni in facenormals) {
					sh(current, ni.Normal, ni.T1, ni.T2, out tta, out ttb, out theu);
					if(theu < minHeu) {
						minHeu = theu;
						nibest = ni.Normal;
						ta = tta;
						tb = ttb;
					}
				}
				LinkedList<RenderItem> cachea = new LinkedList<RenderItem>(), cacheb = new LinkedList<RenderItem>();
				Filter(current, cachea, cacheb, nibest, ta, tb);
				return new BSPNode(nibest, ta, tb, Split(cachea, sh, facenormals, maxDepth, maxSize, depth+1), Split(cacheb, sh, facenormals, maxDepth, maxSize, depth+1));
			}
			else {
				return new BSPNode(current.ToArray());
			}
		}

		public void Filter (LinkedList<RenderItem> source, LinkedList<RenderItem> draina, LinkedList<RenderItem> drainb, Point3 facenormal, double ta, double tb) {
			double xa, xb;
			LinkedListNode<RenderItem> lln = source.First;
			while(lln != null) {
				lln.Value.GetFaceNormalBounds(facenormal, out xa, out xb);
				if(xb >= tb) {
					drainb.AddLast(lln.Value);
				}
				if(xa <= ta) {
					draina.AddLast(lln.Value);
				}
				lln = lln.Next;
			}
		}

		private sealed class BSPNode {

			private readonly Point3 splitNormal;
			private readonly double xa;
			private readonly double xb;
			private readonly RenderItem[] items;
			private readonly BSPNode left, right;

			public BSPNode (RenderItem[] items) {
				splitNormal = null;
				left = right = null;
				xa = xb = double.NaN;
				this.items = items;
			}

			public BSPNode (Point3 splitNormal, double ta, double tb, BSPNode left, BSPNode right) {
				this.splitNormal = splitNormal;
				this.xa = ta;
				this.xb = tb;
				this.left = left;
				this.right = right;
				this.items = null;
			}

			public IEnumerable<RenderItem> Items () {
				if(this.items != null) {
					foreach(RenderItem ri in items) {
						yield return ri;
					}
				}
				else {
					foreach(RenderItem ri in this.left.Items()) {
						yield return ri;
					}
					foreach(RenderItem ri in this.right.Items()) {
						yield return ri;
					}
				}
			}

			public void Hit (Ray ray, Point3 inter, ref RenderItem item, ref double tcur, double tmig, ref double tmax) {
				if(this.splitNormal != null) {//we're not at a leaf
					double x = inter[this.splitNormal];
					double x0 = ray.Offset[this.splitNormal];
					double dxi = Maths.SoftInv(ray.Direction[this.splitNormal]);
					double tt = tcur;
					double tmig0;
					if(x < xa) {
						if(dxi > 0.0d) {
							tt = tcur+dxi*(xb-x);
							tmig0 = Math.Min(tmig, dxi*(xa-x0));
							this.left.Hit(ray, inter, ref item, ref tcur, tmig0, ref tmax);
							tmig = Math.Min(tmig, tmax);
							if(tt <= tmig) {
								tcur = tt;
								ray.PointAt(tcur, inter);
								this.right.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
							}
						}
						else {
							this.left.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
						}
					}
					else if(x > xb) {
						if(dxi < 0.0d) {
							tt = tcur+dxi*(xa-x);
							tmig0 = Math.Min(tmig, dxi*(xb-x0));
							this.right.Hit(ray, inter, ref item, ref tcur, tmig0, ref tmax);
							tmig = Math.Min(tmig, tmax);
							if(tt <= tmig) {
								tcur = tt;
								ray.PointAt(tcur, inter);
								this.left.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
							}
						}
						else {
							this.right.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
						}
					}
					else {//in the death zone
						if(dxi < 0.0d) {
							tcur = dxi*(xa-x);
							ray.PointAt(tcur, inter);
							this.left.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
						}
						else if(dxi > 0.0d) {
							tcur = dxi*(xb-x0);
							ray.PointAt(tcur, inter);
							this.right.Hit(ray, inter, ref item, ref tcur, tmig, ref tmax);
						}
						//else we cannot move in the death zone, thus go back one level
					}
				}
				else {
					double tt;
					foreach(RenderItem ri in items) {
						tt = ri.HitAt(ray);
						if(tt < tmax) {
							tmax = tt;
							item = ri;
						}
					}
				}
			}

		}

		#region Accelerator implementation
		public RenderItem CalculateHit (Ray ray, out double t, double MaxT) {
			double tmin = 0.0d;
			t = MaxT;
			//first setting up the interval
			foreach(NormalInterval ni in intervals) {
				Utils.CloseInterval(ray, ni, ref tmin, ref t);
			}
			if(t > tmin) {//the ray passes through the bounding sheared box
				RenderItem ri = null;
				Point3 inter = new Point3();
				ray.PointAt(tmin, inter);
				this.root.Hit(ray, inter, ref ri, ref tmin, t, ref t);
				return ri;
			}
			else {
				return null;
			}
		}
		#endregion


	}
}

