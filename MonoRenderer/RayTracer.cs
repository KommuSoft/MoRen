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

		private readonly IAccelerator acc;
		private readonly Light[] lights;
		private readonly Color ambientColor = new Color(0x00101010);
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
		private readonly EnviromentMap EnvironmentMap;
		
		public RayTracer (IAccelerator acc, Light[] lights, EnvironmentSettings settings) {
			this.acc = acc;
			this.maxDepth = settings.RecursionDepth;
			this.rayCache = new Ray[maxDepth+0x01];
			for(int i = 0x00; i < this.maxDepth+0x01; i++) {
				this.rayCache[i] = new Ray(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d);
			}
			this.lights = lights;
			this.ambientColor = settings.AmbientColor.Color;
			this.EnvironmentMap = EnvironmentMaps.GetOrBlack(settings.EnvironmentMap);
			this.lightTest = settings.LightTest;
			this.sr = new Ray(new Point3(0.0d, 0.0d, 0.0d), dis);
			this.distanceUnit = settings.DistanceUnit;
		}
		
		public Color CalculateColor (Ray ray, int depth, Color intensityHint) {
			RenderItem best = null;
			double t, tdummy;
			best = acc.CalculateHit(ray, out t);
			if(best != null) {
				best.Cast(ray, nw);
				Point3 norm = nw.Normal;
				ray.PointAt(t, hp);
				Material mat = best.Material;
				Color ambient, diffuse, specular, reflectance, refraction;
				mat.ADSAtAndBump(nw, ray.Direction, out ambient, out diffuse, out specular, out reflectance, out refraction);
				Color clr = this.ambientColor*ambient;
				Point3.ReflectRefract(ray.Direction, nw.Normal, mat.NFactor, rl, rf);
				rayCache[depth].SetWithEpsilon(hp, rf);
				foreach(Light li in this.lights) {
					double len = Point3.DiffLength(hp, li.Position);
					double thetafrac = Math.PI-Math.Asin(li.Radius/len);
					uint light = 0x00;
					double lightD;
					if(!double.IsNaN(thetafrac) && lightTest > 0x00) {
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
						lightD = (double)light/lightTest;
					}
					else {
						lightD = 1.0d;
					}
					if(lightD > 0.0d) {
						dis.SetValues(hp, li.Position);
						dis.Normalize();
						Color clrl = (li.Color*diffuse)*Point3.CosAngleNorm(dis, norm);
						clrl += (li.Color*specular)*Math.Pow(Point3.CosAngleNorm(rl, dis), mat.Shininess);
						clr += Color.LoseIntensity((clrl*lightD), distanceUnit, len);
					}
				}
				if(depth < maxDepth) {
					Color reflint = intensityHint*reflectance;
					if(reflint.IntensityTreshold) {
						ray.SetWithEpsilon(hp, rl);
						clr += this.CalculateColor(ray, depth+1, reflint)*reflectance;
					}
					Color refrint = intensityHint*refraction;
					if(!double.IsNaN(rayCache[depth].Direction.X) && refrint.IntensityTreshold) {
						clr += this.CalculateColor(rayCache[depth], depth+1, refrint)*refrint;
					}
				}
				return Color.LoseIntensity(clr, distanceUnit, t);
			}
			else {
				return this.EnvironmentMap(ray);
			}
		}
		
		public static int Main (string[] args) {
			PerlinCache.InitializeNoiseBuffer();
			Mesh.InvokeLoaders();
			SceneDescription sd = SceneDescription.ParseFromStream("Scene.xml");
			sd.BuildScene();
			return 0x00;
		}
		
	}
	
}