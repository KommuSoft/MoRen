//
//  MatrixTest.cs
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
	public class MatrixTest {
		[Test()]
		public void TestParse () {
			Assert.Equals(new Matrix4(), Matrix4.Parse("id"));
			Assert.Equals(new Matrix4(), Matrix4.Parse("ID"));
			Assert.Equals(new Matrix4(), Matrix4.Parse("IdEnTiTy"));
			Assert.Equals(Matrix4.CreateRotateXMatrix(1.0d), Matrix4.Parse("RotateX 1.0"));
			Assert.Equals(Matrix4.CreateRotateYMatrix(1.0d), Matrix4.Parse("RotateY 1.0"));
			Assert.Equals(Matrix4.CreateRotateZMatrix(1.0d), Matrix4.Parse("RotateZ 1.0"));
		}
	}
}

