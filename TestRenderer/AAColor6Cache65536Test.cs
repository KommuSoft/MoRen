//
//  AAColor6Cache65536Test.cs
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
	public class AAColor6Cache65536Test {

		[Test()]
		public void AddColor6Test () {
			for(int i = 0x00; i < TestParameters.ColorTest; i++) {
				ushort s0 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s1 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s2 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s3 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s4 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				ushort s5 = (ushort)(Maths.RandomGenerator.Next()&0xffff);
				Color6Cache65536 cc = new Color6Cache65536(0x00);
				Color6 c = new Color6(s0, s1, s2, s3, s4, s5);
				uint n = (uint)Maths.RandomGenerator.Next(TestParameters.ColorDepthTest)+0x01;
				for(int j = 0x00; j < n; j++) {
					cc.AddColor6(c);
				}
				Assert.AreEqual(c, cc.Mix(n));
			}
		}
	}
}

