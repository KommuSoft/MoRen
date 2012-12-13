//
//  VersioningDictionary.cs
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

	public class VersioningDictionary<TVersion,TKey,TValue> where TVersion : IComparable<TVersion> {

		private readonly List<TVersion> versions = new List<TVersion>();
		private readonly List<DictionaryVersion<TVersion,TKey,TValue>> dictionaryContainer = new List<DictionaryVersion<TVersion, TKey, TValue>>();

		public TValue GetLatestBefore (TVersion version, TKey key) {
			int index = versions.BinarySearch(version);
			if(index < 0x00) {
				index = (~index)-0x01;
			}
			TValue val = default(TValue);
			while(index >= 0x00 && !dictionaryContainer[index].TryGetValue(key,out val))
				index--;
			return val;
		}
		public void Add (TVersion version, Dictionary<TKey,TValue> dict) {
			this.dictionaryContainer.Add(new DictionaryVersion<TVersion,TKey,TValue>(version, dict));
		}
		public void Sort () {
			this.dictionaryContainer.Sort();
			this.versions.Clear();
			foreach(DictionaryVersion<TVersion,TKey,TValue> dv in this.dictionaryContainer) {
				this.versions.Add(dv.Version);
			}
		}
		public Tuple<TVersion,TValue,TVersion,TValue> GetVersionsBetween (TVersion version, TKey key) {
			TVersion ver1 = version, ver2 = version;
			TValue val1 = default(TValue), val2 = val1;
			int index = versions.BinarySearch(version);
			if(index < 0x00) {
				index = (~index)-0x01;
			}
			int index1 = index;
			while(index1 >= 0x00 && !dictionaryContainer[index1].TryGetValue(key,out val1))
				index1--;
			if(index1 >= 0x00) {
				ver1 = versions[index1];
			}
			index1 = index;
			while(index1 < dictionaryContainer.Count && !dictionaryContainer[index1].TryGetValue(key,out val2))
				index1++;
			if(index1 < 0x00) {
				ver2 = versions[index1];
			}
			return new Tuple<TVersion, TValue, TVersion, TValue>(ver1, val1, ver2, val2);
		}

		private class DictionaryVersion<TTime,TK,TV> : Dictionary<TK,TV>, IComparable<DictionaryVersion<TTime,TK,TV>> where TTime : IComparable<TTime> {

			private TTime version;

			public TTime Version {
				get {
					return this.version;
				}
				set {
					this.version = value;
				}
			}

			public DictionaryVersion (TTime version, Dictionary<TK,TV> dict) : base(dict) {
				this.version = version;
			}

			#region IComparable implementation
			public int CompareTo (DictionaryVersion<TTime, TK, TV> other) {
				return this.version.CompareTo(other.version);
			}
			#endregion

			public override int GetHashCode () {
				return version.GetHashCode()^base.GetHashCode();
			}


		}

	}
}