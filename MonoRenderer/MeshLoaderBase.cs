//
//  MeshLoaderBase.cs
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
using System.IO;

namespace Renderer {

	public abstract class MeshLoaderBase : MeshLoader {

		Material DefaultMaterial {
			get;
			set;
		}

		public abstract void Load (string currentDir, Stream stream);
		public abstract void Inject (List<RenderItem> items, Matrix4 transform, params string[] args);

		public void Load (string currentDir, string filename) {
			FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
			this.Load(currentDir, fs);
			fs.Close();
		}

	}
}

