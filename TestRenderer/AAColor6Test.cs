//
//  Color6Test.cs
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
	public class AAColor6Test {

		[Test()]
		public void ConstructionTest () {
			for(int i = 0; i < TestParameters.ColorTest; i++) {
				ushort s0 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s1 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s2 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s3 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s4 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s5 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				Color6 c = new Color6(s0, s1, s2, s3, s4, s5);
				Assert.AreEqual(s0, c.Seg0);
				Assert.AreEqual(s1, c.Seg1);
				Assert.AreEqual(s2, c.Seg2);
				Assert.AreEqual(s3, c.Seg3);
				Assert.AreEqual(s4, c.Seg4);
				Assert.AreEqual(s5, c.Seg5);
			}
		}

		[Test()]
		public void ToRGBTest () {
			for(uint i = 0; i < 0x1000000; i++) {
				Assert.AreEqual(i, new Color6(i).ToRGB());
			}
		}
	}
}

