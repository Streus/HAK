using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopNode : NetworkNode, IFileSystem {
	public FileSystem fileSystem { get; }

	public DesktopNode(string hostname, string ip) : base(hostname, ip) {
		fileSystem = new FileSystem ();
	}
}
