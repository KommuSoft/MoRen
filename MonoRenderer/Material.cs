//
//  Material.cs
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
using System.IO;

namespace Renderer {

	public sealed class Material {

		public static readonly Material DefaultMaterial = new Material();
		public readonly uint Ambient;
		public readonly uint Specular;
		public readonly uint Diffuse;
		public readonly double NFactor;
		public readonly double Shininess;
		public readonly double Transparent;
		public readonly ColorAtMethod Texture;
		public readonly ColorAtMethod Reflection;
		public readonly NormalTweaker Bump;
		public readonly double FresnelR0;
		public readonly double BrewesterAngle;
		public readonly double Reflectance;
		public readonly double ReflectanceThreshold = 1.0d;
		public readonly FactorFromAngle ReflectanceGetter;
		
		public Material (uint ambient = 0xc0c0c0, uint diffuse = 0xc0c0c0, uint specular = 0xc0c0c0, double shininess = 15.0d, double transparent = 0.0d, ColorAtMethod texture = null, ColorAtMethod reflection = null, NormalTweaker bump = null, double ni=1.0d, double nt = 1.1d, double reflectance = double.NaN, FactorFromAngle reflectanceGetter = null, double reflectanceThreshold = 0.5d) {
			this.Ambient = ambient;
			this.Specular = specular;
			this.Diffuse = diffuse;
			this.NFactor = nt/ni;
			this.BrewesterAngle = Math.Atan(this.NFactor);
			this.Shininess = shininess;
			this.Texture = texture;
			this.Reflection = reflection;
			this.Transparent = transparent;
			this.Bump = bump;
			this.FresnelR0 = (this.NFactor-1.0d)/(this.NFactor+1.0d);
			NFactor = 1.0d/NFactor;
			this.FresnelR0 *= this.FresnelR0;
			this.Reflectance = reflectance;
			this.ReflectanceGetter = reflectanceGetter;
			this.ReflectanceThreshold = reflectanceThreshold;
			if(reflectanceGetter == null) {
				if(double.IsNaN(reflectance)) {
					ReflectanceGetter = FresnelLaw;
				}
				else {
					ReflectanceGetter = ReflectanceConstant;
				}
			}
			if(this.Texture == null) {
				this.Texture = NullTexture;
			}
			if(this.Reflection == null) {
				this.Reflection = NullTexture;
			}
			if(this.Bump == null) {
				this.Bump = NullBump;
			}
		}

		private static uint NullTexture (Point3 p) {
			return 0xffffff;
		}
		private static void NullBump (Point3 tu, Point3 normal, Point3 bumpx, Point3 bumpy) {
		}

		public void ADSAtAndBump (CastResult cr, Point3 raydir, out uint ambient, out uint diffuse, out uint specular, out uint reflectance, out uint refraction) {
			Point3 tu = cr.TU;
			this.Bump(tu, cr.Normal, cr.BumpX, cr.BumpY);
			double cos = -Point3.CosAngleNorm(raydir, cr.Normal);
			ambient = this.Ambient;
			diffuse = this.Diffuse;
			specular = this.Specular;
			uint tex = this.Texture(tu);
			ambient = ColorUtils.Multiply(ambient, tex);
			diffuse = ColorUtils.Multiply(diffuse, tex);
			specular = ColorUtils.Multiply(specular, tex);
			double fres = this.ReflectanceGetter(cos);
			double ta = Math.Max(0.0d, this.ReflectanceThreshold*fres-this.Transparent);
			reflectance = ColorUtils.FromFrac(ta);
			tex = this.Reflection(tu);
			specular = ColorUtils.Multiply(specular, tex);
			reflectance = ColorUtils.Multiply(reflectance, tex);
			refraction = ColorUtils.FromFrac(this.Transparent);
		}

		public double FresnelLaw (double cos) {
			return (FresnelR0+(1.0d-FresnelR0)*Math.Pow(1.0d-cos, 5.0d));
		}
		public double ReflectanceConstant (double cos) {
			return Reflectance;
		}
		
	}
}

