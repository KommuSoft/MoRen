using System;
using System.IO;

namespace Renderer {
	
	public sealed class Material
	{
		
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

		/*public Material (string filename) {
			FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
			BinaryReader sr = new BinaryReader(fs);
			this.Ambient = readInt(sr);
			this.Diffuse = this.Ambient;
			this.Specular = this.Ambient;
			sr.ReadByte();
			this.Specular = Color.Scale(this.Specular, sr.ReadByte());
			sr.ReadBoolean();
			this.NFactor = 1.0d;
			this.Shininess = 15.0d;
			this.Texture = ReadTexture(sr);
			this.Envmap = ReadTexture(sr);
			sr.Close();
			fs.Close();
		}*/
		
		private uint readInt (BinaryReader br) {
			uint bd = (uint)br.ReadByte()<<0x18;
			uint bc = (uint)br.ReadByte()<<0x10;
			uint bb = (uint)br.ReadByte()<<0x08;
			uint ba = (uint)br.ReadByte();
			return bd|bc|bb|ba;
		}

		private float readFloat (BinaryReader br) {
			byte[] data = new byte[4];
			data [3] = br.ReadByte();
			data [2] = br.ReadByte();
			data [1] = br.ReadByte();
			data [0] = br.ReadByte();
			return BitConverter.ToSingle(data, 0x00);
		}

		/*public Texture ReadTexture (BinaryReader sr) {
			byte id = sr.ReadByte();
			if(id == 0x01) {
				return new Texture(sr.ReadString());
			} else if(id == 0x02) {
				int w = (int)readInt(sr);
				int h = (int)readInt(sr);
				byte type = sr.ReadByte();
				float persistency = readFloat(sr);
				float density = readFloat(sr);
				int samples = sr.ReadByte();
				int numColors = sr.ReadByte();
				uint[] colors = new uint[numColors];
				for(int i = 0; i < numColors; i++) {
					colors [i] = readInt(sr);
				}
				uint[] gradient = Color.MakeGradient(1024, colors);
				switch(type) {
					case 0x01:
						return Texture.Perlin(w, h, persistency, density, samples, 1024).Colorize(gradient);
					case 0x02:
						return Texture.Wave(w, h, persistency, density, samples, 1024).Colorize(gradient);
					case 0x03:
						return Texture.Grain(w, h, persistency, density, samples, 20, 1024).Colorize(gradient);
					default :
						return null;
				}
			} else {
				return null;
			}
		}*/
		
	}
}

