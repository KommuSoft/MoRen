using System;
using System.Collections.Generic;

namespace Renderer {
	
	public class MatrixStack {
		
		public readonly Stack<Matrix4> Content = new Stack<Matrix4>();
		public Matrix4 Top = new Matrix4();
		
		public MatrixStack () {}
		
		public void PushMatrix (Matrix4 matrix) {
			Content.Push(new Matrix4(this.Top));
			this.Top.Transform(matrix);
		}
		public void PushMatrix (MatrixManipulator mm) {
			Content.Push(new Matrix4(this.Top));
			mm(this.Top);
		}
		public void PopMatrix () {
			this.Top = Content.Pop();
		}
		
	}
	
}