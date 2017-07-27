using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * An abstract class representing a node in a network.
 * This can be anything- a router, a desktop, or a toaster.
 * 
 * To actually create a node in the game, check out the concrete implementations
 * that extend this class and properly implement the functionality interfaces.
 */
public abstract class NetworkNode {
	private List<NetworkNode> connections;

	public List<NetworkNode> getConnections() {
		return connections;
	}
}
