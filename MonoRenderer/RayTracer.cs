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
using Renderer.SceneBuilding;

namespace Renderer {
	[XmlType("RayTracer")]
	public sealed class RayTracer {

		private readonly Accelerator acc;
		private readonly Light[] lights;
		private readonly uint ambientColor = 0x00101010;
		private readonly CastResult nw = new CastResult();
		private readonly Point3 dis = new Point3();
		private readonly Point3 rl = new Point3();
		private readonly Point3 lp = new Point3();
		private readonly Point3 hp = new Point3();
		private readonly Ray sr;
		private readonly uint lightTest;
		private readonly Ray[] rayCache;
		private readonly uint maxDepth;
		
		public RayTracer (Accelerator acc, Light[] lights, EnvironmentSettings settings) {
			this.acc = acc;
			this.maxDepth = settings.RecursionDepth;
			this.rayCache = new Ray[maxDepth];
			for(int i = 0x00; i < this.maxDepth; i++) {
				this.rayCache[i] = new Ray(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d);
			}
			this.lights = lights;
			this.ambientColor = settings.AmbientColor.Color;
			this.lightTest = settings.LightTest;
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
				uint ambient, diffuse, specular, reflectance;
				mat.ADSAt(nw.TU, -Point3.CosAngle(ray.Direction, norm), out ambient, out diffuse, out specular, out reflectance);
				uint clr;
				clr = Color.Multiply(this.ambientColor, ambient);
				uint clrl;
				Point3.Reflect(ray.Direction, nw.Normal, rl);
				foreach(Light li in this.lights) {
					double len = Point3.DiffLength(hp, li.Position);
					double thetafrac = Math.PI-Math.Asin(li.Radius/len);
					uint light = 0x00;
					if(!double.IsNaN(thetafrac)) {
						dis.SetValues(hp, li.Position);
						dis.Normalize();
						sr.SetOffsetWithEpsilon(hp);
						if(this.acc.CalculateHit(sr, out tdummy, len-li.Radius) == null) {
							light++;
						}
						for(int i = 1; i < lightTest; i++) {
							lp.SetValues(li.Position, li.Radius);
							dis.SetValues(hp, lp);
							dis.Normalize();
							sr.SetOffsetWithEpsilon(hp);
							if(this.acc.CalculateHit(sr, out tdummy, len-li.Radius) == null) {
								light++;
							}
						}
						light = Math.Min((uint)((light<<0x08)/lightTest), 0xff);
						//Console.WriteLine("light resulted in {0}", thetafrac);
					}
					else {
						light = 0xff;
					}
					if(light > 0x00) {
						dis.SetValues(hp, li.Position);
						dis.Normalize();
						clrl = Color.Scale(Color.Multiply(li.Color, diffuse), Point3.CosAngleNorm(dis, norm));
						clrl = Color.Add(clrl, Color.Scale(Color.Multiply(li.Color, specular), Math.Pow(Point3.CosAngleNorm(rl, dis), mat.Shininess)));
						clr = Color.Add(clr, Color.loseIntensity(Color.Scale(clrl, light), len));
					}
				}
				ray.SetWithEpsilon(hp, rl);
				if(depth < maxDepth) {
					clr = Color.Add(clr, Color.Multiply(this.CalculateColor(ray, depth+1), specular));
				}
				/*if(depth < maxDepth) {
					Point3 rl, rf;
					Point3.ReflectRefract(ray.Direction, norm, mat.NFactor, ref rayCache[depth++].Direction);
					//srl = Ray.WithEpsilon(hp, rl);
					//srf = Ray.WithEpsilon(hp, rf);
					//clr = Color.Merge(new L<Color,Ray,int>(calculateColor, srl, depth+1), new L<Color,Ray,int>(calculateColor, srf, depth+1), new L<Color>(clr), nw.ReflectionFactor, nw.RefractionFactor);
				}*/
				return Color.loseIntensity(clr, t);
			}
			else {
				return 0x00000000;
			}
		}
		
		public static int Main (string[] args) {
			/*SceneDescription sd = new SceneDescription(new SceneGraph());
			sd.SceneGraph.Root.TransformerString = "Shift 0 0 40";
			sd.SceneGraph.Root.AddChild(new SceneGraphNode(new Mesh("venus.obj")));
			sd.Save("Scene2.xml");*/
			PerlinCache.InitializeNoiseBuffer();
			/*LoaderObj lo = new LoaderObj();
			//double alpha = 0.0d;
			FileStream fs = File.Open("venus.obj", FileMode.Open, FileAccess.Read);
			lo.Load(null, fs);
			fs.Close();*/
			Light[] lights = new Light[] {
				new Light(0x808080, new Point3(-5.0d, 5.0d, 1.0d)),
				new Light(0x808080, new Point3(5.0d, -5.0d, 1.0d))
			};
			/*Matrix4 M = new Matrix4();
			M.RotateY(Math.PI);
			M.Shift(0.0d, 0.0d, 30.0d);*/
			//Accelerator acc = new OctTreeAccelerator(lo.Inject(M));
			List<CameraPostProcessor> cpps = new List<CameraPostProcessor>();
			//EnvironmentSettings es = new EnvironmentSettings(0x00101010, 0x08, 0x40, 0x01);
			//EnvironmentSettings es = new EnvironmentSettings(0x00101010, 0x01, 0x01, 0x01);
			cpps.Add(new NoisePostProcessor());
			//RenderWindow rw = new RenderWindow(cam);
			//rw.ShowDialog();
			double alpha = 0.0d;
			TimeSpan ts = new TimeSpan();
			//for(int i = 0x00; alpha < 2.0d*Math.PI; i++, alpha += Math.PI/80) {
			SceneDescription sd = SceneDescription.ParseFromStream("Scene.xml");
			Accelerator acc = new OctTreeAccelerator(sd.SceneGraph.Inject());
			Camera cam = new Camera(640, 640, 1.5, 0.25d*Math.PI, acc, lights, sd.EnvironmentSettings, cpps);
			DateTime start = DateTime.Now;
			cam.CalculateImage();
			DateTime stop = DateTime.Now;
			ts = ts.Add(stop-start);
			//cam.Save("fluttershy/result"+i.ToString("000")+".png");
			cam.Save("fluttershy/result.png");
			//}
			Console.WriteLine("Testcase took {0}", ts);
			return 0x00;
		}
		
	}
	
}