//
//  Face.cs
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

namespace Renderer {
	
	public class Face : RenderItem {
		
		public Point3 Normal;
		public double D;
		
		public Face (double a, double b, double c, double d, Material material) : base(material) {
			this.Normal = new Point3(a, b, c);
			this.D = d;
		}
		public override void GetBounds (out double x0, out double x1, out double y0, out double y1, out double z0, out double z1) {
			x0 = double.NegativeInfinity;
			y0 = x0;
			z0 = y0;
			x1 = double.PositiveInfinity;
			y1 = x1;
			z1 = y1;
		}
		public override void GetDimensionBounds (int dim, out double x0, out double x1) {
			x0 = double.NegativeInfinity;
			x1 = double.PositiveInfinity;
		}
		public override double HitAt (Ray ray) {
			double t = (D-Normal.X*ray.X0-Normal.Y*ray.Y0-Normal.Z*ray.Z0)/(Normal.X*ray.DX+Normal.Y*ray.DY+Normal.Z*ray.DZ);
			if(t > 0.0d) {
				return t;
			}
			else {
				return double.PositiveInfinity;
			}
		}
		public override void Cast (Ray ray, CastResult cr) {
			double t = (D-Normal.X*ray.X0-Normal.Y*ray.Y0-Normal.Z*ray.Z0)/(Normal.X*ray.DX+Normal.Y*ray.DY+Normal.Z*ray.DZ);
			if(t > 0.0d) {
				cr.Copy(t, Normal, 0.0d, 0.0d);
			}
			else {
				cr.SetNull();
			}
		}

		public override bool InBox (double xm, double xM, double ym, double yM, double zm, double zM) {
			return true;
		}
		public override double Surface () {
			return double.PositiveInfinity;
		}
		public override double SplitSurface (double sweep, int dim) {
			return double.PositiveInfinity;
		}
		public override Tuple<ProxyRenderItem[], ProxyRenderItem[]> SplitAt (double sweep, int dimension) {
			return null;
		}
		
	}
	
}