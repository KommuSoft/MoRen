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

		private Point3 pa;
		private Point3 pb;
		private Point3 pc;
		private Point3 na;
		private Point3 nb;
		private Point3 nc;
		private Point3 ta;
		private Point3 tb;
		private Point3 tc;

		public TriangleLoader () {
		}

		#region MeshLoaderBase implementation
		public override void Load (string currentDir, string filename) {
			TreeNode<string> tree = filename.ParseTreeBracketsComma();
			if(tree.Count < 0x03) {
				pa = Point3.DummyPoint;
				pb = Point3.DummyXPoint;
				pc = Point3.DummyYPoint;
			}
			else {
				pa = Point3.Parse(tree[0x00].ChildDatas);
				pb = Point3.Parse(tree[0x01].ChildDatas);
				pc = Point3.Parse(tree[0x02].ChildDatas);
			}
			if(tree.Count >= 0x06) {
				na = Point3.Parse(tree[0x03].ChildDatas);
				nb = Point3.Parse(tree[0x04].ChildDatas);
				nc = Point3.Parse(tree[0x05].ChildDatas);
			}
			if(tree.Count >= 0x09) {
				ta = Point3.Parse(tree[0x06].ChildDatas);
				tb = Point3.Parse(tree[0x07].ChildDatas);
				tc = Point3.Parse(tree[0x08].ChildDatas);
			}
			else {
				ta = Point3.DummyYPoint;
				tb = Point3.DummyPoint;
				tc = Point3.DummyXPoint;
			}
		}
		public override void Load (string currentDir, Stream stream) {
			pa = Point3.DummyPoint;
			pb = Point3.DummyXPoint;
			pc = Point3.DummyYPoint;
		}

		public override bool CanParse (string filename) {
			TreeNode<string> tree = filename.ParseTreeBracketsComma();
			return (tree.Data == "triangle");
		}

		public override void Inject (List<RenderItem> items, Matrix4 transform, params string[] args) {
			items.Add(new Triangle(new Point3(pa, transform),
			                       new Point3(pb, transform),
			                       new Point3(pc, transform),
			                       Point3.NullOrTransformedNonShiftCopy(na, transform),
			                       Point3.NullOrTransformedNonShiftCopy(nb, transform),
			                       Point3.NullOrTransformedNonShiftCopy(nc, transform),
			                       ta, tb, tc, this.DefaultMaterial)
			);
		}

		public override IMeshLoader Clone () {
			return new TriangleLoader();
		}
		#endregion

	}
}

