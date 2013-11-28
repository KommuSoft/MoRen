//
//  ColorAtMethod.cs
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

namespace Renderer {

	public delegate Color ColorAtMethod (Point3 parameter);

	public static class ColorAtMethods {

		public static Color GetWhite (Point3 p) {
			return Color.White;
		}
		public static Color GetBlack (Point3 p) {
			return Color.Black;
		}
		public static ColorAtMethod GetOrBlack (ColorAtMethod cam) {
			if(cam == null) {
				return GetBlack;
			}
			else {
				return cam;
			}
		}
		public static ColorAtMethod GetOrWhite (ColorAtMethod cam) {
			if(cam == null) {
				return GetWhite;
			}
			else {
				return cam;
			}
		}

	}

}