using System;

namespace Renderer {
	
	public abstract class OrderingRenderItem : RenderItem {
		
		public OrderingRenderItem (Material material) : base(material) {}
		
		public abstract void generateOrdering (out double a, out double b, out double c, out double d);
	}
}

