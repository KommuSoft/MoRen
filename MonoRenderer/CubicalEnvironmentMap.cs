//
//  CubicalEnvironmentMap.cs
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

	public sealed class CubicalEnvironmentMap {

		private readonly ColorAtMethod front;
		private readonly ColorAtMethod back;
		private readonly ColorAtMethod left;
		private readonly ColorAtMethod right;
		private readonly ColorAtMethod up;
		private readonly ColorAtMethod down;

		public CubicalEnvironmentMap (ColorAtMethod front, ColorAtMethod back, ColorAtMethod left, ColorAtMethod right, ColorAtMethod up, ColorAtMethod down) {
			this.front = ColorAtMethods.GetOrBlack(front);
			this.back = ColorAtMethods.GetOrBlack(back);
			this.left = ColorAtMethods.GetOrBlack(left);
			this.right = ColorAtMethods.GetOrBlack(right);
			this.up = ColorAtMethods.GetOrBlack(up);
			this.down = ColorAtMethods.GetOrBlack(down);
		}

		public Color GetColorAt (Ray ray) {
			double ux = Math.Abs(ray.DX), uy = Math.Abs(ray.DY), uz = Math.Abs(ray.DZ);
			Point3 p = new Point3();
			switch(Maths.MaxIndex(ux, uy, uz)) {
				case 0x00://left-right
					ux = 1.0d/ux;
					p.Y = 0.5d-0.5d*ray.DY*ux;
					if(ray.DX < 0x00) {//left
						p.X = 0.5d+0.5d*ray.DZ*ux;
						return left(p);
					}
					else {//right
						p.X = 0.5d-0.5d*ray.DZ*ux;
						return right(p);
					}
				case 0x01://down-up
					uy = 1.0d/uy;
					p.X = 0.5d+0.5d*ray.DX*uy;
					if(ray.DY < 0x00) {//down
						p.Y = 0.5d-0.5d*ray.DZ*uy;
						return down(p);
					}
					else {//up
						p.Y = 0.5d+0.5d*ray.DZ*uy;
						return up(p);
					}
				default ://front-back
					uz = 1.0d/uz;
					p.Y = 0.5d-0.5d*ray.DY*uz;
					if(ray.DZ < 0x00) {//front
						p.X = 0.5d-0.5d*ray.DX*uz;
						return front(p);
					}
					else {//back
						p.X = 0.5d+0.5d*ray.DX*uz;
						return back(p);
					}
			}
		}

		public static implicit operator EnviromentMap (CubicalEnvironmentMap cem) {
			if(cem != null) {
				return cem.GetColorAt;
			}
			else {
				return null;
			}
		}

	}
}

