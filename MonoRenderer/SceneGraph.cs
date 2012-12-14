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
		private readonly VersioningDictionary<double,string,SceneGraphNode>
			versionDictionary = new VersioningDictionary<double, string, SceneGraphNode>();
		[XmlIgnore]
		private readonly Dictionary<string,Material>
			materialDictionary = new Dictionary<string, Material>();
		[XmlIgnore]
		private SceneTimeGraphNodeCollection[]
			times;
		[XmlIgnore]
		private MaterialWrapper[]
			materials;
		[XmlIgnore]
		private string
			rootName;
		[XmlAttribute("MaxDepth")]
		public int
			MaxDepth = 0x25;
		[XmlAttribute("Root")]
		public string RootName {
			get {
				return this.rootName;
			}
			set {
				this.rootName = value;
			}
		}
		[XmlArray("Materials")]
		[XmlArrayItem("Material")]
		public MaterialWrapper[] Materials {
			get {
				return this.materials;
			}
			set {
				this.materials = value;
				this.ResolveMaterials();
			}
		}
		
		[XmlArray("Times")]
		[XmlArrayItem("Time")]
		public SceneTimeGraphNodeCollection[] Times {
			get {
				return this.times;
			}
			set {
				this.times = value;
				this.ResolveTimes();
			}
		}
		[XmlIgnore]
		public double T0 {
			get {
				return this.versionDictionary.MinVersion;
			}
		}
		[XmlIgnore]
		public double T1 {
			get {
				return this.versionDictionary.MaxVersion;
			}
		}
		public SceneGraph () {
		}

		public void ResolveTimes () {
			this.versionDictionary.Clear();
			foreach(SceneTimeGraphNodeCollection stgnc in this.times) {
				this.versionDictionary.Add(stgnc.Time, stgnc.GenerateDictionary());
			}
			this.versionDictionary.Sort();
		}

		public void ResolveMaterials () {
			this.materialDictionary.Clear();
			foreach(MaterialWrapper mw in this.materials) {
				this.materialDictionary.Add(mw.Name, mw.GenerateMaterial());
			}
		}

		public void Resolve () {
			foreach(SceneGraphNode sgn in this.versionDictionary.GetValues()) {
				sgn.Resolve(materialDictionary);
			}
		}

		public Tuple<List<RenderItem>,List<Light>> Inject (double time) {
			List<RenderItem> ris = new List<RenderItem>();
			List<Light> lis = new List<Light>();
			MatrixStack ms = new MatrixStack();
			this.versionDictionary.GetLatestBefore(time, this.rootName).Inject(this.versionDictionary, time, this.MaxDepth, ms, ris, lis, 0x00);
			return new Tuple<List<RenderItem>, List<Light>>(ris, lis);
		}
		
	}
	
}