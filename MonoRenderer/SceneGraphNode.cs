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

namespace Renderer.SceneBuilding {

	[XmlType("SceneGraphNode")]
	public sealed class SceneGraphNode : NameBase {
		[XmlIgnore]
		private List<string>
			childNames = null;
		[XmlIgnore]
		public readonly List<SceneGraphNode>
			SubNodes = new List<SceneGraphNode>();
		[XmlIgnore]
		private Light
			light;
		[XmlIgnore]
		private Matrix4
			Transformer;
		[XmlArray("SubNodes")]
		[XmlArrayItem("SubNode")]
		public List<string> SubNodeNames {
			get {
				if(this.SubNodes.Count <= 0x00) {
					return null;
				}
				List<string> lg = new List<string>();
				foreach(SceneGraphNode sgn in this.SubNodes) {
					lg.Add(sgn.Name);
				}
				return lg;
			}
			set {
				this.childNames = value;
			}
		}
		[XmlElement("Transformation")]
		public string TransformerString {
			get {
				if(this.Transformer != null) {
					return this.Transformer.ToString();
				}
				else {
					return null;
				}
			}
			set {
				this.Transformer = Matrix4.Parse(value);
			}
		}
		[XmlElement("Mesh")]
		public Mesh
			Mesh = null;
		[XmlElement("Light")]
		public LightWrapper
			LightWrapper = null;
		
		public SceneGraphNode () {
		}
		public SceneGraphNode (IEnumerable<SceneGraphNode> subNodes) {
			foreach(SceneGraphNode sgn in subNodes) {
				this.SubNodes.Add(sgn);
			}
		}
		public SceneGraphNode (Matrix4 transformer, IEnumerable<SceneGraphNode> subNodes) : this(subNodes) {
			this.Transformer = transformer;
		}
		public SceneGraphNode (Mesh mesh) : this() {
			this.Mesh = mesh;
		}
		public SceneGraphNode (Light light) : this() {
			this.light = light;
		}
		public SceneGraphNode (Matrix4 transformer, IEnumerable<SceneGraphNode> subNodes, Mesh mesh, Light light) : this(transformer,subNodes) {
			this.Mesh = mesh;
			this.light = light;
		}
		
		public void AddChild (SceneGraphNode subNode) {
			this.SubNodes.Add(subNode);
		}
		
		public override int GetHashCode () {
			return this.Name.GetHashCode();
		}

		public void Resolve (Dictionary<string,SceneGraphNode> dictionary) {
			if(this.childNames != null) {
				this.SubNodes.AddRange(this.childNames.Select(x => dictionary[x]));
			}
			this.childNames = null;
			if(this.Mesh != null) {
				this.Mesh.Resolve();
			}
		}
		
	}
}