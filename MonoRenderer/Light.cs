using System;

namespace Renderer {
	
	public class Light {
		
		public Point3 Position;
		public uint Color;
		
		public Light (uint color, Point3 position) : this(position,color) {}
		public Light (Point3 position, uint color) {
			this.Position = position;
			this.Color = color;
		}
		
	}
}

