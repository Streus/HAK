﻿using System.Collections;
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
	public List<NetworkNode> connections { get; }
	public string hostname { get; }
	public string ip { get; } // It may be necessary to make a class to enforce the format of this

	protected NetworkNode(string hostname, string ip) {
		this.hostname = hostname;
		this.ip = ip;
		this.connections = new List<NetworkNode> ();
	}

	/**
	 * Establishes a two-way connection between this node and the other node.
	 */
	public void addConnection(NetworkNode other) {
		if (this.connections.Contains (other)) {
			return;
		}
		this.connections.Add (other);
		other.addConnection (this);
	}

	/**
	 * Scan this node's connections for a node with either the hostname or
	 * ip provided.
	 */
	public NetworkNode getConnection(string ipOrHostname) {
		foreach(NetworkNode n in connections) {
			if (n.ip.Equals (ipOrHostname) || n.hostname.Equals(ipOrHostname)) {
				return n;
			}
		}
		return null;
	}

	public void deleteConnection(NetworkNode other) {
		if (this.connections.Contains (other)) {
			this.connections.Remove (other);
		}

		if (other.connections.Contains (this)) {
			other.connections.Remove (this);
		}
	}

	/**
	 * Attempt to ping a node given by the provided hostname or ip. 
	 * 
	 * This will conduct a DFS network search recursively through
	 * all connected nodes until the provided host is found.
	 * 
	 * Returns:
	 * 	>0 if connected to the given hostname (number of hops)
	 *  =0 if "ipOrHostname" refers to this machine
	 *  -1 if could not find a connection to the given hostname.
	 */
	public long ping(string ipOrHostname) {
		return pingRecur (ipOrHostname, new List<NetworkNode> (){ this });
	}

	public long ping(NetworkNode node) {
		return pingRecur(node.ip, new List<NetworkNode>(){ this });
	}

	public long pingRecur(string ipOrHostname, List<NetworkNode> seen) {
		if (ipOrHostname.Equals (this.ip) || ipOrHostname.Equals (this.hostname)) {
			return 0;
		}

		if (ipOrHostname.Equals ("localhost") || ipOrHostname.Equals ("127.0.0.0")) {
			return 0;
		}

		foreach (NetworkNode n in connections) {
			if (n.ip.Equals (ipOrHostname) || n.hostname.Equals (ipOrHostname)) {
				return 1;
			} else {
				if (!(seen.Contains (n))) {
					seen.Add (n);
					long pingVal = n.pingRecur (ipOrHostname, seen);
					if (pingVal > 0) {
						return 1 + pingVal;
					}
				}
			}
		}
		return -1;
	}
}
