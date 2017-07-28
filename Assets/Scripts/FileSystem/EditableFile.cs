using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableFile : File {
	// EditableFile-only. The inner contents of this file.
	private string contents = null;

	public EditableFile(Directory parent, string name) : base(parent, name) {
		this.contents = "";
	}

	public EditableFile(Directory parent, string name, string ext) : base(parent, name, ext) {
		this.contents = "";
	}

	public string getContents() {
		return contents;
	}

	public void setContents(string contents) {
		this.contents = contents;
	}
}
