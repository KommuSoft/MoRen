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
		public readonly Texture Texture;
		public readonly Texture Reflection;
		public readonly Texture Bump;
		public readonly Texture Envmap;
		
		public Material (uint ambient = 0xc0c0c0, uint specular = 0xc0c0c0, uint diffuse = 0xc0c0c0, double shininess = 15.0d, double transparent = 0.0d, Texture texture = null, Texture reflection = null, Texture bump = null, double nfactor=1.0d) {
			this.Ambient = ambient;
			this.Specular = specular;
			this.Diffuse = diffuse;
			this.NFactor = nfactor;
			this.Shininess = shininess;
			this.Texture = texture;
			this.Reflection = reflection;
			this.Bump = bump;
		}

		public void ADSAt (Point3 tu, out uint ambient, out uint diffuse, out uint specular) {
			//return PerlinCache.Sky3(tu);
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
				tex = this.Texture.ColorAt(tu);
				ambient = Color.Multiply(ambient, tex);
				diffuse = Color.Multiply(diffuse, tex);
				specular = Color.Multiply(specular, tex);
			}
		}
		
		private uint readInt (BinaryReader br) {
			uint bd = (uint)br.ReadByte()<<0x18;
			uint bc = (uint)br.ReadByte()<<0x10;
			uint bb = (uint)br.ReadByte()<<0x08;
			uint ba = (uint)br.ReadByte();
			return bd|bc|bb|ba;
		}

		private float readFloat (BinaryReader br) {
			byte[] data = new byte[4];
			data[3] = br.ReadByte();
			data[2] = br.ReadByte();
			data[1] = br.ReadByte();
			data[0] = br.ReadByte();
			return BitConverter.ToSingle(data, 0x00);
		}
		
	}
}
