//
//  ProxyTriangle.cs
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

namespace Renderer {

	public class ProxyTriangle : ProxyRenderItem {

		public readonly Point3 pa, pb, pc;

		public ProxyTriangle (RenderItem t, Point3 pa, Point3 pb, Point3 pc) : base(t) {
			this.pa = pa;
			this.pb = pb;
			this.pc = pc;
		}

		public override void GetBounds (out double x0, out double x1, out double y0, out double y1, out double z0, out double z1) {
			x0 = Maths.Min(this.pa.X, this.pb.X, this.pc.X);
			x1 = Maths.Max(this.pa.X, this.pb.X, this.pc.X);
			y0 = Maths.Min(this.pa.Y, this.pb.Y, this.pc.Y);
			y1 = Maths.Max(this.pa.Y, this.pb.Y, this.pc.Y);
			z0 = Maths.Min(this.pa.Z, this.pb.Z, this.pc.Z);
			z1 = Maths.Max(this.pa.Z, this.pb.Z, this.pc.Z);
		}
		public override void GetDimensionBounds (int dim, out double x0, out double x1) {
			double pat = this.pa[dim], pbt = this.pb[dim], pct = this.pc[dim];
			x0 = Maths.Min(pat, pbt, pct);
			x1 = Maths.Max(pat, pbt, pct);
		}
		public override void GetFaceNormalBounds (Point3 facenormal, out double t0, out double t1) {
			double pat = this.pa[facenormal], pbt = this.pb[facenormal], pct = this.pc[facenormal];
			t0 = Maths.Min(pat, pbt, pct);
			t1 = Maths.Max(pat, pbt, pct);
		}
		public override double Surface () {
			return Triangle.TriangleSurface(this.pa, this.pb, this.pc);
		}
		public override double SplitSurface (double sweep, int dimension) {
			return Triangle.TriangleSplitSurface(this.pa, this.pb, this.pc, sweep, dimension);
		}
		public override Tuple<ProxyRenderItem[], ProxyRenderItem[]> SplitAt (double sweep, int dimension) {
			return Triangle.TriangleSplitAt(this.Source, this.pa, this.pb, this.pc, sweep, dimension);
		}
		public override string ToString () {
			return string.Format("[ProxyTriangle {0} {1} {2}]", this.pa, this.pb, this.pc);
		}

	}
}

