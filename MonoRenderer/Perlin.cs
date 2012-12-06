//
//  Perlin.cs
//
//  Author:
//       Willem Van Onsem <vanonsem.willem@gmail.com>
//
//  Copyright (c) 2012 Willem Van Onsem
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace Renderer {
	public static class PerlinCache
	{

		private static readonly int[] p = new int[512];
		private static readonly int[] permutation = { 151,160,137,91,90,15,
   131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
   190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
   88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
   77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
   102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
   135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
   5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
   223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
   129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
   251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
   49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
   138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
   };
		private static readonly uint[] woodGradient = Color.MakeGradient(0x0400, 0x332211, 0x523121, 0x996633);
		private static readonly uint[] marbleGradient = Color.MakeGradient(0x0400, 0xff111111, 0xff696070, 0xffffffff);
		private static readonly uint[] skyGradient = Color.MakeGradient(0x0400, 0x003399, 0xffffff);

		/*private static readonly float[] noiseBuffer = new float[0x08000];

		private static float interpolatedNoise2 (float x, float y, int octave) {
			octave &= 0x03;
			int intx = (int)x;
			int inty = (int)y;
			float fracx = x-(float)intx;
			float fracy = y-(float)inty;
			float i1 = Maths.Interpolate(noise(intx, inty, octave), noise(intx+1, inty++, octave), fracx);
			float i2 = Maths.Interpolate(noise(intx, inty, octave), noise(intx+1, inty, octave), fracx);
			return Maths.Interpolate(i1, i2, fracy);
		}
		private static float interpolatedNoise3 (double x, double y, double z, int octave) {
			octave &= 0x03;
			int intx = (int)x;
			int inty = (int)y;
			int intz = (int)z;
			double fracx = x-(float)intx;
			double fracy = y-(float)inty;
			double fracz = y-(float)intz;
			float i1, i2, iz;
			i1 = Maths.Interpolate(noise(intx, inty, intz, octave), noise(intx+1, inty+1, intz, octave), fracx);
			i2 = Maths.Interpolate(noise(intx, inty+1, intz, octave), noise(intx+1, inty+1, intz, octave), fracx);
			iz = Maths.Interpolate(i1, i2, fracy);
			i1 = Maths.Interpolate(noise(intx, inty, intz+1, octave), noise(intx+1, inty+1, intz+1, octave), fracx);
			i2 = Maths.Interpolate(noise(intx, inty+1, intz+1, octave), noise(intx+1, inty+1, intz+1, octave), fracx);
			return Maths.Interpolate(iz, Maths.Interpolate(i1, i2, fracy), fracz);
		}
		private static float noise (int x, int y, int octave) {
			return noiseBuffer [(((x+y*0x57)&0x01fff)<<0x02)|octave];
		}
		private static float noise (int x, int y, int z, int octave) {
			return noiseBuffer [(((x+y*0x57+0x1d91*z)&0x01fff)<<0x02)|octave];
		}
		private static float noise (int seed) {
			int id = seed&0x03;
			seed &= 0x7ffc;
			int n = (seed<<0x0b)^(seed>>0x02);
			if(id == 0)
				return (float)(1.0f-((n*(n*n*0x3d73+0x0c0ae5)+0x5208dd0d)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			if(id == 1)
				return (float)(1.0f-((n*(n*n*0x30d1+0x093a37)+0x50356ebf)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			if(id == 2)
				return (float)(1.0f-((n*(n*n*0x4a8f+0x0a0e67)+0x5035710b)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			return (float)(1.0f-((n*(n*n*0x3f8b+0x0a990d)+0x5035708d)&0x7FFFFFFF)*0.000000000931322574615478515625f);
		}
		public static Texture Sky (int width, int height, float density) {
			return Perlin(width, height, 0.5f, 2.8f*density, 0x08, 0x0400).Colorize(Color.MakeGradient(0x0400, 0x003399, 0xffffff));
		}
		private static float Perlin2d (float x, float y, float wavelength, float persistence, int samples) {
			float sum = 0.0f;
			float xfreq = 1.0f/wavelength;
			float yfreq = y*xfreq;
			xfreq = x*xfreq;
			float amp = persistence;
			for(int i = 0x00; i < samples; i++) {
				sum += amp*interpolatedNoise2(xfreq, yfreq, i);
				amp *= persistence;
				xfreq *= 2.0f;
				yfreq *= 2.0f;
			}
			return Maths.Border(0.0f, sum/persistence*0.5f+0.5f, 1.0f);
		}
		private static float Perlin3d (float x, float y, float z, float wavelength, float persistence, int samples) {
			float sum = 0.0f;
			float xfreq = 1.0f/wavelength;
			float yfreq = y*xfreq;
			float zfreq = z*xfreq;
			xfreq = x*xfreq;
			float amp = persistence;
			for(int i = 0x00; i < samples; i++) {
				sum += amp*interpolatedNoise3(xfreq, yfreq, zfreq, i);
				amp *= persistence;
				xfreq *= 2.0f;
				yfreq *= 2.0f;
			}
			return Maths.Border(0.0f, sum/persistence*0.5f+0.5f, 1.0f);
		}
		public static Texture Marble (int width, int height, float density) {
			return Wave(width, height, 0.5f, 0.64f*density, 0x08, 0x0400).Colorize(Color.MakeGradient(0x00000400, 0xff111111, 0xff696070, 0xffffffff));
		}
		public static Texture Perlin (int width, int height, float persistency, float density, int samples, int scale) {
			Texture t = new Texture(width, height);
			int pos = 0x00;
			float wavelength = (float)System.Math.Max(width, height)/density;
			float sc = (float)scale;
			int x;
			for(int y = 0x00; y < height; y++) {
				for(x = 0x00; x < width; x++) {
					t.Pixel [pos++] = (uint)(sc*Perlin2d(x, y, wavelength, persistency, samples));
				}
			}
			return t;
		}
		public static Texture Wave (int width, int height, float persistency, float density, int samples, int scale) {
			Texture t = new Texture(width, height);
			int pos = 0x00;
			float wavelength = (float)System.Math.Max(width, height)/density;
			double sc = (double)scale;
			int x;
			for(int y = 0x00; y < height; y++) {
				for(x = 0x00; x < width; x++) {
					t.Pixel [pos++] = (uint)(sc*wave2dFuncion(x, y, wavelength, persistency, samples));
				}
			}
			return t;
		}
		private static double wave2dFuncion (int x, int y, float wavelength, float persistency, int samples) {
			return (0.5d*System.Math.Sin(Perlin2d(x, y, wavelength, persistency, samples))+0.5d);
		}
		private static double wave3dFuncion (int x, int y, int z, float wavelength, float persistency, int samples) {
			return (0.5d*System.Math.Sin(Perlin3d(x, y, z, wavelength, persistency, samples))+0.5d);
		}
		public static Texture Wood (int width, int height, float density) {
			return Grain(width, height, 0.5f, 3.0f*density, 0x03, 0x08, 0x0400).Colorize(Color.MakeGradient(0x0400, 0x332211, 0x523121, 0x996633));
		}
		public static Texture Grain (int width, int height, float persistency, float density, int samples, int levels, int scale) {
			Texture t = new Texture(width, height);
			int pos = 0x00;
			float wavelength = (float)System.Math.Max(width, height)/density;
			float perlin;
			float sc = (float)scale;
			float le = (float)levels;
			int x;
			for(int y = 0x00; y < height; y++) {
				for(x = 0x00; x < width; x++) {
					perlin = le*Perlin2d(x, y, wavelength, persistency, samples);
					t.Pixel [pos++] = (uint)(sc*(perlin-(float)(int)perlin));
				}
			}
			return t;
		}*/
		public static double perlin (double xf, double yf, double zf) {
			int xi = (int)Math.Floor(xf)&255;
			int yi = (int)Math.Floor(yf)&255;
			int zi = (int)Math.Floor(zf)&255;
			xf -= (int)Math.Floor(xf);
			yf -= (int)Math.Floor(yf);
			zf -= (int)Math.Floor(zf);
			double u = fade(xf);
			double v = fade(yf);
			double w = fade(zf);
			int A = p [xi]+yi, AA = p [A]+zi, AB = p [A+1]+zi,
			B = p [xi+1]+yi, BA = p [B]+zi, BB = p [B+1]+zi;

			return lerp(w, lerp(v, lerp(u, grad(p [AA], xf, yf, zf),
                                     grad(p [BA], xf-1, yf, zf)),
                             lerp(u, grad(p [AB], xf, yf-1, zf),
                                     grad(p [BB], xf-1, yf-1, zf))
			),
                     lerp(v, lerp(u, grad(p [AA+1], xf, yf, zf-1),
                                     grad(p [BA+1], xf-1, yf, zf-1)),
                             lerp(u, grad(p [AB+1], xf, yf-1, zf-1),
                                     grad(p [BB+1], xf-1, yf-1, zf-1))
			)
			);
		}
		static double fade (double t) {
			return t*t*t*(t*(t*6-15)+10);
		}
		static double lerp (double t, double a, double b) {
			return a+t*(b-a);
		}
		static double grad (int hash, double x, double y, double z) {
			int h = hash&15;                      // CONVERT LO 4 BITS OF HASH CODE
			double u = h < 8 ? x : y, // INTO 12 GRADIENT DIRECTIONS.
			v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h&1) == 0 ? u : -u)+((h&2) == 0 ? v : -v);
		}

		/*public static uint Marble3 (Point3 xyz) {
			//uint rgb = Maths.Border(0x00, (uint)Math.Round(0xff*Math.Cos(xyz.X+noise(50.0d*xyz.X, 50.0d*xyz.Y, 50.0d*xyz.Z))), 0xff);
			//Console.WriteLine(noise(50.0d*xyz.X, 50.0d*xyz.Y, 50.0d*xyz.Z));
			uint rgb = Maths.Border(0x00, (uint)Math.Round(0xff*(Math.Cos(25.0d+xyz.X*perlin(25.0d*xyz.X, 25.0d*xyz.Y, 25.0d*xyz.Z))*0.75d)), 0xff);
			//uint rgb = Maths.Border(0x00, (uint)Math.Round(0xff*wave3dFuncion((int)(100*xyz.X), (int)(100*xyz.Y), (int)(512*xyz.Z), 0.5f, 0.064f, 0x08)), 0xff);
			return Color.GetColor(rgb, rgb, rgb);
		}*/
		/*public static uint Marble3 (Point3 xyz) {
			double g = Math.Cos(xyz.X+perlin(xyz.X, xyz.Y, xyz.Z));
			return Color.FromFrac(g);
		}*/
		public static uint Marble3 (Point3 xyz) {
			double sum = 0.0d;
			for(int i = 0x01; i < 0x20; i++) {
				sum += Math.Abs(perlin(xyz.X*i, xyz.Y*i, xyz.Z*i))/i;
			}
			double g = Math.Sin(sum);
			return Utils.FloatIndex(marbleGradient, g);
			//return Color.FromFrac(g);
		}
		public static uint Sky3 (Point3 xyz) {
			double sum = 0.0d;
			for(int i = 0x01; i < 0x20; i++) {
				sum += Math.Abs(perlin(xyz.X*i, xyz.Y*i, xyz.Z*i))/i;
			}
			return Utils.FloatIndex(skyGradient, sum);
			//return Color.FromFrac(g);
		}
		public static uint Wood3 (Point3 xyz) {
			double g = perlin(xyz.X, xyz.Y, xyz.Z)*20;
			g = g-(int)Math.Floor(g);
			//return Color.FromFrac(0.26*g+0.67, 0.27*g+0.33, 0.13);
			return Utils.FloatIndex(woodGradient, g);
		}
		public static void InitializeNoiseBuffer () {
			for(int i=0; i < 256; i++)
				p [256+i] = p [i] = permutation [i];
			/*for(int i = 0; i < 0x7fff;)
				noiseBuffer [i++] = noise(i);*/
		}

	}
}

