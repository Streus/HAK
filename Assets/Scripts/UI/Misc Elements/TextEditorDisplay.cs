using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEditorDisplay : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private InputField input;

	private Text display;

	[SerializeField]
	public float idleTimerMax = 2f;
	private float idleTimer = 2f;

	private bool refreshed;

	/* Instance Methods */
	public void Awake()
	{
		idleTimer = idleTimerMax;
		refreshed = false;

		display = GetComponent<Text> ();
	}
	public void Start()
	{
		input.onValueChanged.AddListener (delegate { 
			idleTimer = idleTimerMax; 
			refreshed = false; 
		});
		updateHighlights ();
	}

	public void Update()
	{
		display.text = input.text;

		idleTimer -= Time.deltaTime;
		if (idleTimer <= 0f && !refreshed)
			updateHighlights ();
	}

	// Highlight all syntax
	public void updateHighlights()
	{
		display.text = input.text;

		display.text = "<b>" + display.text + "</b>";

		refreshed = true;
	}
}
