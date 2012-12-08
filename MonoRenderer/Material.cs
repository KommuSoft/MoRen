using System;
using System.IO;

namespace Renderer {

	public sealed class Material {

		public static readonly Material DefaultMaterial = new Material();
		public static readonly Material RedMaterial = new Material(0xc00000, 0xc00000);
		public static readonly Material GreenMaterial = new Material(0xc000, 0xc000);
		public static readonly Material BlueMaterial = new Material(0xc0, 0xc0);
		public static readonly Material YellowMaterial = new Material(0xffff00, 0xffff00);
		public static readonly Material PurpleMaterial = new Material(0xffff, 0xffff);
		public static readonly Material MetalMaterial = new Material(0x101010, 0x101010, 0xffffff, 15.0d, 0.0d, null, null, null, 1.0d, 2.0d, 0.92d, null);
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
		public readonly FactorFromAngle ReflectanceGetter;
		
		public Material (uint ambient = 0xc0c0c0, uint diffuse = 0xc0c0c0, uint specular = 0xffffff, double shininess = 15.0d, double transparent = 0.0d, Texture texture = null, Texture reflection = null, Texture bump = null, double ni=1.0d, double nt = 1.553d, double reflectance = double.NaN, FactorFromAngle reflectanceGetter = null) {
			this.Ambient = ambient;
			this.Specular = specular;
			this.Diffuse = diffuse;
			this.NFactor = nt/ni;
			this.BrewesterAngle = Math.Atan(this.NFactor);
			this.Shininess = shininess;
			this.Texture = texture;
			this.Reflection = reflection;
			this.Bump = bump;
			this.FresnelR0 = (this.NFactor-1.0d)/(this.NFactor+1.0d);
			this.FresnelR0 *= this.FresnelR0;
			this.Reflectance = reflectance;
			this.ReflectanceGetter = reflectanceGetter;
			if(reflectanceGetter == null) {
				if(double.IsNaN(reflectance)) {
					ReflectanceGetter = FresnelLaw;
				}
				else {
					ReflectanceGetter = ReflectanceConstant;
				}
			}
		}

		public void ADSAt (Point3 tu, double cos, out uint ambient, out uint diffuse, out uint specular, out uint reflectance) {
			//ambient = specular = diffuse = Color.FromWavelength((int)(430*cos+350));
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
			if(this.Reflection != null) {
				tex = this.Reflection.ColorAt(tu);
				specular = Color.Multiply(specular, tex);
			}
			reflectance = Color.Scale(specular, this.ReflectanceGetter(cos));
			//reflectance = specular;
		}

		public double FresnelLaw (double cos) {
			return FresnelR0+(1.0d-FresnelR0)*Math.Pow(cos, 5.0d);
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

