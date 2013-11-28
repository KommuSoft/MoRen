//
//  AddRemoveEvent.cs
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

namespace Renderer {

	public struct AddRemoveEvent : IComparable<AddRemoveEvent> {

		public const long RemoveEvent = 0x000000000;
		public const long AddEvent = 0x100000000;
		public const long IndexMask = 0xffffffff;
		public readonly long Combined;
		public readonly double X;
		public bool Remove {
			get {
				return this.Combined < AddEvent;
			}
		}
		public bool Add {
			get {
				return this.Combined >= AddEvent;
			}
		}
		public int Index {
			get {
				return (int)(this.Combined&IndexMask);
			}
		}

		public AddRemoveEvent (int index, double x) : this(index,x,true) {
		}
		public AddRemoveEvent (int index, double x, bool addevent) {
			this.Combined = index;
			if(addevent) {
				this.Combined |= AddEvent;
			}
			this.X = x;
		}

		public int CompareTo (AddRemoveEvent other) {
			int comp = this.X.CompareTo(other.X);
			if(comp != 0x00) {
				return comp;
			}
			else {
				return this.Combined.CompareTo(other.Combined);
			}
		}

	}
}

