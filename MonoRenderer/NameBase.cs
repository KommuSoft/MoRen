//  
//  NameBase.cs
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
	public abstract class NameBase {
		
		private string name;
		private static int idDispatcher = 0x00;
		
		[XmlAttribute("Name")]
		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}
		
		public NameBase () {
			this.Name = this.GetType().Name+(idDispatcher++).ToString();
		}
		public NameBase (string name) {
		}

		public override int GetHashCode () {
			return this.name.GetHashCode();
		}
	}
}

