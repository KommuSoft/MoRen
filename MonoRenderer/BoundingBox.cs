//
//  BoundingBox.cs
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

	public sealed class BoundingBox {

		public double X0;
		public double X1;
		public double Y0;
		public double Y1;
		public double Z0;
		public double Z1;

		public BoundingBox () : this(double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity,double.PositiveInfinity,double.NegativeInfinity) {

		}

		public BoundingBox (double x0, double x1, double y0, double y1, double z0, double z1) {
			this.SetValues(x0, x1, y0, y1, z0, z1);
		}

		public void SetValues (double x0, double x1, double y0, double y1, double z0, double z1) {
			this.X0 = x0;
			this.X1 = x1;
			this.Y0 = y0;
			this.Y1 = y1;
			this.Z0 = z0;
			this.Z1 = z1;
		}
	}
}

