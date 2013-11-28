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
			Assert.AreEqual(new Matrix4(), Matrix4.Parse("id"));
			Assert.AreEqual(new Matrix4(), Matrix4.Parse("ID"));
			Assert.AreEqual(new Matrix4(), Matrix4.Parse("IdEnTiTy"));
			Assert.AreEqual(Matrix4.CreateRotateXMatrix(1.0d), Matrix4.Parse("RotateX 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateYMatrix(1.0d), Matrix4.Parse("RotateY 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateZMatrix(1.0d), Matrix4.Parse("RotateZ 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateXMatrix(1.0d), Matrix4.Parse("Rotate X 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateYMatrix(1.0d), Matrix4.Parse("Rotate Y 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateZMatrix(1.0d), Matrix4.Parse("Rotate Z 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateXMatrix(1.0d), Matrix4.Parse("Rotate 1.0 0.0 0.0 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateYMatrix(1.0d), Matrix4.Parse("Rotate 0.0 1.0 0.0 1.0"));
			Assert.AreEqual(Matrix4.CreateRotateZMatrix(1.0d), Matrix4.Parse("Rotate 0.0 0.0 1.0 1.0"));
			Assert.AreEqual(Matrix4.CreateScaleMatrix(1.0d), Matrix4.Parse("scale 1.0"));
			Assert.AreEqual(Matrix4.CreateScaleMatrix(2.0d), Matrix4.Parse("scale 2.0"));
			Assert.AreEqual(Matrix4.CreateScaleMatrix(0.5d, 3.0d, 2.0d), Matrix4.Parse("scale 0.5 3.0 2.0"));
			Assert.AreEqual(Matrix4.CreateShiftMatrix(0.5d, 3.0d, 2.0d), Matrix4.Parse("shift 0.5 3.0 2.0"));
		}
	}
}