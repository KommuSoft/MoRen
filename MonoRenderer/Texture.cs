using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Renderer {
	
	public sealed class Texture {
		
		public int BitHeight;
		public int BitWidth;
		public int Height;
		public int Width;
		public double XStep;
		public double YStep;
		public uint[] Pixel;
		
		public Texture (int w, int h) {
			this.Height = h;
			this.Width = w;
			this.XStep = 1.0d/Width;
			this.YStep = 1.0d/Height;
			this.Pixel = new uint[w*h];
		}
		public Texture (int w, int h, uint[] data) {
			this.Height = h;
			this.Width = w;
			this.XStep = 1.0d/Width;
			this.YStep = 1.0d/Height;
			int n = w*h;
			int nn = System.Math.Min(n, data.Length);
			this.Pixel = new uint[n];
			for(int i = 0x00; i < nn; i++) {
				this.Pixel[i] = data[i];
			}
		}
		public unsafe Texture (string filename) : this(new Bitmap(filename)) {
		}
		public unsafe Texture (Bitmap bmp) {
			this.Height = bmp.Height;
			this.Width = bmp.Width;
			this.XStep = 1.0d/Width;
			this.YStep = 1.0d/Height;
			this.Pixel = new uint[this.Width*this.Height];
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00, 0x00, this.Width, this.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
			byte* pix = (byte*)bmd.Scan0;
			uint* pixx;
			int j = 0;
			for(int y = 0x00; y < this.Height; y++) {
				pixx = (uint*)pix;
				for(int x = 0x00; x < this.Width; x++) {
					this.Pixel[j++] = *pixx;
					pixx++;
				}
				pix += bmd.Stride;
			}
			bmp.UnlockBits(bmd);
		}
		
		public unsafe Bitmap ToBitmap () {
			Bitmap bmp = new Bitmap(this.Width, this.Height);
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00, 0x00, this.Width, this.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
			byte* pix = (byte*)bmd.Scan0;
			uint* pixx;
			int j = 0;
			for(int y = 0x00; y < this.Height; y++) {
				pixx = (uint*)pix;
				for(int x = 0x00; x < this.Width; x++) {
					*pixx = this.Pixel[j++];
					pixx++;
				}
				pix += bmd.Stride;
			}
			bmp.UnlockBits(bmd);
			return bmp;
		}
		public unsafe void Save (string name) {
			this.ToBitmap().Save(name);
		}
		private int Coordinate (double tux, double tuy) {
			double x = tux;
			double y = tuy;
			//double x = Math.Abs(tux%1.0d);
			//double y = 1.0d-Math.Abs(tuy%1.0d);
			//Console.WriteLine("fetch {0} {1}", tux, tuy);
			//Console.WriteLine("also known as {0} {1}", Math.Floor(x*Width), Math.Floor(y*Height));
			return (int)(Math.Floor(x*Width)+Math.Floor(y*Height)*Width);
		}
		public uint ColorAt (Point3 tu) {
			//normalize
			//double x = ((tu.X%1.0d)+1.0d)%1.0d;
			//double y = ((tu.Y%1.0d)+1.0d)%1.0d;
			return Pixel[Coordinate(tu.X, tu.Y)];
		}
		public void TweakNormal (Point3 tu, Point3 normal, Point3 bumpx, Point3 bumpy) {
			double xdiff = Pixel[Coordinate(tu.X+XStep, tu.Y)]&Color.ColorChannel-Pixel[Coordinate(tu.X-XStep, tu.Y)]&Color.ColorChannel;
			double ydiff = Pixel[Coordinate(tu.X, tu.Y+YStep)]&Color.ColorChannel-Pixel[Coordinate(tu.X, tu.Y-YStep)]&Color.ColorChannel;
			//Console.WriteLine("XDiff {0} YDiff {1}", xdiff, ydiff);
			//Console.WriteLine(Maths.ColorChannelInvSqrt_2);
			normal.Mix3Normalize(bumpx, bumpy, xdiff*Maths.ColorChannelInvSqrt_2, ydiff*Maths.ColorChannelInvSqrt_2);
		}
		private void setSize (int width, int height) {
			int offset = width*height;
			int offset2;
			if(offset != 0x00) {
				uint[] newPixel = new uint[offset--];
				for(int j = height-0x01; j >= 0x00; j--) {
					offset2 = (j*this.Height/height)*this.Width;
					for(int i = width-0x01; i >= 0x00; i--)
						newPixel[offset--] = this.Pixel[i*this.Width/width+offset2];
				}
				this.Width = width;
				this.Height = height;
				this.Pixel = newPixel;
			}
		}
		
		public Texture Add (Texture additive) {
			uint[] data = additive.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.Add(this.Pixel[i], data[i]);
			return this;
		}
		public void Add (Texture texture, int posx, int posy, int xsize, int ysize) {
			int w = xsize;
			int h = ysize;
			int xBase = posx;
			int yBase = posy;
			int tx = texture.Width*255;
			int ty = texture.Height*255;
			int tw = texture.Width;
			int dtx = tx/w;
			int dty = ty/h;
			int txBase = Maths.Border(0, -xBase*dtx, 255*tx);
			int tyBase = Maths.Border(0, -yBase*dty, 255*ty);
			int xend = Maths.Border(0, xBase+w, Width);
			int yend = Maths.Border(0, yBase+h, Height);
			int offset1, offset2;
			xBase = Maths.Border(0, xBase, Width);
			yBase = Maths.Border(0, yBase, Height);
			ty = tyBase;
			for(int j = yBase; j < yend; j++) {
				tx = txBase;
				offset1 = j*Width;
				offset2 = (ty>>8)*tw;
				for(int i = xBase; i < xend; i++) {
					Pixel[i+offset1] = Color.Add(texture.Pixel[(tx>>8)+offset2], Pixel[i+offset1]);
					tx += dtx;
				}
				ty += dty;
			}
		}
		public static Texture BlendTopDown (Texture top, Texture down) {
			int h = top.Height;
			int w = top.Width;
			down.Resize(w, h);
			Texture t = new Texture(w, h);
			int pos = 0x00;
			int x;
			uint alpha;
			for(int y = 0x00; y < h; y++) {
				alpha = (uint)(0xff*y/(top.Height-0x01));
				for(x = 0x00; x < top.Width; x++) {
					t.Pixel[pos] = Renderer.Color.Transparency(down.Pixel[pos], top.Pixel[pos], alpha);
					pos++;
				}
			}
			return t;
		}
		public static Texture CheckerBoard (int width, int height, int cellbits, uint oddColor, uint evenColor) {
			Texture t = new Texture(width, height);
			int pos = 0x00;
			bool offset = false;
			int x;
			for(int y = 0x00; y < height; y++) {
				offset = ((y>>cellbits)&0x01) == 0x00;
				for(x = 0x00; x < width; x++)
					t.Pixel[pos++] = (((offset^((x>>cellbits)&0x01) == 0x01)) ? evenColor : oddColor);
			}
			return t;
		}
		public Texture Clear () {
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = 0x00;
			return this;
		}
		public Texture Clone () {
			Texture t = new Texture(this.Width, this.Height);
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++)
				t.Pixel[i] = this.Pixel[i];
			return t;
		}
		public Texture Colorize (params uint[] palette) {
			int range = palette.Length-0x01;
			int n = this.Pixel.Length;
			for(int i = 0; i < n; i++)
				this.Pixel[i] = palette[Maths.Border(0x00, this.Pixel[i], range)];
			return this;
		}
		public Texture Invert () {
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.Inverse(this.Pixel[i]);
			return this;
		}
		public Texture Mix (Texture newData) {
			uint[] data = newData.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.Mix(this.Pixel[i], data[i]);
			return this;
		}
		public Texture Multiply (Texture multiplicative) {
			uint[] data = multiplicative.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.Multiply(this.Pixel[i], data[i]);
			return this;
		}
		public Texture Put (Texture newData) {
			uint[] data = newData.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = data[i];
			return this;
		}
		public void Resize () {
			double log2inv = 1.0f/System.Math.Log(2.0f);
			int w = (int)System.Math.Pow(2.0f, this.BitWidth = (int)(System.Math.Log(this.Width)*log2inv));
			int h = (int)System.Math.Pow(2.0f, this.BitHeight = (int)(System.Math.Log(this.Height)*log2inv));
			this.Resize(w, h);
		}
		public void Resize (int w, int h) {
			this.setSize(w, h);
		}
		public Texture Sub (Texture subtractive) {
			uint[] data = subtractive.Pixel;
			int n = System.Math.Min(this.Pixel.Length, data.Length);
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.Sub(this.Pixel[i], data[i]);
			return this;
		}
		public Texture ToAverage () {
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.GetAverage(this.Pixel[i]);
			return this;
		}
		public Texture ToGray () {
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++)
				this.Pixel[i] = Renderer.Color.GetGray(this.Pixel[i]);
			return this;
		}
		public Texture ValueToGray () {
			uint intensity;
			int n = this.Pixel.Length;
			for(int i = 0x00; i < n; i++) {
				intensity = this.Pixel[i]&0xff;
				this.Pixel[i] = Renderer.Color.GetColor(intensity, intensity, intensity);
			}
			return this;
		}
		public static implicit operator ColorAtMethod (Texture tex) {
			return tex.ColorAt;
		}
		
	}
	
}