using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System.Reflection;
using System;

/**
 * A class representing the structure and function of a file.
 * 
 * A file has a name, of the form:
 *     <name>.<extension>
 * as well as a string of contents. 
 * 
 * A directory is a file with a name of the form:
 *     <name>
 * as well as a List of File objects "contained" in it.
 * 
 * Both files and directories are created with a full Path.
 */
public class File {

	// The name of this file/directory.
	private string name;

	// File-only. The extension of this file.
	private string extension;

	// File-only. The inner contents of this file.
	private string contents = null;

	// Get-only; set on instantiation.
	public bool isDirectory {
		private set;
		get;
	}
	// Directory-only. Null for files; set on instantiation for directories.
	private List<File> files = null;

	// The absolute path of this file.
	private string path;

	// The parent file of this file.
	private File parent;

	/**
	 * Create a new File in the provided directory.
	 */
	public File(File parent, string name, bool isDir) {
		if (isDir) {
			Setup (parent, name, "", true);
		} else {
			if (name.Contains (".")) {
				// Create a new file with sliced name and extension
				Setup(
					parent,
					name.Substring(0, name.IndexOf(".")),
					name.Substring (name.LastIndexOf (".") + 1),
					false);
			}
		}
	}

	public File(File parent, string name, string ext, bool isDir) {
		Setup (parent, name, ext, isDir);
	}

	private void Setup(File parent, string name, string ext, bool isDir) {
		this.name = name;
		this.isDirectory = isDir;
		this.parent = parent;
		if (isDir) {
			this.path = parent == null ? "" : parent.path + "/" + name;
			this.extension = null;
			this.contents = null;
			this.files = new List<File> ();
		} else {
			this.path = parent == null ? "" : parent.path + "/" + name + "." + ext;
			this.extension = ext;
			this.contents = "";
			this.files = null;
		}

		if (parent != null) {
			parent.addFile (this);
		}
	}

	public string getContents() {
		if (isDirectory) {
			throw new InvalidOperationException ("Can't access the contents of a directory.");
		} else {
			return contents;
		}
	}

	public void setContents(string contents) {
		if (isDirectory) {
			throw new InvalidOperationException ("Can't set the contents of a directory.");
		} else {
			this.contents = contents;
		}
	}

	public List<File> getFiles() {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot list children of a file, only directories.");
		} else {
			return files;
		}
	}

	public int getNumFiles() {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot list children of a file, only directories.");
		} else {
			return files.Count;
		}
	}

	public void addFile(File file) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot add children to a file, only directories.");
		} else if (file == null) {
			throw new InvalidOperationException ("Cannot add a null file.");
		} else {
			files.Add (file);
		}
	}

	public void deleteFile(File file) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot remove children from a file, only directories.");
		} else if (file == null) {
			return;
		} else {
			files.Remove (file);
		}
	}

	public void deleteAllFiles() {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot remove children from a file, only directories.");
		} else {
			files.Clear ();
		}
	}

	public bool containsFile(File file) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot query children from a file, only directories.");
		} else {
			return files.Contains (file);
		}
	}

	public bool containsFile(string filename) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot query children from a file, only directories.");
		} else {
			foreach (File fi in files) {
				if (fi.getFullName().Equals (filename)) {
					return true;
				}
			}
			return false;
		}
	}

	public File getFile(File file) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot access children of a file, only directories.");
		} else {
			return files.Find (item => item.getFullName ().Equals (file.getFullName ()));
		}
	}

	public File getFile(string fileName) {
		if (!isDirectory) {
			throw new InvalidOperationException ("Cannot access children of a file, only directories.");
		} else {
			return files.Find (item => item.getFullName ().Equals (fileName));
		}
	}

	public string getFullName() {
		if (isDirectory) {
			return name;
		} else {
			return name + "." + extension;
		}
	}

	public string getName() {
		return name;
	}

	public void setName(string name, string ext="") {
		if (isDirectory) {
			this.name = name;
		} else {
			this.name = name;
			this.extension = ext;
		}
	}

	public string getExtension() {
		if (isDirectory) {
			throw new InvalidOperationException ("Directories don't have extensions.");
		} else {
			return this.extension;
		}
	}

	public string getPath() {
		return path;
	}

	public File getParent() {
		return parent;
	}

	public void moveTo(File newDir) {
		this.parent.deleteFile (this);
		newDir.addFile (this);

		this.parent = newDir;
		this.path = newDir.path + "/" + this.name;
		if (!this.isDirectory) {
			this.path += "." + this.extension;
		}
	}
}
