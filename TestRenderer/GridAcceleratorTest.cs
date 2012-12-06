//  
//  GridAcceleratorTest.cs
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
using System.IO;
using NUnit.Framework;
using Renderer;

namespace RendererTests {
	[TestFixture()]
	public class GridAcceleratorTest
	{
		[Test()]
		public void TestHit1 () {
			Matrix4 M = new Matrix4();
			M.Shift(0.0d, 0.0d, 10.0d);
			LoaderObj lo = new LoaderObj();
			FileStream fs = File.Open("triceratops.obj", FileMode.Open, FileAccess.Read);
			lo.Load(null, fs);
			fs.Close();
			List<RenderItem> ris = lo.Inject(M);
			NaiveAccelerator na = new NaiveAccelerator(ris);
			GridAccelerator ga = new GridAccelerator(ris);
			double t;
			for(int i = 0; i < 10000000; i+= 10000) {
				Ray ray = Ray.Random();
				Assert.AreEqual(na.CalculateHit(ray, out t, double.PositiveInfinity), ga.CalculateHit(ray, out t, double.PositiveInfinity));
			}
		}

		[Test()]
		public void TestHit2 () {
			for(int i = 0; i < 6553; i += 1000) {
				int nt = Math.Max(2, Maths.Random(i));
				List<RenderItem> ris = new List<RenderItem>();
				for(int j = 0x00; j < nt; j++) {
					ris.Add(new Triangle(Point3.Random(), Point3.Random(), Point3.Random(), null, null, null, null, null, null, null));
				}
				NaiveAccelerator na = new NaiveAccelerator(ris);
				GridAccelerator ga = new GridAccelerator(ris);
				double t;
				for(int k = 0; k < 65536; k += 1000) {
					Ray ray = Ray.Random();
					RenderItem ria = na.CalculateHit(ray, out t, double.PositiveInfinity), rib = ga.CalculateHit(ray, out t, double.PositiveInfinity);
					Assert.AreEqual(ria, rib);
				}
	
			}
		}
	}
}

