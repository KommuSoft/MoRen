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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace Renderer {
	[XmlType("Camera")]
	public sealed class Camera : NameBase {

		private readonly Point3 position = new Point3(0.0d, 0.0d, 0.0d);
		private readonly Point3 lookAt = new Point3(0.0d, 0.0d, 1.0d);
		private readonly Matrix4 matrix = new Matrix4();
		private readonly Accelerator acc;
		private Texture raster = new Texture(1, 1);
		private double screenDistance = 1.5d;
		private bool dirty = true;
		private double roll = 0.0d;
		private double foVH = 0.5d*Math.PI;
		private int antiAliasingThreshold = 1;
		private readonly List<Light> lights;
		private AntiAliasingTechnique aaTec = AntiAliasingTechnique.Sobel;
		private int antialiasSqrt = 0x04;

		[XmlElement("AntialiasSqrt")]
		public int AntialiasSqrt {
			get {
				return this.antialiasSqrt;
			}
			set {
				this.antialiasSqrt = value;
			}
		}
		[XmlElement("Position")]
		public Point3 Position {
			get {
				return this.position;
			}
			set {
				this.position.SetValues(value);
				this.dirty = true;
			}
		}

		[XmlElement("LookAt")]
		public Point3 LookAt {
			get {
				return this.lookAt;
			}
			set {
				this.lookAt.SetValues(value);
				this.dirty = true;
			}
		}

		[XmlAttribute("Roll")]
		public double Roll {
			get {
				return this.roll;
			}
			set {
				this.roll = value;
				this.dirty = true;
			}
		}

		[XmlAttribute("Width")]
		public int Width {
			get {
				return this.raster.Width;
			}
			set {
				this.raster.Resize(value, this.raster.Height);
			}
		}

		[XmlAttribute("Height")]
		public int Height {
			get {
				return this.raster.Height;
			}
			set {
				this.raster.Resize(this.raster.Width, value);
			}
		}

		[XmlAttribute("ScreenDistance")]
		public double ScreenDistance {
			get {
				return this.screenDistance;
			}
			set {
				this.screenDistance = value;
			}
		}

		[XmlAttribute("FoVH")]
		public double FieldOfViewHeight {
			get {
				return this.foVH;
			}
			set {
				this.foVH = value;
			}
		}

		[XmlIgnore]
		public Texture Raster {
			get {
				return this.raster;
			}
		}

		[XmlAttribute("AntiAliasingThreshold")]
		public int AntiAliasingThreshold {
			get {
				return this.antiAliasingThreshold;
			}
			set {
				this.antiAliasingThreshold = value;
			}
		}

		[XmlAttribute("AntiAliasingTechnique")]
		public AntiAliasingTechnique AntiAliasingTechnique {
			get {
				return this.aaTec;
			}
			set {
				this.aaTec = value;
			}
		}

		public Camera () {
		}

		public Camera (int w, int h, double screenDistance, double foVH, Accelerator acc, List<Light> lights) {
			this.raster = new Texture(w, h);
			this.foVH = foVH;
			this.acc = acc;
			this.lights = lights;
		}

		public void RebuildMatrix () {
			if(!this.dirty) {
				return;
			}
			this.dirty = false;
			Point3 forward = this.lookAt-this.position;
			Point3 right = new Point3(forward.Z, 0.0d, -forward.X);
			right.Normalize();
			Point3 up = Point3.CrossNormalize(forward, right);
			forward.Normalize();
			this.matrix.LoadColumns(right, up, forward);
			this.matrix.RotateZ(this.roll);
			this.matrix.Shift(this.position);
		}
		
		public void CalculateImage () {
			DateTime start = DateTime.Now;
			this.RebuildMatrix();
			int cores = Environment.ProcessorCount;
			System.Threading.Tasks.Parallel.For(0x00, cores, x => CalculateImage(x*Height/cores, (x+1)*Height/cores));
			//this.CalculateImage(0x00, this.Height);
			DateTime stop = DateTime.Now;
			Console.WriteLine("Rendered in {0}", stop-start);
		}
		public void CalculateImage (int yfrom, int yto) {
			double sd = this.screenDistance;
			double sh = sd*Math.Tan(0.5d*this.foVH);
			int w = this.Width;
			int h = this.Height;
			double sw = sh*w/h;
			double dwh = sw/w;
			double dwha = dwh/antialiasSqrt;
			double yg, xg;
			Ray ray = new Ray(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d);
			uint[] pixel = this.raster.Pixel;
			int k = Width*yfrom, ks = Width*yto, kc;
			int l;
			int aasqrt = this.antialiasSqrt;
			uint[] aaCache = new uint[aasqrt*aasqrt];
			RayTracer rt = new RayTracer(this.acc, this.lights);
			int aa = aasqrt*aasqrt, aac;
			yg = -sh+dwh*yfrom;
			for(; k < ks;) {
				kc = k+Width;
				xg = -0.5d*sw;
				for(; k < kc;) {
					l = 0x00;
					for(; l < aa;) {
						aac = l+aasqrt;
						for(; l < aac;) {
							ray.Offset.SetValues(xg, -yg, 0.0d);
							ray.Direction.SetValues(xg, -yg, this.screenDistance);
							ray.NormalizeDirection();
							ray.Transform(this.matrix);
							aaCache[l++] = rt.CalculateColor(ray, 0);
							xg += dwha;
						}
						yg += dwha;
						xg -= dwh;
					}
					pixel[k++] = Color.AlphaChannel|Color.Mix(aaCache);
					xg += dwh;
					yg -= dwh;
				}
				yg += dwh;
			}
			/*for(int y = yfrom; y < yto; y++) {
				xg = -0.5d*sw;
				for(int x = 0x00; x < w; x++) {
					l = 0x00;
					for(int i = 0x00; i < aasqrt; i++) {
						for(int j = 0x00; j < aasqrt; j++) {
							Console.WriteLine("{0} | {1}", xg*dwha, yg*dwha);
							ray.Offset.SetValues(xg, -yg, 0.0d);
							ray.Direction.SetValues(xg, -yg, sh);
							ray.NormalizeDirection();
							ray.Transform(this.matrix);
							aaCache[l++] = rt.CalculateColor(ray, 0);
							xg += dwha;
						}
						yg += dwha;
						xg -= dwh;
					}
					Console.WriteLine("New Tile");
					xg += dwh;
					yg -= dwh;
					pixel[k++] = Color.AlphaChannel|Color.Mix(aaCache);
				}
				yg += dwh;
			}*/
		}
		
		public unsafe Bitmap ToBitmap () {
			return this.raster.ToBitmap();
		}

		public unsafe void Save (string filename) {
			this.raster.Save(filename);
		}
	}
}

