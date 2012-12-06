//
//  BoundingBox.cs
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

	public sealed class BoundingBox : ProxyRenderItem {

		public Point3 xyz0;
		public Point3 xyz1;
		public double X0 {
			get {
				return this.xyz0.X;
			}
		}
		public double Y0 {
			get {
				return this.xyz0.Y;
			}
		}
		public double Z0 {
			get {
				return this.xyz0.Z;
			}
		}
		public double X1 {
			get {
				return this.xyz1.X;
			}
		}
		public double Y1 {
			get {
				return this.xyz1.Y;
			}
		}
		public double Z1 {
			get {
				return this.xyz1.Z;
			}
		}

		public BoundingBox () : this(double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity) {

		}

		public BoundingBox (double x0, double x1, double y0, double y1, double z0, double z1) : base(null) {
			this.SetValues(x0, x1, y0, y1, z0, z1);
		}

		public void SetValues (double x0, double x1, double y0, double y1, double z0, double z1) {
			this.xyz0.SetValues(x0, y0, z0);
			this.xyz1.SetValues(x1, y1, z1);
		}


		public override double Surface () {
			double w = xyz1.X-xyz0.X;
			double h = xyz1.Y-xyz0.Y;
			double d = xyz1.Z-xyz0.Z;
			return 2.0d*(w*h+w*d+h*d);
		}
		public override double SplitSurface (double sweep, int dim) {
			double[] whd = {xyz1.X-xyz0.X,xyz1.Y-xyz0.Y,xyz1.Z-xyz0.Z};
			whd[dim] = sweep-xyz0[dim];
			return 2.0d*(whd[0x00]*whd[0x01]+whd[0x00]*whd[0x02]+whd[0x01]*whd[0x02]);
		}
		public override void SplitAt (double sweep, int dim, out BoundingBox left, out BoundingBox right) {
			left = new BoundingBox();
			left.xyz0.SetValues(this.xyz0);
			left.xyz1.SetValues(this.xyz1);
			left.xyz1[dim] = sweep;
			right = new BoundingBox();
			right.xyz0.SetValues(this.xyz0);
			right.xyz1.SetValues(this.xyz1);
			right.xyz0[dim] = sweep;
		}
		public override Tuple<ProxyRenderItem[],ProxyRenderItem[]> SplitAt (double sweep, int dim) {
			BoundingBox l, r;
			SplitAt(sweep, dim, out l, out r);
			return new Tuple<ProxyRenderItem[], ProxyRenderItem[]>(new ProxyRenderItem[] {l}, new ProxyRenderItem[] {r});
		}
		public override void GetDimensionBounds (int dim, out double x0, out double x1) {
			x0 = xyz0[dim];
			x1 = xyz1[dim];
		}
	}
}