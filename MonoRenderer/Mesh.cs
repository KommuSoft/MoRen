//  
//  Mesh.cs
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

namespace Renderer {
	
	[XmlType("Mesh")]
	public class Mesh {
		
		[XmlIgnore]
		private string
			filename;
		[XmlIgnore]
		private string
			environment;
		[XmlIgnore]
		private MeshLoader
			loader;
		[XmlIgnore]
		private string[]
			parameters;
		
		[XmlAttribute("Filename")]
		public string Filename {
			get {
				return this.filename;
			}
			set {
				this.filename = value;
			}
		}
		[XmlAttribute("Environment")]
		public string Environment {
			get {
				return this.environment;
			}
			set {
				this.environment = value;
			}
		}
		[XmlArray("Parameters")]
		[XmlArrayItem("Parameter")]
		public string[] Parameters {
			get {
				return this.parameters;
			}
			set {
				this.parameters = value;
			}
		}
		
		public Mesh () {
		}
		public Mesh (string Filename) {
			this.Filename = Filename;
			this.Resolve();
		}

		public void Resolve () {
			if(this.filename == null || this.loader != null) {
				return;
			}
			else if(this.filename.EndsWith(".obj")) {
				this.loader = new LoaderObj();
			}
			else if(this.filename.EndsWith(".3ds")) {
				this.loader = new Loader3ds();
			}
			if(this.loader != null) {
				FileStream fs = File.Open(this.filename, FileMode.Open, FileAccess.Read);
				this.loader.Load(this.environment, fs);
				fs.Close();
			}
		}

		public void Inject (Matrix4 transformation, List<RenderItem> items) {
			if(this.loader != null) {
				this.loader.Inject(items, transformation, parameters);
			}
		}
		
		
	}
	
}