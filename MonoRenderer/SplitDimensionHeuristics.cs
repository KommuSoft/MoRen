//
//  HalfSplitDimensionHeuristic.cs
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

	public static class SplitDimensionHeuristics {

		public static double SplitHalfDimensionHeuristic (IEnumerable<RenderItem> items, int dim, double x0, double x1, out double heuristicValue) {
			heuristicValue = 0.0d;
			return 0.5d*x0+0.5d*x1;
		}
		public static double HalfTriangleCountHeuristic (IEnumerable<RenderItem> items, int dim, double x0, double x1, out double heuristicValue) {
			SortedSet<RenderItem> starts = new SortedSet<RenderItem>(items, new RenderItemDimStartComparator(dim));
			SortedSet<RenderItem> activeWindow = new SortedSet<RenderItem>(new RenderItemDimStopComparator(dim));
			heuristicValue = double.PositiveInfinity;
			double px = 0.5d*(x0+x1);
			double x, dummy;
			double leftsf = 0.0d;
			double rightsf = starts.Sum(item => item.Surface());
			double heu;
			double xe;
			int nl = 0x00;
			int nr = starts.Count;
			foreach(RenderItem ri in starts) {
				ri.GetDimensionBounds(dim, out x, out dummy);
				activeWindow.Add(ri);
				leftsf += ri.Surface();
				nl++;
				bool removed = true;
				while(activeWindow.Count > 0x00 && removed) {
					RenderItem min = activeWindow.Min;
					min.GetDimensionBounds(dim, out dummy, out xe);
					removed = xe <= x;
					if(removed) {
						nr--;
						rightsf -= ri.Surface();
						activeWindow.Remove(min);
					}
				}
				heu = leftsf*nl+rightsf*nr;
				if(heu < heuristicValue && x0 < x && x < x1) {
					heuristicValue = heu;
					px = x;
				}
			}
			return px;
		}

	}
}

