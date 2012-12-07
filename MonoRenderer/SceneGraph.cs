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
using System.Xml.Serialization;

namespace Renderer.SceneBuilding {
	
	[XmlType("SceneGraph")]
	public sealed class SceneGraph {
		
		[XmlIgnore]
		public readonly MatrixStack
			Stack = new MatrixStack();

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
		public string RootGuid {
			get {
				return this.Root.Name;
			}
			set {
				this.rootName = value;
			}
		}
		
		[XmlArray("Nodes")]
		[XmlArrayItem("Node")]
		public List<SceneGraphNode> Nodes {
			get {
				List<SceneGraphNode> nodes = new List<SceneGraphNode>();
				HashSet<SceneGraphNode> visited = new HashSet<SceneGraphNode>(new SceneGraphNode[] { this.Root });
				Stack<SceneGraphNode> stack = new Stack<SceneGraphNode>(new SceneGraphNode[] { this.Root });
				while(stack.Count > 0) {
					SceneGraphNode sgn = stack.Pop();
					nodes.Add(sgn);
					if(sgn.SubNodes != null) {
						foreach(SceneGraphNode c in sgn.SubNodes) {
							if(!visited.Contains(c)) {
								visited.Add(c);
								stack.Push(c);
							}
						}
					}
				}
				return nodes;
			}
			set {
				foreach(SceneGraphNode sgn in value) {
					this.cachedNodes.Add(sgn.Name, sgn);
				}
				foreach(SceneGraphNode sgn in this.cachedNodes.Values) {
					sgn.Resolve(this.cachedNodes);
				}
			}
		}
		public SceneGraph () {
			this.Root = new SceneGraphNode();
		}
		
	}
	
}