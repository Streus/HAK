using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static string currentPath = "";

	// TODO: When we have a network system, chance this to be the current network host.
	// That should hold on to a FileSystem reference on its own.
	public static FileSystem currentFileSystem = new FileSystem();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
