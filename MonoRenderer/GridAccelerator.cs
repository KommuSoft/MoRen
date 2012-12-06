//
//  AcceleratorGrid.cs
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
using System.Text;
using System.Collections.Generic;

namespace Renderer {
	public sealed class GridAccelerator : Accelerator
	{

		private readonly RenderItem[] ris;
		private readonly long[] grid;
		private readonly int[]  ixyzM;
		private readonly int[] dtile;
		public readonly int XN, YN, ZN;
		private readonly int nextSlice, nextLine;
		public readonly double x0, dx, y0, dy, z0, dz, x1, y1, z1;

		public GridAccelerator (List<RenderItem> items) {
			grid = buildGrid(items, out XN, out YN, out ZN, out x0, out y0, out z0, out dx, out dy, out dz, out nextSlice, out nextLine, out ris);
			this.x1 = x0+dx*XN;
			this.y1 = y0+dy*YN;
			this.z1 = z0+dz*ZN;
			this.dtile = new int[] {this.nextSlice,this.nextLine,0x01};
			this.ixyzM = new int[] {this.XN,this.YN,this.ZN};
		}

		private static void Align (int xn, int yn, int zn, double x0, double y0, double z0, double dx, double dy, double dz, double x, double y, double z, out int ix, out int iy, out int iz) {
			ix = Math.Min(xn-1, Math.Max(0x00, (int)Math.Floor((x-x0)/dx)));
			iy = Math.Min(yn-1, Math.Max(0x00, (int)Math.Floor((y-y0)/dy)));
			iz = Math.Min(zn-1, Math.Max(0x00, (int)Math.Floor((z-z0)/dz)));
		}

		public RenderItem CalculateHit (Ray ray, out double tHit, double maxT) {
			Point3 inter = new Point3(ray.Offset);
			double t = 0.0d;
			tHit = maxT;
			if(x0 > inter.X || inter.X > x1 || y0 > inter.Y || inter.Y > y1 || z0 > inter.Z || inter.Z > z1) {
				Utils.CalculateBoxHitpoint(ray, inter, out t, this.x0, this.x1, this.y0, this.y1, this.z0, this.z1);
				if(t >= tHit) {
					return null;
				}
			}
			int[] ixyz = new int[] {
				Math.Min(XN-1, Math.Max(0x00, (int)Math.Floor((inter.X-x0)/dx))),
				Math.Min(YN-1, Math.Max(0x00, (int)Math.Floor((inter.Y-y0)/dy))),
				Math.Min(ZN-1, Math.Max(0x00, (int)Math.Floor((inter.Z-z0)/dz)))
			};
			RenderItem current = null;
			SortedSet<MigrationEvent> migrations = new SortedSet<MigrationEvent>();
			addMigrations(migrations, t, 0x00, inter.X, ixyz [0x00], x0, dx, ray.DX);
			addMigrations(migrations, t, 0x01, inter.Y, ixyz [0x01], y0, dy, ray.DY);
			addMigrations(migrations, t, 0x02, inter.Z, ixyz [0x02], z0, dz, ray.DZ);
			int tile = nextSlice*ixyz [0x00]+nextLine*ixyz [0x01]+ixyz [0x02];
			while(t < tHit) {
				long g = grid [tile];
				if(g != 0x00) {
					checkTile(ray, ref current, ref tHit, g);
				}
				MigrationEvent me = migrations.Min;
				migrations.Remove(me);
				t = me.t;
				me.Update();
				tile += me.sgn*dtile [me.dim];
				ixyz [me.dim] += me.sgn;
				if(t >= tHit || ixyz [me.dim] >= ixyzM [me.dim] || ixyz [me.dim] < 0x00) {
					return current;
				}
				migrations.Add(me);
				
			}
			return current;
		}
		
		private void checkTile (Ray ray, ref RenderItem current, ref double maxT, long refs) {
			long end = refs>>0x20;
			for(refs &= 0xffffffff; refs < end; refs++) {
				double t = this.ris [refs].HitAt(ray);
				if(t < maxT) {
					maxT = t;
					current = this.ris [refs];
				}
			}		
		}

		private void addMigrations (SortedSet<MigrationEvent> migs, double to, int dim, double xo, int curi, double x0, double dx, double rdx) {
			int migi = Maths.SoftSign(rdx);
			if(migi != 0x00) {
				int migi2 = (migi+0x01)>>0x01;
				double dto = ((migi2+curi)*dx+x0-xo)/rdx;
				double dt = Math.Abs(dx/rdx);
				migs.Add(new MigrationEvent(to+dto, dt, dim, migi));
			}
		}

		public long[] buildGrid (List<RenderItem> items, out int xn, out int yn, out int zn, out double x0, out double y0, out double z0, out double dx, out double dy, out double dz, out int ns, out int nl, out RenderItem[] ris) {
			double xtM, ytM, ztM;
			Utils.CalculateBoundingBox(items, out x0, out xtM, out y0, out ytM, out z0, out ztM);
			dx = xtM-x0;
			dy = ytM-y0;
			dz = ztM-z0;
			double b = Math.Pow(items.Count/(dx*dy*dz), Maths.OneThird);
			xn = (int)Math.Ceiling(dx*b);
			yn = (int)Math.Ceiling(dy*b);
			zn = (int)Math.Ceiling(dz*b);
			nl = zn;
			ns = yn*zn;
			dx /= xn;
			dy /= yn;
			dz /= zn;
			/*double dxinv = 1.0d/dx;
			double dyinv = 1.0d/dy;
			double dzinv = 1.0d/dz;*/
			SubList[] perms = new SubList[xn*yn*zn];
			for(int i = 0; i < xn*yn*zn; i++) {
				perms [i] = new SubList();
			}
			int ixm, ixM, iym, iyM, izm, izM;
			double rix0, riy0, riz0;
			int rii = 0x00;
			double xm, xM, ym, yM, zm, zM;
			foreach(RenderItem ri in items) {
				ri.GetBounds(out xm, out xM, out ym, out yM, out zm, out zM);
				Align(xn, yn, zn, x0, y0, z0, dx, dy, dz, xm, ym, zm, out ixm, out iym, out izm);
				Align(xn, yn, zn, x0, y0, z0, dx, dy, dz, xM, yM, zM, out ixM, out iyM, out izM);
				for(int ix = ixm; ix <= ixM; ix++) {
					for(int iy = iym; iy <= iyM; iy++) {
						for(int iz = izm; iz <= izM; iz++) {
							if(ri.InBox(x0+ix*dx, x0+ix*dx+dx, y0+iy*dy, y0+iy*dy+dy, z0+iz*dz, z0+iz*dz+dz)) {
								perms [ix*nextSlice+iy*nextLine+iz].Add(rii);
							}
						}
					}
				}
				rii++;
			}
			FinalList fl = new FinalList(perms);
			ris = fl.list.Select(x => items [x]).ToArray();
			return perms.Select(x => x.ToLongRepresentation()).ToArray();
		}

		public override string ToString () {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("GridAcc");
			sb.AppendLine("Grid");
			int l = 0x00;
			for(int i = 0; i < XN; i++) {
				for(int j = 0; j < YN; j++) {
					for(int k = 0; k < ZN; k++) {
						sb.AppendLine(string.Format("{0}/{1}/{2}: {3}|{4}", i, j, k, grid [l]>>0x20, grid [l]&0xffffffff));
						l++;
					}
				}
			}
			sb.AppendLine("Data");
			for(int i = 0; i < ris.Length; i++) {
				sb.AppendLine(string.Format("{0}: {1}", i, ris [i]));
			}
			return sb.ToString();
		}

		private struct MigrationEvent : IComparable<MigrationEvent>
		{

			public double t;
			public readonly double dt;
			public readonly int dim;
			public readonly int sgn;

			public MigrationEvent (double t, double dt, int dim, int sgn) {
				this.t = t;
				this.dt = dt;
				this.dim = dim;
				this.sgn = sgn;
			}

			public int CompareTo (MigrationEvent other) {
				return this.t.CompareTo(other.t);
			}

			public void Update () {
				this.t += this.dt;
			}

		}

	}

}
