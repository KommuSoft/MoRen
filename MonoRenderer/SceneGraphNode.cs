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
	public sealed class SceneGraphNode : NameBase, IMixable<SceneGraphNode> {

		[XmlIgnore]
		private List<string>
			childNames = new List<string>();

		[XmlIgnore]
		private List<CameraFollowing>
			cameraFollowings;

		[XmlIgnore]
		private string
			transformerString;

		[XmlIgnore]
		public Matrix4
			Transformer;
		[XmlIgnore]
		private Mesh
			mesh = null;

		[XmlArray("CameraFollowings")]
		[XmlArrayItem("CameraFollowing")]
		public List<CameraFollowing> CameraFollowings {
			get {
				return this.cameraFollowings;
			}
			set {
				this.cameraFollowings = value;
			}
		}

		[XmlArray("SubNodes")]
		[XmlArrayItem("SubNode")]
		public List<string> SubNodeNames {
			get {
				return this.childNames;
			}
			set {
				this.childNames = value;
			}
		}
		[XmlAttribute("Transformation")]
		public string TransformerString {
			get {
				return this.transformerString;
			}
			set {
				this.transformerString = value;
			}
		}
		[XmlElement("Mesh")]
		public Mesh Mesh {
			get {
				return this.mesh;
			}
			set {
				this.mesh = value;
			}
		}
		[XmlElement("Light")]
		public LightWrapper
			LightWrapper = null;
		
		public SceneGraphNode () {
		}
		public SceneGraphNode (IEnumerable<SceneGraphNode> subNodes) {
			foreach(SceneGraphNode sgn in subNodes) {
				this.childNames.Add(sgn.Name);
			}
		}
		private SceneGraphNode (Matrix4 matrix, List<string> children, List<CameraFollowing> followings, Mesh mesh, LightWrapper lightWrapper) {
			this.Transformer = matrix;
			this.childNames = children;
			this.mesh = mesh;
			this.LightWrapper = lightWrapper;
			this.cameraFollowings = followings;
		}
		public SceneGraphNode (Matrix4 transformer, IEnumerable<SceneGraphNode> subNodes) : this(subNodes) {
			this.Transformer = transformer;
		}
		public SceneGraphNode (Mesh mesh) : this() {
			this.Mesh = mesh;
		}
		public SceneGraphNode (Light light) : this() {
			this.LightWrapper = new LightWrapper(light);
		}
		public SceneGraphNode (Matrix4 transformer, IEnumerable<SceneGraphNode> subNodes, Mesh mesh, Light light) : this(transformer,subNodes) {
			this.Mesh = mesh;
			this.LightWrapper = new LightWrapper(light);
		}
		
		public void AddChild (SceneGraphNode subNode) {
			this.childNames.Add(subNode.Name);
		}
		
		public override int GetHashCode () {
			return this.Name.GetHashCode();
		}
		public IEnumerable<SceneGraphNode> GetChildren (VersioningDictionary<double,string,SceneGraphNode> versioning, double version, int maxDepth, int depth) {
			if(depth < maxDepth) {
				foreach(string name in this.childNames) {
					yield return versioning.GetMixedValue(version, name);
				}
			}
		}
		public void Inject (VersioningDictionary<double,string,SceneGraphNode> versioning, double version, int maxDepth, MatrixStack stack, CameraWrapper cw, List<RenderItem> ris, List<Light> lis, int depth) {
			if(depth < maxDepth) {
				stack.PushMatrix(this.Transformer);
				if(this.Mesh != null) {
					this.Mesh.Inject(stack.Top, ris);
				}
				if(this.LightWrapper != null) {
					this.LightWrapper.Inject(stack.Top, lis);
				}
				foreach(SceneGraphNode sgn in this.GetChildren(versioning,version,maxDepth,depth)) {
					sgn.Inject(versioning, version, maxDepth, stack, cw, ris, lis, depth+1);
				}
				if(this.cameraFollowings != null) {
					foreach(CameraFollowing cf in this.cameraFollowings) {
						cf.Apply(cw, stack.Top);
					}
				}
				stack.PopMatrix();
			}
		}

		public void Resolve (Dictionary<string,Material> materialDictionary) {
			this.Transformer = Matrix4.Parse(this.transformerString);
			if(this.Mesh != null) {
				this.Mesh.Resolve(materialDictionary);
			}
		}
		#region IMixable implementation
		public SceneGraphNode MixWith (SceneGraphNode other, double factor) {
			return new SceneGraphNode(Matrix4.InterpolateParse(this.transformerString, other.transformerString, factor), this.childNames, this.cameraFollowings, this.mesh, this.LightWrapper);
		}
		#endregion

		
	}
}