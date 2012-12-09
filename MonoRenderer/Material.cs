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
		public readonly Texture Texture;
		public readonly Texture Reflection;
		public readonly Texture Bump;
		public readonly Texture Envmap;
		public readonly double FresnelR0;
		public readonly double BrewesterAngle;
		public readonly double Reflectance;
		public readonly double ReflectanceThreshold = 1.0d;
		public readonly FactorFromAngle ReflectanceGetter;
		
		public Material (uint ambient = 0xc0c0c0, uint diffuse = 0xc0c0c0, uint specular = 0xc0c0c0, double shininess = 15.0d, double transparent = 0.0d, Texture texture = null, Texture reflection = null, Texture bump = null, double ni=1.0d, double nt = 1.1d, double reflectance = double.NaN, FactorFromAngle reflectanceGetter = null, double reflectanceThreshold = 0.5d) {
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
		}

		public void ADSAtAndBump (CastResult cr, Point3 raydir, out uint ambient, out uint diffuse, out uint specular, out uint reflectance, out uint refraction) {
			//ambient = specular = diffuse = Color.FromWavelength((int)(430*cos+350));
			Point3 tu = cr.TU;
			if(this.Bump != null) {
				this.Bump.TweakNormal(tu, cr.Normal, cr.BumpX, cr.BumpY);
			}
			double cos = -Point3.CosAngleNorm(raydir, cr.Normal);
			ambient = this.Ambient;
			diffuse = this.Diffuse;
			specular = this.Specular;
			uint tex;
			if(this.Texture != null) {
				tex = this.Texture.ColorAt(tu);
				ambient = Color.Multiply(ambient, tex);
				diffuse = Color.Multiply(diffuse, tex);
				specular = Color.Multiply(specular, tex);
			}
			double fres = this.ReflectanceGetter(cos);
			double ta = Math.Max(0.0d, this.ReflectanceThreshold*fres-this.Transparent);
			//Console.WriteLine("FRES {0} TB {1}", fres, tb);
			reflectance = Color.FromFrac(ta);
			if(this.Reflection != null) {
				tex = this.Reflection.ColorAt(tu);
				specular = Color.Multiply(specular, tex);
				reflectance = Color.Multiply(reflectance, tex);
			}
			refraction = Color.FromFrac(this.Transparent);
		}

		public double FresnelLaw (double cos) {
			//Console.WriteLine("x {0} f {1}", cos, FresnelR0+(1.0d-FresnelR0)*Math.Pow(1.0d-cos, 5.0d));
			return (FresnelR0+(1.0d-FresnelR0)*Math.Pow(1.0d-cos, 5.0d));
		}
		public double ReflectanceConstant (double cos) {
			return Reflectance;
		}

		public void ADSAt11 (Point3 tu, out uint ambient, out uint diffuse, out uint specular) {
			ambient = this.Ambient;
			diffuse = this.Diffuse;
			specular = this.Specular;
			uint tex;
			tex = this.Texture.ColorAt(tu);
			ambient = Color.Multiply(ambient, tex);
			diffuse = Color.Multiply(diffuse, tex);
			specular = Color.Multiply(specular, tex);
			tex = this.Reflection.ColorAt(tu);
			//ambient = Color.Multiply(ambient, tex);
			//diffuse = Color.Multiply(diffuse, tex);
			specular = Color.Multiply(specular, tex);
		}
		
	}
}

