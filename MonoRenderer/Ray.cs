//
//  Ray.cs
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
		public void TransformNormalize (Matrix4 m) {
			this.Offset.Transform(m);
			this.Direction.TransformNonShift(m);
			this.Direction.Normalize();
		}
		public void TransformNormalize (Matrix4 m, out double lengthMul) {
			this.Offset.Transform(m);
			this.Direction.TransformNonShift(m);
			lengthMul = this.Direction.Length;
			double inv = 1.0d/lengthMul;
			this.Direction.X *= inv;
			this.Direction.Y *= inv;
			this.Direction.Z *= inv;
		}
		public void InvTransform (Matrix4 m) {
			this.Offset.InvTransform(m);
			this.Direction.InvTransformNonShift(m);
		}

		public static Ray Random () {
			Ray ray = new Ray(Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random(), Maths.Random());
			ray.NormalizeDirection();
			return ray;
		}

		public override string ToString () {
			return string.Format("( {0} ; {1} ; {2} ; {3} ; {4} ; {5} )", X0.ToString("0.000000000"), Y0.ToString("0.000000000"), Z0.ToString("0.000000000"), DX.ToString("0.000000000"), DY.ToString("0.000000000"), DZ.ToString("0.000000000"));
		}

	}

}