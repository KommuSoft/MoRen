//  
//  SpaceGraph.cs
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
using System.Text;
using System.Collections.Generic;

namespace Renderer {
	
	using Separator = Tuple<double,double,double,double>;
	
	/*public class BinarySpaceTree
	{
		
		private readonly BinarySpaceTreeNode root;
		private readonly BinarySpaceTreeNode[] agenda;
		
		
		public BinarySpaceTree (RenderItem[] items) {
			int maxdepth;
			this.root = doSorting(items, out maxdepth);
			this.agenda = new BinarySpaceTreeNode[maxdepth];
		}
		
		/*private static BinarySpaceTreeNode doSorting (RenderItem[] items,out int maxdepth) {
			int n = items.Length,i = 0x00;
			double A,B,C,D;
			int[][] grid = new int[n][];
			int[] indices = new int[n];
			Tuple<double,double,double,double>[] faces = new Tuple<double,double,double,double>[n];
			foreach(RenderItem ri in items) {
				if(ri is OrderingRenderItem) {
					((OrderingRenderItem)ri).generateOrdering(out A,out B,out C,out D);
					int[] line = new int[n];
					for(int j = 0; j < n; j++) {
						line[j] = items[j].Sort(A,B,C,D);
					}
					grid[i] = line;
					faces[i] = new Tuple<double,double,double,double>(A,B,C,D);
				}
				indices[i] = i;
				i++;
			}
			return SubDivide(grid,items,faces,indices,out maxdepth);
		}*//*
		private static BinarySpaceTreeNode SubDivide (int[][] grid, RenderItem[] items, Tuple<double,double,double,double>[] faces, int[] indices, out int maxDepth) {
			//Console.WriteLine(indices.Length);
			//System.Threading.Thread.Sleep(1000);*//*
			if(indices.Length <= 0x00) {
				maxDepth = 1;
				return null;
			} else if(indices.Length == 0x01) {
				maxDepth = 1;
				return new BinarySpaceTreeNode(items [indices [0x00]]);
			}
			//searching for best match
			int maxmax = int.MaxValue, maxi = -0x01, lm = 0x00, rm = 0x00;
			for(int i = 0; i < indices.Length; i++) {
				int ii = indices [i];
				if(grid [ii] != null) {
					int l = 0x00, r = 0x00;
					for(int j = 0; j < indices.Length; j++) {
						if(ii != indices [j]) {
							l += grid [ii] [indices [j]]&0x01;
							r += grid [ii] [indices [j]]>>0x01;
						}
					}
					if(Math.Max(l, r) < maxmax) {
						maxi = ii;
						maxmax = Math.Max(l, r);
						lm = l;
						rm = r;
					}
				}
			}
			int[] lind = new int[lm];
			int[] rind = new int[rm];
			lm = 0x00;
			rm = 0x00;
			for(int i = 0; i < indices.Length; i++) {
				if(maxi == indices [i]) {
					continue;
				} else {
					if((grid [maxi] [indices [i]]&0x01) != 0x00) {
						lind [lm++] = indices [i];
					}
					if((grid [maxi] [indices [i]]>>0x01) != 0x00) {
						rind [rm++] = indices [i];
					}
				}
			}
			int mdl, mdr;
			BinarySpaceTreeNode bl = SubDivide(grid, items, faces, lind, out mdl);
			BinarySpaceTreeNode br = SubDivide(grid, items, faces, rind, out mdr);
			maxDepth = Math.Max(mdl, mdr)+1;
			Tuple<double,double,double,double> f = faces [maxi];
			return new BinarySpaceTreeNode(f.Item1, f.Item2, f.Item3, f.Item4, bl, br, items [maxi]);
		}
		public RenderItem FindFirst (Ray r) {
			return null;
		}
		
		private class BinarySpaceTreeNode
		{
			
			public readonly double A, B, C, D;
			public readonly BinarySpaceTreeNode Left, Right;
			public readonly RenderItem Item;
			
			public BinarySpaceTreeNode (RenderItem ri) {
				this.A = 0.0d;
				this.B = 0.0d;
				this.C = 0.0d;
				this.D = 0.0d;
				this.Left = null;
				this.Right = null;
				this.Item = ri;
				
			}
			public BinarySpaceTreeNode (double A, double B, double C, double D, BinarySpaceTreeNode Left, BinarySpaceTreeNode Right, RenderItem ri) {
				this.A = A;
				this.B = B;
				this.C = C;
				this.D = D;
				this.Left = Left;
				this.Right = Right;
				this.Item = ri;
			}
			
		}
		
	}*/
	
}