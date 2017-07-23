using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystem : MonoBehaviour {

	public File root;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public File createFile(string name, string ext) {
		File file = new File (name, ext);
		return file;
	}
}
