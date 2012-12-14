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
using System.Xml.Serialization;

namespace Renderer {

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
	}
}