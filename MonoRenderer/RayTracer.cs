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
	public sealed class RayTracer
	{

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
					if(this.acc.CalculateHit(sr, out tdummy, len) == null) {//this.items.Select(ri => ri.Cast(sr)).All(x => x == null || x.T > len)
						clrl = Color.Scale(Color.Multiply(li.Color, dds), Point3.CosAngleNorm(dis, norm));//*mat.Diffuse
						clrl = Color.Add(clrl, Color.Scale(Color.Multiply(li.Color, sds), Math.Pow(Point3.CosAngleNorm(rl, dis), mat.Shininess)));
						clr = Color.Add(clr, Color.loseIntensity(clrl, len));//*mat.Specular
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
			} else {
				return 0xff000000;
			}
		}
		
		public static int Main (string[] args) {
			LoaderObj lo = new LoaderObj();
			//Loader3ds lo = new Loader3ds();
			/*FileStream fs2 = File.Open("snowtrooper_hi/snowtrooper.3ds", FileMode.Open, FileAccess.Read);
			FileStream fs2 = File.Open("clonetrooper/C_PILOT.3ds", FileMode.Open, FileAccess.Read);
			l.Load("clonetrooper/",fs2);
			fs2.Close();*/
			//PerlinCache.InitializeNoiseBuffer();
			double alpha = 0.0d;
			//FileStream fs = File.Open("snowtrooper_hi/snowtrooper.3ds", FileMode.Open, FileAccess.Read);
			//FileStream fs = File.Open("yoda/yoda.3ds", FileMode.Open, FileAccess.Read);
			//FileStream fs = File.Open("anakin/anakinhead.3ds", FileMode.Open, FileAccess.Read);
			//FileStream fs = File.Open("teapot.obj", FileMode.Open, FileAccess.Read);
			//FileStream fs = File.Open("pangea3dgalleon.obj", FileMode.Open, FileAccess.Read);
			FileStream fs = File.Open("venus.obj", FileMode.Open, FileAccess.Read);
			lo.Load(null, fs);
			//lo.Load("snowtrooper_hi", fs);
			//lo.Load("yoda/", fs);
			//lo.Load("anakin/", fs);
			fs.Close();
			TimeSpan ts = new TimeSpan();
			for(int i = 0x00; alpha < 2.0d*Math.PI; i++, alpha += Math.PI/80) {
				Matrix4 M = new Matrix4();
				/*M.RotateY(alpha);
				M.Scale(1.25d, 1.25d, 1.25d);
				M.Shift(0.0d, -5.0d, 15.0d);*/
				//M.RotateX(-0.5d*Math.PI);
				M.RotateY(1.0d*Math.PI+alpha);
				//M.Scale(2.0d, 2.0d, 2.0d);
				//M.Scale(0.075d, 0.075d, 0.075d);
				//M.Shift(0.0d, -7.5d, 15.0d);

				//M.Scale(1.5d, 1.5d, 1.5d);
				//M.Shift(0.0d, -20.0d, 1000.0d);

				//M.Shift(0.0d, 0.0d, 500.0d);
				M.Shift(0.0d, 10.0d, 40.0d);

				//return 0x00;
				/*Triangle t = new Triangle(new Point3(0, 0, 0), new Point3(10, 5, 10), new Point3(5, 10, 5), null, null, null, null, null, null, null);
				Console.WriteLine(t);
				Tuple<ProxyRenderItem[],ProxyRenderItem[]> spl = t.SplitAt(7.5, 0);
				Console.WriteLine("[{0}]/[{1}]", string.Join(",", (IEnumerable<ProxyRenderItem>)spl.Item1), string.Join(",", (IEnumerable<ProxyRenderItem>)spl.Item2));
				return 0x00;*/
				/*KdeTreeAccelerator ac = new KdeTreeAccelerator(new List<IRenderable>(new IRenderable[] {
					new Triangle(new Point3(0, 0, 0), new Point3(10, 5, 10), new Point3(5, 10, 5), null, null, null, null, null, null, null),
					new Triangle(new Point3(20, 0, 0), new Point3(30, 5, 10), new Point3(25, 10, 5), null, null, null, null, null, null, null)
				}
				)
				);*/
				//RayTracer rt = new RayTracer(new GridAccelerator(l.Receive("Trooper Pa")));
				//RayTracer rt = new RayTracer(new GridAccelerator(l.Receive("top33")));
				//RayTracer rt = new RayTracer(new GridAccelerator(lo.Inject(M)));
				Accelerator acc = new OctTreeAccelerator(lo.Inject(M));
				//RayTracer rt = new RayTracer(new OctTreeAccelerator(new RenderItem[] {new Sphere(new Point3(-2.0d, 2.5d, 10.0d), 2.5d, Material.DefaultMaterial)}.ToList()));
				List<Light> lights = new List<Light>();
				lights.Add(new Light(0x808080, new Point3(-5.0d, 5.0d, 0.0d)));
				lights.Add(new Light(0x808080, new Point3(5.0d, -5.0d, 0.0d)));
				Camera cam = new Camera(640, 640, 1.5, 0.25d*Math.PI, acc, lights);
				//SceneDescription sd = new SceneDescription(rt, new SceneGraph(), cam);
				/*XmlSerializer ser = new XmlSerializer(typeof(SceneDescription));
				FileStream fs = File.Open("scene.xml", FileMode.Create, FileAccess.Write);
				TextWriter tw = new StreamWriter(fs);
				ser.Serialize(tw, sd);
				tw.Close();
				fs.Close();*/
				//Material mat = new Material(0x808080, 0x808080, 0x808080, 1.0d, 15.0d);
				DateTime start = DateTime.Now;
				cam.CalculateImage();
				//cam.Save("pangea3dgalleon/pangea3dgalleon"+i.ToString("000")+".png");
				//cam.Save("venus/venus"+i.ToString("000")+".png");
				//cam.Save("yoda/result"+i.ToString("000")+".png");
				DateTime stop = DateTime.Now;
				ts = ts.Add(stop-start);
				cam.Save("anakin/result"+i.ToString("000")+".png");
			}
			Console.WriteLine("Testcase took {0}", ts);
			return 0x00;//*/
		}
		
	}
	
}