using System;

namespace Renderer {
	public sealed class Ray {
		
		public readonly Point3 Offset;
		public readonly Point3 Direction;
		
		public double X0 {
			get {
				return Offset.X;
			}
		}

		public double Y0 {
			get {
				return Offset.Y;
			}
		}

		public double Z0 {
			get {
				return Offset.Z;
			}
		}

		public double DX {
			get {
				return Direction.X;
			}
		}

		public double DY {
			get {
				return Direction.Y;
			}
		}

		public double DZ {
			get {
				return Direction.Z;
			}
		}

		public Ray () : this(0.0d,0.0d,0.0d,0.0d,0.0d,0.0d) {
		}
		public Ray (Point3 offset, Point3 direction) {
			this.Offset = offset;
			this.Direction = direction;
		}

		public Ray (double x, double y, double z, double dx, double dy, double dz) {
			this.Offset = new Point3(x, y, z);
			this.Direction = new Point3(dx, dy, dz);
		}
		
		public static Ray WithEpsilon (Point3 offset, Point3 direction) {
			return new Ray(new Point3(offset.X+Maths.GlobalEpsilon*direction.X, offset.Y+Maths.GlobalEpsilon*direction.Y, offset.Z+Maths.GlobalEpsilon*direction.Z), direction);
		}

		public void SetOffsetWithEpsilon (Point3 offset) {
			this.Offset.X = offset.X+Maths.GlobalEpsilon*this.Direction.X;
			this.Offset.Y = offset.Y+Maths.GlobalEpsilon*this.Direction.Y;
			this.Offset.Z = offset.Z+Maths.GlobalEpsilon*this.Direction.Z;
		}

		public void SetWithEpsilon (Point3 offset, Point3 direction) {
			this.Offset.X = offset.X+Maths.GlobalEpsilon*direction.X;
			this.Offset.Y = offset.Y+Maths.GlobalEpsilon*direction.Y;
			this.Offset.Z = offset.Z+Maths.GlobalEpsilon*direction.Z;
			this.Direction.X = direction.X;
			this.Direction.Y = direction.Y;
			this.Direction.Z = direction.Z;
		}

		public void NormalizeDirection () {
			this.Direction.Normalize();
		}

		[Obsolete]
		public Point3 PointAt (double t) {
			return new Point3(Offset.X+Direction.X*t, Offset.Y+Direction.Y*t, Offset.Z+Direction.Z*t);
		}

		public void PointAt (double t, Point3 pt) {
			pt.SetValues(Offset.X+Direction.X*t, Offset.Y+Direction.Y*t, Offset.Z+Direction.Z*t);
		}

		public void Transform (Matrix4 m) {
			this.Offset.Transform(m);
			this.Direction.TransformNonShift(m);
		}

		public static Ray Random () {
			Ray ray = new Ray(Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random());
			ray.NormalizeDirection();
			return ray;
		}

		public override string ToString () {
			return string.Format("( {0} ; {1} ; {2} ; {3} ; {4} ; {5} )", X0.ToString("0.000"), Y0.ToString("0.000"), Z0.ToString("0.000"), DX.ToString("0.000"), DY.ToString("0.000"), DZ.ToString("0.000"));
		}

	}

}