using System;
using System.Collections.Generic;

namespace Renderer {
	
	public sealed class Triangle : RenderItem
	{
		
		private readonly Point3 p0, p1, p2, n0, n1, n2;
		private readonly Point3 t0, t1, t2;
		
		public Triangle (Point3 a, Point3 b, Point3 c, Point3 na, Point3 nb, Point3 nc, Point3 ta, Point3 tb, Point3 tc, Material material) : base(material) {
			this.p0 = a;
			this.p1 = b;
			this.p2 = c;
			if(na == null || nb == null || nc == null) {
				Point3 norm = new Point3();
				Point3.Cross(p1.X-p0.X, p1.Y-p0.Y, p1.Z-p0.Z, p2.X-p0.X, p2.Y-p0.Y, p2.Z-p0.Z, out norm.X, out norm.Y, out norm.Z);
				norm.Normalize();
				this.n0 = this.n1 = this.n2 = norm;
			} else {
				this.n0 = na;
				this.n1 = nb;
				this.n2 = nc;
			}
			if(ta == null || tb == null || tc == null) {
				this.t0 = Point3.DummyPoint;
				this.t1 = Point3.DummyPoint;
				this.t2 = Point3.DummyPoint;
			} else {
				this.t0 = ta;
				this.t1 = tb;
				this.t2 = tc;
			}
		}
		
		public override void GetBounds (out double xm, out double xM, out double ym, out double yM, out double zm, out double zM) {
			xm = Maths.Min(p0.X, p1.X, p2.X);
			xM = Maths.Max(p0.X, p1.X, p2.X);
			ym = Maths.Min(p0.Y, p1.Y, p2.Y);
			yM = Maths.Max(p0.Y, p1.Y, p2.Y);
			zm = Maths.Min(p0.Z, p1.Z, p2.Z);
			zM = Maths.Max(p0.Z, p1.Z, p2.Z);
		}
		public override void GetDimensionBounds (int dim, out double x0, out double x1) {
			x0 = Maths.Min(p0 [dim], p1 [dim], p2 [dim]);
			x1 = Maths.Max(p0 [dim], p1 [dim], p2 [dim]);
		}
		public override bool InBox (double xm, double xM, double ym, double yM, double zm, double zM) {
			double v0x, v0y, v0z, v1x, v1y, v1z, v2x, v2y, v2z;
			double fex, fey, fez;
			double normalx, normaly, normalz, e0x, e0y, e0z, e1x, e1y, e1z, e2x, e2y, e2z;
			double boxx = 0.5d*(xm+xM), boxy = 0.5d*(ym+yM), boxz = 0.5d*(zm+zM);
			double bhx = xM-boxx, bhy = yM-boxy, bhz = zM-boxz;

			v0x = this.p0.X-boxx;
			v0y = this.p0.Y-boxy;
			v0z = this.p0.Z-boxz;
			v1x = this.p1.X-boxx;
			v1y = this.p1.Y-boxy;
			v1z = this.p1.Z-boxz;
			v2x = this.p2.X-boxx;
			v2y = this.p2.Y-boxy;
			v2z = this.p2.Z-boxz;

			e0x = v1x-v0x;
			e0y = v1y-v0y;
			e0z = v1z-v0z;
			e1x = v2x-v1x;
			e1y = v2y-v1y;
			e1z = v2z-v1z;
			e2x = v0x-v2x;
			e2y = v0y-v2y;
			e2z = v0z-v2z;

			fex = Math.Abs(e0x);
			fey = Math.Abs(e0y);
			fez = Math.Abs(e0z);

			if(invAxisTestYZ(bhy, bhz, e0z, e0y, fez, fey, v0y, v0z, v2y, v2z) || 
				invAxisTestYZ(bhx, bhz, -e0z, -e0x, fez, fex, v0x, v0z, v2x, v2z) ||
				invAxisTestYZ(bhx, bhy, e0y, e0x, fey, fex, v2x, v2y, v1x, v1y)) {
				return false;
			}
			/*AXISTEST_X01(e0 [Z], e0 [Y], fez, fey);
			AXISTEST_Y02(e0 [Z], e0 [X], fez, fex);
			AXISTEST_Z12(e0 [Y], e0 [X], fey, fex);*/

			fex = Math.Abs(e1x);
			fey = Math.Abs(e1y);
			fez = Math.Abs(e1z);
			if(invAxisTestYZ(bhy, bhz, e1z, e1y, fez, fey, v0y, v0z, v2y, v2z) ||
				invAxisTestYZ(bhx, bhz, -e1z, -e1x, fez, fex, v0x, v0z, v2x, v2z) ||
				invAxisTestYZ(bhx, bhy, e1y, e1x, fey, fex, v0x, v0y, v1x, v1y)) {
				return false;
			}
			/*AXISTEST_X01(e1 [Z], e1 [Y], fez, fey);
			AXISTEST_Y02(e1 [Z], e1 [X], fez, fex);
			AXISTEST_Z0(e1 [Y], e1 [X], fey, fex);*/

			fex = Math.Abs(e2x);
			fey = Math.Abs(e2y);
			fez = Math.Abs(e2z);
			if(invAxisTestYZ(bhy, bhz, e2z, e2y, fez, fey, v0y, v0z, v1y, v1z) ||
				invAxisTestYZ(bhx, bhz, -e2z, -e2x, fez, fex, v0x, v0z, v1x, v1z) ||
				invAxisTestYZ(bhx, bhy, e2y, e2x, fey, fex, v2x, v2y, v1x, v1y)) {
				return false;
			}
			/*AXISTEST_X2(e2 [Z], e2 [Y], fez, fey);
			AXISTEST_Y1(e2 [Z], e2 [X], fez, fex);
			AXISTEST_Z12(e2 [Y], e2 [X], fey, fex);*/

/* Bullet 1: */
/*  first test overlap in the {x,y,z}-directions */
/*  find min, max of the triangle each direction, and test for overlap in */
/*  that direction -- this is equivalent to testing a minimal AABB around */
/*  the triangle against the AABB */

/* test in X-direction */
			if(invMinMaxBound(bhx, v0x, v1x, v2x) ||
				invMinMaxBound(bhy, v0y, v1y, v2y) ||
				invMinMaxBound(bhz, v0z, v1z, v2z)) {
				return false;
			}
			/*FINDMINMAX(v0 [X], v1 [X], v2 [X], min, max);
			if(min > boxhalfsize [X] || max < -boxhalfsize [X])
				return 0;
			FINDMINMAX(v0 [Y], v1 [Y], v2 [Y], min, max);
			if(min > boxhalfsize [Y] || max < -boxhalfsize [Y])
				return 0;
			FINDMINMAX(v0 [Z], v1 [Z], v2 [Z], min, max);
			if(min > boxhalfsize [Z] || max < -boxhalfsize [Z])
				return 0;*/

/* Bullet 2: */
/*  test if the box intersects the plane of the triangle */
/*  compute plane equation of triangle: normal*x+d=0 */
			Point3.Cross(e0x, e0y, e0z, e1x, e1y, e1z, out normalx, out normaly, out normalz);	
			return invPlaneBoxOverlap(normalx, normaly, normalz, v0x, v0y, v0z, bhx, bhy, bhz);
			/* plane eq: normal.x+d=0 */
			/*CROSS(normal, e0, e1);
			d = -DOT(normal, v0);
			if(!planeBoxOverlap(normal, d, boxhalfsize))
				return false;*/
		}
		private bool invPlaneBoxOverlap (double normalx, double normaly, double normalz, double v0x, double v0y, double v0z, double bhx, double bhy, double bhz) {
			int q;
			double s0 = Math.Sign(normalx), s1 = Math.Sign(normaly), s2 = Math.Sign(normalz);
			s0 = s0-s0*s0+1;
			s1 = s1-s1*s1+1;
			s2 = s2-s2*s2+1;
			double vmaxx = s0*bhx-v0x, vmaxy = s1*bhy-v0y, vmaxz = s2*bhz-v0z;
			v0x += s0*bhx;
			v0y += s1*bhy;
			v0z += s2*bhz;
			if(Point3.Dot(normalx, normaly, normalz, v0x, v0y, v0z) <= 0.0f) {
				return false;
			}
			if(Point3.Dot(normalx, normaly, normalz, vmaxx, vmaxy, vmaxz) >= 0.0f) {
				return true;
			}
			return false;
		}
		private bool invAxisTestYZ (double bhy, double bhz, double a, double b, double fa, double fb, double v0y, double v0z, double v2y, double v2z) {
			double min, max, p0, p1, p2, rad;
			p0 = a*v0y-b*v0z;
			p2 = a*v2y-b*v2z;
			min = Math.Min(p0, p2);
			max = p0+p2-min;
			rad = fa*bhy+fb*bhz;
			return (min > rad || max < -rad);
		}
		private static bool invMinMaxBound (double bh, double a1, double a2, double a3) {
			double min = Math.Min(a1, Math.Min(a2, a3));
			double max = Math.Max(a1, Math.Max(a2, a3));
			return (min > bh || max < -bh);
		}
		public override double HitAt (Ray ray) {
			double j = ray.X0-p0.X;
			double k = ray.Y0-p0.Y;
			double l = ray.Z0-p0.Z;
			double dx01 = p1.X-p0.X;
			double dy01 = p1.Y-p0.Y;
			double dz01 = p1.Z-p0.Z;
			double dx02 = p2.X-p0.X;
			double dy02 = p2.Y-p0.Y;
			double dz02 = p2.Z-p0.Z;
			double M = 1.0d/(dx01*(ray.DY*dz02-dy02*ray.DZ)+dx02*(dy01*ray.DZ-ray.DY*dz01)+ray.DX*(dy02*dz01-dy01*dz02));
			double beta = (dx02*(ray.DZ*k-ray.DY*l)+ray.DX*(dy02*l-dz02*k)+(ray.DY*dz02-dy02*ray.DZ)*j)*M;
			if(beta < 0.0d) {
				return double.PositiveInfinity;
			}
			double gamma = -(dx01*(ray.DZ*k-ray.DY*l)+ray.DX*(dy01*l-dz01*k)+(ray.DY*dz01-dy01*ray.DZ)*j)*M;
			if(gamma < 0.0d || beta+gamma >= 1.0d) {
				return double.PositiveInfinity;
			}
			double t = -(dx01*(dz02*k-dy02*l)+dx02*(dy01*l-dz01*k)+(dy02*dz01-dy01*dz02)*j)*M;
			if(t <= 0.0d) {
				return double.PositiveInfinity;
			} else {
				return t;
			}
		}
		public override void Cast (Ray ray, CastResult cr) {
			double j = ray.X0-p0.X;
			double k = ray.Y0-p0.Y;
			double l = ray.Z0-p0.Z;
			double dx01 = p1.X-p0.X;
			double dy01 = p1.Y-p0.Y;
			double dz01 = p1.Z-p0.Z;
			double dx02 = p2.X-p0.X;
			double dy02 = p2.Y-p0.Y;
			double dz02 = p2.Z-p0.Z;
			double M = 1.0d/(dx01*(ray.DY*dz02-dy02*ray.DZ)+dx02*(dy01*ray.DZ-ray.DY*dz01)+ray.DX*(dy02*dz01-dy01*dz02));
			double beta = (dx02*(ray.DZ*k-ray.DY*l)+ray.DX*(dy02*l-dz02*k)+(ray.DY*dz02-dy02*ray.DZ)*j)*M;
			double gamma = -(dx01*(ray.DZ*k-ray.DY*l)+ray.DX*(dy01*l-dz01*k)+(ray.DY*dz01-dy01*ray.DZ)*j)*M;
			double t = -(dx01*(dz02*k-dy02*l)+dx02*(dy01*l-dz01*k)+(dy02*dz01-dy01*dz02)*j)*M;
			double alpha = 1.0d-beta-gamma;
			cr.Copy(t,
			        alpha*n0.X+beta*n1.X+gamma*n2.X,
			        alpha*n0.Y+beta*n1.Y+gamma*n2.Y,
			        alpha*n0.Z+beta*n1.Z+gamma*n2.Z,
			        alpha*t0.X+beta*t1.X+gamma*t2.X,
			        alpha*t0.Y+beta*t1.Y+gamma*t2.Y,
			        alpha*t0.Z+beta*t1.Z+gamma*t2.Z);
			cr.NormalizeNormal();
		}

		public override double Surface () {
			return TriangleSurface(this.p0, this.p1, this.p2);
		}
		public static double TriangleSurface (Point3 pa, Point3 pb, Point3 pc) {
			return 0.5d*Point3.CrossLength(pb.X-pa.X,
			                               pb.Y-pa.Y,
			                               pb.Z-pa.Z,
			                               pc.X-pa.X,
			                               pc.Y-pa.Y,
			                               pc.Z-pa.Z);
		}
		public override double SplitSurface (double sweep, int dim) {
			return TriangleSplitSurface(p0, p1, p2, sweep, dim);
		}
		public static double TriangleSplitSurface (Point3 pa, Point3 pb, Point3 pc, double sweep, int dim) {
			IComparer<Point3> co = Point3.Comparers [dim];
			Maths.Order(co, ref pa, ref pb);
			Maths.Order(co, ref pa, ref pc);
			Maths.Order(co, ref pb, ref pc);
			double sf = TriangleSurface(pa, pb, pc);
			if(pb [dim] >= sweep) {
				double fac = (sweep-pa [dim]);
				fac *= fac;
				fac /= (pb [dim]-pa [dim])*(pc [dim]-pa [dim]);
				return fac*sf;
			} else {
				double fac = (pc [dim]-sweep);
				fac *= fac;
				fac /= (pc [dim]-pb [dim])*(pc [dim]-pa [dim]);
				return (1.0d-fac)*sf;
			}
		}
		public override Tuple<ProxyRenderItem[], ProxyRenderItem[]> SplitAt (double sweep, int dim) {
			return TriangleSplitAt(this, this.p0, this.p1, this.p2, sweep, dim);
		}
		public static Tuple<ProxyRenderItem[], ProxyRenderItem[]> TriangleSplitAt (RenderItem parent, Point3 pa, Point3 pb, Point3 pc, double sweep, int dim) {
			IComparer<Point3> co = Point3.Comparers [dim];
			Maths.Order(co, ref pa, ref pb);
			Maths.Order(co, ref pa, ref pc);
			Maths.Order(co, ref pb, ref pc);
			ProxyRenderItem[] la = new ProxyRenderItem[1], lb;
			double fac = (sweep-pa [dim])/(pc [dim]-pa [dim]);
			double fad = 1.0d-fac;
			Point3 pac = new Point3(fac*pc.X+fad*pa.X, fac*pc.Y+fad*pa.Y, fac*pc.Z+fad*pa.Z), pabc;
			if(pb [dim] != sweep) {
				lb = new ProxyRenderItem[2];
				if(pb [dim] > sweep) {
					//left 1/right 2
					fac = (sweep-pa [dim])/(pb [dim]-pa [dim]);
					fad = 1.0d-fac;
					pabc = new Point3(fac*pb.X+fad*pa.X, fac*pb.Y+fad*pa.Y, fac*pb.Z+fad*pa.Z);
					la [0x00] = new ProxyTriangle(parent, pa, pac, pabc);
					lb [0x00] = new ProxyTriangle(parent, pac, pb, pabc);
					lb [0x01] = new ProxyTriangle(parent, pac, pc, pb);
					return new Tuple<ProxyRenderItem[],ProxyRenderItem[]>(la, lb);
				} else {
					//left 2/right 1
					fac = (sweep-pb [dim])/(pc [dim]-pb [dim]);
					fad = 1.0d-fac;
					pabc = new Point3(fac*pc.X+fad*pb.X, fac*pc.Y+fad*pb.Y, fac*pc.Z+fad*pb.Z);
					la [0x00] = new ProxyTriangle(parent, pc, pac, pabc);
					lb [0x00] = new ProxyTriangle(parent, pac, pb, pabc);
					lb [0x01] = new ProxyTriangle(parent, pac, pa, pb);
					return new Tuple<ProxyRenderItem[],ProxyRenderItem[]>(lb, la);
				}

			} else {
				lb = new ProxyRenderItem[1];
				la [0x00] = new ProxyTriangle(parent, pa, pac, pb);
				lb [0x00] = new ProxyTriangle(parent, pac, pc, pb);
				return new Tuple<ProxyRenderItem[],ProxyRenderItem[]>(la, lb);
			}
		}

		public override string ToString () {
			return string.Format("[Triangle {0} {1} {2}]", this.p0, this.p1, this.p2);
		}
		
	}
}

