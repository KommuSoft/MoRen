using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Renderer {
	public class LoaderObj : MeshLoader {
		
		private delegate void LineAction (Match m);

		private static readonly Regex rgx_com = new Regex(@"#.*", RegexOptions.Compiled);
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
		private Material material = Material.DefaultMaterial;

		public Material Material {
			get {
				return this.material;
			}
			set {
				this.material = value;
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
		public List<RenderItem> Inject (Matrix4 transform, params string[] args) {
			Point3[] pos = this.pos.Select(x => new Point3(x, transform)).ToArray();
			Point3[] nor = this.nor.Select(x => new Point3(x)).ToArray();
			for(int i = 0; i < nor.Length; i++) {
				nor[i].TransformNonShift(transform);
				nor[i].Normalize();
			}
			Point3[] tex = this.tex.ToArray();
			List<RenderItem> ris = new List<RenderItem>(this.tri.Count);
			foreach(int[] seq in this.tri) {
				ris.Add(new Triangle(ItemOrNull(pos, seq[0x00]),
				                     ItemOrNull(pos, seq[0x03]),
				                     ItemOrNull(pos, seq[0x06]),
				                     ItemOrNull(nor, seq[0x01]),
				                     ItemOrNull(nor, seq[0x04]),
				                     ItemOrNull(nor, seq[0x07]),
				                     ItemOrNull(tex, seq[0x02]),
				                     ItemOrNull(tex, seq[0x05]),
				                     ItemOrNull(tex, seq[0x08]), this.material)
				);
			}
			return ris;
		}
		private static T ItemOrNull<T> (T[] items, int index) where T : class {
			if(index < 0x00) {
				return null;
			}
			else {
				return items[index-0x01];
			}
		}
		private static int ParseIntOrMinus (string s) {
			int res;
			if(int.TryParse(s, out res)) {
				return res;
			}
			return -0x01;
		}

		public void Load (string currentDir, Stream stream) {
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
			checkTexture();
			double x0, x1, y0, y1, z0, z1;
			Utils.CalculateBoundingBox(this.pos, out x0, out x1, out y0, out y1, out z0, out z1);
			Utils.UnitBox(this.pos);
			//Console.WriteLine("0.5*({0},{2},{4})+0.5*({1},{3},{5})", x0, x1, y0, y1, z0, z1);
		}

		private void checkTexture () {
			if(this.tex.Count <= 0x00) {
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

