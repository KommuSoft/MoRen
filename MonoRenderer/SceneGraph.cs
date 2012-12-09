//
//  SceneGraph.cs
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
using System.Linq;
using System.Xml.Serialization;

namespace Renderer.SceneBuilding {
	
	[XmlType("SceneGraph")]
	public sealed class SceneGraph {

		private SceneGraphNode[] nodes;

		[XmlIgnore]
		private readonly Dictionary<string,SceneGraphNode>
			cachedNodes = new Dictionary<string, SceneGraphNode>();
		[XmlIgnore]
		private string
			rootName;
		[XmlAttribute("MaxDepth")]
		public int
			MaxDepth = 0x25;
		[XmlIgnore]
		public SceneGraphNode
			Root;
		[XmlAttribute("Root")]
		public string RootName {
			get {
				if(this.Root != null) {
					return this.Root.Name;
				}
				else {
					return null;
				}
			}
			set {
				this.rootName = value;
			}
		}
		
		[XmlArray("SceneGraphNodes")]
		[XmlArrayItem("SceneGraphNode")]
		public SceneGraphNode[] GraphNodes {
			get {
				return this.nodes;
			}
			set {
				this.nodes = value;
				this.Resolve();
			}
		}
		public SceneGraph () {
		}

		public SceneGraph (SceneGraphNode root) {
			this.Root = root;
		}

		public void Resolve () {
			foreach(SceneGraphNode sgn in this.nodes) {
				this.cachedNodes.Add(sgn.Name, sgn);
			}
			foreach(SceneGraphNode sgn in this.cachedNodes.Values) {
				sgn.Resolve(this.cachedNodes);
			}
			if(this.Root == null && this.cachedNodes.ContainsKey(this.rootName)) {
				this.Root = this.cachedNodes[this.rootName];
			}
		}

		public Tuple<List<RenderItem>,List<Light>> Inject () {
			List<RenderItem> ris = new List<RenderItem>();
			List<Light> lis = new List<Light>();
			MatrixStack ms = new MatrixStack();
			this.Root.Inject(this.MaxDepth, ms, ris, lis, 0x00);
			return new Tuple<List<RenderItem>, List<Light>>(ris, lis);
		}
		
	}
	
}