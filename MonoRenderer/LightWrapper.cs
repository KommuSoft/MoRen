//
//  LightWrapper.cs
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

	[XmlType("Light")]
	public class LightWrapper {

		[XmlElement("Color")]
		public ColorWrapper
			Color;

		[XmlAttribute("Radius")]
		public double
			Radius = 0.0d;

		[XmlElement("Position")]
		public Point3
			Position = new Point3(0.0d, 0.0d, 0.0d);

		public LightWrapper () {
		}

		public LightWrapper (Light light) {
			this.Position = light.Position;
			this.Color = new ColorWrapper(light.Color);
			this.Radius = light.Radius;
		}
		public LightWrapper (ColorWrapper cw, double radius, Point3 pos) {
			this.Position = pos;
			this.Radius = radius;
			this.Color = cw;
		}

		public void Inject (Matrix4 matrix, List<Light> lis) {
			lis.Add(new Light(this.Color.Color, this.Position, this.Radius));
		}

	}
}

