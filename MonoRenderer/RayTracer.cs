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

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Renderer.SceneBuilding;

namespace Renderer {
	[XmlType("RayTracer")]
	public sealed class RayTracer {

		public const uint DepthThresholdMask = 0xfcfcfc;
		private readonly Accelerator acc;
		private readonly Light[] lights;
		private readonly uint ambientColor = 0x00101010;
		private readonly CastResult nw = new CastResult();
		private readonly Point3 dis = new Point3();
		private readonly Point3 rl = new Point3();
		private readonly Point3 rf = new Point3();
		private readonly Point3 lp = new Point3();
		private readonly Point3 hp = new Point3();
		private readonly Ray sr;
		private readonly uint lightTest;
		private readonly Ray[] rayCache;
		private readonly uint maxDepth;
		private readonly double distanceUnit = 255000000.0d;
		
		public RayTracer (Accelerator acc, Light[] lights, EnvironmentSettings settings) {
			this.acc = acc;
			this.maxDepth = settings.RecursionDepth;
			this.rayCache = new Ray[maxDepth+0x01];
			for(int i = 0x00; i < this.maxDepth+0x01; i++) {
				this.rayCache[i] = new Ray(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d);
			}
			this.lights = lights;
			this.ambientColor = settings.AmbientColor.Color;
			this.lightTest = settings.LightTest;
			this.sr = new Ray(new Point3(0.0d, 0.0d, 0.0d), dis);
		}
		
		public uint CalculateColor (Ray ray, int depth, uint intensityHint) {
			RenderItem best = null;
			double t, tdummy;
			best = acc.CalculateHit(ray, out t);
			if(best != null) {
				best.Cast(ray, nw);
				Point3 norm = nw.Normal;
				ray.PointAt(t, hp);
				Material mat = best.Material;
				uint ambient, diffuse, specular, reflectance, refraction;
				mat.ADSAtAndBump(nw, ray.Direction, out ambient, out diffuse, out specular, out reflectance, out refraction);
				uint clr = Color.Multiply(this.ambientColor, ambient);
				Point3.ReflectRefract(ray.Direction, nw.Normal, mat.NFactor, rl, rf);
				rayCache[depth].SetWithEpsilon(hp, rf);
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
					}
					else {
						light = 0xff;
					}
					if(light > 0x00) {
						dis.SetValues(hp, li.Position);
						dis.Normalize();
						uint clrl = Color.Scale(Color.Multiply(li.Color, diffuse), Point3.CosAngleNorm(dis, norm));
						clrl = Color.Add(clrl, Color.Scale(Color.Multiply(li.Color, specular), Math.Pow(Point3.CosAngleNorm(rl, dis), mat.Shininess)));
						clr = Color.Add(clr, Color.loseIntensity(Color.Scale(clrl, light), distanceUnit, len));
					}
				}
				if(depth < maxDepth) {
					uint reflint = Color.Multiply(intensityHint, reflectance);
					if((reflint&DepthThresholdMask) != 0x00) {
						ray.SetWithEpsilon(hp, rl);
						clr = Color.Add(clr, Color.Multiply(this.CalculateColor(ray, depth+1, reflint), reflectance));
					}
					uint refrint = Color.Multiply(intensityHint, refraction);
					if(!double.IsNaN(rayCache[depth].Direction.X) && (refrint&DepthThresholdMask) != 0x00) {
						uint res = this.CalculateColor(rayCache[depth], depth+1, refrint);
						clr = Color.Add(clr, Color.Multiply(res, refrint));
					}
				}
				return Color.loseIntensity(clr, distanceUnit, t);
			}
			else {
				return 0x00000000;
			}
		}
		
		public static int Main (string[] args) {
			PerlinCache.InitializeNoiseBuffer();
			LoaderObj teapot = new LoaderObj();
			teapot.Load(null, "venus.obj");
			List<RenderItem> ri = new List<RenderItem>();
			Matrix4 m = Matrix4.CreateRotateYMatrix(Math.PI);
			m.Shift(0.0d, 0.0d, 15.0d);
			teapot.Inject(ri, m, null);
			//OctTreeAccelerator bsp = new OctTreeAccelerator(ri);
			BSPAccelerator bsp = new BSPAccelerator(ri);
			Console.WriteLine(bsp);
			Light[] light = new Light[] {
				new Light(0x808080, new Point3(5.0d, 0.0d, -10.0d), 0.0d),
				new Light(0x808080, new Point3(-5.0d, 0.0d, -10.0d), 0.0d)
			};
			Camera cam = new Camera(640, 640, 1.5d, Maths.PI_4, bsp, light, new EnvironmentSettings());
			RenderWindow rw = new RenderWindow(cam);
			rw.ShowDialog();
			/*
			SceneDescription sd = SceneDescription.ParseFromStream("Scene.xml");
			new RenderWindow(sd.BuildScene()).ShowDialog();*/
			return 0x00;
		}
		
	}
	
}