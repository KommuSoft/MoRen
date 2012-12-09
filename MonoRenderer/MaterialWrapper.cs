//
//  MaterialWrapper.cs
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
using System.IO;
using System.Xml.Serialization;

namespace Renderer.SceneBuilding {

	public class MaterialWrapper {

		[XmlAttribute("Texture")]
		public string
			Texture = null;
		[XmlAttribute("Reflection")]
		public string
			Reflection = null;
		[XmlAttribute("Bump")]
		public string
			Bump = null;
		[XmlElement("Ambient")]
		public ColorWrapper
			Ambient = new ColorWrapper(0xc0c0c0);
		[XmlElement("Specular")]
		public ColorWrapper
			Specular = new ColorWrapper(0xc0c0c0);
		[XmlElement("Diffuse")]
		public ColorWrapper
			Diffuse = new ColorWrapper(0xc0c0c0);
		[XmlAttribute("NI")]
		public double
			NI = 1.0d;
		[XmlAttribute("NT")]
		public double
			NT = 1.0d;
		[XmlAttribute("Shininess")]
		public double
			Shininess = 15.0d;
		[XmlAttribute("Transparent")]
		public double
			Transparent = 0.0d;
		[XmlAttribute("Reflectance")]
		public double
			Reflectance = double.NaN;
		[XmlAttribute("ReflectedThreshold")]
		public double
			ReflectedThreshold = 0.5d;

		public MaterialWrapper () {
		}

		public Material GenerateMaterial () {
			uint ambient = this.Ambient.Color;
			uint diffuse = this.Diffuse.Color;
			uint specular = this.Specular.Color;
			ColorAtMethod texture = PerlinNullOrTexture(this.Texture);
			ColorAtMethod reflection = PerlinNullOrTexture(this.Reflection);
			Texture bump = NullOrTexture(this.Bump);
			return new Material(ambient, diffuse, specular, Shininess, Transparent, texture, reflection, bump, NI, NT, Reflectance, null, ReflectedThreshold);
		}

		private ColorAtMethod PerlinNullOrTexture (string name) {
			if(name != null && name != string.Empty) {
				string namelow = name.ToLower();
				if(namelow.StartsWith("perlin")) {
					switch(namelow) {
						case "perlinmarble":
							return PerlinCache.Marble3;
						case "perlinwood":
							return PerlinCache.Wood3;
						case "perlinsky":
							return PerlinCache.Sky3;
						default :
							return NullOrTexture(name);
					}
				}
				else {
					return NullOrTexture(name);
				}
			}
			else {
				return null;
			}
		}
		private Texture NullOrTexture (string name) {
			if(name != null && name != string.Empty && File.Exists(name)) {
				return new Texture(name);
			}
			else {
				return null;
			}
		}

	}

}

