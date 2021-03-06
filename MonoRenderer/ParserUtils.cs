//
//  ParserUtils.cs
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
using System.Text.RegularExpressions;

namespace Renderer {

	public static class ParserUtils {

		public const string Real = @"-?[0-9]+(\.[0-9]*)?";
		public static readonly Regex colorRegex = new Regex(@"^#?(?<color>[0-9a-fA-F]{6})$", RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.ExplicitCapture);

		public static string FormGroup (string regex, string name) {
			return string.Format(@"(?<{0}>{1})", name, regex);
		}

		public static uint ParseColor (string toParse) {
			if(toParse == null) {
				return 0x00;
			}
			else {
				Match m = colorRegex.Match(toParse);
				if(m.Success) {
					return Convert.ToUInt32(m.Groups["color"].Value, 0x10);
				}
				else {
					return 0x00;
				}
			}
		}
		public static ColorAtMethod PerlinNullOrTexture (this string name) {
			if(name != null && name != string.Empty) {
				string namelow = name.ToLower();
				if(namelow.StartsWith("perlin")) {
					switch(namelow) {
						case "perlinmarble":
							return Perlin.Marble3;
						case "perlinwood":
							return Perlin.Wood3;
						case "perlinsky":
							return Perlin.Sky3;
						default :
							return NullOrTexture(name);
					}
				}
				else {
					return NullOrTexture(name);
				}
			}
			else {
				return null;
			}
		}
		public static Texture NullOrTexture (this string name) {
			if(name != null && name != string.Empty && File.Exists(name)) {
				return new Texture(name);
			}
			else {
				return null;
			}
		}
		public static TreeNode<string> ParseTreeBracketsComma (this string toParse) {
			//Console.WriteLine("parsing {0}");
			int funcsep = toParse.IndexOf('(');
			if(funcsep < 0x00) {
				funcsep = toParse.Length;
			}
			int b = 0x00;
			int last = funcsep+0x01;
			char c;
			TreeNode<string> result = new TreeNode<string>(toParse.Substring(0x00, funcsep));
			for(int i = funcsep+0x01; i < toParse.Length; i++) {
				c = toParse[i];
				if(c == '(') {
					b++;
				}
				else if(c == ')') {
					b--;
					if(b < 0x00) {
						result.Add(ParseTreeBracketsComma(toParse.Substring(last, i-last)));
						return result;
					}
				}
				else if(c == ',') {
					if(b <= 0x00) {
						result.Add(ParseTreeBracketsComma(toParse.Substring(last, i-last)));
						last = i+0x01;
					}
				}
			}
			return result;
		}
		public static string ParseBracketsComma (string toParse, out List<string> arguments) {
			int funcsep = toParse.IndexOf('(');
			if(funcsep < 0x00) {
				funcsep = toParse.Length;
			}
			int b = 0x00;
			int last = funcsep+0x01;
			char c;
			arguments = new List<string>();
			string result = toParse.Substring(0x00, funcsep);
			for(int i = funcsep+0x01; i < toParse.Length; i++) {
				c = toParse[i];
				if(c == '(') {
					b++;
				}
				else if(c == ')') {
					b--;
					if(b < 0x00) {
						arguments.Add(toParse.Substring(last, i-last));
						return result;
					}
				}
				else if(c == ',') {
					if(b <= 0x00) {
						arguments.Add(toParse.Substring(last, i-last));
						last = i+0x01;
					}
				}
			}
			return result;
		}
	}
}

