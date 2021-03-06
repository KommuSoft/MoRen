//
//  TestParameters.cs
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
using Renderer;
using NUnit.Framework;

namespace TestRenderer {

	public static class TestParameters {


		public const int TriceratopsTest = short.MaxValue;
		public const int BuildTest = short.MaxValue;
		public const int RayTest = short.MaxValue>>0x02;
		public const int PointTest = short.MaxValue<<0x04;
		public const int ColorTest = short.MaxValue<<0x04;
		public const int ColorDepthTest = short.MaxValue<<0x04;

		public static void TestRIEqual (Ray ray, double ta, double tb, List<RenderItem> ris, RenderItem ria, RenderItem rib) {
			if(ria != rib) {
				Assert.AreEqual(ria, rib, string.Format("The hitpoint was {0}/{3} with ray {2} and scenario {1}", ray.PointAt(ta), string.Join(",", ris), ray, ray.PointAt(tb)));
			}
			else {
				Assert.AreEqual(ria, rib);
			}
		}

	}
}

