//
//  OctTreeAcceleratorTest.cs
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
using System.IO;
using System.Collections.Generic;
using Renderer;
using Renderer.SceneBuilding;
using NUnit.Framework;

namespace TestRenderer {
	[TestFixture()]
	public class BinarySpacePartitionAcceleratorTest {
		[Test()]
		public void TestHit1 () {
			Matrix4 M = new Matrix4();
			M.Shift(0.0d, 0.0d, 10.0d);
			LoaderObj lo = new LoaderObj();
			FileStream fs = File.Open("triceratops.obj", FileMode.Open, FileAccess.Read);
			lo.Load(null, fs);
			fs.Close();
			List<RenderItem> ris = new List<RenderItem>();
			lo.Inject(ris, M);
			NaiveAccelerator ga = new NaiveAccelerator(ris);
			BSPAccelerator oa = new BSPAccelerator(ris);
			double ta, tb;
			RenderItem ria, rib;
			for(int i = 0x00; i < TestParameters.TriceratopsTest; i++) {
				Ray ray = Ray.Random();
				ray.NormalizeDirection();
				ria = ga.CalculateHit(ray, out ta, double.PositiveInfinity);
				rib = oa.CalculateHit(ray, out tb, double.PositiveInfinity);
				TestParameters.TestRIEqual(ray, ta, tb, ris, ria, rib);
			}
		}

		[Test()]
		public void TestHit2 () {
			double ta, tb;
			RenderItem ria, rib;
			for(int i = 0; i < TestParameters.BuildTest; i++) {
				int nt = Math.Max(2, Maths.Random(i));
				List<RenderItem> ris = new List<RenderItem>();
				for(int j = 0x00; j < nt; j++) {
					ris.Add(new Triangle(Point3.Random(), Point3.Random(), Point3.Random(), null, null, null, null, null, null, null));
				}
				NaiveAccelerator ga = new NaiveAccelerator(ris);
				BSPAccelerator oa = new BSPAccelerator(ris);
				for(int k = 0; k < TestParameters.RayTest; k++) {
					Ray ray = Ray.Random();
					ria = ga.CalculateHit(ray, out ta, double.PositiveInfinity);
					rib = oa.CalculateHit(ray, out tb, double.PositiveInfinity);
					TestParameters.TestRIEqual(ray, ta, tb, ris, ria, rib);
				}
	
			}
		}

	}
}
