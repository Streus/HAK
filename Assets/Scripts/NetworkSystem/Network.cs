using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A data structure encapsulating an entire network.
 * 
 * This is essentially a NetworkNode web, with additional information 
 * such as a starting point, which is needed for level data.
 */
public class Network {
	// Where do we start in this level?
	public NetworkNode defaultStart;

	public Network() {}

	public void setStart(NetworkNode node) {
		this.defaultStart = node;
	}

	public NetworkNode getStart() {
		return this.defaultStart;
	}

	public static Network getLevel1() {
		Network newNetwork = new Network ();

		RouterNode router = new RouterNode ("home-router", "192.168.0.2");
		DesktopNode home = new DesktopNode ("home-desktop", "192.168.0.3");
		DesktopNode mobile = new DesktopNode ("george-phone", "192.168.0.4");

		FileSystem hfs = home.fileSystem;
		hfs.createDirectory ("home");
		hfs.createDirectory ("/home/desktop");
		hfs.createFile ("/home/desktop/pass.txt")
			.setContents ("Don't tell anyone, but my password...\nis...\nhelloworld123");
		hfs.createDirectory ("tmp");
		hfs.createFile ("/tmp/flies.src")
			.setContents ("Nothing here but us flies.");
		hfs.createDirectory ("bin");

		router.addConnection (home);
		router.addConnection (mobile);
		newNetwork.setStart (home);

		return newNetwork;
	}
}
