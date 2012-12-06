using System;
using System.Text;
using System.IO;

namespace Renderer {
	
	public static class DebugFile {
		
		private static StringBuilder sb = new StringBuilder();
		
		public static void WriteEntry (string entry) {
			sb.Append(string.Format("[{0}] {1}\n",DateTime.Now.ToShortTimeString(),entry));
		}
		public static void WriteEntry (string entry, params object[] args) {
			WriteEntry(string.Format(entry,args));
		}
		public static void Flush (string name) {
			FileStream fs = File.Open(name,FileMode.Create,FileAccess.Write);
			TextWriter tw = new StreamWriter(fs);
			tw.Write(sb.ToString());
			tw.Close();
			fs.Close();
		}
		
	}
}

