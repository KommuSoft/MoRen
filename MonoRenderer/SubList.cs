//
//  SubList.cs
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

	public sealed class SubList : IComparable<SubList> {

		public List<int> Indices = new List<int>();
		public int Frequency = 1;
		public long Data = 0x00;
		private SubList representative;
		public int Offset;

		public bool Empty {
			get {
				return this.Indices.Count <= 0x00;
			}
		}
		public SubList Representative {
			get {
				return this.representative;
			}
			set {
				if(value != null && value != this) {
					this.representative = value;
					this.Indices = null;
				}
			}
		}

		public SubList () {
			this.representative = this;
		}

		public void Add (int index) {
			this.Indices.Add(index);
		}

		public bool SubListOf (SubList other, out bool equivalent) {
			IEnumerator<int> par = other.Indices.GetEnumerator();
			equivalent = false;
			foreach(int ind in this.Indices) {
				while(par.MoveNext() && par.Current != ind)
					;
				if(par.Current != ind) {
					return false;
				}
			}
			equivalent = other.Indices.Count == Indices.Count;
			return true;
		}

		public override bool Equals (object obj) {
			if(obj is SubList) {
				SubList slo = (SubList)obj;
				if(this.Indices.Count != slo.Indices.Count) {
					return false;
				}
				for(int i = 0; i < this.Indices.Count; i++) {
					if(this.Indices[i] != slo.Indices[i]) {
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode () {
			int res = 0x00;
			for(int i = 0; i < this.Indices.Count; i++) {
				res = (res<<0x03)|((int)(res&0xe0000000)>>0x28);
				res += this.Indices[i].GetHashCode();
			}
			return res;
		}

		public int CompareTo (SubList other) {
			if(this.Frequency != other.Frequency) {
				return other.Frequency.CompareTo(this.Frequency);
			}
			else {
				int n = Math.Min(this.Indices.Count, other.Indices.Count);
				if(this.Indices.Count != other.Indices.Count) {
					return this.Indices.Count.CompareTo(other.Indices.Count);
				}
				for(int i = 0; i < n; i++) {
					if(this.Indices[i] != other.Indices[i]) {
						return this.Indices[i].CompareTo(other.Indices[i]);
					}
				}
				return 0x00;
			}
		}
		public void Clear () {
			this.Data = this.Indices.Count;
			this.Indices = null;
		}

		public long ToLongRepresentation () {
			if(this.representative != this) {
				return this.Representative.ToLongRepresentation();
			}
			else {
				return (((long)this.Indices.Count+this.Offset)<<0x20)|((long)this.Offset);
			}
		}

	}
}

