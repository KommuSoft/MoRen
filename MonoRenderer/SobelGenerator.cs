using System;

namespace Renderer {
	
	public class SobelGenerator {
		
		public readonly Texture texture;
		
		public SobelGenerator (Texture source) {
			this.texture = apply (source);
		}
		
		public Texture apply (Texture source) {
			int w = source.Width;
			int h = source.Height;
			Texture t = new Texture(w,h);
			int scan0 = 1;
			int scan1 = w+1;
			int scan2 = w*2+1;
			int r1, r2, g1, g2, b1, b2;
			for(int y = 1; y < h-1; y++) {
				for(int x = 1; x < w-1; x++) {
					r1 = 2*(int) Color.GetRed(source.Pixel[scan1+1])+
						(int) Color.GetRed(source.Pixel[scan0+1])+
						(int) Color.GetRed(source.Pixel[scan2+1])-
						2*(int) Color.GetRed(source.Pixel[scan1+1])-
						(int) Color.GetRed(source.Pixel[scan0+1])-
						(int) Color.GetRed(source.Pixel[scan2+1]);
					g1 = 2*(int) Color.GetGreen(source.Pixel[scan1+1])+
						(int) Color.GetGreen(source.Pixel[scan0+1])+
						(int) Color.GetGreen(source.Pixel[scan2+1])-
						2*(int) Color.GetGreen(source.Pixel[scan1+1])-
						(int) Color.GetGreen(source.Pixel[scan0+1])-
						(int) Color.GetGreen(source.Pixel[scan2+1]);
					b1 = 2*(int) Color.GetBlue(source.Pixel[scan1+1])+
						(int) Color.GetBlue(source.Pixel[scan0+1])+
						(int) Color.GetBlue(source.Pixel[scan2+1])-
						2*(int) Color.GetBlue(source.Pixel[scan1+1])-
						(int) Color.GetBlue(source.Pixel[scan0+1])-
						(int) Color.GetBlue(source.Pixel[scan2+1]);
					r2 = 2*(int) Color.GetRed(source.Pixel[scan2])+
						(int) Color.GetRed(source.Pixel[scan2+1])+
						(int) Color.GetRed(source.Pixel[scan2-1])-
						2*(int) Color.GetRed(source.Pixel[scan0])-
						(int) Color.GetRed(source.Pixel[scan0+1])-
						(int) Color.GetRed(source.Pixel[scan0-1]);
					g2 = 2*(int) Color.GetGreen(source.Pixel[scan2])+
						(int) Color.GetGreen(source.Pixel[scan2+1])+
						(int) Color.GetGreen(source.Pixel[scan2-1])-
						2*(int) Color.GetGreen(source.Pixel[scan0])-
						(int) Color.GetGreen(source.Pixel[scan0+1])-
						(int) Color.GetGreen(source.Pixel[scan0-1]);
					b2 = 2*(int) Color.GetBlue(source.Pixel[scan2])+
						(int) Color.GetBlue(source.Pixel[scan2+1])+
						(int) Color.GetBlue(source.Pixel[scan2-1])-
						2*(int) Color.GetBlue(source.Pixel[scan0])-
						(int) Color.GetBlue(source.Pixel[scan0+1])-
						(int) Color.GetBlue(source.Pixel[scan0-1]);
					r1 = Math.Min(255,(int) Math.Sqrt(r1*r1+r2*r2));
					g1 = Math.Min(255,(int) Math.Sqrt(g1*g1+g2*g2));
					b1 = Math.Min(255,(int) Math.Sqrt(b1*b1+b2*b2));
					r1 = (r1+g1+b1+2)/3;
					t.Pixel[scan1] = Color.GetColor((uint) r1, (uint) r1, (uint) r1);
					scan0++;
					scan1++;
					scan2++;
				}
				scan0 += 2;
				scan1 += 2;
				scan2 += 2;
			}
			return t;
		}
		
	}
}

