using System;

namespace Renderer {

	public class Triangle3 {

		public readonly Point3 A;
		public readonly Point3 B;
		public readonly Point3 C;

		public Triangle3 (Point3 a, Point3 b, Point3 c) {
			this.A = a;
			this.B = b;
			this.C = c;
		}

		public Point3 CalculateNormal () {
			return Point3.CrossNormalize(B-A,C-A);
		}

	}

}