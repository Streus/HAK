using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directory : File {
	// Directory-only. Null for files; set on instantiation for directories.
	private List<File> files = null;

	public Directory(Directory parent, string name) {
		Setup (parent, name);
	}

	private void Setup(Directory parent, string name) {
		this.name = name;
		this.parent = parent;
		this.path = parent == null ? "" : parent.path + "/" + name;
		this.files = new List<File> ();
		if (parent != null) {
			parent.addFile (this);
		}
	}

	public List<File> getFiles() {
		return files;
	}

	public int getNumFiles() {
		return files.Count;
	}

	public void addFile(File file) {
		if (file == null) {
			throw new InvalidOperationException ("Cannot add a null file.");
		} else {
			files.Add (file);
		}
	}

	public void deleteFile(File file) {
		if (file == null) {
			return;
		} else {
			files.Remove (file);
		}
	}

	public void deleteAllFiles() {
		files.Clear ();
	}

	public bool containsFile(File file) {
		return files.Contains (file);
	}

	public bool containsFile(string filename) {
		if (filename == null || filename.Length == 0) {
			return false;
		}

		foreach (File fi in files) {
			if (fi.getFullName().Equals (filename)) {
				return true;
			}
		}
		return false;
	}

	public File getFile(File file) {
		return files.Find (item => item.getFullName ().Equals (file.getFullName ()));
	}

	public File getFile(string fileName) {
		return files.Find (item => item.getFullName ().Equals (fileName));
	}

	// "ext" is only there to match override. It can be ignored.
	public void setName(string name, string ext="") {
		this.name = name;
	}

	// Returns the name of this directory. Ignores any extensions.
	public string getFullName() {
		return name;
	}
}
