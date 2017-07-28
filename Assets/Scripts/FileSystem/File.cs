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
	protected string name;

	// Non-directory-only. The extension of this file.
	private string extension = null;

	// The absolute path of this file.
	protected string path;

	// The parent file of this file.
	protected Directory parent;

	// Allows for subclasses to figure out their own constructors
	protected File() {}

	/**
	 * Create a new File in the provided directory.
	 */
	public File(Directory parent, string name) {
		if (name.Contains (".")) {
			// Create a new file with sliced name and extension
			Setup(
				parent,
				name.Substring(0, name.IndexOf(".")),
				name.Substring (name.LastIndexOf (".") + 1));
		}
		//Should this throw an exception otherwise?
	}

	public File(Directory parent, string name, string ext) {
		Setup (parent, name, ext);
	}

	private void Setup(Directory parent, string name, string ext) {
		this.name = name;
		this.parent = parent;
		this.path = parent == null ? "" : parent.path + "/" + name + "." + ext;
		this.extension = ext;

		if (parent != null) {
			parent.addFile (this);
		}
	}

	public string getFullName() {
		return name + "." + extension;
	}

	public string getName() {
		return name;
	}

	public void setName(string name, string ext) {
		if (ext == null || ext.Length == 0) {
			throw new InvalidFileException ("Files must have a valid extension.");
		}
		this.name = name;
		this.extension = ext;
	}

	// Non-directory-file only
	public string getExtension() {
		return this.extension;
	}

	public string getPath() {
		return path;
	}

	public File getParent() {
		return parent;
	}

	public void moveTo(Directory newDir) {
		this.parent.deleteFile (this);
		newDir.addFile (this);

		this.parent = newDir;
		this.path = newDir.path + "/" + this.getFullName();
	}
}
