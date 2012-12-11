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
	public sealed class Point3 : ITransformable {
		
		[XmlIgnore]
		public static readonly Point3
			DummyPoint = new Point3(0.0d, 0.0d, 0.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyZPoint = new Point3(0.0d, 0.0d, 1.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyYPoint = new Point3(0.0d, 1.0d, 0.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyYZPoint = new Point3(0.0d, 1.0d, 1.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyXPoint = new Point3(1.0d, 0.0d, 0.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyXZPoint = new Point3(1.0d, 0.0d, 1.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyXYPoint = new Point3(1.0d, 1.0d, 0.0d);
		[XmlIgnore]
		public static readonly Point3
			DummyXYZPoint = new Point3(1.0d, 1.0d, 1.0d);
		public static readonly Point3[] UnitDummies = new Point3[] {DummyXPoint,DummyYZPoint,DummyZPoint};

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

		[XmlAttribute("X")]
		public double
			X;
		[XmlAttribute("Y")]
		public double
			Y;
		[XmlAttribute("Z")]
		public double
			Z;
		[XmlIgnore]
		public double this [int dim] {
			get {
				switch(dim) {
					case 0x00:
						return X;
					case 0x01:
						return Y;
					default :
						return Z;
				}
			}
			set {
				switch(dim) {
					case 0x00:
						X = value;
						break;
					case 0x01:
						Y = value;
						break;
					default :
						Z = value;
						break;
				}
			}
		}
		public double this [Point3 facenormal] {
			get {
				return this.X*facenormal.X+this.Y*facenormal.Y+this.Z*facenormal.Z;
			}
		}
		
		[XmlIgnore]
		public double Length {
			get {
				return Math.Sqrt(X*X+Y*Y+Z*Z);
			}
		}
		[XmlIgnore]
		public double SquareLength {
			get {
				return X*X+Y*Y+Z*Z;
			}
		}

		public Point3 () {
			this.X = 0.0d;
			this.Y = 0.0d;
			this.Z = 0.0d;
		}
		public Point3 (double x, double y) {
			this.X = x;
			this.Y = y;
			this.Z = 0.0d;
		}
		public Point3 (double x, double y, double z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		public Point3 (Point3 p) {
			this.X = p.X;
			this.Y = p.Y;
			this.Z = p.Z;
		}
		public Point3 (Point3 p, Matrix4 trans) {
			this.X = p.X;
			this.Y = p.Y;
			this.Z = p.Z;
			this.Transform(trans);
		}
		public Point3 (double x, double y, double z, Matrix4 trans) {
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Transform(trans);
		}
		public Point3 (Point3 frm, Point3 to) : this(to.X-frm.X,to.Y-frm.Y,to.Z-frm.Z) {
		}
		public Point3 (double x, double y, double z, double r) {//generate a random point on a sphere with radius r
			double dx = Maths.Random();
			double dy = Maths.Random();
			double dz = Maths.Random();
			double rinv = r/Math.Sqrt(dx*dx+dy*dy+dz*dz);
			this.X = rinv*dx+x;
			this.Y = rinv*dx+y;
			this.Z = rinv*dx+z;
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
		public void SetValues (Point3 mid, double r) {//set to a random point on a sphere with radius r
			double dx = Maths.Random();
			double dy = Maths.Random();
			double dz = Maths.Random();
			double rinv = r/Math.Sqrt(dx*dx+dy*dy+dz*dz);
			this.X = rinv*dx+mid.X;
			this.Y = rinv*dx+mid.Y;
			this.Z = rinv*dx+mid.Z;
		}
		public static void Reflect (Point3 init, Point3 normal, Point3 result) {
			double factor = -2.0d*(init.X*normal.X+init.Y*normal.Y+init.Z*normal.Z);
			result.X = factor*normal.X+init.X;
			result.Y = factor*normal.Y+init.Y;
			result.Z = factor*normal.Z+init.Z;
		}
		public static void ReflectRefract (Point3 init, Point3 normal, double nfrac, Point3 reflect, Point3 refract) {
			double cost1 = -init.X*normal.X-init.Y*normal.Y-init.Z*normal.Z;
			if(cost1 < 0.0d) {
				Point3 nnormal = new Point3(-normal.X, -normal.Y, -normal.Z);
				ReflectRefract(init, nnormal, 1.0d/nfrac, reflect, refract);
				return;
			}
			double ncost1 = nfrac*cost1;
			double cost2 = Math.Sqrt(1.0d+nfrac*nfrac*(cost1*cost1-1.0d));
			double factora = 2.0d*cost1;
			double factorb = (ncost1-cost2);
			reflect.X = factora*normal.X+init.X;
			reflect.Y = factora*normal.Y+init.Y;
			reflect.Z = factora*normal.Z+init.Z;
			reflect.Normalize();
			refract.X = nfrac*init.X+factorb*normal.X;
			refract.Y = nfrac*init.Y+factorb*normal.Y;
			refract.Z = nfrac*init.Z+factorb*normal.Z;
			refract.Normalize();
		}
		public void Mix3Normalize (Point3 xv, Point3 yv, double xf, double yf) {
			this.X = this.X+xv.X*xf+yv.X*yf;
			this.Y = this.Y+xv.Y*xf+yv.Y*yf;
			this.Z = this.Z+xv.Z*xf+yv.Z*yf;
			this.Normalize();
		}
		public static double CosAngle (Point3 pa, Point3 pb) {
			return pa.X*pb.X+pa.Y*pb.Y+pa.Z*pb.Z/Math.Sqrt((pa.X*pa.X+pa.Y*pa.Y+pa.Z*pa.Z)*(pb.X*pb.X+pb.Y*pb.Y+pb.Z*pb.Z));
		}
		public static int Order (double x, double y, double z, double a, double b, double c, double d) {
			return (3+(int)Math.Sign(x*a+b*y+c*z+d))>>1;
		}
		public void RotateLikeZVector (double xdiff, double ydiff) {
			double xyinv = xdiff*xdiff+ydiff*ydiff;
			if(xyinv > Maths.GlobalEpsilon) {
				double cost = 1.0d/Math.Sqrt(xyinv+1.0d);
				xyinv *= cost;
				double vx = xdiff*cost;
				double vy = ydiff*cost;
				double costasint2 = (1.0d-cost)/xyinv;
				double lina = (vy*this.X+vx*this.Y);
				double nx = costasint2*lina*vy+cost*this.X+vx*this.Z;
				double ny = costasint2*lina*vx+cost*this.Y+vy*this.Z;
				this.Z = vx*this.X+vy*this.Y+cost*this.Z;
				this.X = nx;
				this.Y = ny;
			}
		}
		//assumption: start is normalized
		public static IEnumerable<Point3> NormalizedConeGenerator (Point3 start, double thetamax, int n, double rotationSpeed) {
			double vx = start.X;
			double vy = start.Y;
			double vz = start.Z;
			double xyinv = vx*vx+vy*vy;
			double cost = 1.0d;
			double costasint2 = 0.0d;
			if(xyinv > Maths.GlobalEpsilon) {
				cost = vz;
				costasint2 = (1.0d-cost)/xyinv;
			}
			else if(vz < 1.0d) {
				cost = -1.0d;
			}
			double x = 1.0d, y = 1.0d, xt;
			double R2 = Math.Cos(0.5d*thetamax);
			R2 *= R2;
			double dr2 = R2/n;
			double xxyy = Math.Cos(rotationSpeed);
			double xyyx = Math.Sin(rotationSpeed);
			double xr, yr;
			for(double r2 = 0.0d; r2 < R2; r2 += dr2) {
				double r = Math.Sqrt(r2);
				double h = Math.Sqrt(1.0d-r2);
				xr = x*r;
				yr = y*r;
				double lina = (vy*xr+vx*yr);
				start.X = costasint2*lina*vy+cost*xr+vx*h;
				start.Y = costasint2*lina*vx+cost*yr+vy*h;
				start.Z = vx*xr+vy*yr+cost*h;
				yield return start;
				xt = xxyy*x-xyyx*y;
				y = xyyx*x+xxyy*y;
				x = xt;
			}
		}
		//assumption: the given vector is normalized
		public void NormalizedRotateLikeZVector (double vx, double vy, double vz) {
			double xyinv = vx*vx+vy*vy;
			if(xyinv > Maths.GlobalEpsilon) {
				double cost = vz;
				double costasint2 = (1.0d-cost)/xyinv;
				double lina = (vy*this.X+vx*this.Y);
				double nx = costasint2*lina*vy+cost*this.X+vx*this.Z;
				double ny = costasint2*lina*vx+cost*this.Y+vy*this.Z;
				this.Z = vx*this.X+vy*this.Y+cost*this.Z;
				this.X = nx;
				this.Y = ny;

			}
			else if(vz < 0.0d) {
				this.X = -this.X;
				this.Y = -this.Y;
				this.Z = -this.Z;
			}
		}
		//assumption: both vectors are normalized
		public static double CosAngleNorm (Point3 pa, Point3 pb) {
			return pa.X*pb.X+pa.Y*pb.Y+pa.Z*pb.Z;
		}
		public static double DiffLength (Point3 a, Point3 b) {
			double dx = a.X-b.X;
			double dy = a.Y-b.Y;
			double dz = a.Z-b.Z;
			return Math.Sqrt(dx*dx+dy*dy+dz*dz);
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

		public class FaceComp : IComparer<Point3> {

			public readonly Point3 Facenormal;

			public FaceComp (Point3 facenormal) {
				this.Facenormal = facenormal;
			}

			public int Compare (Point3 pa, Point3 pb) {
				return pa[this.Facenormal].CompareTo(pb[Facenormal]);
			}

		}
		#region ITransformable implementation
		public void Rotate (double ux, double uy, double uz, double theta) {
			double cost = Math.Cos(theta);
			double costa = 1.0d-cost;
			double sint = Math.Sin(theta);
			double nx = (cost+costa*ux*ux)*this.X+(ux*uy*costa-uz*sint)*this.Z+(ux*uz*costa+uy*sint)*this.Z;
			double ny = (ux*uy*costa+uz*sint)*this.X+(cost+costa*uy*uy)*this.Y+(uy*uz*costa-ux*sint)*this.Z;
			this.Z = (ux*uz*costa-uy*sint)*this.X+(uz*uy*costa+ux*sint)*this.Y+(cost+costa*uz*uz)*this.Z;
			this.X = nx;
			this.Y = ny;
		}
		public void RotateX (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double newy = cost*this.Y-sint*this.Z;
			this.Z = sint*this.Y+cost*this.Z;
			this.Y = newy;
		}

		public void RotateY (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double newx = cost*this.X+sint*this.Z;
			this.Z = cost*this.Z-sint*this.X;
			this.X = newx;
		}

		public void RotateZ (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double newx = cost*this.X-sint*this.Y;
			this.Y = sint*this.X+cost*this.Y;
			this.X = newx;
		}

		public void Shift (double dx, double dy, double dz) {
			this.X += dx;
			this.Y += dy;
			this.Z += dz;
		}

		public void Scale (double sx, double sy, double sz) {
			this.X *= sx;
			this.Y *= sy;
			this.Z *= sz;
		}
		public void Transform (Matrix4 M) {
			double nx = M.M00*X+M.M01*Y+M.M02*Z+M.M03;
			double ny = M.M10*X+M.M11*Y+M.M12*Z+M.M13;
			double nz = M.M20*X+M.M21*Y+M.M22*Z+M.M23;
			this.X = nx;
			this.Y = ny;
			this.Z = nz;
		}
		#endregion

		
	}
		
}
