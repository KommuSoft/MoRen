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
			double xmin = Maths.ClosestInterval(this.Center.X, xm, xM);
			double ymin = Maths.ClosestInterval(this.Center.Y, ym, yM);
			double zmin = Maths.ClosestInterval(this.Center.Z, zm, zM);
			return (xmin*xmin+ymin*ymin+zmin*zmin <= Radius*Radius);
		}
		public override double HitAt (Ray ray) {
			double xa = ray.X0-Center.X;
			double ya = ray.Y0-Center.Y;
			double za = ray.Z0-Center.Z;
			double b_2 = ray.DX*xa+ray.DY*ya+ray.DZ*za;
			double c = xa*xa+ya*ya+za*za-Radius*Radius;
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
			Console.WriteLine("testing {0}", ray);
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
					Point3 norm = new Point3(Rinv*(x0+t*dx), Rinv*(y0+t*dy), Rinv*(z0+t*dz));
					double tu = 0.5d*Math.Atan2(norm.Z, norm.X)/Math.PI-0.5d+0.3d;
					if(tu >= 1.0d) {
						tu -= 1.0d;
					}
					double tv = 0.5d-Math.Asin(norm.Y)/Math.PI;
					cr.Copy(t, norm, tu, tv);
				}
				else {
					cr.SetNull();
				}
			}
			else {
				cr.SetNull();
			}
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