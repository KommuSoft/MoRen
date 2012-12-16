//
//  SuperCamera.cs
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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Renderer.SceneBuilding {

	[XmlType("SuperCamera")]
	public class SuperCamera {

		private double closureTime = 2.0d;
		private int timeSamples = 0x01;
		private SuperCameraTask task = SuperCameraTask.MakeImage;
		private string outputFile;
		private double t0, t1;

		[XmlAttribute("ClosureTime")]
		public double ClosureTime {
			get {
				return this.closureTime;
			}
			set {
				this.closureTime = value;
			}
		}
		[XmlAttribute("T0")]
		public double T0 {
			get {
				return this.t0;
			}
			set {
				this.t0 = value;
			}
		}
		[XmlAttribute("T1")]
		public double T1 {
			get {
				return this.t1;
			}
			set {
				this.t1 = value;
			}
		}
		[XmlAttribute("TimeSamples")]
		public int TimeSamples {
			get {
				return this.timeSamples;
			}
			set {
				this.timeSamples = value;
			}
		}
		[XmlAttribute("OutputFile")]
		public string OutputFile {
			get {
				return this.outputFile;
			}
			set {
				this.outputFile = value;
			}
		}
		[XmlAttribute("Task")]
		public SuperCameraTask Task {
			get {
				return this.task;
			}
			set {
				this.task = value;
			}
		}

		public SuperCamera () {
		}

		private Camera buildCameraCalculateAt (SceneDescription description, double time) {
			Tuple<List<RenderItem>,List<Light>> scene = description.SceneGraph.Inject(time);
			List<RenderItem> ris = scene.Item1;
			Light[] lights = scene.Item2.ToArray();
			EnvironmentSettings es = description.EnvironmentSettings;
#if DEBUG
			DateTime old = DateTime.Now;
#endif
			IAccelerator acc = description.AcceleratorWrapper.CreateAccelerator(ris);
#if DEBUG
			DateTime now = DateTime.Now;
			Console.Write("{0}\t", (now-old).TotalMilliseconds.ToString("0.000"));
#endif
			Camera cam = description.CameraWrapper.Camera(acc, lights, es);
			cam.CalculateImage();
			return cam;
		}
		private void clearTmpFolder () {
			foreach(var file in Directory.EnumerateFiles ("/tmp", "output*")) {
				try {
					File.Delete(file);
				}
				catch(Exception e) {
					Console.Error.WriteLine(e);
				}
			}
		}
		private void convertToMovie () {
			Process proc = new Process();
			proc.StartInfo.FileName = "ffmpeg";
			proc.StartInfo.Arguments = string.Format("-y -i /tmp/output%05d.jpg -vcodec mpeg4 {0}", this.outputFile);
			proc.Start();
			proc.WaitForExit();
		}
		private void fillMotionBlurCache (SceneDescription sd, Texture[] motionblurCache, CacheTexture sum, double t0, double dt) {
			double t = t0-dt;
			Texture tex;
			for(int i = motionblurCache.Length-0x02; i >= 0x00; i--) {
				tex = buildCameraCalculateAt(sd, t).Raster;
				motionblurCache[i] = tex;
				sum.AddTexture(tex);
				t -= dt;
			}
		}

		public void Execute (SceneDescription description) {
			double min = Math.Max(this.T0, description.SceneGraph.T0);
			double max = Math.Min(this.T1, description.SceneGraph.T1);
			double dt = (max-min)/this.TimeSamples;
			uint nDelta = (uint)Math.Round(this.ClosureTime/dt)+0x01;
			Texture[] motionblurCache = new Texture[nDelta];
			CacheTexture blurCache = new CacheTexture((int)description.CameraWrapper.Width, (int)description.CameraWrapper.Height);
			Texture tex;
			this.fillMotionBlurCache(description, motionblurCache, blurCache, min, dt);
			if(this.Task == SuperCameraTask.MakeImage) {
				blurCache.AddTexture(this.buildCameraCalculateAt(description, min).Raster);
				blurCache.MixWithAlpha(nDelta).Save(this.outputFile);
			}
			else if(this.Task == SuperCameraTask.MakeMovie) {
				this.clearTmpFolder();
				int index = 0;
				Process proc = new Process();
				proc.StartInfo.FileName = "convert";
				string imagename;
				string jpegname;
				uint j = nDelta-0x01;
				for(double t = min; t <= max; t += dt) {
					imagename = string.Format("/tmp/output{0}.png", index.ToString("00000"));
					jpegname = string.Format("{0} /tmp/output{1}.jpg", imagename, index.ToString("00000"));
					tex = this.buildCameraCalculateAt(description, t).Raster;
					blurCache.RemoveTexture(motionblurCache[j]);
					blurCache.AddTexture(tex);
					blurCache.MixWithAlpha(nDelta).Save(imagename);
					motionblurCache[j++] = tex;
					j %= nDelta;
					index++;
					proc.WaitForExit();
					proc.StartInfo.Arguments = jpegname;
					proc.Start();
				}
				proc.WaitForExit();
				this.convertToMovie();
				this.clearTmpFolder();
			}
		}

	}
}