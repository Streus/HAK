using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTexture {
	/**
	 * Okay. crazy idea time. here's the flowchart.
	 * 
	 * - take as input a string and a preferred output size in pixels (w, h).
	 * - get some set font that is monospace
	 * - find the maximum font size for which:
	 * 		- (80 * charWidth) <= w
	 * - draw 80 characters per line of the input string to a bitmap
	 * - save this bitmap into bytes
	 * - turn those bytes into a texture2d
	 * - ???
	 * - profit
	 */
	public static Texture2D renderTexture(string input) {
		// So, I've tried to use System.Drawing and it's just ridiculously not supported
		// cross-platform in Unity. There's a small chance it'll work for Windows, but it'll
		// require some intense hunting and definite recompiling for Mac for .Net 3.5 or less,
		// and that's just enough hassle to not be worth it.

		// TODO: next attempt is to outsource this to a python script and externally call it.
		// Then read the generated file (or bytes, preferrably), and turn that into a Texture2D.
		return null;
	}
}
