using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopNode : IFileSystem {
	FileSystem fs;

	public FileSystem getFileSystem() {
		return fs;
	}
}
