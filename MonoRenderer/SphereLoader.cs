//
//  SphereLoader.cs
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
using System.Collections.Generic;
using System.IO;

namespace Renderer.SceneBuilding {

	[MeshLoaderAttribute]
	public class SphereLoader : MeshLoaderBase {

		public SphereLoader () {
		}

		#region implemented abstract members of Renderer.MeshLoaderBase
		public override void Load (string currentDir, Stream stream) {
		}

		public override bool CanParse (string filename) {
			return filename.Trim().ToLower() == "sphere";
		}

		public override IMeshLoader Clone () {
			return new SphereLoader();
		}

		public override void Inject (List<RenderItem> items, Matrix4 transform, params string[] args) {
			Point3 p = new Point3(0.0d, 0.0d, 0.0d);
			p.Transform(transform);
			Point3 q = new Point3(Maths.Sqrt_3, Maths.Sqrt_3, Maths.Sqrt_3);
			q.TransformNonShift(transform);
			items.Add(new Sphere(p, q.Length, this.DefaultMaterial));
		}
		#endregion

		#region implemented virtual members of Renderer.MeshLoaderBase
		public override void Load (string currentDir, string filename) {
		}
		#endregion

	}
}

