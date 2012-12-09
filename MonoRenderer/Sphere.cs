using System;

namespace Renderer {
	
	public sealed class Sphere : RenderItem {
		
		public Point3 Center;
		public readonly double Radius;
		private readonly double Rinv;
		public Material material;
		
		public Sphere (Point3 center, double radius, Material material) : base(material) {
			this.Center = center;
			this.Radius = radius;
			this.Rinv = 1.0d/Radius;
			this.material = material;
		}
		
		public override void GetBounds (out double xm, out double xM, out double ym, out double yM, out double zm, out double zM) {
			xm = Center.X-Radius;
			xM = Center.X+Radius;
			ym = Center.Y-Radius;
			yM = Center.Y+Radius;
			zm = Center.Z-Radius;
			zM = Center.Z+Radius;
		}
		public override void GetDimensionBounds (int dim, out double x0, out double x1) {
			x0 = Center[dim]-Radius;
			x1 = Center[dim]+Radius;
		}
		public override bool InBox (double xm, double xM, double ym, double yM, double zm, double zM) {
			double xmin = Maths.ClosestInterval(this.Center.X, xm, xM)-Center.X;
			double ymin = Maths.ClosestInterval(this.Center.Y, ym, yM)-Center.Y;
			double zmin = Maths.ClosestInterval(this.Center.Z, zm, zM)-Center.Z;
			return (xmin*xmin+ymin*ymin+zmin*zmin <= Radius*Radius);
		}
		public override double HitAt (Ray ray) {
			double x0 = ray.X0-Center.X;
			double y0 = ray.Y0-Center.Y;
			double z0 = ray.Z0-Center.Z;
			double dx = ray.DX;
			double dy = ray.DY;
			double dz = ray.DZ;
			double b_2 = dx*x0+dy*y0+dz*z0;
			double c = x0*x0+y0*y0+z0*z0-Radius*Radius;
			double D_4 = b_2*b_2-c;
			if(D_4 >= 0.0d) {
				D_4 = Math.Sqrt(D_4);
				double t = Maths.MinGeqZero(-D_4-b_2, D_4-b_2);
				if(t > 0.0d) {
					return t;
				}
				else {
					return double.PositiveInfinity;
				}
			}
			else {
				return double.PositiveInfinity;
			}
		}
		public override void Cast (Ray ray, CastResult cr) {
			double x0 = ray.X0-Center.X;
			double y0 = ray.Y0-Center.Y;
			double z0 = ray.Z0-Center.Z;
			double dx = ray.DX;
			double dy = ray.DY;
			double dz = ray.DZ;
			double b_2 = dx*x0+dy*y0+dz*z0;
			double c = x0*x0+y0*y0+z0*z0-Radius*Radius;
			double D_4 = b_2*b_2-c;
			D_4 = Math.Sqrt(D_4);
			double t = Maths.MinGeqZero(-D_4-b_2, D_4-b_2);
			double normx = Rinv*(x0+t*dx);
			double normy = Rinv*(y0+t*dy);
			double normz = Rinv*(z0+t*dz);
			double phi = Math.Atan2(normx, -normz);
			double tu = 0.5d*phi/Math.PI+0.5d;
			double tv = 0.5d-Math.Asin(normy)/Math.PI;
			cr.Copy(t, normx, normy, normz, tu, tv, 0.0d, new Point3(-normz, normy, normx), new Point3(-normy, normx, normz));
		}

		public override double Surface () {
			return Maths.PI4*Radius*Radius;
		}
		public override double SplitSurface (double sweep, int dim) {
			return Maths.PI2*Radius*(sweep-Radius-this.Center[dim]);
		}
		public override Tuple<ProxyRenderItem[], ProxyRenderItem[]> SplitAt (double sweep, int dimension) {
			return null;
		}
		
	}
	
}