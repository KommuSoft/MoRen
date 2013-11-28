//
//  CrossEnvironmentMap.cs
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

	public sealed class CrossEnvironmentMap {

		private readonly ColorAtMethod cross;

		public CrossEnvironmentMap (ColorAtMethod cross) {
			this.cross = ColorAtMethods.GetOrBlack(cross);
		}

		public Color GetColorAt (Ray ray) {
			double ux = Math.Abs(ray.DX), uy = Math.Abs(ray.DY), uz = Math.Abs(ray.DZ);
			Point3 p = new Point3();
			switch(Maths.MaxIndex(ux, uy, uz)) {
				case 0x00://left-right
					ux = 1.0d/ux;
					p.Y = 0.5d-0.5d*ray.DY*ux/3.0d;
					if(ray.DX < 0x00) {//left
						p.X = 0.125d+0.125d*ray.DZ*ux;
					}
					else {//right
						p.X = 0.625d-0.125d*ray.DZ*ux;
					}
					break;
				case 0x01://down-up
					uy = 1.0d/uy;
					p.X = 0.375d+0.125d*ray.DX*uy;
					if(ray.DY < 0x00) {//down
						p.Y = (2.5d-0.5d*ray.DZ*uy)/3;
					}
					else {//up
						p.Y = (0.5d+0.5d*ray.DZ*uy)/3;
					}
					break;
				default ://front-back
					uz = 1.0d/uz;
					p.Y = 0.5d-0.5d*ray.DY*uz/3.0d;
					if(ray.DZ < 0x00) {//front
						p.X = 0.875d-0.125d*ray.DX*uz;
					}
					else {//back
						p.X = 0.375d+0.125d*ray.DX*uz;
					}
					break;
			}
			return cross(p);
		}

		public static implicit operator EnviromentMap (CrossEnvironmentMap cem) {
			if(cem != null) {
				return cem.GetColorAt;
			}
			else {
				return null;
			}
		}

	}
}

