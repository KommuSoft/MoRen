//  
//  CastResult.cs
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
	
	public sealed class CastResult
	{
		
		public double T = double.PositiveInfinity;
		public Point3 Normal = new Point3(0.0d, 0.0d, 0.0d);
		public Point3 TU = new Point3(0.0d, 0.0d, 0.0d);
		
		public void SetNull () {
			this.T = double.PositiveInfinity;
		}
		public bool IsNull () {
			return this.T >= double.PositiveInfinity;
		}
		public bool IsActive () {
			return this.T < double.PositiveInfinity;
		}
		public void Copy (double t, Point3 norm, Material material, Point3 tu) {
			this.T = t;
			this.Normal.SetValues(norm);
			this.TU.SetValues(tu);
		}
		public void Copy (double t, Point3 norm, double tu, double tv) {
			this.T = t;
			this.Normal.SetValues(norm);
			this.TU.X = tu;
			this.TU.Y = tv;
		}
		public void Copy (double t, double nx, double ny, double nz, double tu, double tv) {
			this.T = t;
			this.Normal.SetValues(nx, ny, nz);
			this.TU.X = tu;
			this.TU.Y = tv;
		}
		public void Copy (double t, double nx, double ny, double nz, double tu, double tv, double tw) {
			this.T = t;
			this.Normal.SetValues(nx, ny, nz);
			this.TU.X = tu;
			this.TU.Y = tv;
			this.TU.Z = tw;
		}
		public void Copy (CastResult cr) {
			this.T = cr.T;
			this.Normal.SetValues(cr.Normal);
			this.TU.SetValues(cr.TU);
		}
		public void NormalizeNormal () {
			this.Normal.Normalize();
		}
		
	}
	
}