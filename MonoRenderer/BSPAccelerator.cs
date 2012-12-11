//
//  BSPAccelerator.cs
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

	using NormalInterval = Tuple<Point3,double,double>;

	public class BSPAccelerator : Accelerator {

		public BSPAccelerator (IEnumerable<RenderItem> items) : this(items,ImplementedSplitHeuristics.SurfaceAreaHeuristic,Point3.UnitDummies) {
		}
		public BSPAccelerator (IEnumerable<RenderItem> items, SplitHeuristic sh) : this(items,sh,Point3.UnitDummies) {
		}
		public BSPAccelerator (IEnumerable<RenderItem> items, SplitHeuristic sh, IEnumerable<Point3> facenormals) {
			double ta, tb;
			List<NormalInterval> fn = new List<NormalInterval>();
			foreach(Point3 normal in facenormals) {
				double tta = double.PositiveInfinity;
				double ttb = double.NegativeInfinity;
				foreach(RenderItem ri in items) {
					ri.GetFaceNormalBounds(normal, out ta, out tb);
					tta = Math.Min(tta, ta);
					ttb = Math.Max(ttb, tb);
				}
				fn.Add(new NormalInterval(normal, tta, ttb));
			}
			Split(sh, fn, items);
		}

		private void Split (SplitHeuristic sh, IEnumerable<NormalInterval> facenormals, IEnumerable<RenderItem> items) {
			NormalInterval nibest = null;
			double minHeu = double.PositiveInfinity;
			double ta = double.NaN, tb = ta, tta, ttb, theu;
			foreach(NormalInterval ni in facenormals) {
				sh(items, ni.Item1, ni.Item2, ni.Item3, out tta, out ttb, out theu);
				if(theu < minHeu) {
					minHeu = theu;
					nibest = ni;
					ta = tta;
					tb = ttb;
				}
			}
			Console.WriteLine("As split criterium, we will use {0}/{1}/{2}", nibest, ta, tb);
		}

		#region Accelerator implementation
		public RenderItem CalculateHit (Ray ray, out double t, double MaxT) {
			throw new System.NotImplementedException();
		}
		#endregion


	}
}

