using System;
using System.IO;

namespace Renderer {

	public sealed class Material {

		public static readonly Material DefaultMaterial = new Material();
		public static readonly Material RedMaterial = new Material(0xc00000, 0xc00000);
		public static readonly Material GreenMaterial = new Material(0xc000, 0xc000);
		public static readonly Material BlueMaterial = new Material(0xc0, 0xc0, 0xc0);
		public static readonly Material YellowMaterial = new Material(0xc0c000, 0xc0c000, 0xc0c000);
		public static readonly Material PurpleMaterial = new Material(0x800080, 0x800080, 0x800080);
		public static readonly Material MetalMaterial = new Material(0x303030, 0x303030, 0xffffff, 15.0d, 0.0d, null, null, null, 1.0d, 2.0d, 0.92d, null);
		public static readonly Material GlassMaterial = new Material(0xffffff, 0x101010, 0xffffff, 88.0d, 0.75d, null, null, null, 1.0d, 1.0d, double.NaN, null, 0.001d);
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
			ambient = Color.Multiply(ambient, tex);
			diffuse = Color.Multiply(diffuse, tex);
			specular = Color.Multiply(specular, tex);
			double fres = this.ReflectanceGetter(cos);
			double ta = Math.Max(0.0d, this.ReflectanceThreshold*fres-this.Transparent);
			reflectance = Color.FromFrac(ta);
			tex = this.Reflection(tu);
			specular = Color.Multiply(specular, tex);
			reflectance = Color.Multiply(reflectance, tex);
			refraction = Color.FromFrac(this.Transparent);
		}

		public double FresnelLaw (double cos) {
			return (FresnelR0+(1.0d-FresnelR0)*Math.Pow(1.0d-cos, 5.0d));
		}
		public double ReflectanceConstant (double cos) {
			return Reflectance;
		}
		
	}
}

