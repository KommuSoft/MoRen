//
//  Point3Test.cs
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
using NUnit.Framework;
using Renderer;

namespace TestRenderer {

	[TestFixture()]
	public class APoint3Test {
		[Test()]
		public void TestRotateLikeZVector () {
			Point3 norm;
			Random rand = new Random();
			for(int i = int.MinValue; i < int.MaxValue; i++) {
				double x = 200.0*rand.NextDouble()-100.0d;
				double y = 200.0*rand.NextDouble()-100.0d;
				norm = new Point3(0.0d, 0.0d, 1.0d);
				double rinv = 1.0d/Math.Sqrt(x*x+y*y+1.0d);
				norm.RotateLikeZVector(x, y);
				Assert.IsTrue(Math.Abs(norm.X-x*rinv) <= Math.Sqrt(Maths.GlobalEpsilon));
				Assert.IsTrue(Math.Abs(norm.Y-y*rinv) <= Math.Sqrt(Maths.GlobalEpsilon));
				Assert.IsTrue(Math.Abs(norm.Z-rinv) <= Math.Sqrt(Maths.GlobalEpsilon));
			}
		}

		[Test()]
		public void TestNormalizedRotateLikeZVector () {
			Point3 norm, p;
			Random rand = new Random();
			for(int i = 0x00; i < short.MaxValue; i++) {
				double x = 2.0*rand.NextDouble()-1.0d;
				double y = 2.0d*rand.NextDouble()-1.0d;
				double z = 2.0d*rand.NextDouble()-1.0d;
				p = new Point3(x, y, z);
				p.Normalize();
				norm = new Point3(0.0d, 0.0d, 1.0d);
				norm.NormalizedRotateLikeZVector(p.X, p.Y, p.Z);
				Assert.IsTrue(Math.Abs(norm.X-p.X) <= Math.Sqrt(Maths.GlobalEpsilon));
				Assert.IsTrue(Math.Abs(norm.Y-p.Y) <= Math.Sqrt(Maths.GlobalEpsilon));
				Assert.IsTrue(Math.Abs(norm.Z-p.Z) <= Math.Sqrt(Maths.GlobalEpsilon));
			}
		}
	}
}

