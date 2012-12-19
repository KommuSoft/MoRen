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
			if(starts.Count < 0x01) {
				tb = ta = 0.5d*(t0+t1);
				heuristic = double.PositiveInfinity;
			}
			else {
				int ln = 0x00, rn = starts.Count;
				starts.First().GetFaceNormalBounds(facenormal, out tb, out ta);
				ta = tb;
				heuristic = (t1-tb)*rn;
				double tmpheu, x, x2, dummy;
				ProxyRenderItem minfree;
				bool removing;
				foreach(ProxyRenderItem pri in starts) {
					pri.GetFaceNormalBounds(facenormal, out x, out dummy);
					removing = true;
					while(removing && active.Count > 0x00) {
						minfree = active.Min;
						minfree.GetFaceNormalBounds(facenormal, out dummy, out x2);
						if(x2 <= x) {
							active.Remove(minfree);
							rn--;
							tmpheu = (x2-t0)*ln+(t1-x2)*rn;
							if(tmpheu < heuristic) {
								heuristic = tmpheu;
								ta = x2;
							}
						}
						else {
							removing = false;
						}
					}
					tmpheu = (x-t0)*ln+(t1-x)*rn;
					if(tmpheu < heuristic) {
						heuristic = tmpheu;
						ta = x;
					}
					ln++;
					active.Add(pri);
				}
				tb = ta;
			}
		}

	}
}

