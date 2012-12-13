//
//  LoaderObj.cs
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
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Renderer {
	public class LoaderObj : MeshLoaderBase {
		
		private delegate void LineAction (Match m);

		private static readonly Regex rgx_pos = new Regex(@"v +([0-9.-]+) ([0-9.-]+) ([0-9.-]+)", RegexOptions.Compiled);
		private static readonly Regex rgx_tex = new Regex(@"vt +([0-9.-]+) ([0-9.-]+)", RegexOptions.Compiled);
		private static readonly Regex rgx_nor = new Regex(@"vn +([0-9.-]+) ([0-9.-]+) ([0-9.-]+)", RegexOptions.Compiled);
		private static readonly Regex rgx_fac = new Regex(@"f +(([0-9]*)(/([0-9]*)/([0-9]*))? ){3,}$", RegexOptions.Compiled);
		private static readonly NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
		private readonly List<Tuple<Regex,LineAction>> actions = new List<Tuple<Regex, LineAction>>();
		private List<Point3> pos = new List<Point3>();
		private List<Point3> tex = new List<Point3>();
		private List<Point3> nor = new List<Point3>();
		private List<int[]> tri = new List<int[]>();
		private Material defaultMaterial = Material.DefaultMaterial;

		public override Material DefaultMaterial {
			get {
				return this.defaultMaterial;
			}
			set {
				this.defaultMaterial = value;
			}
		}
		
		public LoaderObj () {
			InvokeActions();
		}

		private void InvokeActions () {
			actions.Add(new Tuple<Regex, LineAction>(rgx_pos, AddPos));
			actions.Add(new Tuple<Regex, LineAction>(rgx_tex, AddTex));
			actions.Add(new Tuple<Regex, LineAction>(rgx_nor, AddNor));
			actions.Add(new Tuple<Regex, LineAction>(rgx_fac, AddFac));
		}

		private void AddPos (Match m) {
			pos.Add(new Point3(double.Parse(m.Groups[1].Value, nfi), double.Parse(m.Groups[2].Value, nfi), double.Parse(m.Groups[3].Value, nfi)));
		}
		private void AddTex (Match m) {
			tex.Add(new Point3(double.Parse(m.Groups[2].Value, nfi), double.Parse(m.Groups[1].Value, nfi), 0.0d));
		}
		private void AddNor (Match m) {
			Point3 p = new Point3(double.Parse(m.Groups[1].Value, nfi), double.Parse(m.Groups[2].Value, nfi), double.Parse(m.Groups[3].Value, nfi));
			p.Normalize();
			nor.Add(p);
		}
		private void AddFac (Match m) {
			CaptureCollection cc = m.Groups[0x01].Captures;
			int n = cc.Count, i0, i2, n0, n2, t0, t2;
			i0 = ParseIntOrMinus(m.Groups[0x02].Captures[n-0x01].Value);
			i2 = ParseIntOrMinus(m.Groups[0x02].Captures[0x00].Value);
			if(m.Groups[0x03].Captures.Count > 0x00) {
				t0 = ParseIntOrMinus(m.Groups[0x04].Captures[n-0x01].Value);
				t2 = ParseIntOrMinus(m.Groups[0x04].Captures[0x00].Value);
				n0 = ParseIntOrMinus(m.Groups[0x05].Captures[n-0x01].Value);
				n2 = ParseIntOrMinus(m.Groups[0x05].Captures[0x00].Value);
			}
			else {
				t0 = -0x01;
				t2 = -0x01;
				n0 = -0x01;
				n2 = -0x01;
			}
			int i1, t1 = -0x01, n1 = -0x01;
			for(int i = 0x01; i < n-0x01; i++) {
				i1 = i2;
				t1 = t2;
				n1 = n2;
				i2 = ParseIntOrMinus(m.Groups[0x02].Captures[i].Value);
				if(m.Groups[0x03].Captures.Count > 0x00) {
					t2 = ParseIntOrMinus(m.Groups[0x04].Captures[i].Value);
					n2 = ParseIntOrMinus(m.Groups[0x05].Captures[i].Value);
				}
				this.tri.Add(new int[] {i0,n0,t0,i1,n1,t1,i2,n2,t2});
			}
		}
		public override void Inject (List<RenderItem> ris, Matrix4 transform, params string[] args) {
			Point3[] pos = this.pos.Select(x => new Point3(x, transform)).ToArray();
			Point3[] nor = this.nor.Select(x => new Point3(x)).ToArray();
			for(int i = 0; i < nor.Length; i++) {
				nor[i].TransformNonShift(transform);
				nor[i].Normalize();
			}
			Point3[] tex = this.tex.ToArray();
			foreach(int[] seq in this.tri) {
				ris.Add(new Triangle(ItemOrNull(pos, seq[0x00]),
				                     ItemOrNull(pos, seq[0x03]),
				                     ItemOrNull(pos, seq[0x06]),
				                     ItemOrNull(nor, seq[0x01]),
				                     ItemOrNull(nor, seq[0x04]),
				                     ItemOrNull(nor, seq[0x07]),
				                     ItemOrNull(tex, seq[0x02]),
				                     ItemOrNull(tex, seq[0x05]),
				                     ItemOrNull(tex, seq[0x08]), this.defaultMaterial)
				);
			}
		}
		private static T ItemOrNull<T> (T[] items, int index) where T : class {
			if(index < 0x00) {
				return null;
			}
			else {
				return items[index];
			}
		}
		private static int ParseIntOrMinus (string s) {
			int res;
			if(int.TryParse(s, out res)) {
				return res-0x01;
			}
			return -0x01;
		}

		public override void Load (string currentDir, Stream stream) {
			TextReader tr = new StreamReader(stream);
			string line = tr.ReadLine();
			while(line != null) {
				line = line.Trim()+" ";
				Match m;
				foreach(Tuple<Regex,LineAction> trl in actions) {
					m = trl.Item1.Match(line);
					if(m.Success) {
						trl.Item2(m);
						break;
					}
				}
				line = tr.ReadLine();
			}
			tr.Close();
			checkNormTeX();
			double x0, x1, y0, y1, z0, z1;
			Utils.CalculateBoundingBox(this.pos, out x0, out x1, out y0, out y1, out z0, out z1);
		}

		private void checkNormTeX () {
			if(this.nor.Count <= 0x00) {
				foreach(int[] tri in this.tri) {
					tri[0x01] = tri[0x00];
					tri[0x04] = tri[0x03];
					tri[0x07] = tri[0x06];
				}
				this.nor = Utils.GenerateSmoothNormalsFromTriangles(this.pos, this.tri.Select(x => new Tuple<int,int,int>(x[0x00], x[0x03], x[0x06])));
			}
			if(this.tex.Count <= 0x00) {
				Console.WriteLine("+tex");
				foreach(int[] tri in this.tri) {
					tri[0x02] = tri[0x00];
					tri[0x05] = tri[0x03];
					tri[0x08] = tri[0x06];
				}
				this.tex = Utils.GenerateNormalizedCopies(this.pos).ToList();
			}
		}
		
		private void readline (string s, Material mat, Matrix4 transform) {
		}
		
	}
}

