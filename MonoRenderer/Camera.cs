//  
//  Camera.cs
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

#undef FAST_COLOR_INTERSECTION
#undef FAST_COLOR_MIGRATION

using System;
using System.Collections.Generic;
using System.Drawing;
using Renderer.SceneBuilding;

namespace Renderer {

	public sealed class Camera : NameBase {

		private readonly Point3 position = new Point3(0.0d, 0.0d, 0.0d);
		private readonly Point3 lookAt = new Point3(0.0d, 0.0d, 25.0d);
		private readonly Matrix4 matrix = new Matrix4();
		private readonly IAccelerator acc;
		private readonly Texture raster = new Texture(1, 1);
		private double screenDistance = 1.5d;
		private bool dirty = true;
		private double displacement = 0.0d;
		private double roll = 0.0d;
		private double foVH = 0.5d*Math.PI;
		public readonly Light[] Lights;
		public readonly List<CameraPostProcessor> postProcessors;
		private readonly uint antialiasSqrt = 0x01;
		private readonly EnvironmentSettings settings;
		private readonly double dispersion = 0.0d;
		private readonly uint dispersionAntialiasSqrt = 0x01;

		public Point3 Position {
			get {
				return this.position;
			}
			set {
				this.position.SetValues(value);
				this.dirty = true;
			}
		}

		public Point3 LookAt {
			get {
				return this.lookAt;
			}
			set {
				this.lookAt.SetValues(value);
				this.dirty = true;
			}
		}
		public double Displacement {
			get {
				return this.displacement;
			}
			set {
				this.displacement = value;
			}
		}
		public double Roll {
			get {
				return this.roll;
			}
			set {
				this.roll = value;
				this.dirty = true;
			}
		}

		public int Width {
			get {
				return this.raster.Width;
			}
			set {
				this.raster.Resize(value, this.raster.Height);
			}
		}

		public int Height {
			get {
				return this.raster.Height;
			}
			set {
				this.raster.Resize(this.raster.Width, value);
			}
		}

		public double ScreenDistance {
			get {
				return this.screenDistance;
			}
			set {
				this.screenDistance = value;
			}
		}

		public double FieldOfViewHeight {
			get {
				return this.foVH;
			}
			set {
				this.foVH = value;
			}
		}

		public Texture Raster {
			get {
				return this.raster;
			}
		}

		public Camera () {
		}

		public Camera (int w, int h, double screenDistance, double foVH, IAccelerator acc, Light[] lights, EnvironmentSettings settings, List<CameraPostProcessor> postprocessors = null) {
			this.raster = new Texture(w, h);
			this.foVH = foVH;
			this.acc = acc;
			this.Lights = lights;
			this.antialiasSqrt = settings.AntiAliasingSqrt;
			this.dispersion = settings.Dispersion;
			this.dispersionAntialiasSqrt = settings.DispersingAntiAliasingSqrt;
			this.settings = settings;
			if(postprocessors != null) {
				this.postProcessors = postprocessors;
			}
			else {
				this.postProcessors = new List<CameraPostProcessor>();
			}
		}

		public void MakeDirty () {
			this.dirty = true;
		}
		public void RebuildMatrix () {
			if(!this.dirty) {
				return;
			}
			this.dirty = false;
			Point3 forward = this.lookAt-this.position;
			forward.Normalize();
			Point3 right = new Point3(forward.Z, 0.0d, -forward.X);
			right.Normalize();
			Point3 up = Point3.CrossNormalize(forward, right);
			forward.Normalize();
			this.matrix.LoadColumns(right, up, forward);
			this.matrix.RotateZ(this.roll);
			this.matrix.Shift(this.position);
		}
		
		public void CalculateImage () {
#if DEBUG
			DateTime start = DateTime.Now;
#endif
			this.RebuildMatrix();
			int cores = Environment.ProcessorCount;
			System.Threading.Tasks.Parallel.For(0x00, cores, x => CalculateImage(x*Height/cores, (x+1)*Height/cores));
			foreach(CameraPostProcessor cpp in this.postProcessors) {
				cpp.Process(this, this.raster, this.acc);
			}
#if DEBUG
			DateTime stop = DateTime.Now;
			Console.WriteLine("{0}", (stop-start).TotalMilliseconds.ToString("0.000"));
#endif
		}
		private void CalculateImage (int yfrom, int yto) {
			double sd = this.screenDistance;
			double sh = 2.0d*sd*Math.Tan(0.5d*this.foVH);
			int w = this.Width;
			int h = this.Height;
			double sw = sh*w/h;
			double dwh = sw/w;
			Ray ray = new Ray(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d);
			uint[] pixel = this.raster.Pixel;
			int k = Width*yfrom, ks = Width*yto, kc;
			uint l, m;
			uint aasqrt = this.antialiasSqrt;
			uint aadsqrt = this.dispersionAntialiasSqrt;
			RayTracer rt = new RayTracer(this.acc, this.Lights, settings);
			uint aa = aasqrt*aasqrt, aac, aad = aadsqrt*aadsqrt, aadc;
			Point3 tmp = new Point3(0.0d, 0.0d, 0.0d);
			double focusLength = Point3.DiffLength(this.position, this.lookAt);
			double frac = 1.0d+focusLength/this.screenDistance;
			uint aaaad = aa*aad;
			double dwha = dwh/aasqrt;
			double dwhad = dispersion*dwh;
			double yp = dwh*yfrom-0.5d*sh-0.5d*dwha*aasqrt, xp;
			double yd, xd;
			double dis = this.displacement;
			ColorCache cc;
			#region PIXEL
			for(; k < ks;) {
				kc = k+Width;
				xp = -0.5d*sw-0.5d*dwha*aasqrt-dis;
				for(; k < kc;) {
					l = 0x00;
					cc = new ColorCache(0x00);
					#region ANTIALIASING
					for(; l < aa;) {
						aac = l+aasqrt;
						for(; l < aac; l++) {
							m = 0x00;
							tmp.SetValues(xp*frac, -yp*frac, focusLength);
							yd = -0.5d*dwhad*(aadsqrt-0x01);
							#region DISPERSION
							for(; m < aad;) {
								xd = -0.5d*dwhad*(aadsqrt-0x01);
								aadc = m+aadsqrt;
								for(; m < aadc; m++) {
									ray.Offset.SetValues(xp+xd, -yp-yd, 0.0d);
									ray.Direction.SetValues(ray.Offset, tmp);
									ray.Direction.Normalize();
									ray.NormalizeDirection();
									ray.Transform(this.matrix);
#if FAST_COLOR_INTERSECTION
									rt.CalculateColor(ray, 0, Color.White);
									cc.AddColor(new Color(ColorUtils.FromWavelength(350+5*(int)SystemDiagnostics.Intersections)));
									SystemDiagnostics.Intersections = 0x00;
#elif FAST_COLOR_MIGRATION
									rt.CalculateColor(ray, 0, Color.White);
									cc.AddColor(new Color(ColorUtils.FromWavelength(350+5*(int)SystemDiagnostics.Migrations)));
									SystemDiagnostics.Migrations = 0x00;
#else
									cc.AddColor(rt.CalculateColor(ray, 0, Color.White));
#endif
									xd += dwhad;
								}
								yd += dwhad;
							}
							#endregion
							xp += dwha;
						}
						yp += dwha;
						xp -= dwh;
					}
					#endregion
					yp -= dwh;
					xp += dwh;
					pixel[k++] = ColorUtils.AlphaChannel|cc.Mix(aaaad).RGB8;
				}
				#endregion
				yp += dwh;
			}
		}
		
		public unsafe Bitmap ToBitmap () {
			return this.raster.ToBitmap();
		}

		public unsafe void Save (string filename) {
			this.raster.Save(filename);
		}
	}
}

