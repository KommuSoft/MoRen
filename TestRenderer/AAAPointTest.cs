//
//  AAAPointTest.cs
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
	public class AAAPointTest {
		[Test()]
		public void TestInvTransform () {
			for(int i = 0x00; i < TestParameters.PointTest; i++) {
				Point3 p = Point3.Random();
				Matrix4 M = new Matrix4(Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble());
				Point3 q = new Point3(p);
				q.Transform(M);
				q.InvTransform(M);
				Assert.IsTrue(Math.Abs(p.X-q.X) <= Maths.GlobalEpsilon);
				Assert.IsTrue(Math.Abs(p.Y-q.Y) <= Maths.GlobalEpsilon);
				Assert.IsTrue(Math.Abs(p.Z-q.Z) <= Maths.GlobalEpsilon);
			}
		}

		[Test()]
		public void TestInvTransformNonShift () {
			for(int i = 0x00; i < TestParameters.PointTest; i++) {
				Point3 p = Point3.Random();
				Matrix4 M = new Matrix4(Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble(),
				                        Maths.RandomGenerator.NextDouble());
				Point3 q = new Point3(p);
				q.TransformNonShift(M);
				q.InvTransformNonShift(M);
				Assert.IsTrue(Math.Abs(p.X-q.X) <= Maths.GlobalEpsilon);
				Assert.IsTrue(Math.Abs(p.Y-q.Y) <= Maths.GlobalEpsilon);
				Assert.IsTrue(Math.Abs(p.Z-q.Z) <= Maths.GlobalEpsilon);
			}
		}
	}
}

