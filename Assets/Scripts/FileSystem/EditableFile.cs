using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFile : File {
	// EditableFile-only. The inner contents of this file.
	private string contents = null;

	public EditableFile(FileSystem fs, Directory parent, string name) : base(fs, parent, name) {
		this.contents = "";
	}

	public EditableFile(FileSystem fs, Directory parent, string name, string ext) : base(fs, parent, name, ext) {
		this.contents = "";
	}

	public string getContents() {
		if (!filesystem.currentUser.canRead (this.permissions)) {
			throw new InvalidUserException("Cannot read file: insufficient permissions. (requires read)");
		}
		return contents;
	}

	public void setContents(string contents) {
		if (!filesystem.currentUser.canWrite (this.permissions)) {
			throw new InvalidUserException ("Cannot write to file: insufficient permissions. (requires write)");
		}
		this.contents = contents;
	}
}
