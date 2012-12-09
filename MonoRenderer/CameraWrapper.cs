//
//  CameraWrapper.cs
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

	[XmlType("Camera")]
	public class CameraWrapper {

		[XmlElement("Position")]
		public Point3
			Position = new Point3(0.0d, 0.0d, 0.0d);
		[XmlElement("Lookat")]
		public Point3
			Lookat = new Point3(0.0d, 0.0d, 1.0d);
		[XmlAttribute("ScreenDistance")]
		public double
			ScreenDistance = 1.5d;
		[XmlAttribute("Roll")]
		public double
			Roll = 0.0d;
		[XmlAttribute("FoVH")]
		public double
			FoVH = 0.25d*Math.PI;
		[XmlAttribute("Width")]
		public uint
			width;
		[XmlAttribute("Height")]
		public uint
			height;


		public CameraWrapper () {
		}

		public Camera Camera (Accelerator acc, Light[] lights, EnvironmentSettings es) {
			Camera cam = new Camera((int)this.width, (int)this.height, this.ScreenDistance, this.FoVH, acc, lights, es);
			cam.Position = this.Position;
			cam.LookAt = this.Lookat;
			cam.Roll = Roll;
			cam.MakeDirty();
			return cam;
		}

	}
}

