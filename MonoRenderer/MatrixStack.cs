using System;
using System.Collections.Generic;

namespace Renderer {
	
	public class MatrixStack {
		
		public readonly Stack<Matrix4> Content = new Stack<Matrix4>();
		
		public Matrix4 Top {
			get {
				return this.Content.Peek();
			}
		}

		public MatrixStack () {
			Content.Push(new Matrix4());
		}
		
		public void PushMatrix (Matrix4 matrix) {
			if(matrix == null) {
				Content.Push(this.Top);//simply add a reference to save time
			}
			else {
				Content.Push(this.Top.CopyTransform(matrix));
			}
		}
		public void PopMatrix () {
			Content.Pop();
		}
		
	}
	
}