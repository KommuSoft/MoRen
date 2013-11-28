//
//  AAColorTest.cs
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
	public class AAColorTest {

		[Test()]
		public void ConstructionTest () {
			for(int i = 0; i < TestParameters.ColorTest; i++) {
				uint r = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint g = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint b = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				Color c = new Color(r, g, b);
				Assert.AreEqual(r, c.RedInt);
				Assert.AreEqual(g, c.GreenInt);
				Assert.AreEqual(b, c.BlueInt);
			}
		}

		[Test()]
		public void AddTest () {
			for(int i = 0; i < TestParameters.ColorTest; i++) {
				uint r1 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint g1 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint b1 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint r2 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint g2 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				uint b2 = (uint)(Maths.RandomGenerator.Next()&0xfffff);
				Color c1 = new Color(r1, g1, b1);
				Color c2 = new Color(r2, g2, b2);
				Color c3 = c1+c2;
				Assert.AreEqual(Math.Min(r1+r2, Color.MaxValue), c3.RedInt);
				Assert.AreEqual(Math.Min(g1+g2, Color.MaxValue), c3.GreenInt);
				Assert.AreEqual(Math.Min(b1+b2, Color.MaxValue), c3.BlueInt);
			}
		}

		[Test()]
		public void AddTest2 () {
			for(int i = 0; i < TestParameters.ColorTest; i++) {
				uint c1 = (uint)Maths.RandomGenerator.Next()&0xffffff;
				uint c2 = (uint)Maths.RandomGenerator.Next()&0xffffff;
				uint c3 = ColorUtils.Add(c1, c2);
				uint c4 = (new Color(c1)+new Color(c2)).RGB8;
				string message = string.Format("{0} vs {1} from {2} and {3}", c3.ToString("X"), c4.ToString("X"), c1.ToString("X"), c2.ToString("X"));
				Assert.IsTrue(Math.Abs(((int)(c3&0xff))-((int)(c4&0xff))) <= 0x01, message);
				Assert.IsTrue(Math.Abs(((int)(c3&0xff00))-((int)(c4&0xff00))) <= 0x0100, message);
				Assert.IsTrue(Math.Abs(((int)(c3&0xff0000))-((int)(c4&0xff0000))) <= 0x010000, message);
			}
		}

		[Test()]
		public void MulTest () {
			for(int i = 0; i < TestParameters.ColorTest; i++) {
				uint c1 = (uint)Maths.RandomGenerator.Next()&0xffffff;
				uint c2 = (uint)Maths.RandomGenerator.Next()&0xffffff;
				uint c3 = ColorUtils.Multiply(c1, c2);
				uint c4 = (new Color(c1)*new Color(c2)).RGB8;
				string message = string.Format("{0} vs {1} from {2} and {3}", c3.ToString("X"), c4.ToString("X"), c1.ToString("X"), c2.ToString("X"));
				Assert.IsTrue(Math.Abs(((int)(c3&0xff))-((int)(c4&0xff))) <= 0x02, message);
				Assert.IsTrue(Math.Abs(((int)(c3&0xff00))-((int)(c4&0xff00))) <= 0x0200, message);
				Assert.IsTrue(Math.Abs(((int)(c3&0xff0000))-((int)(c4&0xff0000))) <= 0x020000, message);
			}
		}

		[Test()]
		public void ToRGBTest () {
			for(uint i = 0; i < 0x1000000; i++) {
				Assert.AreEqual(i, new Color(i).RGB8);
			}
		}
	}
}

