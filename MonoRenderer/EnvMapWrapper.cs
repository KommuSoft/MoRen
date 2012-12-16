//
//  EnvMapWrapper.cs
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

	[XmlType("EnvironmentMap")]
	public class EnvMapWrapper {

		private string specification;

		[XmlAttribute("Specification")]
		public string Specification {
			get {
				return this.specification;
			}
			set {
				this.specification = value;
			}
		}

		public EnvMapWrapper () {
		}

		public IEnviromentMap ToEnvironmentMap () {
			TreeNode<string> parsed = this.specification.ParseTreeBracketsComma();
			switch(parsed.Data.Trim().ToLower()) {
				case "spherical":
					if(parsed.Count >= 0x02) {
						return new SphericalEnvironmentMap(parsed[0x00].Data.PerlinNullOrTexture(),
						                                   parsed[0x01].Data.PerlinNullOrTexture());
					}
					else if(parsed.Count >= 0x01) {
						ColorAtMethod cam = parsed[0x00].Data.PerlinNullOrTexture();
						return new SphericalEnvironmentMap(cam, cam);
					}
					return null;
				case "cubical":
					if(parsed.Count >= 0x06) {
						return new CubicalEnvironmentMap(parsed[0x00].Data.PerlinNullOrTexture(),
						                                 parsed[0x01].Data.PerlinNullOrTexture(),
						                                 parsed[0x02].Data.PerlinNullOrTexture(),
						                                 parsed[0x03].Data.PerlinNullOrTexture(),
						                                 parsed[0x04].Data.PerlinNullOrTexture(),
						                                 parsed[0x05].Data.PerlinNullOrTexture());
					}
					else if(parsed.Count >= 0x01) {
						ColorAtMethod cam = parsed[0x00].Data.PerlinNullOrTexture();
						return new CubicalEnvironmentMap(cam, cam, cam, cam, cam, cam);
					}
					return null;
				default :
					return null;
			}
		}

	}
}

