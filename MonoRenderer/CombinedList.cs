//
//  CombinedList.cs
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
	public class CombinedList
	{

		private readonly List<int> left, right, middle;
		private readonly List<Tuple<int,SubList>> subLists = new List<Tuple<int, SubList>>();

		public int Count {
			get {
				return this.left.Count+this.middle.Count+this.right.Count;
			}
		}
		public int CountA {
			get {
				return this.left.Count;
			}
		}
		public int CountB {
			get {
				return this.middle.Count;
			}
		}
		public int CountC {
			get {
				return this.right.Count;
			}
		}

		public CombinedList (SubList sl) {
			this.left = sl.Indices;
			this.middle = new List<int>();
			this.right = new List<int>();
			this.subLists.Add(new Tuple<int, SubList>(0x00, sl));
		}
		public CombinedList (SubList sl1, SubList sl2) {
			Tuple<List<int>,List<int>,List<int>> res = Combine(sl1, sl2);
			this.left = res.Item1;
			this.middle = res.Item2;
			this.right = res.Item3;
			this.subLists.Add(new Tuple<int, SubList>(0x00, sl1));
			this.subLists.Add(new Tuple<int, SubList>(this.left.Count, sl2));
		}

		public void Fill (int offset) {
			foreach(Tuple<int,SubList> sl in subLists) {
				sl.Item2.Offset = offset+sl.Item1;
				//Console.WriteLine("now low {0} high {1} (length {2})", sl.Item2.Offset, sl.Item2.Offset+sl.Item2.Indices.Count, sl.Item2.Indices.Count);
			}
		}

		public bool AddSubList (SubList list) {
			List<int> sli = list.Indices;
			if(sli.Count < this.middle.Count) {
				int offset = this.middle.BinarySearch(sli [0x00]);
				if(offset >= 0x00 && offset < this.middle.Count-list.Indices.Count) {
					for(int i = 1; i < list.Indices.Count; i++) {
						if(list.Indices [i] != this.middle [offset+i]) {
							return false;
						}
					}
					this.subLists.Add(new Tuple<int,SubList>(offset+this.left.Count, list));
					return true;
				}
			}
			return false;
		}

		public static Tuple<List<int>,List<int>,List<int>> Combine (SubList a, SubList b) {
			List<int> left = new List<int>(), middle = new List<int>(), right = new List<int>();
			IEnumerator<int> ea = a.Indices.GetEnumerator();
			IEnumerator<int> eb = b.Indices.GetEnumerator();
			bool aa = ea.MoveNext(), ab = eb.MoveNext();
			while(aa && ab) {
				if(ea.Current < eb.Current) {
					left.Add(ea.Current);
					aa = ea.MoveNext();
				} else if(ea.Current > eb.Current) {
					right.Add(eb.Current);
					ab = eb.MoveNext();
				} else {
					middle.Add(ea.Current);
					aa = ea.MoveNext();
					ab = eb.MoveNext();
				}
			}
			while(aa) {
				left.Add(ea.Current);
				aa = ea.MoveNext();
			}
			
			while(ab) {
				right.Add(eb.Current);
				ab = eb.MoveNext();
			}
			return new Tuple<List<int>, List<int>, List<int>>(left, middle, right);
		}
		public static int CalculateReduction (SubList sublist1, SubList sublist2) {
			int n = 0x00;
			IEnumerator<int> ea = sublist1.Indices.GetEnumerator();
			IEnumerator<int> eb = sublist2.Indices.GetEnumerator();
			bool aa = ea.MoveNext(), ab = eb.MoveNext();
			while(aa && ab) {
				if(ea.Current < eb.Current) {
					aa = ea.MoveNext();
				} else if(ea.Current > eb.Current) {
					ab = eb.MoveNext();
				} else {
					n++;
					aa = ea.MoveNext();
					ab = eb.MoveNext();
				}
			}
			return n;
		}

		public IEnumerable<int> GetItems () {
			foreach(int a in this.left) {
				yield return a;
			}
			foreach(int a in this.middle) {
				yield return a;
			}
			foreach(int a in this.right) {
				yield return a;
			}
		}

	}
}

