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
using System.Xml.Serialization;

namespace Renderer.SceneBuilding {

	public class MaterialWrapper {

		[XmlAttribute("Texture")]
		public string
			Texture = null;
		[XmlAttribute("Reflection")]
		private string
			Reflection;
		[XmlAttribute("Bump")]
		private string
			Bump;
		[XmlElement("Ambient")]
		public ColorWrapper
			Ambient;
		[XmlElement("Specular")]
		public ColorWrapper
			Specular;
		[XmlElement("Diffuse")]
		public ColorWrapper
			Diffuse;
		[XmlElement("NI")]
		public double
			NI;
		[XmlElement("NT")]
		public double
			NT;
		[XmlElement("Shininess")]
		public double
			Shininess;
		[XmlElement("Transparent")]
		public double
			Transparent;
		[XmlElement("Reflectance")]
		public double
			Reflectance;
		[XmlAttribute("ReflectedThreshold")]
		public double
			ReflectedThreshold;

		public MaterialWrapper () {
		}

		public Material GenerateMaterial () {
			return null;
		}
	}

}

