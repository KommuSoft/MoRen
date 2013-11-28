//
//  EnvironmentSettings.cs
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
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Renderer.SceneBuilding;

namespace Renderer.SceneBuilding {

	[XmlType("EnvironmentSettings")]
	public sealed class EnvironmentSettings {

		[XmlElement("AmbientColor")]
		public ColorWrapper
			AmbientColor = new ColorWrapper(0x101010);
		[XmlAttribute("RecursionDepth")]
		public uint
			RecursionDepth = 0x01;
		[XmlAttribute("LightTest")]
		public uint
			LightTest = 0x01;
		[XmlAttribute("AntiAliasingSqt")]
		public uint
			AntiAliasingSqrt = 0x01;
		[XmlAttribute("Dispersion")]
		public double
			Dispersion = 0.0d;
		[XmlAttribute("DispersionAntiAliasSqt")]
		public uint
			DispersingAntiAliasingSqrt = 0x01;
		[XmlElement("EnvironmentMap")]
		public EnvMapWrapper
			envmapWrapper;
		[XmlIgnore]
		private EnviromentMap
			envmapCache;
		[XmlAttribute("DistanceUnit")]
		public double
			DistanceUnit = 255000000.0d;
		[XmlIgnore]
		public EnviromentMap EnvironmentMap {
			[MethodImpl(MethodImplOptions.Synchronized)]
			get {
				if(envmapCache == null && envmapWrapper != null) {
					this.envmapCache = envmapWrapper.ToEnvironmentMap();
				}
				return envmapCache;
			}
		}

		public EnvironmentSettings () {
		}
		public EnvironmentSettings (uint ambientcolor = 0x101010, uint recursionDepth = 0x01, uint lightTest = 0x01, uint antialiasingSqrt = 0x01, double dispersion = 0.0d, uint dispersionAntialias = 0x01) {
			this.AmbientColor = new ColorWrapper(ambientcolor);
			this.RecursionDepth = recursionDepth;
			this.LightTest = lightTest;
			this.AntiAliasingSqrt = antialiasingSqrt;
			this.DispersingAntiAliasingSqrt = dispersionAntialias;
		}

	}
}

