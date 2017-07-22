using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorDisplay : MonoBehaviour
{
	public Text textDisplay;

	public void Awake ()
	{
		RectTransform rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = Vector2.zero;

		gameObject.SetActive (false);
	}

	public void displayError(string errText)
	{
		if(textDisplay != null)
			textDisplay.text = errText;
	}

	public void dismissWindow()
	{
		gameObject.SetActive (false);
	}
}
