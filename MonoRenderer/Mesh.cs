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
using System.Reflection;
using System.IO;
using Renderer.SceneBuilding;

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
		private IMeshLoader
			loader = null;
		[XmlIgnore]
		private string[]
			parameters;
		[XmlIgnore]
		private string
			material;
		
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
		[XmlAttribute("Material")]
		public string Material {
			get {
				return this.material;
			}
			set {
				this.material = value;
			}
		}

		[XmlIgnore]
		private static List<IMeshLoader>
			loaderPrototypes = null;
		
		public Mesh () {
		}
		public Mesh (string filename) {
			this.Filename = filename;
		}

		public static void InvokeLoaders () {
			if(loaderPrototypes == null) {
				loaderPrototypes = new List<IMeshLoader>();
				foreach(Type t in Assembly.GetExecutingAssembly().GetTypes()) {
					if(t.GetCustomAttributes(typeof(MeshLoaderAttribute), false).Length > 0x00 && typeof(IMeshLoader).IsAssignableFrom(t)) {
						loaderPrototypes.Add((IMeshLoader)t.GetConstructor(new Type[0x00]).Invoke(new object[0x00]));
					}
				}
			}
		}

		public void Resolve (Dictionary<string,Material> materialDictionary) {
			if(this.filename == null || this.loader != null) {
				return;
			}
			else {
				foreach(IMeshLoader iml in loaderPrototypes) {
					if(iml.CanParse(filename)) {
						this.loader = iml.Clone();
						break;
					}
				}
			}
			if(this.loader != null) {
				Material mat = null;
				if(this.material != null && this.material != string.Empty) {
					materialDictionary.TryGetValue(this.material, out mat);
				}
				if(mat != null) {
					this.loader.DefaultMaterial = mat;
				}
				this.loader.Load(this.environment, this.filename);
			}
		}

		public void Inject (Matrix4 transformation, List<RenderItem> items) {
			if(this.loader != null) {
				this.loader.Inject(items, transformation, parameters);
			}
		}
		public void Inject (List<RenderItem> items) {
			if(this.loader != null) {
				this.loader.Inject(items, parameters);
			}
		}
		public override int GetHashCode () {
			int hash = 0x00;
			if(this.filename != null) {
				hash ^= this.filename.GetHashCode();
			}
			if(this.environment != null) {
				hash ^= this.environment.GetHashCode();
			}
			if(this.material != null) {
				hash ^= this.material.GetHashCode();
			}
			if(this.parameters != null) {
				for(int i = 0x00; i < this.parameters.Length; i++) {
					hash ^= this.parameters[i].GetHashCode();
				}
			}
			return hash;
		}
		public override bool Equals (object obj) {
			if(obj is Mesh) {
				Mesh m = (Mesh)obj;
				if(this.Filename == m.Filename && this.Environment == m.Environment && this.Material == m.Material) {
					if(this.Parameters == null && m.Parameters == null) {
						return true;
					}
					else if(this.Parameters != null && m.Parameters != null) {
						int n = this.Parameters.Length;
						if(n != m.Parameters.Length) {
							return false;
						}
						for(int i = 0x00; i < n; i++) {
							if(this.parameters[i] != m.parameters[i]) {
								return false;
							}
						}
						return true;
					}
					else {
						return false;
					}
				}
				return false;
			}
			else {
				return false;
			}
		}
		
		
	}
	
}