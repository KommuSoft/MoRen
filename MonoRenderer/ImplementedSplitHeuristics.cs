//
//  ImplementedSplitHeuristics.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Renderer {

	public static class ImplementedSplitHeuristics {

		public static void SplitHalfAreaHeuristic (IEnumerable<IRenderable> items, Point3 facenormal, double t0, double t1, out double ta, out double tb, out double heuristic) {
			tb = ta = 0.5d*(t0+t1);
			heuristic = 0.0d;
		}
		public static void SurfaceAreaHeuristic (IEnumerable<ProxyRenderItem> items, Point3 facenormal, double t0, double t1, out double ta, out double tb, out double heuristic) {
			SortedSet<ProxyRenderItem> starts = new SortedSet<ProxyRenderItem>(items, new RenderItemNormalStartComparator(facenormal));
			SortedSet<ProxyRenderItem> active = new SortedSet<ProxyRenderItem>(new RenderItemNormalStopComparator(facenormal));
			tb = ta = 0.5d*(t0+t1);
			int ln = 0x00, rn = starts.Count;
			double ls = 0.0d, rs = starts.Sum(a => a.Surface());
			heuristic = double.PositiveInfinity;
			ProxyRenderItem min;
			bool activeRemove;
			double x, xa, dummy, lastx = t0;
			IEnumerator<ProxyRenderItem> prie = starts.GetEnumerator();
			bool hasNext = prie.MoveNext();
			while(hasNext) {
				min = prie.Current;
				//Console.WriteLine("Considering: {0}", min);
				ln++;
				ls += min.Surface();
				min.GetFaceNormalBounds(facenormal, out x, out dummy);
				active.Add(min);
				activeRemove = true;
				while(activeRemove && active.Count > 0x00) {
					//Console.WriteLine("Check removing with count {0}", active.Count);
					min = active.Min;
					min.GetFaceNormalBounds(facenormal, out dummy, out xa);
					activeRemove = xa <= x;
					if(activeRemove) {
						//Console.WriteLine("Removing: {0}", min);
						active.Remove(min);
						rn--;
						lastx = xa;
						//Console.WriteLine("Lastxa is now: {0}", lastx);
						rs -= min.Surface();
					}
				}
				hasNext = prie.MoveNext();
				if(t0 < x && x < t1) {
					dummy = ln*ls+rn*rs;
					//Console.WriteLine("EVAL f({0})={1}", x, dummy);
					if(dummy < heuristic) {
						heuristic = dummy;
						ta = tb = x;
						if(active.Count <= 0x01) {
							ta = lastx;
							//prie.Current.GetFaceNormalBounds(facenormal, out tb, out t1);
						}
					}
				}
			}
		}

	}
}

