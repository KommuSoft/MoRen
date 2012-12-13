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
using System.Collections;
using System.Collections.Generic;

namespace Renderer {

	public class VersioningDictionary<TVersion,TKey,TValue> : IDictionary<TVersion,Dictionary<TKey,TValue>> where TVersion : IComparable<TVersion> {

		private readonly List<TVersion> versions = new List<TVersion>();
		private readonly List<DictionaryVersion<TVersion,TKey,TValue>> dictionaryContainer = new List<DictionaryVersion<TVersion, TKey, TValue>>();

		public TValue GetLatestBefore (TVersion version, TKey key) {
			int index = versions.BinarySearch(version);
			if(index < 0x00) {
				index = (~index)-0x01;
			}
			TValue val = default(TValue);
			while(index >= 0x00 && !dictionaryContainer[index].TryGetValue(key,out val)) {
				index--;
			}
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

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator () {
			return this.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<TVersion, Dictionary<TKey, TValue>>> GetEnumerator () {
			return this.GetDictionaries().GetEnumerator();
		}
		#endregion

		public IEnumerable<KeyValuePair<TVersion, Dictionary<TKey, TValue>>> GetDictionaries () {
			foreach(DictionaryVersion<TVersion,TKey,TValue> dict in this.dictionaryContainer) {
				yield return new KeyValuePair<TVersion, Dictionary<TKey, TValue>>(dict.Version, dict);
			}
		}

		public IEnumerable<TValue> GetValues () {
			foreach(DictionaryVersion<TVersion,TKey,TValue> dict in this.dictionaryContainer) {
				foreach(TValue val in dict.Values) {
					yield return val;
				}
			}
		}

		#region ICollection implementation
		public void Add (KeyValuePair<TVersion, Dictionary<TKey, TValue>> item) {
			this.Add(item.Key, item.Value);
		}

		public void Clear () {
			this.dictionaryContainer.Clear();
			this.versions.Clear();
		}

		public bool Contains (KeyValuePair<TVersion, Dictionary<TKey, TValue>> item) {
			int index = this.versions.BinarySearch(item.Key);
			if(index >= 0x00) {
				return this.dictionaryContainer[index] == item.Value;
			}
			return false;
		}

		public void CopyTo (KeyValuePair<TVersion, Dictionary<TKey, TValue>>[] array, int arrayIndex) {
			throw new System.NotImplementedException();
		}

		public bool Remove (KeyValuePair<TVersion, Dictionary<TKey, TValue>> item) {
			int index = this.versions.BinarySearch(item.Key);
			if(index >= 0x00 && this.dictionaryContainer[index] == item.Value) {
				this.versions.RemoveAt(index);
				this.dictionaryContainer.RemoveAt(index);
				return true;
			}
			return false;
		}

		public int Count {
			get {
				return this.dictionaryContainer.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return true;
			}
		}
		#endregion

		#region IDictionary implementation
		public bool ContainsKey (TVersion key) {
			return this.versions.BinarySearch(key) >= 0x00;
		}

		public bool Remove (TVersion key) {
			int index = this.versions.BinarySearch(key);
			if(index >= 0x00) {
				this.versions.RemoveAt(index);
				this.dictionaryContainer.RemoveAt(index);
				return true;
			}
			return false;
		}

		public bool TryGetValue (TVersion key, out Dictionary<TKey, TValue> value) {
			int index = this.versions.BinarySearch(key);
			if(index >= 0x00) {
				value = this.dictionaryContainer[index];
				return true;
			}
			else {
				value = default(Dictionary<TKey, TValue>);
				return false;
			}
		}

		public Dictionary<TKey, TValue> this [TVersion key] {
			get {
				int index = this.versions.BinarySearch(key);
				if(index >= 0x00) {
					return this.dictionaryContainer[index];
				}
				else {
					throw new KeyNotFoundException();
				}
			}
			set {
				int index = this.versions.BinarySearch(key);
				if(index >= 0x00) {
					this.dictionaryContainer[index] = new DictionaryVersion<TVersion, TKey, TValue>(key, value);
				}
				else {
					throw new KeyNotFoundException();
				}
			}
		}

		public ICollection<TVersion> Keys {
			get {
				return this.versions;
			}
		}

		public ICollection<Dictionary<TKey, TValue>> Values {
			get {
				List<Dictionary<TKey, TValue>> list = new List<Dictionary<TKey, TValue>>();
				foreach(Dictionary<TKey, TValue> val in this.dictionaryContainer) {
					list.Add(val);
				}
				return list;
			}
		}
		#endregion


	}
}