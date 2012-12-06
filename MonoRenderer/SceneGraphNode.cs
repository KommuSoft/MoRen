//
//  SceneGraphNode.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Renderer {

	[XmlType("SceneGraphNode")]
	public sealed class SceneGraphNode
	{
		
		[XmlAttribute("Guid")]
		public readonly Guid Guid;
		[XmlIgnore]
		private List<Guid> childGuids = new List<Guid>();
		[XmlIgnore]
		public readonly List<SceneGraphNode> SubNodes = new List<SceneGraphNode>();
		[XmlArray("SubNodes")]
		[XmlArrayItem("SubNode")]
		public List<Guid> SubNodeGuides {
			get {
				if(this.SubNodes != null) {
					List<Guid> lg = new List<Guid>();
					foreach(SceneGraphNode sgn in this.SubNodes) {
						lg.Add(sgn.Guid);
					}
					return lg;
				} else {
					return null;
				}
			}
			set {
				this.childGuids = value;
			}
		}
		[XmlElement("TransformationMatrix")]
		public readonly Matrix4 Transformer = null;
		[XmlElement("Mesh")]
		public readonly Mesh Mesh = null;
		[XmlElement("Light")]
		public readonly Light Light = null;
		
		public SceneGraphNode () {
			this.Guid = Guid.NewGuid();
		}
		public SceneGraphNode (IEnumerable<SceneGraphNode> subNodes) {
			foreach(SceneGraphNode sgn in subNodes) {
				this.SubNodes.Add(sgn);
			}
		}
		public SceneGraphNode (Matrix4 transformer,IEnumerable<SceneGraphNode> subNodes) : this(subNodes) {
			this.Transformer = transformer;
		}
		public SceneGraphNode (Mesh mesh) : this() {
			this.Mesh = mesh;
		}
		public SceneGraphNode (Light light) : this() {
			this.Light = light;
		}
		public SceneGraphNode (Matrix4 transformer,IEnumerable<SceneGraphNode> subNodes,Mesh mesh,Light light) : this(transformer,subNodes) {
			this.Mesh = mesh;
			this.Light = light;
		}
		
		public void AddChild (SceneGraphNode subNode) {
			this.SubNodes.Add(subNode);
		}
		
		public override int GetHashCode () {
			return this.Guid.GetHashCode();
		}

		public void Resolve (Dictionary<Guid,SceneGraphNode> dictionary) {
			if(this.childGuids != null) {
				this.SubNodes.AddRange(this.childGuids.Select(x => dictionary[x]));
			}
		}
		
	}
}