//  
//  SceneDescription.cs
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
using System.IO;

namespace Renderer.SceneBuilding {

	[XmlRoot("SceneDescription")]
	public class SceneDescription {
		
		private readonly List<Camera> cameras = new List<Camera>();
		private SceneGraph sg;

		/*[XmlArray("Cameras")]
		[XmlArrayItem("Camera")]
		public List<Camera> Cameras {
			get {
				return this.cameras;
			}
			set {
				this.cameras.Clear();
				foreach(Camera c in value) {
					this.cameras.Add(c);
				}
			}
		}*/
		[XmlElement("SceneGraph")]
		public SceneGraph SceneGraph {
			get {
				return this.sg;
			}
			set {
				this.sg = value;
			}
		}
		
		public SceneDescription () {
		}
		public SceneDescription (SceneGraph sg) {
			this.sg = sg;
		}
		public SceneDescription (SceneGraph sg, params Camera[] cameras) {
			this.sg = sg;
			this.cameras.AddRange(cameras);
		}

		public static SceneDescription ParseFromStream (Stream stream) {
			XmlSerializer ser = new XmlSerializer(typeof(SceneDescription));
			SceneDescription sd = (SceneDescription)ser.Deserialize(stream);
			return sd;
		}
		public static SceneDescription ParseFromStream (string filename) {
			FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
			SceneDescription sd = ParseFromStream(fs);
			fs.Close();
			return sd;
		}
		public void Save (Stream s) {
			XmlSerializer ser = new XmlSerializer(typeof(SceneDescription));
			ser.Serialize(s, this);
		}
		public void Save (string filename) {
			FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write);
			this.Save(fs);
			fs.Close();
		}
		
	}
}

