//
//  Light.cs
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

namespace Renderer {

	public sealed class Light {
		
		public readonly Point3 Position;
		public readonly uint Color;
		public readonly double Radius;
		
		public Light (uint color, Point3 position, double radius = 0.0d) : this(position,color,radius) {
		}
		public Light (Point3 position, uint color = 0xffffff, double radius = 0.0d) {
			this.Position = position;
			this.Color = color;
			this.Radius = radius;
		}
		
	}
}

