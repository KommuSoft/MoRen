//
//  FinalList.cs
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
	public sealed class FinalList {

		public List<int> list = new List<int>();

		public FinalList (IEnumerable<SubList> list) {
			Dictionary<SubList,Tuple<SubList,int>> freqTable = new Dictionary<SubList,Tuple<SubList,int>>();
			List<SubList> sorted = new List<SubList>();
			Tuple<SubList,int> val;
			foreach(SubList sl in list) {
				if(freqTable.TryGetValue(sl, out val)) {
					sl.Representative = val.Item1;
					freqTable[val.Item1] = new Tuple<SubList, int>(val.Item1, val.Item2+1);
				}
				else {
					freqTable.Add(sl, new Tuple<SubList, int>(sl, 1));
				}
			}
			Dictionary<int,SubList> singles = new Dictionary<int, SubList>();
			foreach(KeyValuePair<SubList,Tuple<SubList,int>> kvp in freqTable) {
				kvp.Key.Frequency = kvp.Value.Item2;
				if(kvp.Key.Indices.Count > 0x01) {
					sorted.Add(kvp.Key);
				}
				else if(!kvp.Key.Empty) {
					singles.Add(kvp.Key.Indices[0x00], kvp.Key);
				}
			}
			freqTable.Clear();
			SubList sla;
			SubList slb;
			int best, bestj, tmp;
			for(int i = 0; i < sorted.Count; i++) {
				sla = sorted[i];
				if(sla != null) {
					best = -0x01;
					bestj = i;
					for(int j = i+1; j < sorted.Count; j++) {
						slb = sorted[j];
						if(slb != null) {
							tmp = CombinedList.CalculateReduction(sla, slb);
							if(tmp > best) {
								best = tmp;
								bestj = j;
							}
						}
					}
					if(bestj > i) {
						CombinedList cl = new CombinedList(sla, sorted[bestj]);
						sorted[bestj] = null;
						for(int j = 0x00; j < sorted.Count; j++) {
							slb = sorted[j];
							if(slb != null && cl.AddSubList(slb)) {
								sorted[j] = null;
							}
						}
						cl.Fill(this.list.Count);
						this.list.AddRange(cl.GetItems());
					}
					else {
						CombinedList cl = new CombinedList(sla);
						cl.Fill(this.list.Count);
						this.list.AddRange(cl.GetItems());
					}
					sorted[i] = null;
				}
			}
			foreach(KeyValuePair<int,SubList> s in singles) {
				int index = this.list.FindIndex(x => x == s.Key);
				if(index > 0x00) {
					s.Value.Offset = index;
				}
				else {
					s.Value.Offset = this.list.Count;
					this.list.Add(s.Key);
				}
			}
		}

	}
}