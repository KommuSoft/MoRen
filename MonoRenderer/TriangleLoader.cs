//
//  TriangleLoader.cs
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

namespace Renderer.SceneBuilding {

	[MeshLoader]
	public class TriangleLoader : MeshLoaderBase {

		//private 

		public TriangleLoader () {
		}

		#region MeshLoaderBase implementation
		public override void Load (string currentDir, string filename) {
			TreeNode<string> tree = filename.ParseTreeBracketsComma();

		}
		public override void Load (string currentDir, Stream stream) {
		}

		public override bool CanParse (string filename) {
			TreeNode<string> tree = filename.ParseTreeBracketsComma();
			return (tree.Data == "triangle");
		}

		public override void Inject (List<RenderItem> items, Matrix4 transform, params string[] args) {
			//TODO: implement
			//items.Add();
		}

		public override IMeshLoader Clone () {
			return new TriangleLoader();
		}
		#endregion

	}
}

