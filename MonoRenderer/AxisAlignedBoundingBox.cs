//
//  AxisAlignedBoundingBox.cs
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

namespace Renderer {

	public class AxisAlignedBoundingBox : ITransformable {

		public double X0, X1, Y0, Y1, Z0, Z1;

		public bool IsEmpty {
			get {
				return (this.X0 >= this.X1 && this.Y0 >= this.Y1 && this.Z0 >= this.Z1);
			}
		}

		public AxisAlignedBoundingBox (AxisAlignedBoundingBox aabb, Matrix4 transform) : this(aabb) {
			this.Transform(transform);
		}
		public AxisAlignedBoundingBox (AxisAlignedBoundingBox aabb) : this(aabb.X0,aabb.X1,aabb.Y0,aabb.Y1,aabb.Z0,aabb.Z1) {
		}
		public AxisAlignedBoundingBox () : this (double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity) {
		}
		public AxisAlignedBoundingBox (double x0, double x1, double y0, double y1, double z0, double z1) {
			this.X0 = x0;
			this.X1 = x1;
			this.Y0 = y0;
			this.Y1 = y1;
			this.Z0 = z0;
			this.Z1 = z1;
		}

		public bool IntersectingBox (Ray ray, out double tin, out double tout) {
			if(IsEmpty) {
				tin = double.PositiveInfinity;
				tout = double.NegativeInfinity;
			}
			else {
				tin = 0.0d;
				tout = double.PositiveInfinity;
				Utils.CloseInterval(ray.X0, ray.DX, X0, X1, ref tin, ref tout);
				Utils.CloseInterval(ray.Y0, ray.DY, Y0, Y1, ref tin, ref tout);
				Utils.CloseInterval(ray.Z0, ray.DZ, Z0, Z1, ref tin, ref tout);
			}
			return (tin < tout);
		}

		public static AxisAlignedBoundingBox operator | (AxisAlignedBoundingBox aabb0, AxisAlignedBoundingBox aabb1) {
			double x0 = Math.Min(aabb0.X0, aabb1.X0);
			double x1 = Math.Max(aabb0.X1, aabb1.X1);
			double y0 = Math.Min(aabb0.Y0, aabb1.Y0);
			double y1 = Math.Max(aabb0.Y1, aabb1.Y1);
			double z0 = Math.Min(aabb0.Z0, aabb1.Z0);
			double z1 = Math.Max(aabb0.Z1, aabb1.Z1);
			return new AxisAlignedBoundingBox(x0, x1, y0, y1, z0, z1);
		}

		public static AxisAlignedBoundingBox operator & (AxisAlignedBoundingBox aabb0, AxisAlignedBoundingBox aabb1) {
			double x0 = Math.Max(aabb0.X0, aabb1.X0);
			double x1 = Math.Min(aabb0.X1, aabb1.X1);
			double y0 = Math.Max(aabb0.Y0, aabb1.Y0);
			double y1 = Math.Min(aabb0.Y1, aabb1.Y1);
			double z0 = Math.Max(aabb0.Z0, aabb1.Z0);
			double z1 = Math.Min(aabb0.Z1, aabb1.Z1);
			return new AxisAlignedBoundingBox(x0, x1, y0, y1, z0, z1);
		}

		public static AxisAlignedBoundingBox UnionAll (IEnumerable<AxisAlignedBoundingBox> aabbs) {
			double x0 = double.PositiveInfinity, x1 = double.NegativeInfinity, y0 = x0, y1 = x1, z0 = x0, z1 = x1;
			foreach(AxisAlignedBoundingBox aabb in aabbs) {
				x0 = Math.Min(x0, aabb.X0);
				x1 = Math.Max(x1, aabb.X1);
				y0 = Math.Min(y0, aabb.Y0);
				y1 = Math.Max(y1, aabb.Y1);
				z0 = Math.Min(z0, aabb.Z0);
				z1 = Math.Max(z1, aabb.Z1);
			}
			return new AxisAlignedBoundingBox(x0, x1, y0, y1, z0, z1);
		}
		public static AxisAlignedBoundingBox UnionAll (IEnumerable<RenderItem> ris) {
			double x0 = double.PositiveInfinity, x1 = double.NegativeInfinity, y0 = x0, y1 = x1, z0 = x0, z1 = x1;
			double x0t, x1t, y0t, y1t, z0t, z1t;
			foreach(RenderItem ri in ris) {
				ri.GetBounds(out x0t, out x1t, out y0t, out y1t, out z0t, out z1t);
				x0 = Math.Min(x0, x0t);
				x1 = Math.Max(x1, x1t);
				y0 = Math.Min(y0, y0t);
				y1 = Math.Max(y1, y1t);
				z0 = Math.Min(z0, z0t);
				z1 = Math.Max(z1, z1t);
			}
			return new AxisAlignedBoundingBox(x0, x1, y0, y1, z0, z1);
		}
		public void Union (IEnumerable<RenderItem> ris) {
			double x0t, x1t, y0t, y1t, z0t, z1t;
			foreach(RenderItem ri in ris) {
				ri.GetBounds(out x0t, out x1t, out y0t, out y1t, out z0t, out z1t);
				this.X0 = Math.Min(this.X0, x0t);
				this.X1 = Math.Max(this.X1, x1t);
				this.Y0 = Math.Min(this.Y0, y0t);
				this.Y1 = Math.Max(this.Y1, y1t);
				this.Z0 = Math.Min(this.Z0, z0t);
				this.Z1 = Math.Max(this.Z1, z1t);
			}
		}
		public void Union (AxisAlignedBoundingBox aabb) {
			this.X0 = Math.Min(this.X0, aabb.X0);
			this.X1 = Math.Max(this.X1, aabb.X1);
			this.Y0 = Math.Min(this.Y0, aabb.Y0);
			this.Y1 = Math.Max(this.Y1, aabb.Y1);
			this.Z0 = Math.Min(this.Z0, aabb.Z0);
			this.Z1 = Math.Max(this.Z1, aabb.Z1);
		}
		public static AxisAlignedBoundingBox WedgeAll (IEnumerable<AxisAlignedBoundingBox> aabbs) {
			double x0 = double.NegativeInfinity, x1 = double.PositiveInfinity, y0 = x0, y1 = x1, z0 = x0, z1 = x1;
			foreach(AxisAlignedBoundingBox aabb in aabbs) {
				x0 = Math.Max(x0, aabb.X0);
				x1 = Math.Min(x1, aabb.X1);
				y0 = Math.Max(y0, aabb.Y0);
				y1 = Math.Min(y1, aabb.Y1);
				z0 = Math.Max(z0, aabb.Z0);
				z1 = Math.Min(z1, aabb.Z1);
			}
			return new AxisAlignedBoundingBox(x0, x1, y0, y1, z0, z1);
		}

		public void Adapt (Action<Point3> adaptFunction) {
			if(this.IsEmpty) {
				return;
			}
			double x0n = double.PositiveInfinity, y0n = x0n, z0n = x0n, x1n = double.NegativeInfinity, y1n = x1n, z1n = x1n;
			Point3 p = new Point3(X0, Y0, Z0);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X0, Y0, Z1);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X0, Y1, Z0);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X0, Y1, Z1);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X1, Y0, Z0);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X1, Y0, Z1);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X1, Y1, Z0);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			p.SetValues(X1, Y1, Z1);
			adaptFunction(p);
			AdaptInclude(ref x0n, ref x1n, ref y0n, ref y1n, ref z0n, ref z1n, p);
			this.X0 = x0n;
			this.X1 = x1n;
			this.Y0 = y0n;
			this.Y1 = y1n;
			this.Z0 = z0n;
			this.Z1 = z1n;
		}
		public static void AdaptInclude (ref double x0, ref double x1, ref double y0, ref double y1, ref double z0, ref double z1, Point3 p) {
			x0 = Math.Min(x0, p.X);
			x1 = Math.Max(x1, p.X);
			y0 = Math.Min(y0, p.Y);
			y1 = Math.Max(y1, p.Y);
			z0 = Math.Min(z0, p.Z);
			z1 = Math.Max(z1, p.Z);
		}
		#region ITransformable implementation

		public void Rotate (double ux, double uy, double uz, double theta) {
			this.Adapt(x => x.Rotate(ux, uy, uz, theta));
		}


		public void RotateX (double theta) {
			this.Adapt(x => x.RotateX(theta));
		}

		public void RotateY (double theta) {
			this.Adapt(x => x.RotateY(theta));
		}

		public void RotateZ (double theta) {
			this.Adapt(x => x.RotateZ(theta));
		}

		public void Shift (double dx, double dy, double dz) {
			this.Adapt(x => x.Shift(dx, dy, dz));
		}

		public void Scale (double sx, double sy, double sz) {
			this.Adapt(x => x.Scale(sx, sy, sz));
		}

		public void Transform (Matrix4 N) {
			this.Adapt(x => x.Transform(N));
		}
		#endregion

		public override string ToString () {
			return string.Format("[AxisAlignedBoundingBox: X:[{0};{1}] Y:[{2};{3}] Z:[{3};{4}]]", X0, X1, Y0, Y1, Z0, Z1);
		}

		public class BoundingBoxRayComparator : IComparer<AxisAlignedBoundingBox> {

			private readonly Ray ray;

			public BoundingBoxRayComparator (Ray ray) {
				this.ray = ray;
			}

			#region IComparer implementation
			public int Compare (AxisAlignedBoundingBox x, AxisAlignedBoundingBox y) {
				double t0, t1, tdummy;
				if(y.IntersectingBox(this.ray, out t1, out tdummy)) {
					if(y.IntersectingBox(this.ray, out t0, out tdummy)) {
						return t0.CompareTo(t1);
					}
					return 0x01;
				}
				return -0x01;
			}
			#endregion

		}


	}
}