using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollingText : MonoBehaviour {

    public Font font;

    private int lineWidthInPixels;
    private int screenWidthInPixels;

    private RectTransform rectTransform;

    List<Text> texts;
    private static GameObject infiniteTextSection;

	// Use this for initialization
	void Start () {
        //rectTransform = GetComponent<RectTransform>();

        //texts = new List<Text>();

        //infiniteTextSection = Resources.Load<GameObject>("Prefabs/InfiniteTextSection");

        //// TODO replace 80 with lineWidth from manager
        //lineWidthInPixels = GetWidthOfString(new string(' ', 80));
        //screenWidthInPixels = Screen.width;

        //float scale = (float)lineWidthInPixels / screenWidthInPixels;

        //Debug.Log(GetWidthOfString("Hello world!"));
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.SetText(new string('*', 128 * 1000));
        text.ForceMeshUpdate();
        Debug.Log("Chars: " + text.textInfo.characterCount);
        //Debug.Log("Meshes: " + text.textInfo.meshInfo.Length);

    }
	
    public int GetWidthOfString(string message)
    {
        int width = 0;
        char[] charArray = message.ToCharArray();
        for (int i = 0; i < charArray.Length; i++) 
        {
            CharacterInfo info;
            font.GetCharacterInfo(charArray[i], out info, font.fontSize, FontStyle.Normal);
            width += info.advance;
            //width += info.glyphWidth;
        }
        return width;
    }
}
