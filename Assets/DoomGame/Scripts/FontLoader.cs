using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontLoader {

	private static Dictionary<string, Texture> small_grey = new Dictionary<string, Texture>();
	private static Dictionary<string, Texture> small_yellow = new Dictionary<string, Texture>();
	private static Dictionary<string, Texture> small_red = new Dictionary<string, Texture>();
	private static Dictionary<string, Texture> big_red = new Dictionary<string, Texture>();

	public static Texture GetTexture (string letter, FontType fontType)
	{

		if (small_grey.Count == 0) LoadAll();

		switch (fontType) {
			case FontType.SmallGrey:
			return small_grey[letter];
			case FontType.SmallYellow:
			return small_yellow[letter];
			case FontType.SmallRed:
			return small_red[letter];
			default:// FontType.BigRed
			return big_red[letter];
		}
	}

	public enum FontType {
		SmallGrey,
		SmallYellow,
		SmallRed,
		BigRed
	}

	private static void LoadAll ()
	{
		if (WadLoader.lumps.Count == 0) {
			Debug.LogError ("MapLoader: Load: WadLoader.lumps == 0");
			return;
		}
		// fuck it adding them manually hold onto something
		// TODO: small_red
		foreach (Lump l in WadLoader.lumps) {
			switch (l.lumpName) {
			#region small_grey
				case "STGNUM0":
				small_grey.Add("0", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM1":
				small_grey.Add("1", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM2":
				small_grey.Add("2", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM3":
				small_grey.Add("3", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM4":
				small_grey.Add("4", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM5":
				small_grey.Add("5", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM6":
				small_grey.Add("6", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM7":
				small_grey.Add("7", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM8":
				small_grey.Add("8", TextureLoader.Instance.TextureFromLump (l));
				break;
				case "STGNUM9":
				small_grey.Add("9", TextureLoader.Instance.TextureFromLump (l));
				break;
			#endregion
			#region small_yellow
				case "STYSNUM0":
				small_yellow.Add("0", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM1":
				small_yellow.Add("1", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM2":
				small_yellow.Add("2", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM3":
				small_yellow.Add("3", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM4":
				small_yellow.Add("4", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM5":
				small_yellow.Add("5", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM6":
				small_yellow.Add("6", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM7":
				small_yellow.Add("7", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM8":
				small_yellow.Add("8", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STYSNUM9":
				small_yellow.Add("9", TextureLoader.Instance.TextureFromLump(l));
				break;
			#endregion
			#region small_red
				case "STCFN033":
				small_red.Add("!", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN034":
				small_red.Add("\"", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN035":
				small_red.Add("#", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN036":
				small_red.Add("$", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN037":
				small_red.Add("%", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN038":
				small_red.Add("&", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN039":
				small_red.Add("`", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN040":
				small_red.Add("(", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN041":
				small_red.Add(")", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN042":
				small_red.Add("*", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN043":
				small_red.Add("+", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN044":
				small_red.Add("'", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN045":
				small_red.Add("_", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN046":
				small_red.Add("-", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN047":
				small_red.Add("/", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN048":
				small_red.Add("0", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN049":
				small_red.Add("1", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN050":
				small_red.Add("2", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN051":
				small_red.Add("3", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN052":
				small_red.Add("4", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN053":
				small_red.Add("5", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN054":
				small_red.Add("6", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN055":
				small_red.Add("7", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN056":
				small_red.Add("8", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN057":
				small_red.Add("9", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN058":
				small_red.Add(":", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN059":
				small_red.Add(";", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN060":
				small_red.Add("<", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN061":
				small_red.Add("=", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN062":
				small_red.Add(">", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN063":
				small_red.Add("?", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN064":
				small_red.Add("@", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN065":
				small_red.Add("a", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("A", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN066":
				small_red.Add("b", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("B", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN067":
				small_red.Add("c", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("C", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN068":
				small_red.Add("d", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("D", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN069":
				small_red.Add("e", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("E", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN070":
				small_red.Add("f", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("F", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN071":
				small_red.Add("g", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("G", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN072":
				small_red.Add("h", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("H", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN073":
				small_red.Add("i", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("I", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN074":
				small_red.Add("j", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("J", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN075":
				small_red.Add("k", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("K", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN076":
				small_red.Add("l", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("L", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN077":
				small_red.Add("m", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("M", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN078":
				small_red.Add("n", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("N", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN079":
				small_red.Add("o", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("O", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN080":
				small_red.Add("p", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("P", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN081":
				small_red.Add("q", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("Q", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN082":
				small_red.Add("r", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("R", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN083":
				small_red.Add("s", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("S", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN084":
				small_red.Add("t", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("T", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN085":
				small_red.Add("u", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("U", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN086":
				small_red.Add("v", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("V", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN087":
				small_red.Add("w", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("W", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN088":
				small_red.Add("x", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("X", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN089":
				small_red.Add("y", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("Y", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN090":
				small_red.Add("z", TextureLoader.Instance.TextureFromLump(l));
				small_red.Add("Z", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN091":
				small_red.Add("[", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN092":
				small_red.Add("\\", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN093":
				small_red.Add("]", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STCFN094":
				small_red.Add("^", TextureLoader.Instance.TextureFromLump(l));
				break;
				// bit confused here, this already exists as STCFN045 I must be missing something
				//case "STCFN095":
				//small_red.Add("_", TextureLoader.Instance.TextureFromLump(l));
				//break;
			#endregion
			#region big_red
				case "STTMINUS":
				big_red.Add("-", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM0":
				big_red.Add("0", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM1":
				big_red.Add("1", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM2":
				big_red.Add("2", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM3":
				big_red.Add("3", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM4":
				big_red.Add("4", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM5":
				big_red.Add("5", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM6":
				big_red.Add("6", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM7":
				big_red.Add("7", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM8":
				big_red.Add("8", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTNUM9":
				big_red.Add("9", TextureLoader.Instance.TextureFromLump(l));
				break;
				case "STTPRCNT":
				big_red.Add("%", TextureLoader.Instance.TextureFromLump(l));
				break;
			#endregion
			}
		}
	}
}
