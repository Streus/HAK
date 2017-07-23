using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class File {
	
	private string name;
	private string extension;
	private string contents;

	// Get-only; set on instantiation.
	public bool isDirectory {
		private set;
		get;
	}
	// Null for files; set on instantiation for directories.
	private List<File> files = null;

	public File(string name) {
		if (name.Contains (".")) {
			this.name = name.Substring (0, name.IndexOf ("."));
			this.extension = name.Substring (name.IndexOf ("."));

		}
	}

	public File(string name, string ext) {
		this.name = name;
		this.extension = ext;
		this.isDirectory = false;
		this.contents = "";
	}

	public string getContents() {
		if (isDirectory) {
			return null;
		} else {
			return contents;
		}
	}

	public List<File> getFiles() {
		if (isDirectory) {
			return files;
		} else {
			return null;
		}
	}

	public void setName(string name, string ext="") {
		if (isDirectory) {
			this.name = name;
		} else {
			this.name = name;
			this.extension = ext;
		}
	}

	public string getName() {
		if (isDirectory) {
			return name;
		} else {
			return name + "." + extension;
		}
	}
}
