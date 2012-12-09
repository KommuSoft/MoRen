//
//  MatrixStack.cs
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
				Content.Push(this.Top);
			}
			else {
				Content.Push(matrix.CopyTransform(this.Top));
			}
		}
		public void PopMatrix () {
			Content.Pop();
		}
		
	}
	
}