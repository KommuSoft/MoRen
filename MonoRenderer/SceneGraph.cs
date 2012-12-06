using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Renderer {
	
	[XmlType("SceneGraph")]
	public sealed class SceneGraph {
		
		[XmlIgnore]
		public readonly MatrixStack Stack = new MatrixStack();
		[XmlIgnore]
		private Guid rootGuid;
		[XmlAttribute("MaxDepth")]
		public int MaxDepth = 0x25;
		[XmlIgnore]
		public SceneGraphNode Root;
		[XmlAttribute("Root")]
		public Guid RootGuid {
			get {
				return this.Root.Guid;
			}
			set {
				this.rootGuid = value;
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
				
			}
		}
		public SceneGraph () {
			this.Root = new SceneGraphNode();
		}
		
	}
	
}