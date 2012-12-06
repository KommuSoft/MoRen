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
namespace Renderer {
	[XmlRoot("SceneDescription")]
	public class SceneDescription
	{
		
		private readonly List<Camera> cameras = new List<Camera>();
		private SceneGraph sg;
		private RayTracer rt;
		
		[XmlElement("RayTracer")]
		public RayTracer RayTracer {
			get {
				return this.rt;
			}
			set {
				this.rt = value;
			}
		}
		[XmlArray("Cameras")]
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
		}
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
		public SceneDescription (RayTracer rt, SceneGraph sg, params Camera[] cameras) {
			this.rt = rt;
			this.sg = sg;
			this.cameras.AddRange(cameras);
		}
		
	}
}
