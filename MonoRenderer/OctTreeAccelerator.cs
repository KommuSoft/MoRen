//
//  OctTreeAccelerator.cs
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

	using MarkedItem = Tuple<int,RenderItem>;

	public sealed class OctTreeAccelerator : Accelerator {

		public const int ValidPositionMask = 0x2a;
		private readonly double x0, x1, y0, y1, z0, z1;
		private readonly int maxDepth, maxNodeItems;
		private readonly FastOctTreeNode root;
		private readonly RenderItem[] ris;

		public OctTreeAccelerator (List<RenderItem> items) : this(items,(int)Math.Ceiling(Math.Log(Math.Max(items.Count,8.0d), 8.0d))) {
		}
		public OctTreeAccelerator (List<RenderItem> items, int maxDepth, int maxNodeItems = 8) {
			this.maxDepth = maxDepth;
			this.maxNodeItems = maxNodeItems;
			Utils.CalculateBoundingBox(items, out x0, out x1, out y0, out y1, out z0, out z1);
			OctTreeNode otn = Order(items.Mark(), 0x00, x0, x1, y0, y1, z0, z1);
			FinalList fl = new FinalList(otn.PropagateSubLists());
			this.root = new FastOctTreeNode(otn);
			this.ris = fl.list.Select(x => items[x]).ToArray();
		}

		public RenderItem CalculateHit (Ray ray, out double tHit, double maxT) {
			Point3 inter = new Point3(ray.Offset);
			double t = 0.0d;
			tHit = maxT;
			if(x0 > inter.X || inter.X > x1 || y0 > inter.Y || inter.Y > y1 || z0 > inter.Z || inter.Z > z1) {
				Utils.CalculateBoxHitpoint(ray, inter, out t, this.x0, this.x1, this.y0, this.y1, this.z0, this.z1);
				if(t >= tHit) {
					return null;
				}
			}
			RenderItem ri = null;
			double[] seqxyz = new double[] {
				Maths.SoftInv(ray.DX),
				Maths.SoftInv(ray.DY),
				Maths.SoftInv(ray.DZ),
				x0,
				x1,
				x1,
				y0,
				y1,
				y1,
				z0,
				z1,
				z1
			};
			int dpos = Maths.BinarySign(ray.DX)|(Maths.BinarySign(ray.DY)<<0x02)|(Maths.BinarySign(ray.DZ)<<0x04);
			proceedSubTree(ray, dpos, seqxyz, inter, ref ri, ref t, ref tHit, this.root);
			return ri;
		}

		private void proceedSubTree (Ray ray, int dpos, double[] seqxyz, Point3 inter, ref RenderItem ri, ref double t, ref double tHit, FastOctTreeNode fotn) {
			double tt;
			if(fotn.IsLeaf) {
				long refs = fotn.data;
				if(refs != 0x00) {
					//checktile
					long end = refs>>0x20;
					for(refs &= 0xffffffff; refs < end; refs++) {
						tt = ris[refs].HitAt(ray);
						if(tt < tHit) {
							tHit = tt;
							ri = ris[refs];
						}
					}
				}
			}
			else if(t < tHit) {
				int pos = Maths.BinarySign(inter.X-fotn.x)|(Maths.BinarySign(inter.Y-fotn.y)<<0x02)|(Maths.BinarySign(inter.Z-fotn.z)<<0x04);
				double xt, yt, zt;
				int nextdim = 0x00;
				do {
					seqxyz[0x04] = fotn.x;
					seqxyz[0x07] = fotn.y;
					seqxyz[0x0a] = fotn.z;
					tt = double.PositiveInfinity;
					int pos2 = pos+dpos;
					calcDim(ref tt, ref nextdim, ray.X0, seqxyz[(pos2&0x03)+0x03], seqxyz[0x00], 0x00);//seqxyz [0x00]
					calcDim(ref tt, ref nextdim, ray.Y0, seqxyz[((pos2&0x0c)>>0x02)+0x06], seqxyz[0x01], 0x02);
					calcDim(ref tt, ref nextdim, ray.Z0, seqxyz[((pos2&0x30)>>0x04)+0x09], seqxyz[0x02], 0x04);
					xt = seqxyz[0x05-((pos&0x01)<<0x01)];
					yt = seqxyz[0x08-((pos&0x04)>>0x01)];
					zt = seqxyz[0x0b-((pos&0x10)>>0x03)];
					seqxyz[0x05-((pos&0x01)<<0x01)] = fotn.x;
					seqxyz[0x08-((pos&0x04)>>0x01)] = fotn.y;
					seqxyz[0x0b-((pos&0x10)>>0x03)] = fotn.z;
					proceedSubTree(ray, dpos, seqxyz, inter, ref ri, ref t, ref tHit, fotn.node[((pos&0x01)<<0x02)|((pos&0x04)>>0x01)|((pos&0x10)>>0x04)]);
					seqxyz[0x05-((pos&0x01)<<0x01)] = xt;
					seqxyz[0x08-((pos&0x04)>>0x01)] = yt;
					seqxyz[0x0b-((pos&0x10)>>0x03)] = zt;
					t = tt;
					ray.PointAt(t, inter);
					pos += (0x02*((dpos>>nextdim)&0x03)-0x01)<<nextdim;
				}
				while(t < tHit && (pos&ValidPositionMask) == 0x00);
			}
		}

		private static void calcDim (ref double tmin, ref int mindim, double x, double xt, double dxinv, int dim) {
			double tt = (xt-x)*dxinv;
			if(tt < tmin) {
				tmin = tt;
				mindim = dim;
			}
		}

		private OctTreeNode Order (IEnumerable<MarkedItem> items, int depth, double x0, double x1, double y0, double y1, double z0, double z1) {
			List<MarkedItem> itemCache = new List<MarkedItem>(items);
			if(itemCache.Count > maxNodeItems && (depth < maxDepth || depth <= 0x00)) {
				double x2 = 0.5d*(x0+x1), y2 = 0.5d*(y0+y1), z2 = 0.5d*(z0+z1);
				OctTreeNode[] children = new OctTreeNode[0x08];
				children[0x00] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x0, x2, y0, y2, z0, z2) && x.Item2.InBox(x0, x2, y0, y2, z0, z2)), depth+0x01, x0, x2, y0, y2, z0, z2);
				children[0x01] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x0, x2, y0, y2, z2, z1) && x.Item2.InBox(x0, x2, y0, y2, z2, z1)), depth+0x01, x0, x2, y0, y2, z2, z1);
				children[0x02] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x0, x2, y2, y1, z0, z2) && x.Item2.InBox(x0, x2, y2, y1, z0, z2)), depth+0x01, x0, x2, y2, y1, z0, z2);
				children[0x03] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x0, x2, y2, y1, z2, z1) && x.Item2.InBox(x0, x2, y2, y1, z2, z1)), depth+0x01, x0, x2, y2, y1, z2, z1);
				children[0x04] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x2, x1, y0, y2, z0, z2) && x.Item2.InBox(x2, x1, y0, y2, z0, z2)), depth+0x01, x2, x1, y0, y2, z0, z2);
				children[0x05] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x2, x1, y0, y2, z2, z1) && x.Item2.InBox(x2, x1, y0, y2, z2, z1)), depth+0x01, x2, x1, y0, y2, z2, z1);
				children[0x06] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x2, x1, y2, y1, z0, z2) && x.Item2.InBox(x2, x1, y2, y1, z0, z2)), depth+0x01, x2, x1, y2, y1, z0, z2);
				children[0x07] = Order(itemCache.Where(x => x.Item2.BoxOverlap(x2, x1, y2, y1, z2, z1) && x.Item2.InBox(x2, x1, y2, y1, z2, z1)), depth+0x01, x2, x1, y2, y1, z2, z1);
				return new OctTreeInnerNode(x2, y2, z2, children);

			}
			else {
				return new OctTreeLeaf(itemCache.Select(x => x.Item1));
			}
		}

		private abstract class OctTreeNode {

			public abstract IEnumerable<SubList> PropagateSubLists ();

		}

		private sealed class OctTreeInnerNode : OctTreeNode {

			public readonly double x, y, z;
			public readonly OctTreeNode[] children;

			public OctTreeInnerNode (double x, double y, double z, OctTreeNode[] children) {
				this.x = x;
				this.y = y;
				this.z = z;
				this.children = children;
			}

			public override IEnumerable<SubList> PropagateSubLists () {
				foreach(OctTreeNode c in children) {
					foreach(SubList sl in c.PropagateSubLists()) {
						yield return sl;
					}
				}
			}

		}

		private sealed class OctTreeLeaf : OctTreeNode {

			public readonly SubList sl = new SubList();

			public OctTreeLeaf (IEnumerable<int> items) {
				foreach(int item in items) {
					sl.Add(item);
				}
			}

			public override IEnumerable<SubList> PropagateSubLists () {
				yield return sl;
			}

		}

		private sealed class FastOctTreeNode {

			public readonly FastOctTreeNode[] node;
			public readonly double x, y, z;
			public readonly long data;

			public bool IsLeaf {
				get {
					return (this.node == null);
				}
			}

			public FastOctTreeNode (OctTreeNode node) {
				if(node is OctTreeInnerNode) {
					OctTreeInnerNode otin = (OctTreeInnerNode)node;
					this.x = otin.x;
					this.y = otin.y;
					this.z = otin.z;
					this.node = new FastOctTreeNode[0x08];
					for(int i = 0; i < 0x08; i++) {
						this.node[i] = new FastOctTreeNode(otin.children[i]);
					}
				}
				else {
					this.node = null;
					this.data = ((OctTreeLeaf)node).sl.ToLongRepresentation();
				}
			}

		}

	}

}