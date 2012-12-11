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

	public class VersioningDictionary<TVersion,TKey,TValue> : IComparable<VersioningDictionary<TVersion,TKey,TValue>>, IDictionary<TKey,TValue> where TVersion : IComparable<TVersion> {

		private VersioningDictionary<TVersion,TKey,TValue> older;
		private TVersion version;
		private readonly Dictionary<TKey,TValue> dictionary;

		public TVersion Version {
			get {
				return this.version;
			}
		}


		public VersioningDictionary (TVersion version, VersioningDictionary<TVersion,TKey,TValue> older = null) {
			this.version = version;
			this.older = older;
		}
		public VersioningDictionary (TVersion version, VersioningDictionary<TVersion,TKey,TValue> older, Dictionary<TKey,TValue> newversion) : this(version,older) {
			this.dictionary = newversion;
		}

		public int CompareTo (VersioningDictionary<TVersion,TKey,TValue> other) {
			return this.version.CompareTo(other.version);

		}
		#region IEnumerable implementation
		public System.Collections.IEnumerator GetEnumerator () {
			throw new System.NotImplementedException();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator () {
			throw new System.NotImplementedException();
		}
		#endregion

		#region ICollection implementation
		public void Add (KeyValuePair<TKey, TValue> item) {
			this.dictionary.Add(item.Key, item.Value);
		}

		public void Clear () {
			this.dictionary.Clear();
		}

		public bool Contains (KeyValuePair<TKey, TValue> item) {
			TValue val;
			return ((this.dictionary.TryGetValue(item.Key, out val) && val.Equals(item.Value)) || (this.older != null && this.older.Contains(item)));
		}

		public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			throw new System.NotImplementedException();
		}

		public bool Remove (KeyValuePair<TKey, TValue> item) {
			return this.Remove(item.Key);
		}

		public int Count {
			get {
				int count = this.dictionary.Count;
				if(this.older != null) {
					count = Math.Max(count, this.older.Count);
				}
				return count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion

		#region IDictionary implementation
		public void Add (TKey key, TValue value) {
			this.dictionary.Add(key, value);
		}

		public bool ContainsKey (TKey key) {
			return (this.dictionary.ContainsKey(key) || (this.older != null && this.older.ContainsKey(key)));
		}

		public bool Remove (TKey key) {
			return this.dictionary.Remove(key);
		}

		public bool TryGetValue (TKey key, out TValue value) {
			return (this.dictionary.TryGetValue(key, out value) || (this.older != null && this.older.TryGetValue(key, out value)));
		}

		public TValue this [TKey key] {
			get {
				TValue val;
				if(this.TryGetValue(key, out val)) {
					return val;
				}
				else {
					throw new KeyNotFoundException();
				}
			}
			set {
				if(this.dictionary.ContainsKey(key)) {
					this.dictionary[key] = value;
				}
				else if(this.older != null) {
					this.older[key] = value;
				}
				else {
					throw new KeyNotFoundException();
				}
			}
		}

		public bool TryGetClosestValues (TVersion version, TKey key, out TValue value1, out TVersion version1, out TValue value2, out TVersion version2) {
			if(this.older == null) {
				bool succeeds = this.dictionary.TryGetValue(key, out value1);
				value2 = value1;
				version1 = version2 = this.version;
				return succeeds;
			}
			else if(true) {
				//TODO: implement
				value1 = value2 = default(TValue);
				version1 = version2 = version;
				return false;
			}
			else {
				return this.older.TryGetClosestValues(version, key, out value1, out version1, out value2, out version2);
			}
		}

		public ICollection<TKey> Keys {
			get {
				throw new System.NotImplementedException();
			}
		}

		public ICollection<TValue> Values {
			get {
				throw new System.NotImplementedException();
			}
		}
		#endregion



	}
}

