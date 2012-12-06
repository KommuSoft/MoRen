//  
//  RayTracer.cs
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

#define AMBIENT
#define DIFFUSE
#define SPECULAR
#undef REFLECTION
#undef REFRACTION
#define INTENSITYLOSS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Renderer {
	[XmlType("RayTracer")]
	public sealed class RayTracer {

		[XmlIgnore]
		private readonly Accelerator
			acc;
		[XmlIgnore]
		private readonly List<Light>
			lights = new List<Light>();
		[XmlAttribute("AmbientColor")]
		public uint
			AmbientColor = 0x00101010;
		[XmlIgnore]
		private readonly CastResult
			nw = new CastResult();
		[XmlIgnore]
		private readonly Point3
			dis = new Point3();
		[XmlIgnore]
		private readonly Point3
			rl = new Point3();
		[XmlIgnore]
		private readonly Point3
			hp = new Point3();
		[XmlIgnore]
		private readonly Ray
			sr;
		
		public RayTracer (Accelerator acc, List<Light> lights) {
			this.acc = acc;
			this.lights = lights;
			this.sr = new Ray(new Point3(0.0d, 0.0d, 0.0d), dis);
		}
		
		public uint CalculateColor (Ray ray, int depth) {
			RenderItem best = null;
			double t, tdummy;
			best = acc.CalculateHit(ray, out t);
			if(best != null) {
				best.Cast(ray, nw);
				Point3 norm = nw.Normal;
				ray.PointAt(t, hp);
				Material mat = best.Material;
				uint ads, dds, sds;
				mat.ADSAt(nw.TU, out ads, out dds, out sds);
				uint clr;
				clr = Color.Multiply(this.AmbientColor, ads);
				uint clrl;
				foreach(Light li in this.lights) {
					Point3.Reflect(ray.Direction, nw.Normal, rl);
					dis.SetValues(hp, li.Position);
					double len = dis.Length;
					dis.Normalize();
					sr.SetOffsetWithEpsilon(hp);		
					if(this.acc.CalculateHit(sr, out tdummy, len) == null) {
						clrl = Color.Scale(Color.Multiply(li.Color, dds), Point3.CosAngleNorm(dis, norm));
						clrl = Color.Add(clrl, Color.Scale(Color.Multiply(li.Color, sds), Math.Pow(Point3.CosAngleNorm(rl, dis), mat.Shininess)));
						clr = Color.Add(clr, Color.loseIntensity(clrl, len));
					}
				}
				/*if(depth < 0) {
					Point3 rl, rf;
					Point3.ReflectRefract(ray.Direction,norm,nw.Material.NFactor,out rl, out rf);
					srl = Ray.WithEpsilon(hp,rl);
					srf = Ray.WithEpsilon(hp,rf);
					clr = Color.Merge(new L<Color,Ray,int>(calculateColor,srl,depth+1),new L<Color,Ray,int>(calculateColor,srf,depth+1),new L<Color>(clr),nw.ReflectionFactor,nw.RefractionFactor);
				}*/
				return Color.loseIntensity(clr, nw.T);
			}
			else {
				return 0xff000000;
			}
		}
		
		public static int Main (string[] args) {
			PerlinCache.InitializeNoiseBuffer();
			LoaderObj lo = new LoaderObj();
			double alpha = 0.0d;
			FileStream fs = File.Open("venus.obj", FileMode.Open, FileAccess.Read);
			lo.Load(null, fs);
			fs.Close();
			List<Light> lights = new List<Light>();
			lights.Add(new Light(0x808080, new Point3(-5.0d, 5.0d, 0.0d)));
			lights.Add(new Light(0x808080, new Point3(5.0d, -5.0d, 0.0d)));
			TimeSpan ts = new TimeSpan();
			for(int i = 0x00; alpha < 2.0d*Math.PI; i++, alpha += Math.PI/80) {
				Matrix4 M = new Matrix4();
				M.RotateY(1.0d*Math.PI+alpha);
				M.Shift(0.0d, 10.0d, 40.0d);
				Accelerator acc = new OctTreeAccelerator(lo.Inject(M));
				Camera cam = new Camera(640, 640, 1.5, 0.25d*Math.PI, acc, lights);
				DateTime start = DateTime.Now;
				cam.CalculateImage();
				DateTime stop = DateTime.Now;
				ts = ts.Add(stop-start);
				cam.Save("venus/result"+i.ToString("000")+".png");
			}
			Console.WriteLine("Testcase took {0}", ts);
			return 0x00;
		}
		
	}
	
}