using System;
using System.Runtime.CompilerServices;

namespace Renderer {
	
	public static class Color {
		
		public const uint AlphaChannel = 0xff000000;
		public const uint RedChannel = 0x00ff0000;
		public const uint GreenChannel = 0x0000ff00;
		public const uint BlueChannel = 0x000000ff;
		public const ulong RedChannel64 = 0xffff00000000;
		public const ulong GreenChannel64 = 0x0000ffff0000;
		public const ulong BlueChannel64 = 0x00000000ffff;
		public const uint RedGreenBlue = 0x00ffffff;
		public const uint Mask7Bit = 0x00fefeff;
		public const uint MaskOverflow = 0x01010100;
		public const uint White = 0xffffffff;
		public const uint Black = 0xff000000;
		public static double Gamma = 2.33d;
		
		public static uint loseIntensity (uint c, double distance) {
			return Scale(c, (uint)Math.Round(Math.Min(255000000.0d/(distance*distance+1.0d), 255.0d)));
		}
		public static uint Scale (uint c, double s) {
			return Scale(c, (uint)Math.Round(Maths.Border(0.0d, s*255.0d, 255.0d)));
		}
		public static uint Scale (uint color, uint factor) {
			uint r = ((((color>>0x10)&0xff)*factor)<<0x08)&RedChannel;
			uint g = (((color>>0x08)&0xff)*factor)&GreenChannel;
			uint b = ((color&0xff)*factor)>>0x08;
			return r|g|b;
		}
		public static uint FromFrac (double frac) {
			uint r = (uint)Maths.Border(0x00, (int)Math.Round(0xff*frac), 0xff);
			return (r<<0x10)|(r<<8)|r;
		}
		public static uint Mix (uint[] colors) {
			uint l = (uint)colors.Length;
			uint r = l>>0x01;
			uint g = r;
			uint b = r;
			foreach(uint col in colors) {
				r += (col>>0x10)&0xff;
				g += (col>>0x08)&0xff;
				b += col&0xff;
			}
			r /= l;
			g /= l;
			b /= l;
			return (r<<0x010)|(g<<0x08)|b;
		}
		public static uint Mix (uint r, uint g, uint b, uint n) {
			r /= n;
			g /= n;
			b /= n;
			return (r<<0x010)|(g<<0x08)|b;
		}
		public static void AddComponents (uint color, ref uint red, ref uint green, ref uint blue) {
			red += color>>0x10;
			green += (color>>0x08)&0xff;
			blue += color&0xff;
		}
		public static uint FromFrac (double fr, double fg, double fb) {
			uint r = (uint)Maths.Border(0x00, (int)Math.Round(0xff*fr), 0xff);
			uint g = (uint)Maths.Border(0x00, (int)Math.Round(0xff*fg), 0xff);
			uint b = (uint)Maths.Border(0x00, (int)Math.Round(0xff*fb), 0xff);
			return (r<<0x10)|(g<<8)|b;
		}
		public static ulong ToColor64 (uint color) {
			ulong cu = color;
			return ((cu&RedChannel)<<0x10)|((cu&GreenChannel)<<0x08)|(cu&BlueChannel);
		}
		public static uint MixFrom64 (ulong color, uint n) {
			uint red = (uint)(((color&RedChannel64)>>0x20)/n);
			uint green = (uint)(((color&GreenChannel64)>>0x10)/n);
			uint blue = (uint)((color&BlueChannel64)/n);
			return (red<<0x10)|(green<<0x08)|blue;
		}
		public static uint Random (uint color, uint delta) {
			int r = (int)(color>>16)&255;
			int g = (int)(color>>8)&255;
			int b = (int)color&255;
			r += (int)(Maths.Random()*(float)delta);
			g += (int)(Maths.Random()*(float)delta);
			b += (int)(Maths.Random()*(float)delta);
			return GetCropColor(r, g, b);
		}
		public static uint Add (uint color1, uint color2) {
			uint pixel = (color1&Mask7Bit)+(color2&Mask7Bit);
			uint overflow = pixel&0x01010100;
			overflow = overflow-(overflow>>0x08);
			return overflow|pixel;
		}
		public static uint GetAverage (uint color) {
			return (((color&RedChannel)>>0x10)+((color&GreenChannel)>>0x08)+(color&BlueChannel))/3;
		}
		public static uint GetBlue (uint color) {
			return color&BlueChannel;
		}
		public static uint GetColor (uint red, uint green, uint blue) {
			return (red<<0x10)|(green<<0x08)|blue;
		}
		public static uint GetCropColor (int red, int green, int blue) {
			return ((uint)Maths.Border(0x00, red, 0xff)<<0x10)|((uint)Maths.Border(0x00, green, 0xff)<<0x08)|(uint)Maths.Border(0x00, blue, 0xff);
		}
		public static uint GetCropColor (uint red, uint green, uint blue) {
			return (Maths.Border(0x00, red, 0xff)<<0x10)|(Maths.Border(0x00, green, 0xff)<<0x08)|Maths.Border(0x00, blue, 0xff);
		}
		public static uint GetGray (uint color) {
			uint r = (color&RedChannel)>>0x10;
			uint g = (color&GreenChannel)>>0x08;
			uint b = color&BlueChannel;
			uint t = (0x03*r+0x06*g+b)/0x0a;
			return (t<<0x10)|(t<<0x08)|t;
		}
		public static uint GetGreen (uint color) {
			return (color&GreenChannel)>>0x08;
		}
		public static uint GetRed (uint color) {
			return (color&RedChannel)>>0x10;
		}
		public static uint Inverse (uint color) {
			return ~color;
		}
		public static uint[] MakeGradient (int size, params uint[] colors) {
			int n = colors.Length;
			if(n == 0x00)
				return null;
			uint[] pal = new uint[size];
			uint c2 = colors[0x00];
			if(n == 0x01) {
				for(int i = 0; i < size; i++)
					pal[i] = c2;
			}
			else {
				uint c1, r, g, b, r1, g1, b1, r2, g2, b2;
				int dr, dg, db, pos1, pos2, range, i = 0;
				n--;
				for(int c = 0; c < n; c++) {
					c1 = c2;
					c2 = colors[c+0x01];
					pos1 = c*size/n;
					pos2 = (c+0x01)*size/n;
					range = pos2-pos1;
					r1 = GetRed(c1)<<0x10;
					g1 = GetGreen(c1)<<0x10;
					b1 = GetBlue(c1)<<0x10;
					r2 = GetRed(c2)<<0x10;
					g2 = GetGreen(c2)<<0x10;
					b2 = GetBlue(c2)<<0x10;
					dr = (int)(r2-r1)/range;
					dg = (int)(g2-g1)/range;
					db = (int)(b2-b1)/range;
					r = r1;
					g = g1;
					b = b1;
					for(; i < pos2; i++) {
						pal[i] = GetColor(r>>0x10, g>>0x10, b>>0x10);
						r = (uint)(r+dr);
						g = (uint)(g+dg);
						b = (uint)(b+db);
					}
				}
			}
			return pal;
		}
		public static uint Mix (uint color1, uint color2) {
			return (((color1&Mask7Bit)>>0x01)+((color2&Mask7Bit)>>0x01));
		}
		public static uint Multiply (uint color1, uint color2) {
			if((color1&RedGreenBlue) == 0)
				return 0;
			if((color2&RedGreenBlue) == 0)
				return 0;
			uint r = (((color1>>0x10)&0xff)*((color2>>0x10)&0xff))>>0x08;
			uint g = (((color1>>0x08)&0xff)*((color2>>0x08)&0xff))>>0x08;
			uint b = ((color1&0xff)*(color2&0xff))>>0x08;
			return (r<<0x10)|(g<<0x08)|b;
		}
		public static uint Sub (uint color1, uint color2) {
			uint pixel = (color1&Mask7Bit)+(~color2&Mask7Bit);
			uint overflow = ~pixel&0x01010100;
			overflow = overflow-(overflow>>0x08);
			return (~overflow&pixel);
		}
		public static uint SubNeg (uint color1, uint color2) {
			uint pixel = (color1&Mask7Bit)+(color2&Mask7Bit);
			uint overflow = ~pixel&0x01010100;
			overflow = overflow-(overflow>>0x08);
			return (~overflow&pixel);
		}
		public static uint Transparency (uint bkgrd, uint color, uint alpha) {
			if(alpha == 0x00)
				return color;
			if(alpha == 0xff)
				return bkgrd;
			if(alpha == 0x7f)
				return Mix(bkgrd, color);
			uint r = (alpha*(((bkgrd>>0x10)&0xff)-((color>>0x10)&0xff))>>0x08)+((color>>0x10)&0xff);
			uint g = (alpha*(((bkgrd>>0x08)&0xff)-((color>>0x08)&0xff))>>0x08)+((color>>0x08)&0xff);
			uint b = (alpha*((bkgrd&0xff)-(color&0xff))>>0x08)+(color&0xff);
			return (r<<0x10)|(g<<0x08)|b;
		}
		
	}
	
}