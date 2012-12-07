//  
//  Point3.cs
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
using System.Xml.Serialization;

namespace Renderer {
	
	[XmlType("Point3")]
	public sealed class Point3 {
		
		[XmlIgnore]
		public static readonly Point3
			DummyPoint = new Point3(0.0d, 0.0d, 0.0d);

		[XmlIgnore]
		public static readonly IComparer<Point3>
			XComparator = new XComp();
		[XmlIgnore]
		public static readonly IComparer<Point3>
			YComparator = new YComp();
		[XmlIgnore]
		public static readonly IComparer<Point3>
			ZComparator = new ZComp();
		[XmlIgnore]
		public static readonly IComparer<Point3>[]
			Comparers = new IComparer<Point3>[] {XComparator,YComparator,ZComparator};

		[XmlIgnore]
		public readonly double[]
			vals;
		[XmlIgnore]
		[Obsolete]
		public double this [int dim] {
			get {
				return this.vals[dim];
			}
			set {
				this.vals[dim] = value;
			}
		}
		[XmlAttribute("X")]
		[Obsolete]
		public double X {
			get {
				return this.vals[0x00];
			}
			set {
				this.vals[0x00] = value;
			}
		}
		[XmlAttribute("Y")]
		[Obsolete]
		public double Y {
			get {
				return this.vals[0x01];
			}
			set {
				this.vals[0x01] = value;
			}
		}
		[XmlAttribute("Z")]
		[Obsolete]
		public double Z {
			get {
				return this.vals[0x02];
			}
			set {
				this.vals[0x02] = value;
			}
		}
		
		[XmlIgnore]
		public double Length {
			get {
				return Math.Sqrt(vals[0x00]*vals[0x00]+vals[0x01]*vals[0x01]+vals[0x02]*vals[0x02]);
			}
		}
		[XmlIgnore]
		public double SquareLength {
			get {
				return vals[0x00]*vals[0x00]+vals[0x01]*vals[0x01]+vals[0x02]*vals[0x02];
			}
		}

		public Point3 () {
			this.vals = new double[]{0.0d,0.0d,0.0d};
		}
		public Point3 (double x, double y, double z) {
			this.vals = new double[]{0.0d,0.0d,0.0d};
		}
		public Point3 (Point3 p) {
			this.vals = new double[]{p[0x00],p[0x01],p[0x02]};
		}
		public Point3 (Point3 p, Matrix4 trans) {
			this.vals = new double[]{p[0x00],p[0x01],p[0x02]};
			this.Transform(trans);
		}
		public Point3 (double x, double y, double z, Matrix4 trans) {
			this.vals = new double[]{x,y,z};
			this.Transform(trans);
		}
		public Point3 (Point3 frm, Point3 to) : this(to.X-frm.X,to.Y-frm.Y,to.Z-frm.Z) {
		}
		
		public void Transform (Matrix4 M) {
			double nx = M.M00*vals[0x00]+M.M01*vals[0x01]+M.M02*vals[0x02]+M.M03;
			double ny = M.M10*vals[0x00]+M.M11*vals[0x01]+M.M12*vals[0x02]+M.M13;
			double nz = M.M20*vals[0x00]+M.M21*vals[0x01]+M.M22*vals[0x02]+M.M23;
			this.vals[0x00] = nx;
			this.vals[0x01] = ny;
			this.vals[0x02] = nz;
		}
		public void TransformNonShift (Matrix4 M) {
			double nx = M.M00*X+M.M01*Y+M.M02*Z;
			double ny = M.M10*X+M.M11*Y+M.M12*Z;
			double nz = M.M20*X+M.M21*Y+M.M22*Z;
			this.X = nx;
			this.Y = ny;
			this.Z = nz;
		}
		public static Point3 Random () {
			return new Point3(Maths.Random(), Maths.Random(), Maths.Random());
		}
		public static Point3 Cross (Point3 a, Point3 b) {
			return new Point3(a.Y*b.Z-b.Y*a.Z, a.Z*b.X-b.Z*a.X, a.X*b.Y-b.X*a.Y);
		}

		public void AddDirect (Point3 p) {
			this.X += p.X;
			this.Y += p.Y;
			this.Z += p.Z;
		}

		public static void Cross (double xa, double ya, double za, double xb, double yb, double zb, out double xc, out double yc, out double zc) {
			xc = ya*zb-yb*za;
			yc = za*xb-zb*xa;
			zc = xa*yb-xb*ya;
		}
		public static double Dot (double xa, double ya, double za, double xb, double yb, double zb) {
			return xa*xb+ya*yb+za*zb;
		}
		public bool InBox (double xm, double xM, double ym, double yM, double zm, double zM) {
			return (this.X >= xm && this.X <= xM && this.Y >= ym && this.Y <= yM && this.Z >= zm && this.Z <= zM);
		}
		public static double CrossLength (double xa, double ya, double za, double xb, double yb, double zb) {
			double xc = ya*zb-yb*za;
			double yc = za*xb-zb*xa;
			double zc = xa*yb-xb*ya;
			return Math.Sqrt(xc*xc+yc*yc+zc*zc);
		}
		public static Point3 CrossNormalize (Point3 a, Point3 b) {
			Point3 c = Cross(a, b);
			c.Normalize();
			return c;
		}
		public static void CrossNormalize (double xa, double ya, double za, double xb, double yb, double zb, out double xc, out double yc, out double zc) {
			xc = ya*zb-yb*za;
			yc = za*xb-zb*xa;
			zc = xa*yb-xb*ya;
			double rinv = 1.0d/Math.Sqrt(xc*xc+yc*yc+zc*zc);
			xc *= rinv;
			yc *= rinv;
			zc *= rinv;
		}
		public void Normalize () {
			double r = X*X+Y*Y+Z*Z;
			if(r > 0.0d) {
				r = 1.0d/Math.Sqrt(r);
				X *= r;
				Y *= r;
				Z *= r;
			}
		}
		public static Point3 Reflect (Point3 init, Point3 normal) {
			double factor = -2.0d*(init.X*normal.X+init.Y*normal.Y+init.Z*normal.Z);
			return new Point3(factor*normal.X+init.X, factor*normal.Y+init.Y, factor*normal.Z+init.Z);
		}
		public void SetValues (Point3 frm) {
			this.X = frm.X;
			this.Y = frm.Y;
			this.Z = frm.Z;
		}
		public void SetValues (double x, double y, double z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		public void SetValues (Point3 frm, Point3 to) {
			this.X = to.X-frm.X;
			this.Y = to.Y-frm.Y;
			this.Z = to.Z-frm.Z;
		}
		public static void Reflect (Point3 init, Point3 normal, Point3 result) {
			double factor = -2.0d*(init.X*normal.X+init.Y*normal.Y+init.Z*normal.Z);
			result.X = factor*normal.X+init.X;
			result.Y = factor*normal.Y+init.Y;
			result.Z = factor*normal.Z+init.Z;
		}
		public static void ReflectRefract (Point3 init, Point3 normal, double nfrac, out Point3 reflect, out Point3 refract) {
			double cost1 = -init.X*normal.X-init.Y*normal.Y-init.Z*normal.Z;
			double ncost1 = nfrac*cost1;
			double cost2 = Math.Sqrt(1.0d+ncost1*ncost1-nfrac*nfrac);
			double factora = 2.0d*cost1;
			double factorb = (ncost1-cost2)*Math.Sign(ncost1);
			reflect = new Point3(factora*normal.X+init.X, factora*normal.Y+init.Y, factora*normal.Z+init.Z);
			refract = new Point3(nfrac*init.X+factorb*normal.X, nfrac*init.Y+factorb*normal.Y, nfrac*init.Z+factorb*normal.Z);
		}
		public static double CosAngle (Point3 pa, Point3 pb) {
			return pa.X*pb.X+pa.Y*pb.Y+pa.Z*pb.Z/Math.Sqrt((pa.X*pa.X+pa.Y*pa.Y+pa.Z*pa.Z)*(pb.X*pb.X+pb.Y*pb.Y+pb.Z*pb.Z));
		}
		public static int Order (double x, double y, double z, double a, double b, double c, double d) {
			return (3+(int)Math.Sign(x*a+b*y+c*z+d))>>1;
		}
		public static double CosAngleNorm (Point3 pa, Point3 pb) {
			return pa.X*pb.X+pa.Y*pb.Y+pa.Z*pb.Z;//assumption: both vectors are normalized
		}
		public static Point3 operator + (Point3 a, Point3 b) {
			return new Point3(a.X+b.X, a.Y+b.Y, a.Z+b.Z);
		}
		public static Point3 operator - (Point3 a, Point3 b) {
			return new Point3(a.X-b.X, a.Y-b.Y, a.Z-b.Z);
		}
		public static double operator * (Point3 a, Point3 b) {
			return a.X*b.X+a.Y*b.Y+a.Z*b.Z;
		}
		public override string ToString () {
			return string.Format("({0};{1};{2})", X.ToString("0.000"), Y.ToString("0.000"), Z.ToString("0.000"));
		}

		private class XComp : IComparer<Point3> {

			public int Compare (Point3 pa, Point3 pb) {
				return pa.X.CompareTo(pb.X);
			}

		}

		private class YComp : IComparer<Point3> {

			public int Compare (Point3 pa, Point3 pb) {
				return pa.Y.CompareTo(pb.Y);
			}

		}

		private class ZComp : IComparer<Point3> {

			public int Compare (Point3 pa, Point3 pb) {
				return pa.Z.CompareTo(pb.Z);
			}

		}

		private class FaceComp : IComparer<Point3> {

			public readonly double A, B, C;

			public FaceComp (double a, double b, double c) {
				this.A = a;
				this.B = b;
				this.C = c;
			}

			public int Compare (Point3 pa, Point3 pb) {
				return (A*pa.X+B*pa.Y+C*pa.Z).CompareTo(A*pb.X+B*pb.Y+C*pb.Z);
			}

		}
		
	}
		
}
