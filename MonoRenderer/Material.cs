using System;
using System.IO;

namespace Renderer {
	
	public delegate void MaterialGetter (Point3 tu,out uint ambient,out uint diffuse,out uint specular,out uint reflectance);

	public sealed class Material {

		public static readonly Material DefaultMaterial = new Material();
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
		
		public Material (uint ambient = 0xc0c0c0, uint specular = 0xc0c0c0, uint diffuse = 0xc0c0c0, double shininess = 15.0d, double transparent = 0.0d, Texture texture = null, Texture reflection = null, Texture bump = null, double ni=1.0d, double nt = 1.0d) {
			this.Ambient = ambient;
			this.Specular = specular;
			this.Diffuse = diffuse;
			double nfactor = nt/ni;
			this.NFactor = nfactor;
			this.Shininess = shininess;
			this.Texture = texture;
			this.Reflection = reflection;
			this.Bump = bump;
			this.FresnelR0 = (nfactor-1.0d)/(nfactor+1.0d);
			this.FresnelR0 *= this.FresnelR0;
			/*if(this.Texture == null && this.Reflection == null) {
				this.ADSAt = ADSAt00;
			}*/
		}

		public void ADSAt (Point3 tu, double cos, out uint ambient, out uint diffuse, out uint specular, out uint reflectance) {
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
			reflectance = specular;
			reflectance = Color.Scale(reflectance, FresnelR0+(1.0d-FresnelR0)*Math.Pow(cos, 5.0d));
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

