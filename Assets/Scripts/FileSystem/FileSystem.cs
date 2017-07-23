using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System;

public class FileSystem : MonoBehaviour {

	// The only valid filenames are "([0-9][a-z][A-Z]-_)*"
	private static Regex validNameRegex = new Regex (@"(\w?-?_?)+");
	private static string[] validFileExtensions = new string[] {
		"txt", "src"
	};
	public static readonly File root = new File (null, "", "" , true);

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static File createFile(string name, File dir=null) {
		if (!isValidFullFileName (name)) {
			throw new InvalidFileException ("Invalid file name.");
		}

		if (dir != null && !dir.isDirectory) {
			throw new InvalidFileException ("Can't create a child of a non-directory file.");
		}

		string newPath = dir == null ? name : dir.getPath () + "/" + name;
		if (getFile (newPath) == null) {
			if (dir == null) {
				File file = new File (root, name, false);
				return file;
			} else {
				File file = new File (dir, name, false);
				return file;
			}
		} else {
			throw new InvalidFileException ("File already exists!");
		}
	}

	public static File createFile(string name, string ext, File dir=null) {
		if (!isValidFileName (name) || !isValidExtension(ext)) {
			throw new InvalidFileException ("Invalid file name.");
		}

		if (dir != null && !dir.isDirectory) {
			throw new InvalidFileException ("Can't create a child of a non-directory file.");
		}

		string newPath = dir == null ? name : dir.getPath () + "/" + name;
		if (getFile (newPath) == null) {
			if (dir == null) {
				File file = new File (root, name, ext, false);
				return file;
			} else {
				File file = new File (dir, name, ext, false);
				return file;
			}
		} else {
			throw new InvalidFileException ("File already exists!");
		}
	}

	public static File createDirectory(string name, File dir=null) {
		if (!isValidFileName (name)) {
			throw new InvalidFileException ("Invalid file name.");
		}

		if (dir != null && !dir.isDirectory) {
			throw new InvalidFileException ("Can't create a child of a non-directory file.");
		}

		string newPath = dir == null ? name : dir.getPath () + "/" + name;
		if (getFile (newPath) == null) {
			if (dir == null) {
				File file = new File (root, name, true);
				return file;
			} else {
				File file = new File (dir, name, true);
				return file;
			}
		} else {
			throw new InvalidFileException ("Directory already exists!");
		}
	}

	/**
	 * Moves a file from one directory to a new one.
	 */
	public static void moveFile(File toMove, File newDir) {
		if (toMove == null || newDir == null) {
			throw new InvalidFileException ("Input or output files are invalid.");
		}
		if (!newDir.isDirectory) {
			throw new InvalidFileException ("Output file is not a directory.");
		}
		if (newDir.getPath () == null) {
			throw new InvalidFileException ("Output file has an invalid path. (bug in path creation?)");
		}
		if (toMove == newDir) {
			throw new InvalidFileException ("Output directory cannot match input directory.");
		}
		if (newDir.getPath ().Contains (toMove.getPath ())) {
			throw new InvalidFileException ("Cannot move directory within itself.");
		}

		toMove.moveTo (newDir);
	}

	public static File getFile(string path) {
		if (path == null) {
			return null;
		}

		string[] pathElements = path.Split( new char[]{'/'} );
		File currentFile = root;
		for (int i = 0; i < pathElements.Length; i++) {
			if (currentFile.isDirectory) {
				if (pathElements [i].Equals ("..")) {
					if (currentFile == root) {
						continue;
					} else {
						currentFile = currentFile.getParent ();
					}
				} else if (pathElements [i].Equals (".") || pathElements[i].Equals("")) {
					continue;
				} else if (currentFile.containsFile (pathElements [i])) {
					currentFile = currentFile.getFile (pathElements [i]);
				} else {
					// We could not find a file, so fail
					return null;
				}
			} else {
				// We found a terminal file in the middle of the path
				if (i != pathElements.Length - 1) {
					return null;
				}
			}
		}
		return currentFile;
	}

	/**
	 * True iff: 
	 *     - name is not null and name length is > 0
	 *     - filename contains "."
	 *     - name before "." contains word chars, "-", or "_" only
	 *     - name after "." is a valid file extension
	 */
	private static bool isValidFullFileName(string name) {
		if (name == null || name.Length == 0) {
			return false;
		}
		if (name.IndexOf (".") <= 0) {
			return false;
		}

		string nm = name.Substring (0, name.IndexOf ("."));
		string ex = name.Substring (name.LastIndexOf (".") + 1).ToLower();

		return (isValidFileName (nm) && isValidExtension (ex));
	}

	/**
	 * True iff name is not null or zero length, and contains word chars, "-", or "_" only.
	 */
	private static bool isValidFileName(string name) {
		if (name == null || name.Length == 0) {
			return false;
		}

		return validNameRegex.Matches (name).Count <= 2;
	}

	/**
	 * True iff extension is in validFileExtensions
	 */
	private static bool isValidExtension(string extension) {
		bool isValid = false;
		foreach (string str in validFileExtensions) {
			if (extension.Equals (str)) {
				isValid = true;
			}
		}
		return isValid;
	}
}

/** 
 * Thrown if you try to create or modify an invalid file; i.e., an invalid
 * name on creation.
 */
public class InvalidFileException : Exception {
	public InvalidFileException() : base("Attempted to create or modify invalid file.") {}
	public InvalidFileException(string msg) : base(msg) {}
}

/*
 * Thrown if you try to do an unsupported operation, like setting the contents of a directory.
 */
public class InvalidOperationException : Exception {
	public InvalidOperationException() : base("Unsupported file operation") {}
	public InvalidOperationException(string msg) : base(msg) {}
}