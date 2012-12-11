//
//  RenderItem.cs
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
	
	public abstract class RenderItem : IRenderable {
		
		public RenderItem Root {
			get {
				return this;
			}
		}
		public readonly int Id = 0;
		private static int IdDispatcher = 0x00;
		public readonly Material Material;
		
		public RenderItem (Material material) {
			this.Id = IdDispatcher++;
			this.Material = material;
		}
		
		/// <summary>
		/// Calculates the T-value at which the given ray will hit the object. If the ray doesn't hit the object <see cref="Double.PositiveInfinity"/>
		/// is returned.
		/// </summary>
		/// <param name="ray">
		/// The given ray that intersects with the object.
		/// </param>
		/// <returns>
		/// The T-value of the ray at the point where the given ray hits the object or <see cref="Double.PositiveInfinity"/> if the ray doesn't hit the object.
		/// </returns>
		public abstract double HitAt (Ray ray);
		public abstract void Cast (Ray ray, CastResult cr);
		public bool BoxOverlap (double xm, double xM, double ym, double yM, double zm, double zM) {
			double x0, x1, y0, y1, z0, z1;
			GetBounds(out x0, out x1, out y0, out y1, out z0, out z1);
			return (Math.Max(x0, xm) <= Math.Min(x1, xM) && Math.Max(y0, ym) <= Math.Min(y1, yM) && Math.Max(z0, zm) <= Math.Min(z1, zM));
		}
		public bool BoxOverlap (BoundingBox bb) {
			return BoxOverlap(bb.X0, bb.X1, bb.Y0, bb.Y1, bb.Z0, bb.Z1);
		}
		public virtual void GetBounds (out double x0, out double x1, out double y0, out double y1, out double z0, out double z1) {
			this.GetDimensionBounds(0x00, out x0, out x1);
			this.GetDimensionBounds(0x01, out y0, out y1);
			this.GetDimensionBounds(0x02, out z0, out z1);
		}
		public abstract void GetDimensionBounds (int dim, out double x0, out double x1);
		public abstract void GetFaceNormalBounds (Point3 facenormal, out double t0, out double t1);
		public abstract bool InBox (double xm, double xM, double ym, double yM, double zm, double zM);
		public bool InBox (BoundingBox bb) {
			return InBox(bb.X0, bb.X1, bb.Y0, bb.Y1, bb.Z0, bb.Z1);
		}
		public abstract double Surface ();
		public abstract double SplitSurface (double sweep, int dimension);
		public abstract Tuple<ProxyRenderItem[],ProxyRenderItem[]> SplitAt (double sweep, int dimension);
		public override int GetHashCode () {
			return this.Id.GetHashCode();
		}
		
	}
	
}