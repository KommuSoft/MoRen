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
	public static class Perlin {

		private static readonly int[] permutationResult = new int[512];
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
		private static readonly uint[] woodGradient = ColorUtils.MakeGradient(0x0400, 0x332211, 0x523121, 0x996633);
		private static readonly uint[] marbleGradient = ColorUtils.MakeGradient(0x0400, 0xff111111, 0xff696070, 0xffffffff);
		private static readonly uint[] skyGradient = ColorUtils.MakeGradient(0x0400, 0x003399, 0xffffff);

		public static double Perlin3d (double xf, double yf, double zf) {
			int xi = (int)Math.Floor(xf)&255;
			int yi = (int)Math.Floor(yf)&255;
			int zi = (int)Math.Floor(zf)&255;
			xf -= (int)Math.Floor(xf);
			yf -= (int)Math.Floor(yf);
			zf -= (int)Math.Floor(zf);
			double u = PerlinFade(xf);
			double v = PerlinFade(yf);
			double w = PerlinFade(zf);
			int L = permutationResult[xi]+yi, LL = permutationResult[L]+zi, LR = permutationResult[L+0x01]+zi,
			R = permutationResult[xi+0x01]+yi, RL = permutationResult[R]+zi, RR = permutationResult[R+0x01]+zi;

			return Maths.LinearInterpolate(w, Maths.LinearInterpolate(v, Maths.LinearInterpolate(u, PerlinGradient(permutationResult[LL], xf, yf, zf), PerlinGradient(permutationResult[RL], xf-0x01, yf, zf)), Maths.LinearInterpolate(u, PerlinGradient(permutationResult[LR], xf, yf-0x01, zf), PerlinGradient(permutationResult[RR], xf-0x01, yf-0x01, zf))),
                                                Maths.LinearInterpolate(v, Maths.LinearInterpolate(u, PerlinGradient(permutationResult[LL+0x01], xf, yf, zf-1), PerlinGradient(permutationResult[RL+0x01], xf-0x01, yf, zf-0x01)), Maths.LinearInterpolate(u, PerlinGradient(permutationResult[LR+0x01], xf, yf-1, zf-0x01), PerlinGradient(permutationResult[RR+0x01], xf-0x01, yf-0x01, zf-0x01))));
		}
		static double PerlinFade (double t) {
			return t*t*t*(t*(0x06*t-0x0f)+0x0a);
		}
		static double PerlinGradient (int hash, double x, double y, double z) {
			int h = hash&15;
			double u = h < 8 ? x : y,
			v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h&1) == 0 ? u : -u)+((h&2) == 0 ? v : -v);
		}

		public static Color Marble3 (Point3 xyz) {
			double sum = 0.0d;
			for(int i = 0x01; i < 0x20; i++) {
				sum += Math.Abs(Perlin3d(xyz.X*i, xyz.Y*i, xyz.Z*i))/i;
			}
			double g = Math.Sin(sum);
			return new Color((uint)Utils.FloatIndex(marbleGradient, g));
		}
		public static Color Sky3 (Point3 xyz) {
			double sum = 0.0d;
			for(int i = 0x01; i < 0x20; i++) {
				sum += Math.Abs(Perlin3d(xyz.X*i, xyz.Y*i, xyz.Z*i))/i;
			}
			return new Color((uint)Utils.FloatIndex(skyGradient, sum));
		}
		public static Color Wood3 (Point3 xyz) {
			double g = Perlin3d(xyz.X, xyz.Y, xyz.Z)*20;
			g = g-(int)Math.Floor(g);
			return new Color((uint)Utils.FloatIndex(woodGradient, g));
		}
		public static void InitializeNoiseBuffer () {
			for(int i=0; i < 256; i++)
				permutationResult[256+i] = permutationResult[i] = permutation[i];
		}

	}
}