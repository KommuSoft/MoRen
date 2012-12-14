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
			ushort s0 = 0x00, s1 = 0x00, s2 = 0x00, s3 = 0x00, s4 = 0x00, s5 = 0x00;
			for(; s0 < 0xffff-0x3e5; s0 += 0x3e5) {
				for(; s1 < 0xffff-0x3e5; s1 += 0x3e5) {
					for(; s2 < 0xffff-0x3e5; s2 += 0x3e5) {
						for(; s3 < 0xffff-0x3e5; s3 += 0x3e5) {
							for(; s4 < 0xffff-0x3e5; s4 += 0x3e5) {
								for(; s5 < 0xffff-0x3e5; s5 += 0x3e5) {
									Color6Cache65536 cache = new Color6Cache65536(0x00);
									Color6 c = new Color6(s0, s1, s2, s3, s4, s5);
									cache.AddColor6(c);
									Assert.AreEqual(c, cache.Mix(0x01));
								}
								s5 += 0x3e5;
							}
							s4 += 0x3e5;
						}
						s3 += 0x3e5;
					}
					s2 += 0x3e5;
				}
				s1 += 0x3e5;
			}
		}
	}
}

