//
//  SceneTimeGraphNodeCollection.cs
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

	[XmlType("SceneTimeGraphNode")]
	public class SceneTimeGraphNodeCollection {

		private double time = 0.0d;

		private List<SceneGraphNode> nodes = new List<SceneGraphNode>();

		[XmlAttribute("Time")]
		public double Time {
			get {
				return this.time;
			}
			set {
				this.time = value;
			}
		}
		[XmlArray("SceneGraphNodes")]
		[XmlArrayItem("SceneGraphNode")]
		public List<SceneGraphNode> Nodes {
			get {
				return this.nodes;
			}
			set {
				this.nodes = value;
			}
		}


		public SceneTimeGraphNodeCollection () {
		}

		public SceneTimeGraphNodeCollection (double time, ICollection<SceneGraphNode> nodes) {
			this.time = time;
			this.nodes.AddRange(nodes);
		}

		public Dictionary<string,SceneGraphNode> GenerateDictionary () {
			Dictionary<string,SceneGraphNode> dictionary = new Dictionary<string, SceneGraphNode>();
			foreach(SceneGraphNode sgn in this.nodes) {
				dictionary.Add(sgn.Name, sgn);
			}
			return dictionary;
		}

	}
}

