//
//  AcceleratorWrapper.cs
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
using Renderer.SceneBuilding;

namespace Renderer {

	[XmlType("Accelerator")]
	public class AcceleratorWrapper {

		[XmlAttribute("Type")]
		public AcceleratorType
			type;

		public AcceleratorWrapper () {
		}

		public IAccelerator CreateAccelerator (SceneGraph sg, double time, List<RenderItem> items) {
			switch(this.type) {
				case AcceleratorType.Grid:
					return new GridAccelerator(items);
				case AcceleratorType.Naive:
					return new NaiveAccelerator(items);
				case AcceleratorType.BinSpaceTree:
					return new BSPAccelerator(items);
				default :
					return new OctTreeAccelerator(items);
			}
		}

	}
}

