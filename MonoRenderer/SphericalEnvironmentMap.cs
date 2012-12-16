//
//  SphericalEnvironmentMap.cs
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

	public sealed class SphericalEnvironmentMap {

		private readonly ColorAtMethod positiveZ;
		private readonly ColorAtMethod negativeZ;

		public SphericalEnvironmentMap (ColorAtMethod negativeZ, ColorAtMethod positiveZ) {
			this.negativeZ = ColorAtMethods.GetOrBlack(negativeZ);
			this.positiveZ = ColorAtMethods.GetOrBlack(positiveZ);
		}

		public uint GetColorAt (Ray ray) {
			Point3 p = new Point3(0.5d*ray.DX+0.5d, 0.5d*ray.DY+0.5d, 0.0d);
			if(ray.Z0 >= 0.0d) {
				return this.positiveZ(p);
			}
			else {
				return this.negativeZ(p);
			}
		}

		public static implicit operator EnviromentMap (SphericalEnvironmentMap sem) {
			if(sem != null) {
				return sem.GetColorAt;
			}
			else {
				return null;
			}
		}


	}
}

