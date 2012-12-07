//
//  Test.cs
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
using Renderer;
using NUnit.Framework;

namespace TestRenderer {
	[TestFixture()]
	public class Test {
		[Test()]
		public void TestInBox () {
			Random rnd = new Random();
			Point3 pa = new Point3(), pb = new Point3(), pc = new Point3();
			Triangle t = new Triangle(pa, pb, pc, null, null, null, null, null, null, null);
			for(int i = 0x00; i < 100; i++) {
				pa.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				pb.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				pc.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				double a = rnd.NextDouble(), b = rnd.NextDouble(), c = rnd.NextDouble(), d = rnd.NextDouble(), e = rnd.NextDouble(), f = rnd.NextDouble();
				double mx = Math.Min(a, b), Mx = Math.Max(a, b), my = Math.Min(c, d), My = Math.Max(c, d), mz = Math.Min(e, f), Mz = Math.Max(e, f);
				if(pa.InBox(mx, Mx, my, My, mz, Mz) || pb.InBox(mx, Mx, my, My, mz, Mz) || pc.InBox(mx, Mx, my, My, mz, Mz)) {
					Assert.IsTrue(t.InBox(mx, Mx, my, My, mz, Mz));
				}
			}
		}

		/*[Test()]
		public void TestInBox2 () {
			Random rnd = new Random();
			Point3 pa = new Point3(), pb = new Point3(), pc = new Point3();
			Triangle t = new Triangle(pa, pb, pc, null, null, null, null, null, null, null);
			long j = 0x00;
			for(int i = int.MinValue; i < int.MaxValue; i++) {
				pa.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				pb.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				pc.SetValues(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
				double a = rnd.NextDouble(), b = rnd.NextDouble(), c = rnd.NextDouble(), d = rnd.NextDouble(), e = rnd.NextDouble(), f = rnd.NextDouble();
				double mx = Math.Min(a, b), Mx = Math.Max(a, b), my = Math.Min(c, d), My = Math.Max(c, d), mz = Math.Min(e, f), Mz = Math.Max(e, f);
				if(t.InBox(mx, Mx, my, My, mz, Mz)) {
					j++;
				}
			}
			Console.WriteLine(j/((double)int.MaxValue-int.MinValue));
		}*/
	}
}

