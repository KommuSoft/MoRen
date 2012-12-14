//  
//  Matrix4.cs
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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Renderer {
	
	public delegate void MatrixManipulator (Matrix4 m);
	
	[XmlType("Matrix")]
	public sealed class Matrix4 : ITransformable {

		[XmlIgnore]
		private static readonly Regex
			rgx = new Regex(string.Format(@"^((?<id>Id(entity)?)|(?<rotxyz>Rotate ?(?<dim>X|Y|Z) {0})|(?<rotv>Rotate {1} {2} {3} {0})|(?<shift>Shift {1} {2} {3})|(?<scale>Scale {1}( {2} {3})?)|(?<matrix>Matrix {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}))$", ParserUtils.FormGroup(ParserUtils.Real, "theta"), ParserUtils.FormGroup(ParserUtils.Real, "ux"), ParserUtils.FormGroup(ParserUtils.Real, "uy"), ParserUtils.FormGroup(ParserUtils.Real, "uz"), ParserUtils.FormGroup(ParserUtils.Real, "m10"), ParserUtils.FormGroup(ParserUtils.Real, "m11"), ParserUtils.FormGroup(ParserUtils.Real, "m12"), ParserUtils.FormGroup(ParserUtils.Real, "m13"), ParserUtils.FormGroup(ParserUtils.Real, "m20"), ParserUtils.FormGroup(ParserUtils.Real, "m21"), ParserUtils.FormGroup(ParserUtils.Real, "m22"), ParserUtils.FormGroup(ParserUtils.Real, "m23")), RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.ExplicitCapture);
		
		[XmlAttribute("M00")]
		public double
			M00 = 1.0d;
		[XmlAttribute("M01")]
		public double
			M01 = 0.0d;
		[XmlAttribute("M02")]
		public double
			M02 = 0.0d;
		[XmlAttribute("M03")]
		public double
			M03 = 0.0d;
		[XmlAttribute("M10")]
		public double
			M10 = 0.0d;
		[XmlAttribute("M11")]
		public double
			M11 = 1.0d;
		[XmlAttribute("M12")]
		public double
			M12 = 0.0d;
		[XmlAttribute("M13")]
		public double
			M13 = 0.0d;
		[XmlAttribute("M20")]
		public double
			M20 = 0.0d;
		[XmlAttribute("M21")]
		public double
			M21 = 0.0d;
		[XmlAttribute("M22")]
		public double
			M22 = 1.0d;
		[XmlAttribute("M23")]
		public double
			M23 = 0.0d;
		
		public Matrix4 () {
		}
		public Matrix4 (Point3 c1, Point3 c2, Point3 c3) {
			this.M00 = c1.X;
			this.M10 = c1.Y;
			this.M20 = c1.Z;
			this.M01 = c2.X;
			this.M11 = c2.Y;
			this.M21 = c2.Z;
			this.M02 = c3.X;
			this.M12 = c3.Y;
			this.M22 = c3.Z;
		}
		public Matrix4 (Point3 c1, Point3 c2, Point3 c3, Point3 c4) {
			this.M00 = c1.X;
			this.M10 = c1.Y;
			this.M20 = c1.Z;
			this.M01 = c2.X;
			this.M11 = c2.Y;
			this.M21 = c2.Z;
			this.M02 = c3.X;
			this.M12 = c3.Y;
			this.M22 = c3.Z;
			this.M03 = c4.X;
			this.M13 = c4.Y;
			this.M23 = c4.Z;
		}
		public Matrix4 (double M00, double M01, double M02, double M03, double M10, double M11, double M12, double M13, double M20, double M21, double M22, double M23) {
			this.M00 = M00;
			this.M01 = M01;
			this.M02 = M02;
			this.M03 = M03;
			this.M10 = M10;
			this.M11 = M11;
			this.M12 = M12;
			this.M13 = M13;
			this.M20 = M20;
			this.M21 = M21;
			this.M22 = M22;
			this.M23 = M23;
		}
		public Matrix4 (Matrix4 s) {
			this.M00 = s.M00;
			this.M01 = s.M01;
			this.M02 = s.M02;
			this.M03 = s.M03;
			this.M10 = s.M10;
			this.M11 = s.M11;
			this.M12 = s.M12;
			this.M13 = s.M13;
			this.M20 = s.M20;
			this.M21 = s.M21;
			this.M22 = s.M22;
			this.M23 = s.M23;
		}
		public void LoadColumns (Point3 c1, Point3 c2, Point3 c3) {
			this.M00 = c1.X;
			this.M10 = c1.Y;
			this.M20 = c1.Z;
			this.M01 = c2.X;
			this.M11 = c2.Y;
			this.M21 = c2.Z;
			this.M02 = c3.X;
			this.M12 = c3.Y;
			this.M22 = c3.Z;
			this.M03 = 0.0d;
			this.M13 = 0.0d;
			this.M23 = 0.0d;
		}
		public void LoadMatrix (Matrix4 M) {
			this.M00 = M.M00;
			this.M01 = M.M01;
			this.M02 = M.M02;
			this.M03 = M.M03;
			this.M10 = M.M00;
			this.M11 = M.M01;
			this.M12 = M.M02;
			this.M13 = M.M03;
			this.M20 = M.M00;
			this.M21 = M.M01;
			this.M22 = M.M02;
			this.M23 = M.M03;
		}
		public void Reset () {
			this.M00 = 1.0d;
			this.M01 = 0.0d;
			this.M02 = 0.0d;
			this.M03 = 0.0d;
			this.M10 = 0.0d;
			this.M11 = 1.0d;
			this.M12 = 0.0d;
			this.M13 = 0.0d;
			this.M20 = 0.0d;
			this.M21 = 0.0d;
			this.M22 = 1.0d;
			this.M23 = 0.0d;
		}
		public void Invert () {
			double af = this.M00*this.M11, ag = this.M00*this.M12, be = this.M01*this.M10;
			double bg = this.M01*this.M12, ce = this.M02*this.M10, cf = this.M02*this.M11;
			double invdet = 1.0f/(this.M20*(bg-cf)+this.M21*(ce-ag)+this.M22*(af-be));
			double N00 = (this.M11*this.M22-this.M12*this.M21)*invdet, N01 = (this.M02*this.M21-this.M01*this.M22)*invdet, N02 = (bg-cf)*invdet, N03 = -(N00*this.M03+N01*this.M13+N02*this.M23);
			double N10 = (this.M12*this.M20-this.M10*this.M22)*invdet, N11 = (this.M00*this.M22-this.M02*this.M20)*invdet, N12 = (ce-ag)*invdet, N13 = -(N10*this.M03+N11*this.M13+N12*this.M23);
			double N20 = (this.M10*this.M21-this.M11*this.M20)*invdet, N21 = (this.M01*this.M20-this.M00*this.M21)*invdet, N22 = (af-be)*invdet, N23 = -(N20*this.M03+N21*this.M13+N22*this.M23);
			this.M00 = N00;
			this.M01 = N01;
			this.M02 = N02;
			this.M03 = N03;
			this.M10 = N10;
			this.M11 = N11;
			this.M12 = N12;
			this.M13 = N13;
			this.M20 = N20;
			this.M21 = N21;
			this.M22 = N22;
			this.M23 = N23;
		}
		public static Matrix4 CreateRotateMatrix (double ux, double uy, double uz, double theta) {
			double cost = Math.Cos(theta);
			double costa = 1.0d-cost;
			double sint = Math.Sin(theta);
			return new Matrix4(
				cost+costa*ux*ux, ux*uy*costa-uz*sint, ux*uz*costa+uy*sint, 0.0d,
				ux*uy*costa+uz*sint, cost+costa*uy*uy, uy*uz*costa-ux*sint, 0.0d,
				ux*uz*costa-uy*sint, uz*uy*costa+ux*sint, cost+costa*uz*uz, 0.0d
			);
		}
		public static Matrix4 CreateRotateMatrix (Point3 p, double theta) {
			return CreateRotateMatrix(p.X, p.Y, p.Z, theta);
		}
		public void Rotate (double ux, double uy, double uz, double theta) {
			double cost = Math.Cos(theta);
			double costa = 1.0d-cost;
			double sint = Math.Sin(theta);
			double r0 = cost+costa*ux*ux, r1 = ux*uy*costa-uz*sint, r2 = ux*uz*costa+uy*sint;
			double N00 = r0*M00+r1*M10+r2*M20, N01 = r0*M01+r1*M11+r2*M21, N02 = r0*M02+r1*M12+r2*M22, N03 = r0*M03+r1*M13+r2*M23;
			r0 = ux*uy*costa+uz*sint;
			r1 = cost+costa*uy*uy;
			r2 = uy*uz*costa-ux*sint;
			double N10 = r0*M00+r1*M10+r2*M20, N11 = r0*M01+r1*M11+r2*M21, N12 = r0*M02+r1*M12+r2*M22, N13 = r0*M03+r1*M13+r2*M23;
			r0 = ux*uz*costa-uy*sint;
			r1 = uz*uy*costa+ux*sint;
			r2 = cost+costa*uz*uz;
			this.M20 = r0*M00+r1*M10+r2*M20;
			this.M21 = r0*M01+r1*M11+r2*M21;
			this.M22 = r0*M02+r1*M12+r2*M22;
			this.M23 = r0*M03+r1*M13+r2*M23;
			this.M00 = N00;
			this.M01 = N01;
			this.M02 = N02;
			this.M03 = N03;
			this.M10 = N10;
			this.M11 = N11;
			this.M12 = N12;
			this.M13 = N13;
		}
		public void RotateX (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double N10 = M10*cost-M20*sint, N11 = M11*cost-M21*sint, N12 = M12*cost-M22*sint, N13 = M13*cost-M23*sint;
			this.M20 = M20*cost+M10*sint;
			this.M21 = M21*cost+M11*sint;
			this.M22 = M22*cost+M12*sint;
			this.M23 = M23*cost+M13*sint;
			this.M10 = N10;
			this.M11 = N11;
			this.M12 = N12;
			this.M13 = N13;
		}
		public static Matrix4 CreateRotateXMatrix (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			return new Matrix4(1.0d, 0.0d, 0.0d, 0.0d, 0.0d, cost, -sint, 0.0d, 0.0d, sint, cost, 0.0d);
		}
		public void RotateY (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double N00 = M00*cost+M20*sint, N01 = M01*cost+M21*sint, N02 = M02*cost+M22*sint, N03 = M03*cost+M23*sint;
			this.M20 = M20*cost-M00*sint;
			this.M21 = M21*cost-M01*sint;
			this.M22 = M22*cost-M02*sint;
			this.M23 = M23*cost-M03*sint;
			this.M00 = N00;
			this.M01 = N01;
			this.M02 = N02;
			this.M03 = N03;
		}
		public static Matrix4 CreateRotateYMatrix (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			return new Matrix4(cost, 0.0d, sint, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, -sint, 0.0d, cost, 0.0d);
		}
		public void RotateZ (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			double N00 = M00*cost-M10*sint, N01 = M01*cost-M11*sint, N02 = M02*cost-M12*sint, N03 = M03*cost-M13*sint;
			this.M10 = M10*cost+M00*sint;
			this.M11 = M11*cost+M01*sint;
			this.M12 = M12*cost+M02*sint;
			this.M13 = M13*cost+M03*sint;
			this.M00 = N00;
			this.M01 = N01;
			this.M02 = N02;
			this.M03 = N03;
		}
		public static Matrix4 CreateRotateZMatrix (double theta) {
			double cost = Math.Cos(theta);
			double sint = Math.Sin(theta);
			return new Matrix4(cost, -sint, 0.0d, 0.0d, sint, cost, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d);
		}
		public void Shift (double dx, double dy, double dz) {
			this.M03 += dx;
			this.M13 += dy;
			this.M23 += dz;
		}
		public static Matrix4 CreateShiftMatrix (double dx, double dy, double dz) {
			return new Matrix4(1.0d, 0.0d, 0.0d, dx, 0.0d, 1.0d, 0.0d, dy, 0.0d, 0.0d, 1.0d, dz);
		}
		public static Matrix4 CreateShiftMatrix (Point3 p) {
			return CreateShiftMatrix(p.X, p.Y, p.Z);
		}
		public void Shift (Point3 p) {
			this.M03 += p.X;
			this.M13 += p.Y;
			this.M23 += p.Z;
		}
		public void Scale (double sx, double sy, double sz) {
			this.M00 *= sx;
			this.M01 *= sx;
			this.M02 *= sx;
			this.M03 *= sx;
			this.M10 *= sy;
			this.M11 *= sy;
			this.M12 *= sy;
			this.M13 *= sy;
			this.M20 *= sz;
			this.M21 *= sz;
			this.M22 *= sz;
			this.M23 *= sz;
		}
		public static Matrix4 CreateScaleMatrix (double sx, double sy, double sz) {
			return new Matrix4(sx, 0.0d, 0.0d, 0.0d, 0.0d, sy, 0.0d, 0.0d, 0.0d, 0.0d, sz, 1.0d);
		}
		public static Matrix4 CreateScaleMatrix (Point3 p) {
			return CreateScaleMatrix(p.X, p.Y, p.Z);
		}
		public static Matrix4 CreateScaleMatrix (double s) {
			return CreateScaleMatrix(s, s, s);
		}
		public void Transform (Matrix4 N) {
			double n00 = M00*N.M00+M10*N.M01+M20*N.M02;
			double n01 = M01*N.M00+M11*N.M01+M21*N.M02;
			double n02 = M02*N.M00+M12*N.M01+M22*N.M02;
			double n03 = M03*N.M00+M13*N.M01+M23*N.M02+N.M03;
			double n10 = M00*N.M10+M10*N.M11+M20*N.M12;
			double n11 = M01*N.M10+M11*N.M11+M21*N.M12;
			double n12 = M02*N.M10+M12*N.M11+M22*N.M12;
			double n13 = M03*N.M10+M13*N.M11+M23*N.M12+N.M13;
			double n20 = M00*N.M20+M10*N.M21+M20*N.M22;
			double n21 = M01*N.M20+M11*N.M21+M21*N.M22;
			double n22 = M02*N.M20+M12*N.M21+M22*N.M22;
			double n23 = M03*N.M20+M13*N.M21+M23*N.M22+N.M23;
			this.M00 = n00;
			this.M01 = n01;
			this.M02 = n02;
			this.M03 = n03;
			this.M10 = n10;
			this.M11 = n11;
			this.M12 = n12;
			this.M13 = n13;
			this.M20 = n20;
			this.M21 = n21;
			this.M22 = n22;
			this.M23 = n23;
		}
		public Matrix4 CopyTransform (Matrix4 N) {
			double n00 = M00*N.M00+M10*N.M01+M20*N.M02;
			double n01 = M01*N.M00+M11*N.M01+M21*N.M02;
			double n02 = M02*N.M00+M12*N.M01+M22*N.M02;
			double n03 = M03*N.M00+M13*N.M01+M23*N.M02+N.M03;
			double n10 = M00*N.M10+M10*N.M11+M20*N.M12;
			double n11 = M01*N.M10+M11*N.M11+M21*N.M12;
			double n12 = M02*N.M10+M12*N.M11+M22*N.M12;
			double n13 = M03*N.M10+M13*N.M11+M23*N.M12+N.M13;
			double n20 = M00*N.M20+M10*N.M21+M20*N.M22;
			double n21 = M01*N.M20+M11*N.M21+M21*N.M22;
			double n22 = M02*N.M20+M12*N.M21+M22*N.M22;
			double n23 = M03*N.M20+M13*N.M21+M23*N.M22+N.M23;
			return new Matrix4(n00, n01, n02, n03, n10, n11, n12, n13, n20, n21, n22, n23);
		}
		public static void TransformMatrix (Matrix4 source, Matrix4 manipulator) {
			source.Transform(manipulator);
		}
		public override string ToString () {
			return string.Format("Matrix {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}", M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23);
		}
		public override bool Equals (object obj) {
			if(obj is Matrix4) {
				Matrix4 N = (Matrix4)obj;
				return (this.M00 == N.M00 && this.M01 == N.M01 && this.M02 == N.M02 && this.M03 == N.M03 &&
					this.M10 == N.M10 && this.M11 == N.M11 && this.M12 == N.M12 && this.M13 == N.M13 &&
					this.M20 == N.M20 && this.M21 == N.M21 && this.M22 == N.M22 && this.M23 == N.M23);
			}
			else {
				return false;
			}
		}
		public override int GetHashCode () {
			return this.M00.GetHashCode()^this.M01.GetHashCode()^this.M02.GetHashCode()^this.M03.GetHashCode()
				^this.M10.GetHashCode()^this.M11.GetHashCode()^this.M12.GetHashCode()^this.M13.GetHashCode()
				^this.M20.GetHashCode()^this.M21.GetHashCode()^this.M22.GetHashCode()^this.M23.GetHashCode();
		}
		public static Matrix4 InterpolateParse (string matrix1, double time1, string matrix2, double time2, double time) {
			Match m1 = null, m2 = null;
			if(matrix1 != null) {
				matrix1 = matrix1.Trim().ToLower();
				m1 = rgx.Match(matrix1);
			}
			if(matrix2 != null) {
				matrix2 = matrix2.Trim().ToLower();
				m2 = rgx.Match(matrix1);
			}
			if(matrix2 == null || matrix2 == string.Empty || time <= time1 || time2 <= time1 || !m2.Success) {
				return Parse(matrix1);
			}
			else if(matrix1 == null || matrix1 == string.Empty || time >= time2 || !m1.Success) {
				return Parse(matrix2);
			}
			else {
				double a2 = (time-time1)/(time2-time1);
				double a1 = 1.0d-a2;
				if(m1.Groups["rotxyz"].Captures > 0x00 && m2.Groups["rotxyz"].Captures > 0x00 && m1.Groups["dim"].Value == m2.Groups["dim"].Value) {//same rotation, different angle
					double theta = a1*double.Parse(m1.Groups["theta"].Value)+a2*double.Parse(m2.Groups["theta"].Value);
					switch(m1.Groups["dim"].Value) {
						case "x":
							return Matrix4.CreateRotateXMatrix(theta);
						case "y":
							return Matrix4.CreateRotateYMatrix(theta);
						default :
							return Matrix4.CreateRotateZMatrix(theta);
					}
				}
				else if(m1.Groups["shift"].Captures.Count > 0x00 && m2.Groups["shift"].Captures.Count > 0x00) {
					Point3 u = new Point3(a1*double.Parse(m1.Groups["ux"].Value)+a2*double.Parse(m2.Groups["ux"].Value),
					                      a1*double.Parse(m1.Groups["uy"].Value)+a2*double.Parse(m2.Groups["uy"].Value),
					                      a1*double.Parse(m1.Groups["uz"].Value)+a2*double.Parse(m2.Groups["uz"].Value));
					return CreateShiftMatrix(u);
				}
				else if(m1.Groups["scale"].Captures.Count > 0x00 && m2.Groups["scale"].Captures.Count > 0x00) {
					Point3 u;
					if(m1.Groups["uy"].Captures.Count > 0x00) {
						u = new Point3(a1*double.Parse(m1.Groups["ux"].Value)+a2*double.Parse(m2.Groups["ux"].Value),
						               a1*double.Parse(m1.Groups["uy"].Value)+a2*double.Parse(m2.Groups["uy"].Value),
						               a1*double.Parse(m1.Groups["uz"].Value)+a2*double.Parse(m2.Groups["uz"].Value));
					}
					else {
						double xyz = a1*double.Parse(m1.Groups["ux"].Value)+a2*double.Parse(m2.Groups["ux"].Value);
						u = new Point3(xyz, xyz, xyz);
					}
					return CreateScaleMatrix(u);
				}
				else {
					return Parse(matrix1);
				}
			}
		}
		public static Matrix4 Parse (string toParse) {
			if(toParse == null || toParse == string.Empty) {
				return null;
			}
			toParse = toParse.Trim().ToLower();
			Match m = rgx.Match(toParse);
			if(m.Success) {
				if(m.Groups["id"].Captures.Count > 0x00) {
					return new Matrix4();
				}
				else if(m.Groups["rotxyz"].Captures.Count > 0x00) {
					double theta = double.Parse(m.Groups["theta"].Value);
					switch(m.Groups["dim"].Value) {
						case "x":
							return Matrix4.CreateRotateXMatrix(theta);
						case "y":
							return Matrix4.CreateRotateYMatrix(theta);
						default :
							return Matrix4.CreateRotateZMatrix(theta);
					}
				}
				else if(m.Groups["rotv"].Captures.Count > 0x00) {
					double theta = double.Parse(m.Groups["theta"].Value);
					Point3 u = new Point3(double.Parse(m.Groups["ux"].Value), double.Parse(m.Groups["uy"].Value), double.Parse(m.Groups["uz"].Value));
					u.Normalize();
					return CreateRotateMatrix(u, theta);
				}
				else if(m.Groups["shift"].Captures.Count > 0x00) {
					Point3 u = new Point3(double.Parse(m.Groups["ux"].Value), double.Parse(m.Groups["uy"].Value), double.Parse(m.Groups["uz"].Value));
					return CreateShiftMatrix(u);
				}
				else if(m.Groups["scale"].Captures.Count > 0x00) {
					Point3 u;
					if(m.Groups["uy"].Captures.Count > 0x00) {
						u = new Point3(double.Parse(m.Groups["ux"].Value), double.Parse(m.Groups["uy"].Value), double.Parse(m.Groups["uz"].Value));
					}
					else {
						double xyz = double.Parse(m.Groups["ux"].Value);
						u = new Point3(xyz, xyz, xyz);
					}
					return CreateScaleMatrix(u);
				}
				else if(m.Groups["matrix"].Captures.Count > 0x00) {
					double m00 = double.Parse(m.Groups["theta"].Value);
					double m01 = double.Parse(m.Groups["ux"].Value);
					double m02 = double.Parse(m.Groups["uy"].Value);
					double m03 = double.Parse(m.Groups["uz"].Value);
					double m10 = double.Parse(m.Groups["m10"].Value);
					double m11 = double.Parse(m.Groups["m11"].Value);
					double m12 = double.Parse(m.Groups["m12"].Value);
					double m13 = double.Parse(m.Groups["m13"].Value);
					double m20 = double.Parse(m.Groups["m10"].Value);
					double m21 = double.Parse(m.Groups["m11"].Value);
					double m22 = double.Parse(m.Groups["m12"].Value);
					double m23 = double.Parse(m.Groups["m13"].Value);
					return new Matrix4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
				}
			}
			return null;
		}
		
	}
}

