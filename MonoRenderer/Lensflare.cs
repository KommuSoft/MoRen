using System;

namespace Renderer {

	public class Lensflare {
		
		private int flares = 0;
		private Texture[] flare;
		private float[] flareDist;
		private Point3 position = new Point3(0.0d, 0.0d, 0.0d);
		
		public Point3 Position {
			get {
				return this.position;
			}
			set {
				this.position = value;
			}
		}
	
		public Lensflare () {
		}
	
		public void preset1 () {
			Clear();
			addGlow(144, 0x330099);
			addStrike(320, 48, 0x003366);
			addStrike(48, 240, 0x660033);
			addRing(120, 0x660000);
			addRays(320, 32, 20, 0x111111);
			addSecs(12, 100, 64, 0x6633cc, 64);
		}
		
		public void preset2 () {
			Clear();
			addGlow(144, 0x995500);
			addStrike(640, 64, 0xCC0000);
			addStrike(32, 480, 0x0000FF);
			addStrike(64, 329, 0x330099);
			addRing(160, 0x990000);
			addRays(320, 24, 32, 0x332211);
			addSecs(12, 100, 64, 0x333333, 100);
			addSecs(12, 60, 40, 0x336699, 80);
		}
		
		public void preset3 () {
			Clear();
			addGlow(144, 0x993322);
			addStrike(400, 200, 0xCC00FF);
			addStrike(480, 32, 0x0000FF);
			addRing(180, 0x990000);
			addRays(240, 32, 48, 0x332200);
			addSecs(12, 80, 64, 0x555555, 50);
			addSecs(12, 60, 40, 0x336699, 80);
		}
	
		public void Clear () {
			flares = 0;
			flare = null;
			flareDist = null;
		}
		public void addGlow (int size, uint color) {
			addFlare(createGlow(size, size, color, 256), 0.0f);
		}
		public void addStrike (int width, int height, uint color) {
			addFlare(createGlow(width, height, color, 48), 0.0f);
		}
		public void addRing (int size, uint color) {
			addFlare(createRing(size, color), 0.0f);
		}
		public void addRays (int size, int num, int rad, uint color) {
			addFlare(createRays(size, num, rad, color), 0.0f);
		}
		public void addSecs (int count, int averidgeSize, int sizeDelta, uint averidgeColor, uint colorDelta) {
			for(int i = 0; i < count; i++) {
				addFlare(createSec(averidgeSize, sizeDelta, averidgeColor, colorDelta), Maths.Random(-0.5f, 3f));
			}
		}
					

		public void Apply (Texture screen) {
			//TODO: calculate position
			//int px=flareObject.vertex[0].x;
			//int py=flareObject.vertex[0].y;
			int px = 250;
			int py = 250;
			//if(!flareObject.vertex[0].visible) return;
			//if(zBufferSensitive&&(flareObject.vertex[0].z>scene.renderPipeline.zBuffer[px+py*screen.w])) return;
			int cx = 500;
			int cy = 500;
			float dx = (float)(cx-px);
			float dy = (float)(cy-py);
			int posx, posy, xsize, ysize;
			float zoom;
			for(int i = 0; i < flares; i++) {
				zoom = Maths.Pythagoras(dx, dy)/Maths.Pythagoras(cx, cy);
				zoom = 1.5f-0.5f*zoom;
				xsize = flare[i].Width;
				ysize = flare[i].Height;
				posx = px+(int)(dx*flareDist[i]);
				posy = py+(int)(dy*flareDist[i]);
				screen.Add(flare[i], posx-xsize/2, posy-ysize/2, xsize, ysize);
			}
		}
		private void addFlare (Texture texture, float relPos) {
			flares++;
			if(flares == 1) {
				flare = new Texture[1];
				flareDist = new float[1];
			}
			else {
				Texture[] temp1 = new Texture[flares];
				Array.Copy(flare, 0, temp1, 0, flares-1);
				flare = temp1;
				
				float[] temp2 = new float[flares];
				Array.Copy(flareDist, 0, temp2, 0, flares-1);
				flareDist = temp2;
			}
			flare[flares-1] = texture;
			flareDist[flares-1] = relPos;
		}
			
		private Texture createRadialTexture (int w, int h, uint[] colormap, uint[] alphamap) {
			int offset;
			float relX, relY;
			Texture newTexture = new Texture(w, h);
			uint[] palette = getPalette(colormap, alphamap);
			for(int y = h-1; y >= 0; y--) {
				offset = y*w;
				for(int x = w-1; x >= 0; x--) {
					relX = (float)(x-(w>>1))/(float)(w>>1);
					relY = (float)(y-(h>>1))/(float)(h>>1);
					newTexture.Pixel[offset+x] = palette[Maths.Border(0, (int)(255*Math.Sqrt(relX*relX+relY*relY)), 255)];
				}
			}
			return newTexture;			
		}
		
		private uint[] getPalette (uint[] color, uint[] alpha) {
			uint r, g, b;
			uint[] palette = new uint[256];
			for(int i = 255; i >= 0; i--) {
				r = (((color[i]>>16)&255)*alpha[i])>>8;
				g = (((color[i]>>8)&255)*alpha[i])>>8;
				b = ((color[i]&255)*alpha[i])>>8;
				palette[i] = Color.GetColor(r, g, b);
			}
			return palette;
		}
		
		private Texture createGlow (int w, int h, uint color, uint alpha) {
			return createRadialTexture(w, h, getGlowPalette(color), getConstantAlpha(alpha));
		}
		
		private Texture createRing (int size, uint color) {
			return createRadialTexture(size, size, getColorPalette(color, color), getRingAlpha(40));
		}
		
		private Texture createSec (int size, int sizedelta, uint color, uint colordelta) {
			int s = (int)Maths.RandomWithDelta(size, sizedelta);
			uint c1 = Color.Random(color, colordelta);
			uint c2 = Color.Random(color, colordelta);
			return createRadialTexture(s, s, getColorPalette(c1, c2), getSecAlpha());
		}
		
		private Texture createRays (int size, int rays, int rad, uint color) {
			int pos;
			float relPos;
			Texture texture = new Texture(size, size);
			int[] radialMap = new int[1024];
			for(int i = 0; i < rays; i++) {
				pos = (int)Maths.Random(rad, 1023-rad);
				for(int k = pos-rad; k <= pos+rad; k++) {
					relPos = (float)(k-pos+rad)/(float)(rad*2);
					radialMap[k] += (int)(255*(1+Math.Sin((relPos-0.25)*3.14159*2))/2);
				}
			}
			int angle, offset;
			float reldist;
			float xrel, yrel;
			for(int y = size-1; y >= 0; y--) {
				offset = y*size;
				for(int x = size-1; x >= 0; x--) {
					xrel = (float)(2*x-size)/(float)size;
					yrel = (float)(2*y-size)/(float)size;
					angle = (int)(1023*Math.Atan2(xrel, yrel)/3.14159/2)&1023;
					reldist = Math.Max(1.0f-Maths.Pythagoras(xrel, yrel), 0.0f);
					texture.Pixel[x+offset] = Color.Scale(color, (uint)(radialMap[angle]*reldist));
				}
			}
			return texture;
		}
		
		private uint[] getGlowPalette (uint color) {
			int r, g, b;
			float relDist, diffuse, specular;
			uint[] palette = new uint[256];
			uint cr = (color>>16)&255;
			uint cg = (color>>8)&255;
			uint cb = color&255;
			for(int i = 255; i >= 0; i--) {
				relDist = (float)i/255;
				diffuse = (float)Math.Cos(relDist*1.57);
				specular = (float)(255/Math.Pow(2.718, relDist*2.718)-(float)i/16);
				r = (int)((float)cr*diffuse+specular);
				g = (int)((float)cg*diffuse+specular);
				b = (int)((float)cb*diffuse+specular);
				palette[i] = Color.GetCropColor(r, g, b);
			}
			return palette;
		}
		
		private uint[] getConstantAlpha (uint alpha) {
			uint[] alphaPalette = new uint[256];
			for(int i = 255; i >= 0; i--) {
				alphaPalette[i] = alpha;
			}
			return alphaPalette;
		}
		
		private uint[] getLinearAlpha () {//TODO: needed??
			uint[] alphaPalette = new uint[256];
			for(int i = 255; i >= 0; i--) {
				alphaPalette[i] = (uint)(255-i);
			}
			return alphaPalette;
		}
		private uint[] getRingAlpha (int ringsize) {
			uint[] alphaPalette = new uint[256];
			float angle;
			for(int i = 0; i < ringsize; i++) {
				angle = 3.14159f*(float)i/ringsize;				
				alphaPalette[255-ringsize+i] = (uint)(64*Math.Sin(angle));
			}
			return alphaPalette;
		}
		
		private uint[] getSecAlpha () {
			uint[] alphaPalette = getRingAlpha((int)Maths.Random(0, 255));
			for(int i=0; i<256; i++) {
				alphaPalette[i] = (uint)((alphaPalette[i]+255-i)>>2);
			}
			return alphaPalette;
		}
		
		
		private uint[] getColorPalette (uint color1, uint color2) {
			uint[] palette = new uint[256];
			int r1 = (int)(color1>>16)&255;
			int g1 = (int)(color1>>8)&255;
			int b1 = (int)color1&255;
			int r2 = (int)(color2>>16)&255;
			int g2 = (int)(color2>>8)&255;
			int b2 = (int)color2&255;
			int dr = r2-r1;
			int dg = g2-g1;
			int db = b2-b1;
			int r = r1<<8;
			int g = g1<<8;
			int b = b1<<8;
			for(int i=0; i<256; i++) {
				palette[i] = Color.GetColor((uint)(r>>8), (uint)(g>>8), (uint)(b>>8));
				r += dr;
				g += dg;
				b += db;
			}
			return palette;
		}
	}
}