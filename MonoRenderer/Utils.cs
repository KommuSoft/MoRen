//
//  Utils.cs
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
using System.IO;
using System.Text;

namespace Renderer {
	public static class Utils
	{

		public static string ReadZeroEndByteString (BinaryReader br) {
			StringBuilder sb = new StringBuilder();
			byte val = br.ReadByte();
			while(val != 0x00) {
				sb.Append((char)val);
				val = br.ReadByte();
			}
			return sb.ToString();
		}

		public static void ReadPoint3DFloatList (BinaryReader br, IList<Point3> drain) {
			int n = br.ReadInt16();
			for(int i = 0; i < n; i++) {
				float x = br.ReadSingle();
				float y = br.ReadSingle();
				float z = br.ReadSingle();
				drain.Add(new Point3(x, y, z));
			}
		}
		
		public static void ReadPoint2DFloatList (BinaryReader br, IList<Point3> drain) {
			int n = br.ReadInt16();
			for(int i = 0; i < n; i++) {
				float u = br.ReadSingle();
				float v = br.ReadSingle();
				drain.Add(new Point3(u, v, 0.0d));
			}
		}

		public static T FloatIndex<T> (T[] values, double val) {
			val = Maths.Border(0.0d, val, 1.0d);
			return values [(int)Math.Floor(val*(values.Length-0x01))];
		}

		public static uint ReadColorChunk (BinaryReader br) {
			uint type = br.ReadUInt16();
			br.ReadInt32();
			switch(type) {
				case 0x0010:
					goto case 0x0013;
				case 0x0013:
					float r = br.ReadSingle();
					float g = br.ReadSingle();
					float b = br.ReadSingle();
					return Color.FromFrac(r, g, b);
				default:
					byte rb = br.ReadByte();
					byte gb = br.ReadByte();
					byte bb = br.ReadByte();
					return Color.GetColor(rb, gb, bb);
			}
		}
		public static IEnumerable<Tuple<ushort, ushort, ushort, ushort>> GenerateIndicesRange (int n) {
			for(ushort j = 0x02, k = 0x00; j < n; j++, k++) {
				yield return new Tuple<ushort, ushort, ushort, ushort>(k, (ushort)(k+0x01), (ushort)(k+0x02), 0x00);
			}
		}
		public static double CalculateOptimalDivision<T> (List<T> items, double x0, double x1, int dim, out double heuristic) where T : IRenderable {
			heuristic = double.NegativeInfinity;
			SortedSet<AddRemoveEvent> events = new SortedSet<AddRemoveEvent>();
			double xa, xr;
			double totalSurface = 0.0d;
			int i = 0x00;
			foreach(T item in items) {
				item.GetDimensionBounds(dim, out xa, out xr);
				events.Add(new AddRemoveEvent(i, xa, true));
				events.Add(new AddRemoveEvent(i, xr, false));
				totalSurface += item.Surface();
			}
			HashSet<int> activ = new HashSet<int>();
			HashSet<int> toRem = new HashSet<int>();
			double sl = 0.0d, sls = 0.0d;//surface left, surface left soft
			double xold = x0, x, xheu = double.NaN, tempheu;
			int index;
			foreach(AddRemoveEvent are in events) {
				index = are.Index;
				x = are.X;
				if(are.Remove) {
					if(!activ.Remove(index)) {
						toRem.Add(index);
					}
					sl += items [index].Surface();
				} else {
					if(!toRem.Remove(index)) {
						activ.Add(index);
					}
				}
				if(x > xold && x < x1) {
					xold = x;
					sls = 0.0d;
					foreach(int ind in activ) {
						sls += items [ind].SplitSurface(x, dim);
					}
					tempheu = (sl+sls)/(x-x0)+(totalSurface-sl-sls)/(x1-x);
					if(tempheu > heuristic) {
						heuristic = tempheu;
						xheu = x;
					}
				}
			}
			if(double.IsNaN(xheu)) {
				return 0.5d*(x0+x1);
			} else {
				return xheu;
			}
		}
		public static double CalculateOptimalDivision<U,T> (List<Tuple<U,T>> items, double x0, double x1, int dim, out double heuristic) where T : IRenderable {
			heuristic = double.NegativeInfinity;
			return 0.5d*(x0+x1);
			/*heuristic = double.PositiveInfinity;
			SortedSet<AddRemoveEvent> events = new SortedSet<AddRemoveEvent>();
			double xa, xr;
			double totalSurface = 0.0d;
			int i = 0x00;
			foreach(Tuple<U,T> item in items) {
				item.Item2.GetDimensionBounds(dim, out xa, out xr);
				events.Add(new AddRemoveEvent(i, xa, true));
				events.Add(new AddRemoveEvent(i++, xr, false));
				totalSurface += item.Item2.Surface();
			}
			HashSet<int> activ = new HashSet<int>();
			HashSet<int> toRem = new HashSet<int>();
			//double sl = 0.0d, sls = 0.0d;//surface left, surface left soft
			int N = items.Count;
			int ln = 0x00;
			double xold = x0, x, xheu = double.NaN, tempheu;
			int index;
			foreach(AddRemoveEvent are in events) {
				index = are.Index;
				x = are.X;
				if(are.Remove) {
					if(!activ.Remove(index)) {
						toRem.Add(index);
					}
					ln++;
				} else {
					if(!toRem.Remove(index)) {
						activ.Add(index);
					}
				}
				if(x > x0 && x < x1) {
					int lm = activ.Count;
					tempheu = Math.Max((ln+3*lm)/(x-x0), (N-ln+2*lm)/(x1-x));
					if(tempheu < heuristic) {
						heuristic = tempheu;
						xheu = x;
					}
				}
			}
			if(double.IsNaN(xheu)) {
				return 0.5d*(x0+x1);
			} else {
				return xheu;
			}*/
		}
		public static Matrix4 ReadMatrixChunk (BinaryReader br) {
			float t0 = br.ReadSingle();
			float t1 = br.ReadSingle();
			float t2 = br.ReadSingle();
			float t3 = br.ReadSingle();
			float t4 = br.ReadSingle();
			float t5 = br.ReadSingle();
			float t6 = br.ReadSingle();
			float t7 = br.ReadSingle();
			float t8 = br.ReadSingle();
			float t9 = br.ReadSingle();
			float ta = br.ReadSingle();
			float tb = br.ReadSingle();
			/*Matrix4 m = new Matrix4(t0, t3, t6, -t9,//Point3.Dot(t0, t3, t6, t9, t9, t9)
			                        t1, t4, t7, -ta,//Point3.Dot(t1, t4, t7, ta, ta, ta)
			                        t2, t5, t8, -tb);//Point3.Dot(t2, t5, t8, tb, tb, tb)*/
			//Matrix4 m = new Matrix4(t0, t1, t2, t0*t9+t1*ta+t2*tb,//
			//                        t3, t4, t5, t3*t9+t4*ta+t5*tb,//
			//                        t6, t7, t8, t6*t9+t7*ta+t8*tb);//
			//m.Invert();
			//p.Transform(m);
			Matrix4 m = new Matrix4();
			//m.Shift(Point3.Dot(t0, t3, t6, t9, ta, tb), Point3.Dot(t1, t4, t7, t9, ta, tb), Point3.Dot(t2, t5, t8, t9, ta, tb));
			//m.Shift(-Point3.Dot(t0, t3, t6, t9, ta, tb), -Point3.Dot(t1, t4, t7, t9, ta, tb), -Point3.Dot(t2, t5, t8, t9, ta, tb));
			//m.Shift(Point3.Dot(t0, t1, t2, t9, t9, t9), Point3.Dot(t3, t4, t5, t9, ta, tb), Point3.Dot(t6, t7, t8, t9, ta, tb));
			//m.Shift(-Point3.Dot(t0, t1, t2, t9, t9, t9), -Point3.Dot(t3, t4, t5, t9, ta, tb), -Point3.Dot(t6, t7, t8, t9, ta, tb));
			//m.Shift(Point3.Dot(t0, t3, t6, t9, t9, t9), Point3.Dot(t1, t4, t7, ta, ta, ta), Point3.Dot(t2, t5, t8, tb, tb, tb));
			//m.Shift(-Point3.Dot(t0, t3, t6, t9, t9, t9), -Point3.Dot(t1, t4, t7, ta, ta, ta), -Point3.Dot(t2, t5, t8, tb, tb, tb));
			//m.Shift(Point3.Dot(t0, t1, t2, t9, t9, t9), Point3.Dot(t3, t4, t5, ta, ta, ta), Point3.Dot(t6, t7, t8, t9, tb, tb));
			//m.Shift(-Point3.Dot(t0, t1, t2, t9, t9, t9), -Point3.Dot(t3, t4, t5, ta, ta, ta), -Point3.Dot(t6, t7, t8, tb, tb, tb));
			return m;
			/*Console.WriteLine("[{0}\t{1}\t{2}]", m00.ToString("0.000"), m10.ToString("0.000"), m20.ToString("0.000"));
			Console.WriteLine("[{0}\t{1}\t{2}]", m01.ToString("0.000"), m11.ToString("0.000"), m21.ToString("0.000"));
			Console.WriteLine("[{0}\t{1}\t{2}]", m02.ToString("0.000"), m12.ToString("0.000"), m22.ToString("0.000"));
			Console.WriteLine("[{0}\t{1}\t{2}]", m03.ToString("0.000"), m13.ToString("0.000"), m23.ToString("0.000"));//*/
			//return new Matrix4(m02, m01, m00, m03, m12, m11, m10, m13, m20, m21, m20, m03);
			/*Matrix4 m = new Matrix4(new Point3(m00, m10, m20), new Point3(m01, m11, m21), new Point3(m02, m12, m22));
			Point3 p = new Point3(m03, m13, m23);*/
			//m.Invert();
			/*p.Transform(m);
			m.Reset();
			m.Shift(p);*/
			//delta.TransformNonShift(m);
			//Console.WriteLine(m);
			//return new Matrix4();
			//return new Matrix4(new Point3(1.0d, 0.0d, 0.0d), new Point3(0.0d, 1.0d, 0.0d), new Point3(0.0d, 0.0d, 1.0d), delta);
			//m.Invert();*/
			//return new Matrix4(1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d);
			//return m;
			//return new Matrix4(m00, m01, m02, -m00*m03-m10*m13-m20*m23, m10, m11, m12, -m01*m03-m11*m13-m21*m23, m20, m21, m22, -m02*m03-m12*m13-m22*m23);
			//return new Matrix4(1.0d, 0.0d, 0.0d, -m03, 0.0d, 1.0d, 0.0d, -m13, 0.0d, 0.0d, 1.0d, -m23);
		}
		public static double ReadPercentageChunk (BinaryReader br) {
			ushort type = br.ReadUInt16();
			br.ReadInt32();
			switch(type) {
				case 0x0030:
					return br.ReadUInt16()*Maths.ToPercentage;
				case 0x0031:
					return br.ReadSingle()*Maths.ToPercentage;
				default :
					throw new FormatException("Expects percentage chunk!");
			}
		}

		public static void CalculateBoxHitpoint (Ray ray, Point3 inter, out double t, double x0, double x1, double y0, double y1, double z0, double z1) {
			t = double.PositiveInfinity;
			Point3 other = new Point3();
			calculateOffset(ray, inter, other, ref t, 0, x0, x1, 1, y0, y1, 2, z0, z1);
			calculateOffset(ray, inter, other, ref t, 1, y0, y1, 0, x0, x1, 2, z0, z1);
			calculateOffset(ray, inter, other, ref t, 2, z0, z1, 0, x0, x1, 1, y0, y1);
		}

		public static void NormalizeCollection (IEnumerable<Point3> pts) {
			double xa, xb, ya, yb, za, zb;
			Utils.CalculateNormalizationFactors(pts, out xa, out xb, out ya, out yb, out za, out zb);
			foreach(Point3 p in pts) {
				p.X = xa*(p.X+xb);
				p.Y = ya*(p.Y+yb);
				p.Z = za*(p.Z+zb);
			}
		}

		public static void CalculateNormalizationFactors (IEnumerable<Point3> pts, out double xa, out double xb, out double ya, out double yb, out double za, out double zb) {
			xa = double.NegativeInfinity;
			ya = xa;
			za = xa;
			xb = double.PositiveInfinity;
			yb = xb;
			zb = xb;
			foreach(Point3 p in pts) {
				xb = Math.Min(xb, p.X);
				yb = Math.Min(yb, p.Y);
				zb = Math.Min(zb, p.Z);
				xa = Math.Max(xa, p.X);
				ya = Math.Max(ya, p.Y);
				za = Math.Max(za, p.Z);
			}
			xb = -xb;
			yb = -yb;
			zb = -zb;
			xa = 1.0d/(xb+xa);
			ya = 1.0d/(yb+ya);
			za = 1.0d/(zb+za);
		}

		private static void calculateOffset (Ray ray, Point3 inter, Point3 other, ref double t, int xdim, double x0, double x1, int ydim, double y0, double y1, int zdim, double z0, double z1) {
			if(Math.Abs(ray.Direction [xdim]) >= Maths.GlobalEpsilon) {
				double dinv = 1.0d/ray.Direction [xdim];
				double tt = Maths.MinPos((x0-ray.Offset [xdim])*dinv, (x1-ray.Offset [xdim])*dinv);
				if(tt >= 0.0d && tt < t) {
					ray.PointAt(tt, other);
					double y = other [ydim], z = other [zdim];
					if(y0 <= y && y <= y1 && z0 <= z && z <= z1) {
						t = tt;
						inter.SetValues(other);
					}
				}
			}
		}

		public static IEnumerable<Tuple<int,T>> Mark<T> (this IEnumerable<T> source) {
			int i = 0x00;
			foreach(T elem in source) {
				yield return new Tuple<int,T>(i++, elem);
			}
		}

		public static void CalculateBoundingBox (IEnumerable<RenderItem> items, out double xtm, out double xtM, out double ytm, out double ytM, out double ztm, out double ztM) {
			xtm = double.PositiveInfinity;
			xtM = double.NegativeInfinity;
			ytm = xtm;
			ytM = xtM;
			ztm = xtm;
			ztM = xtM;
			double xm, xM, ym, yM, zm, zM;
			foreach(RenderItem ri in items) {
				ri.GetBounds(out xm, out xM, out ym, out yM, out zm, out zM);
				xtm = Math.Min(xtm, xm);
				xtM = Math.Max(xtM, xM);
				ytm = Math.Min(ytm, ym);
				ytM = Math.Max(ytM, yM);
				ztm = Math.Min(ztm, zm);
				ztM = Math.Max(ztM, zM);
			}
		}

		public static void CalculateBoundingBox (IEnumerable<Point3> items, out double xtm, out double xtM, out double ytm, out double ytM, out double ztm, out double ztM) {
			xtm = double.PositiveInfinity;
			xtM = double.NegativeInfinity;
			ytm = xtm;
			ytM = xtM;
			ztm = xtm;
			ztM = xtM;
			double xm, xM, ym, yM, zm, zM;
			foreach(Point3 p in items) {
				xtm = Math.Min(xtm, p.X);
				xtM = Math.Max(xtM, p.X);
				ytm = Math.Min(ytm, p.Y);
				ytM = Math.Max(ytM, p.Y);
				ztm = Math.Min(ztm, p.Z);
				ztM = Math.Max(ztM, p.Z);
			}
		}

		public static void ReadShortList (BinaryReader br, IList<short> drain, int mul) {
			int n = mul*br.ReadInt16();
			for(int i = 0; i < n; i++) {
				drain.Add(br.ReadInt16());
			}
		}
		public static void ReadUShortList (BinaryReader br, IList<ushort> drain, int mul) {
			int n = mul*br.ReadInt16();
			for(int i = 0; i < n; i++) {
				drain.Add(br.ReadUInt16());
			}
		}

		public static void ReadShortListLength (BinaryReader br, IList<short> drain, int n) {
			for(int i = 0; i < n; i++) {
				drain.Add(br.ReadInt16());
			}
		}
		public static IEnumerable<Point3> GenerateNormalizedCopies (IEnumerable<Point3> pt) {
			double xa, xb, ya, yb, za, zb;
			Utils.CalculateNormalizationFactors(pt, out xa, out xb, out ya, out yb, out za, out zb);
			foreach(Point3 p in pt) {
				yield return new Point3(xa*(p.X+xb), ya*(p.Y+yb), za*(p.Z+zb));
			}
		}

		public static IEnumerable<int> GenerateRange (int start, int stop) {
			for(int i = start; i < stop; i++) {
				yield return i;
			}
		}

	}
}

