//  
//  SceneDescription.cs
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
using System.IO;

namespace Renderer.SceneBuilding {

	[XmlRoot("SceneDescription")]
	public class SceneDescription {
		
		private CameraWrapper camera;
		private AcceleratorWrapper acceleratorWrapper;
		private SceneGraph sg;
		private EnvironmentSettings es;


		[XmlElement("EnvironmentSettings")]
		public EnvironmentSettings EnvironmentSettings {
			get {
				return this.es;
			}
			set {
				this.es = value;
			}
		}
		[XmlElement("SceneGraph")]
		public SceneGraph SceneGraph {
			get {
				return this.sg;
			}
			set {
				this.sg = value;
			}
		}
		[XmlElement("Camera")]
		public CameraWrapper CameraWrapper {
			get {
				return this.camera;
			}
			set {
				this.camera = value;
			}
		}
		[XmlElement("Accelerator")]
		public AcceleratorWrapper AcceleratorWrapper {
			get {
				return this.acceleratorWrapper;
			}
			set {
				this.acceleratorWrapper = value;
			}
		}
		
		public SceneDescription () {
		}
		public SceneDescription (SceneGraph sg) {
			this.sg = sg;
		}

		public Camera BuildScene () {
			Tuple<List<RenderItem>,List<Light>> scene = this.SceneGraph.Inject();
			List<RenderItem> ris = scene.Item1;
			Point3 ld = new Point3(-50.0d, -25.0d, 50.0d);
			Point3 lu = new Point3(-50.0d, 25.0d, 50.0d);
			Point3 rd = new Point3(50.0d, -25.0d, 50.0d);
			Point3 ru = new Point3(50.0d, 25.0d, 50.0d);
			Material mat = new Material(0xffffff, 0xffffff, 0xffffff, 15.0d, 0.0d, new Texture("earthmap1k.jpg"), new Texture("earthspec1k.jpg"), new Texture("earthbump1k.jpg"));
			ris.Add(new Triangle(ld, lu, ru, null, null, null, Point3.DummyYPoint, Point3.DummyPoint, Point3.DummyXPoint, mat));
			ris.Add(new Triangle(ld, ru, rd, null, null, null, Point3.DummyYPoint, Point3.DummyXPoint, Point3.DummyXYPoint, mat));
			Light[] lights = scene.Item2.ToArray();
			EnvironmentSettings es = this.es;
			//Accelerator acc = new OctTreeAccelerator(ris);
			Accelerator acc = this.AcceleratorWrapper.CreateAccelerator(ris);
			//return cam;
			Camera cam = this.camera.Camera(acc, lights, es);
			//cam.CalculateImage();
			//cam.Save("refraction.png");
			return cam;
		}
		public static SceneDescription ParseFromStream (Stream stream) {
			XmlSerializer ser = new XmlSerializer(typeof(SceneDescription));
			SceneDescription sd = (SceneDescription)ser.Deserialize(stream);
			return sd;
		}
		public static SceneDescription ParseFromStream (string filename) {
			FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
			SceneDescription sd = ParseFromStream(fs);
			fs.Close();
			return sd;
		}
		public void Save (Stream s) {
			XmlSerializer ser = new XmlSerializer(typeof(SceneDescription));
			ser.Serialize(s, this);
		}
		public void Save (string filename) {
			FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write);
			this.Save(fs);
			fs.Close();
		}
		
	}
}

