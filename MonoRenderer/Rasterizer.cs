using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace Renderer {
	
	public class Rasterizer {
		
		public readonly uint[][] Raster;
		public readonly double[][] ZBuffer;
		public readonly int W;
		public readonly int H;
		
		public Rasterizer (int w, int h, double zFar) {
			this.Raster = new uint[h][];
			this.ZBuffer = new double[h][];
			for(int y = 0; y < h; y++) {
				this.Raster[y] = new uint[w];
			}
			for(int y = 0; y < h; y++) {
				this.ZBuffer[y] = new double[w];
			}
			this.W = w;
			this.H = h;
			this.ResetDepth(zFar);
		}
		
		public void ResetDepth (double zFar) {
			for(int y = 0; y < H; y++) {
				double[] l = ZBuffer[y];
				for(int x = 0; x < W; x++) {
					l[x] = zFar;
				}
			}
		}
		public unsafe void FillTriangle (double x1, double y1, double x2, double y2, double x3, double y3, uint color) {
			//Console.WriteLine("{0}/{1}/{2}/{3}/{4}/{5}",Math.Round(x1),Math.Round(y1),Math.Round(x2),Math.Round(y2),Math.Round(x3),Math.Round(y3));
			int minx = Math.Max(0x00,(int) Math.Min(x1,Math.Min(x2,x3)));
			int maxx = Math.Min(W-0x01,(int) Math.Ceiling(Math.Max(x1,Math.Max(x2,x3))));
			int miny = Math.Max(0x00,(int) Math.Min(y1,Math.Min(y2,y3)));
			int maxy = Math.Min(H-0x01,(int) Math.Ceiling(Math.Max(y1,Math.Max(y2,y3))));
			double b = x3-x1, a = x2-x1;
			double d = y3-y1, c = y2-y1;
			double den = b*c-a*d;
			double sgn = Math.Sign(den);
			den = Math.Abs(den);
			a *= sgn; b *= sgn;
			c *= sgn; d *= sgn;
			//ParallelOptions po = new ParallelOptions();
			//po.MaxDegreeOfParallelism = 2;
			//int np = 2;
			//Parallel.For(miny, maxy+1, po, y => {
			/*Parallel.For(0,np, p => {
				int frm = miny+p*(maxy-miny)/np;
				int to = miny+(p+1)*(maxy-miny)/np;
				for(int y = frm; y <= to; y++) {//*/
				for(int y = miny; y <= maxy; y++) {
					double dy = y-y1;
					double dx = minx-x1;
					double nomb = b*dy-d*dx;
					double nomg = c*dx-a*dy;
					//double x1a, x2a, x3a, x1b, x2b, x3b;
					//fixed(uint* l0 = &Raster[y][0]) {
					uint[] line = Raster[y];
					//Utils.LinEq(-d,nomb,0.0d,den,out x1a,out x1b); Utils.Order(ref x1a,ref x1b);
					//Utils.LinEq(c,nomg,0.0d,den,out x2a,out x2b); Utils.Order(ref x2a,ref x2b);
					//Utils.LinEq(c-d,nomb+nomg,0.0d,den,out x3a,out x3b); Utils.Order(ref x3a,ref x3b);
					//int x = Math.Max((int) Math.Ceiling(Math.Max(x1a,Math.Max(x2a,x3a))),minx);
					//x1b = Math.Min((int) Math.Floor(Math.Min(x1b,Math.Min(x2b,x3b))),maxx);
					//for(; x <= x1b; x++) {
					int x = minx;
					if(d < 0.0d) {
						x = Math.Max(x,(int) Math.Ceiling(minx+nomb/d));
					}
					if(c > 0.0d) {
						x = Math.Max(x,(int) Math.Ceiling(minx-nomg/c));
					}
					if(c-d < 0.0d) {
						x = Math.Max(x,(int) Math.Ceiling(minx+(den-nomg-nomb)/(c+d)));
					}
					dx = x-minx;
					nomb -= d*dx;
					nomg += c*dx;
					for(; x <= maxx && (nomb < 0.0d || nomg < 0.0d || den < nomg+nomb); x++) {
						nomb -= d;
						nomg += c;
					}
					for(; x <= maxx && nomb >= 0.0d && nomg >= 0.0d && den >= nomg+nomb; x++) {
						line[x] = color;
						nomb -= d;
						nomg += c;
					}
					//}
				}
			//});
		}
		
		public unsafe void SaveBitmap (string filename) {
			Bitmap bmp = new Bitmap(this.W,this.H);
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,this.W,this.H),ImageLockMode.WriteOnly,PixelFormat.Format32bppPArgb);
			byte* pix = (byte*) bmd.Scan0;
			uint* pixx;
			for(int y = 0x00; y < this.H; y++) {
				pixx = (uint*) pix;
				for(int x = 0x00; x < this.W; x++) {
					*pixx = Raster[y][x];
					pixx++;
				}
				pix += bmd.Stride;
			}
			bmp.UnlockBits(bmd);
			bmp.Save(filename);
		}
		
		/*public static int Main (string[] args) {
			Rasterizer ra = new Rasterizer(1000,1000,1000);
			Random rnd = new Random();
			DateTime start = DateTime.Now;
			//double r = 25.0d;//ART
			double r = 100.0d;
			for(int i = 0; i < 10000000; i++) {
				double x0, y0;
				ulong co;
				uint c = 0x00;*/
				/*mondriaan
				double x0 = Math.Sqrt((i%1000000)&0xaaaaaa);
				double y0 = Math.Sqrt((i%1000000)&0x555555);
				double x1 = x0+2.0d*r*rnd.NextDouble()-r;
				double y1 = y0+2.0d*r*rnd.NextDouble()-r;
				double x2 = x0+2.0d*r*rnd.NextDouble()-r;
				double y2 = y0+2.0d*r*rnd.NextDouble()-r;
				ulong co = ((ulong) i*0xffffff);
				uint c = 0xff000000|(uint) (co/10000000);*/
				/* herfst
				double x0 = (i&0xaaaaaa)%1000;
				double y0 = (i&0x555555)%1000;
				ulong co = ((ulong) i*0xffffff);
				uint c = 0xff000000|(uint) (co/10000000);
				*/
				/*gear inception
				x0 = 500.0d+i/14100.0d*Math.Cos(i/30000.0d);
				y0 = 500.0d+i/14100.0d*Math.Sin(i/30000.0d);
				co = (ulong) ((i&0xff<<0x04)|(i&0xf00>>0x08)|(i&0xf000));
				c = 0xff000000|(uint) co;*/
				/*rainbowdash eye
				x0 = 500.0d+i/14100.0d*Math.Cos(i/30000.0d);
				y0 = 500.0d+i/14100.0d*Math.Sin(i/20000.0d);
				int ci = (int) Math.Round(i/(100000*Math.PI)+3*x0/1000+3*y0/1000)%6;
				switch(ci) {
					case 0x00:
						c = 0xee4144;
						break;
					case 0x01:
						c = 0xf37033;
						break;
					case 0x02:
						c = 0xfdf6af;
						break;
					case 0x03:
						c = 0x62bc4d;
						break;
					case 0x04:
						c = 0x1e98d3;
						break;
					case 0x05:
						c = 0x672f89;
						break;
				}
				c -= (uint) (rnd.Next()&0x0f0f0f);*/
				/*x0 = 1000.0d*rnd.NextDouble();
				y0 = 1000.0d*rnd.NextDouble();
				c = 0xff000000|(uint) rnd.Next();
				ra.FillTriangle(x0,y0,x0+2.0d*r*rnd.NextDouble()-r,y0+2.0d*r*rnd.NextDouble()-r,x0+2.0d*r*rnd.NextDouble()-r,y0+2.0d*r*rnd.NextDouble()-r,c);
			}
			DateTime stop = DateTime.Now;
			Console.WriteLine("Rendered 10 000 000 triangles in {0}",stop-start);
			ra.SaveBitmap("result.png");
			return 0x00;
		}*/
		
	}
	
}