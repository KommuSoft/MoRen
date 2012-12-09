//
//  Loader3ds.cs
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
using System.Linq;
using System.IO;
using System.Text;

namespace Renderer {
	public class Loader3ds : MeshLoader {
		
		private ushort currentJunk;
		private int nextJunkOffset;
		private RootJunk root;
		private Material defaultMaterial = Material.DefaultMaterial;

		public Material DefaultMaterial {
			get {
				return this.defaultMaterial;
			}
			set {
				this.defaultMaterial = value;
			}
		}

		public Loader3ds () {
			
		}

		public void Inject (List<RenderItem> items, Matrix4 transform, params string[] args) {
			items.AddRange(this.root.GetItems(transform));
		}

		public void Load (string dir, Stream s) {
			ParsingContext ctx = new ParsingContext(dir);
			BinaryReader br = new BinaryReader(s);
			readHeader(br, s);
			if(this.currentJunk != 0x4d4d) {
				throw new FormatException("Stream has no valid 3ds format!");
			}
			this.root = new RootJunk();
			this.readChildren(br, s, this.root, s.Length, ctx);
		}

		private void readChildren (BinaryReader br, Stream s, Junk3ds parent, long until, ParsingContext ctx) {
			while(s.Position < until) {
				readJunk(br, s, parent, ctx);
			}
		}

		private void readHeader (BinaryReader br, Stream st) {
			this.currentJunk = br.ReadUInt16();
			this.nextJunkOffset = br.ReadInt32();
		}

		private void readJunk (BinaryReader br, Stream st, Junk3ds parent, ParsingContext ctx) {
			this.readHeader(br, st);
			string f;
			double pct;
			long until = st.Position+this.nextJunkOffset-6;
			Junk3ds child;
			switch(this.currentJunk) {
				case 0x4000:
					f = Utils.ReadZeroEndByteString(br);
#if DEBUG
					Console.WriteLine("Loaded \"{0}\"", f);
#endif
					child = new ObjectJunk(f);
					break;
				case 0x4100:
					child = new LoadModeJunk(LoadMode.Triangle);
					break;
				case 0x4110:
					List<Point3> vertexList = new List<Point3>();
					Utils.ReadPoint3DFloatList(br, vertexList);
					child = new ListJunk<Point3>(vertexList, ListPurpose.Vertex);
					break;
				case 0x4120:
					List<ushort> indices = new List<ushort>();
					Utils.ReadUShortList(br, indices, 4);
					child = new IndexJunk(indices);
					break;
				case 0x4130:
					f = Utils.ReadZeroEndByteString(br);
					List<ushort> faces = new List<ushort>();
					Utils.ReadUShortList(br, faces, 0x01);
					child = new UseMaterialJunk(f, faces);
					break;
				case 0x4140:
					List<Point3> textureList = new List<Point3>();
					Utils.ReadPoint2DFloatList(br, textureList);
					child = new ListJunk<Point3>(textureList, ListPurpose.Texture);
					break;
				case 0x4160:
					child = new MatrixJunk(Utils.ReadMatrixChunk(br));
					break;
				case 0x3d3d:
					child = new MeshJunk();
					break;
				case 0xafff:
					child = new MaterialJunk();
					break;
				case 0xa000:
					f = Utils.ReadZeroEndByteString(br);
					child = new MaterialNameJunk(f);
					break;
				case 0xa010:
					child = new MaterialColorJunk(Utils.ReadColorChunk(br), MaterialColorType.Ambient);
					break;
				case 0xa020:
					child = new MaterialColorJunk(Utils.ReadColorChunk(br), MaterialColorType.Diffuse);
					break;
				case 0xa030:
					child = new MaterialColorJunk(Utils.ReadColorChunk(br), MaterialColorType.Specular);
					break;
				case 0xa040:
					child = new MaterialPercentageJunk(Utils.ReadPercentageChunk(br), MaterialPercentageType.Shininess);
					break;
				case 0xa050:
					child = new MaterialPercentageJunk(Utils.ReadPercentageChunk(br), MaterialPercentageType.Transparency);
					break;
				case 0xa200:
					child = new MaterialTextureJunk(Utils.ReadPercentageChunk(br), MaterialTextureType.Texture);
					break;
				case 0xa220:
					child = new MaterialTextureJunk(Utils.ReadPercentageChunk(br), MaterialTextureType.Reflection);
					break;
				case 0xa230:
					child = new MaterialTextureJunk(Utils.ReadPercentageChunk(br), MaterialTextureType.Bump);
					break;
				case 0xa300:
					child = new TextureNameJunk(Utils.ReadZeroEndByteString(br));
					break;
				case 0x351:
					child = new TextureFlagsJunk(br.ReadUInt16());
					break;
				default :
#if DEBUG
					Console.Error.WriteLine("unknown chunk is {0}/{1}/{2} @ {3}", this.currentJunk.ToString("X"), this.nextJunkOffset, this.nextJunkOffset.ToString("X"), st.Position.ToString("X"));
#endif
					readToNext(br, st);
					return;
			}
			parent.AddChild(child);
			this.readChildren(br, st, child, until, ctx);
			child.Resolve(ctx, this.defaultMaterial);
		}

		private class ParsingContext {

			public readonly string activedir;
			public readonly Dictionary<string,Material> material = new Dictionary<string, Material>();

			public ParsingContext (string dir) {
				this.activedir = dir;
			}

			public Texture CreateTexture (string name) {
#if DEBUG
				Console.WriteLine("Loading \"{0}\"", name);
#endif
				if(name == null || name == string.Empty) {
					return null;
				}
				else if(activedir == null) {
					if(File.Exists(name)) {
						return new Texture(name);
					}
					else {
#if DEBUG
						Console.Error.WriteLine("Cannot open \"{0}\": file doesn't exist!", name);
#endif
						return null;
					}
				}
				else {
					string path = Path.GetDirectoryName(activedir)+Path.DirectorySeparatorChar+name;
					if(File.Exists(path)) {
						return new Texture(path);
					}
					else {
#if DEBUG
						Console.Error.WriteLine("Cannot open \"{0}\": file doesn't exist!", path);
#endif
						return null;
					}
				}
			}

		}

		private abstract class Junk3ds {

			private List<Junk3ds> children = new List<Junk3ds>();

			public virtual void AddChild (Junk3ds junk) {
				this.children.Add(junk);
			}

			protected virtual string ElementString () {
				return this.GetType().Name;
			}

			public string ToString (int t) {
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < t; i++) {
					sb.Append("\t");
				}
				sb.AppendLine(this.ElementString());
				if(this.children != null) {
					foreach(Junk3ds j in children) {
						sb.Append(j.ToString(t+1));
					}
				}
				return sb.ToString();
			}
			protected IEnumerable<T> FindChild<T> (Func<T,bool> predicate) where T : Junk3ds {
				if(this.children != null) {
					return children.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().Where(predicate);
				}
				else {
					return null;
				}
			}
			protected IEnumerable<T> FindChild<T> () where T : Junk3ds {
				if(this.children != null) {
					return children.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>();
				}
				else {
					return null;
				}
			}
			public virtual void Resolve (ParsingContext ctx, Material material) {
			}
			protected void Collapse () {
				this.children = null;
			}
			protected void Collapse<T> () {
				this.children = this.children.Where(x => !typeof(T).IsAssignableFrom(x.GetType())).ToList();
				if(this.children.Count <= 0x00) {
					this.children = null;
				}
			}

		}

		private class UseMaterialJunk : NameJunk {

			public List<ushort> faces;

			public UseMaterialJunk (string name, List<ushort> faces) : base(name) {
				this.faces = faces;
			}

			protected override string ElementString () {
				return string.Format("{0} {1} [{2}]", this.GetType().Name, this.name, string.Join(",", this.faces));
			}

		}

		private class MaterialPercentageJunk : Junk3ds {

			public double percentage;
			public MaterialPercentageType type;

			public MaterialPercentageJunk (double percentage, MaterialPercentageType mpt) {
				this.percentage = percentage;
				this.type = mpt;
			}

			protected override string ElementString () {
				return string.Format("{0} {1} {2}%", this.GetType().Name, this.type, (this.percentage*100.0d).ToString());
			}

		}

		private enum MaterialPercentageType {
			Shininess,
			Transparency
		}

		private class ListJunk<T> : Junk3ds {

			public List<T> list;
			public ListPurpose purpose;

			public ListJunk (List<T> items, ListPurpose purpose) {
				this.list = items;
				this.purpose = purpose;
			}

			protected override string ElementString () {
				return string.Format("{1} {2} [{0}]", string.Join(",", this.list), this.GetType().Name, this.purpose);
			}

		}

		private class IndexJunk : ListJunk<ushort> {

			public List<string> materials;
			public List<Tuple<ushort,ushort,ushort,ushort>> tris;

			public IndexJunk (List<ushort> items) : base(items,ListPurpose.Index) {
			}

			public override void Resolve (ParsingContext ctx, Material material) {
				List<ushort> l = this.list;
				this.list = null;
				this.materials = new List<string>();
				this.tris = new List<Tuple<ushort,ushort,ushort,ushort>>();
				HashSet<int> todo = new HashSet<int>(Utils.GenerateRange(0, l.Count/4));
				ushort i = 0;
				foreach(UseMaterialJunk umj in this.FindChild<UseMaterialJunk>()) {
					this.materials.Add(umj.name);
					foreach(ushort it in umj.faces) {
						todo.Remove(it);
						int k = 4*it;
						this.tris.Add(new Tuple<ushort, ushort, ushort, ushort>(l[k], l[k+1], l[k+2], i));
					}
					i++;
				}
				foreach(ushort it in todo) {
					int k = 4*it;
					this.tris.Add(new Tuple<ushort, ushort, ushort, ushort>(l[k], l[k+1], l[k+2], i));
				}
				this.Collapse();
			}

			protected override string ElementString () {
				return string.Format("{1} [{0}] [{2}]", string.Join(",", this.materials), this.GetType().Name, string.Join(",", this.tris));
			}

		}

		private enum ListPurpose {
			Vertex,
			Texture,
			Index
		}

		private abstract class NameJunk : Junk3ds {

			public string name;

			public NameJunk (string name) {
				this.name = name;
			}

			protected override string ElementString () {
				return string.Format("{1} {0}", this.name, this.GetType().Name);
			}

		}

		private class ObjectJunk : NameJunk {

			public ObjectJunk (string name) : base(name) {
			}

			public void CacheMaterial (ParsingContext pc, Material material) {
				foreach(LoadModeJunk lmj in this.FindChild<LoadModeJunk>()) {
					lmj.CacheMaterial(pc, material);
				}
			}

			public IEnumerable<RenderItem> GetItems (Matrix4 mat) {
				foreach(LoadModeJunk lmj in this.FindChild<LoadModeJunk>()) {
					foreach(RenderItem ri in lmj.GetItems(mat)) {
						yield return ri;
					}
				}
			}

		}

		private class TextureNameJunk : NameJunk {

			public TextureNameJunk (string name) : base(name) {
			}

		}

		private class TextureFlagsJunk : Junk3ds {

			public ushort flags;

			public TextureFlagsJunk (ushort flags) {
				this.flags = flags;
			}

			protected override string ElementString () {
				return string.Format("{0} {1}", this.GetType().Name, Convert.ToString(flags, 2));
			}

		}

		private class MaterialColorJunk : Junk3ds {

			public uint color;
			public MaterialColorType type;

			public MaterialColorJunk (uint color, MaterialColorType mct) {
				this.color = color;
				this.type = mct;
			}

			protected override string ElementString () {
				return string.Format("{0} {1} {2}", this.GetType().Name, this.type, this.color.ToString("X"));
			}

		}

		private enum MaterialColorType {
			Ambient,
			Diffuse,
			Specular
		}

		private class MaterialTextureJunk : Junk3ds {

			public MaterialTextureType type;
			public double percentage;
			public Texture texture;

			public MaterialTextureJunk (double percentage, MaterialTextureType mct) {
				this.percentage = percentage;
				this.type = mct;
			}

			protected override string ElementString () {
				return string.Format("{0} {1} {2}%", this.GetType().Name, this.type, (this.percentage*100.0d));
			}

			public override void Resolve (ParsingContext ctx, Material material) {
				this.texture = ctx.CreateTexture(this.FindChild<TextureNameJunk>().First().name);
				this.Collapse();
			}

		}

		private class MatrixJunk : Junk3ds {

			public Matrix4 mat;

			public MatrixJunk (Matrix4 mat) {
				this.mat = mat;
			}

			protected override string ElementString () {
				return string.Format("{0} {1}", this.GetType().Name, this.mat);
			}

		}

		private enum MaterialTextureType {
			Texture,
			Reflection,
			Bump
		}

		private class MaterialNameJunk : NameJunk {

			public MaterialNameJunk (string name) : base(name) {
			}

		}

		private class RootJunk : Junk3ds {

			public IEnumerable<RenderItem> GetItems (Matrix4 mat) {
				foreach(MeshJunk oj in this.FindChild<MeshJunk>()) {
					foreach(RenderItem ri in oj.GetItems(mat)) {
						yield return ri;
					}
				}
			}

		}


		private class MaterialJunk : Junk3ds {

			public string name;
			public Material material;

			public override void Resolve (ParsingContext ctx, Material material) {
				this.name = this.FindChild<MaterialNameJunk>().First().name;
				uint ambient = Color.White, diffuse = Color.Black, specular = Color.Black;
				double shininess = 15.0d, transparent = 0.0d;
				Texture texture = null, bump = null, reflection = null;
				foreach(MaterialColorJunk mcj in this.FindChild<MaterialColorJunk>()) {
					switch(mcj.type) {
						case MaterialColorType.Ambient:
							ambient = mcj.color;
							break;
						case MaterialColorType.Diffuse:
							diffuse = mcj.color;
							break;
						case MaterialColorType.Specular:
							specular = mcj.color;
							break;
					}
				}
				foreach(MaterialPercentageJunk mpj in this.FindChild<MaterialPercentageJunk>()) {
					switch(mpj.type) {
						case MaterialPercentageType.Shininess:
							shininess = 100.0d-mpj.percentage*100.0d;
							break;
						case MaterialPercentageType.Transparency:
							transparent = mpj.percentage;
							break;
					}
				}
				foreach(MaterialTextureJunk mt in this.FindChild<MaterialTextureJunk>()) {
					switch(mt.type) {
						case MaterialTextureType.Texture:
							texture = mt.texture;
							break;
						case MaterialTextureType.Bump:
							bump = mt.texture;
							break;
						case MaterialTextureType.Reflection:
							reflection = mt.texture;
							break;
					}
				}
				this.material = new Material(ambient, specular, diffuse, shininess, transparent, texture, reflection, bump);
				this.Collapse();
			}

		}

		private class MeshJunk : Junk3ds {

			public override void Resolve (ParsingContext ctx, Material material) {
				foreach(MaterialJunk mj in this.FindChild<MaterialJunk>()) {
					ctx.material.Add(mj.name, mj.material);
				}
				foreach(ObjectJunk oj in this.FindChild<ObjectJunk>()) {
					oj.CacheMaterial(ctx, material);
				}
				this.Collapse<MaterialJunk>();
			}

			public IEnumerable<RenderItem> GetItems (Matrix4 mat) {
				foreach(ObjectJunk oj in this.FindChild<ObjectJunk>()) {
					foreach(RenderItem ri in oj.GetItems(mat)) {
						yield return ri;
					}
				}
			}

		}

		private class LoadModeJunk : Junk3ds {

			public LoadMode lm;
			public List<Point3> vertices;
			public List<Point3> normals;
			public List<string> materialstr;
			public List<Material> materials;
			public List<Tuple<ushort,ushort,ushort,ushort>> indices;
			public List<Point3> texture;

			public LoadModeJunk (LoadMode lm) {
				this.lm = lm;
			}

			protected override string ElementString () {
				return string.Format("LoadMode: {0}", this.lm);
			}

			public override void Resolve (ParsingContext ctx, Material material) {
				ListJunk<Point3> plj;
				MatrixJunk mj;
				this.vertices = this.FindChild<ListJunk<Point3>>(x => x.purpose == ListPurpose.Vertex).First().list;
				mj = this.FindChild<MatrixJunk>().FirstOrDefault();
				Matrix4 M;
				if(mj != null) {
					M = mj.mat;
					foreach(Point3 v in vertices) {
						v.Transform(M);
					}
				}
				plj = this.FindChild<ListJunk<Point3>>(x => x.purpose == ListPurpose.Texture).FirstOrDefault();
				if(plj != null) {
					this.texture = plj.list;
				}
				else {
					this.texture = Utils.GenerateNormalizedCopies(this.vertices).ToList();
				}
				IndexJunk ij = this.FindChild<IndexJunk>().FirstOrDefault();
				if(ij != null) {
					this.indices = ij.tris;
					this.materialstr = ij.materials;
				}
				else {
					this.indices = Utils.GenerateIndicesRange(this.vertices.Count).ToList();
					this.materialstr = new List<string>();
				}
				this.normals = Utils.GenerateSmoothNormalsFromTriangles(this.vertices, this.indices.Select(x => new Tuple<int,int,int>(x.Item1, x.Item2, x.Item3)));
				this.Collapse();
			}

			public void CacheMaterial (ParsingContext pc, Material material) {
				this.materials = this.materialstr.Select(x => pc.material[x]).ToList();
				this.materials.Add(material);
				this.materialstr = null;
			}

			public IEnumerable<RenderItem> GetItems (Matrix4 m) {
				List<Point3> vertexc = this.vertices.Select(x => new Point3(x, m)).ToList();
				List<Point3> normalc = this.normals.Select(x => new Point3(x)).ToList();
				foreach(Point3 p in normalc) {
					p.TransformNonShift(m);
					p.Normalize();
				}
				List<Point3> texturc = this.texture.Select(x => new Point3(x)).ToList();
				foreach(Tuple<ushort,ushort,ushort,ushort> tri in this.indices) {
					yield return new Triangle(vertexc[tri.Item1], vertexc[tri.Item2], vertexc[tri.Item3], normalc[tri.Item1], normalc[tri.Item2], normalc[tri.Item3], texturc[tri.Item1], texturc[tri.Item2], texturc[tri.Item3], materials[tri.Item4]);
				}
			}

		}
		
		private void readToNext (BinaryReader br, Stream s) {
			int n = Math.Min(this.nextJunkOffset, (int)(s.Length-s.Position+6));
			for(int i = 6; i < n; i++) {
				br.ReadByte();
			}
		}

		private enum LoadMode : byte {
			None		= 0x00,
			Triangle	= 0x01
		}
	}
	
}