//
//  Accelerator.cs
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

	public class KdeTreeAccelerator
	{

		public const long AddEvent = 0x0100000000L;
		public const long IndexMask = 0x00ffffffffL;
		public const long Dimensions = 0x03;

		public KdeTreeAccelerator (List<IRenderable> items) {
			this.sort(items);
		}

		private void sort (List<IRenderable> items) {
			HashSet<int>[] sets = new HashSet<int>[2*Dimensions];
			HashSet<int> elements = new HashSet<int>();
			SortedSet<Event>[] events = new SortedSet<Event>[Dimensions];
			double xm, xM, ym, yM, zm, zM, xbm = double.PositiveInfinity, xbM = double.NegativeInfinity, ybm = xbm, ybM = xbM, zbm = ybm, zbM = ybM;
			long i = 0x00;
			double totalSurface = 0.0d;
			foreach(RenderItem ri in items) {
				totalSurface += ri.Surface();
				ri.GetBounds(out xm, out xM, out ym, out yM, out zm, out zM);
				events [0x00].Add(new Event(xm, i));
				events [0x00].Add(new Event(xM, AddEvent|i));
				events [0x01].Add(new Event(ym, i));
				events [0x01].Add(new Event(yM, AddEvent|i));
				events [0x02].Add(new Event(zm, i));
				events [0x02].Add(new Event(zM, AddEvent|i));
				xbm = Math.Min(xbm, xm);
				xbM = Math.Max(xbM, xM);
				ybm = Math.Min(ybm, ym);
				ybM = Math.Max(ybM, yM);
				zbm = Math.Min(zbm, zm);
				zbM = Math.Max(zbM, zM);
				i++;
			}
			double vol = (xbM-xbm)*(ybM-ybm)*(zbM-zbm);
			subDivide(items, events, sets, totalSurface, vol);
		}

		private KDTreeNode subDivide (List<IRenderable> items, SortedSet<Event>[] events, HashSet<int>[] sets, double s, double v) {
			int maxHDim = -0x01;
			double r, h, ls, lv, rM, hM = double.NegativeInfinity, lsM, lvM;
			for(int dim = 0x00, dim2 = 0x00; dim < Dimensions; dim++, dim2 += 0x02) {
				sets [dim2].Clear();
				sets [dim2+0x01].Clear();
				r = CalculateSplit(items, s, v, events [dim], sets [dim2], sets [dim2+0x01], dim, out h, out ls, out lv);
				if(h > hM) {
					maxHDim = dim;
					hM = h;
					rM = r;

				}
			}
			return null;
		}


		public double CalculateSplit (List<IRenderable> items, double totalSurface, double volume, SortedSet<Event> events, HashSet<int>active, HashSet<int> toRemove, int dim, out double heuMax, out double leftSurface, out double leftVolume) {
			double left = 0.0d;
			double leftSoft;
			double sm = events.Min.Sweep;
			double sM = events.Max.Sweep;
			int ind;
			double heu, heuSweep = double.NaN;
			leftSurface = double.NaN;
			leftVolume = double.NaN;
			heuMax = double.NegativeInfinity;
			foreach(Event e in events) {
				ind = (int)(e.ActionItem&IndexMask);
				if(e.ActionItem >= AddEvent) {
					if(!toRemove.Remove(ind)) {
						active.Add(ind);
					}
				} else {
					if(!active.Remove(ind)) {
						toRemove.Add(ind);
					}
					left += items [ind].Surface();
				}
				if(toRemove.Count <= 0x00) {
					double sw = e.Sweep;
					leftSoft = 0.0d;
					foreach(int a in active) {
						leftSoft += items [a].SplitSurface(sw, dim);
					}
					sw = (sw-sm)/(sM-sm);
					if(sw > 0.0d && sw < 1.0d) {
						heu = (left+leftSoft)/sw+(totalSurface-left-leftSoft)/(1.0d-sw);
						if(heu > heuMax) {
							heuMax = heu;
							heuSweep = e.Sweep;
							leftVolume = volume*sw;
							leftSurface = left+leftSoft;
						}
					}
				}
			}
			return heuSweep;
		}

		public sealed class KDTreeNode
		{

			public readonly int Dimension;
			public readonly double SweepLeft;
			public readonly double SweepRight;
			public readonly KDTreeNode Left;
			public readonly KDTreeNode Right;
			public readonly RenderItem item ;

		}

		public struct Event : IComparable<Event>
		{
			public double Sweep;
			public long ActionItem;

			public Event (double sweep, long triangleAction) {
				this.Sweep = sweep;
				this.ActionItem = triangleAction;
			}

			public int CompareTo (Event other) {
				int cs = Sweep.CompareTo(other.Sweep);
				if(cs != 0x00) {
					return cs;
				}
				return ActionItem.CompareTo(other.ActionItem);
			}

		}

	}

}

