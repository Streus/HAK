using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileSystemNS;

/// <summary>
/// An abstract class representing a node in a network.
/// This can be anything- a router, a desktop, or a toaster.
/// 
/// To actually create a node in the game, check out the concrete implementations
/// that extend this class and properly implement the functionality interfaces.
/// </summary>
public abstract class NetworkNode
{
    public List<NetworkNode> connections { get; }
    public string hostname { get; }
    public string ip { get; } // It may be necessary to make a class to enforce the format of this
    public bool active;

    protected NetworkNode(string hostname, string ip)
    {
        this.hostname = hostname;
        this.ip = ip;
        this.connections = new List<NetworkNode>();
        this.active = true;
    }

    /**
	 * Establishes a two-way connection between this node and the other node.
	 */
    public void addConnection(NetworkNode other)
    {
        if (this.connections.Contains(other))
        {
            return;
        }
        this.connections.Add(other);
        other.addConnection(this);
    }

    /**
	 * Scan this node's connections for a node with either the hostname or
	 * ip provided.
	 */
    public NetworkNode getConnection(string ipOrHostname)
    {
        foreach (NetworkNode n in connections)
        {
            if (n.ip.Equals(ipOrHostname) || n.hostname.Equals(ipOrHostname))
            {
                return n;
            }
        }
        return null;
    }

    public void deleteConnection(NetworkNode other)
    {
        if (this.connections.Contains(other))
        {
            this.connections.Remove(other);
        }

        if (other.connections.Contains(this))
        {
            other.connections.Remove(this);
        }
    }

    /**
	 * Attempt to ping a node given by the provided hostname or ip. 
	 * 
	 * This will conduct a DFS network search recursively through
	 * all connected nodes until the provided host is found. It will then
	 * continue through all paths to find a shorter one, ignoring any
	 * cycles in the graph. 
	 * 
	 * Returns:
	 * 	>0 if connected to the given hostname (number of hops)
	 *  =0 if "ipOrHostname" refers to this machine
	 *  -1 if could not find a connection to the given hostname.
	 */
    public long ping(string ipOrHostname)
    {
        return pingRecur(ipOrHostname, new Dictionary<NetworkNode, int>() { { this, 0 } });
    }

    public long ping(NetworkNode node)
    {
        return pingRecur(node.ip, new Dictionary<NetworkNode, int>() { { this, 0 } });
    }

    public long pingRecur(string ipOrHostname, Dictionary<NetworkNode, int> seen)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("This node is down. It can't perform any operations.");
        }

        if (ipOrHostname.Equals(this.ip) || ipOrHostname.Equals(this.hostname))
        {
            return 0;
        }

        if (ipOrHostname.Equals("localhost") || ipOrHostname.Equals("127.0.0.0"))
        {
            return 0;
        }

        long minVal = -1;
        foreach (NetworkNode n in connections)
        {
            if (!n.active)
            {
                continue;
            }

            if (n.ip.Equals(ipOrHostname) || n.hostname.Equals(ipOrHostname))
            {
                return 1;
            }

            if (!seen.ContainsKey(n))
            {
                Dictionary<NetworkNode, int> newSeen = new Dictionary<NetworkNode, int>(seen);
                newSeen.Add(n, 0);
                long pingVal = 1 + n.pingRecur(ipOrHostname, newSeen);
                if (pingVal > 0)
                {
                    if (minVal == -1)
                    {
                        minVal = pingVal;
                    }
                    else if (pingVal < minVal)
                    {
                        minVal = pingVal;
                    }
                }
            }
        }
        return minVal;
    }

    /**
	 * Like ping, but returns a NetworkNode if it can be reached, or null if not.
	 */
    public NetworkNode search(string ipOrHostname)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("This node is down. It can't perform any operations.");
        }

        if (ipOrHostname.Equals(this.ip) || ipOrHostname.Equals(this.hostname))
        {
            return this;
        }

        if (ipOrHostname.Equals("localhost") || ipOrHostname.Equals("127.0.0.0"))
        {
            return this;
        }

        return searchRecur(ipOrHostname, new HashSet<NetworkNode>() { { this } });
    }

    public NetworkNode searchRecur(string ipOrHostname, HashSet<NetworkNode> seen)
    {
        foreach (NetworkNode n in connections)
        {
            if (!n.active)
            {
                continue;
            }

            if (n.ip.Equals(ipOrHostname) || n.hostname.Equals(ipOrHostname))
            {
                return n;
            }

            if (!seen.Contains(n))
            {
                seen.Add(n);
                NetworkNode found = n.searchRecur(ipOrHostname, seen);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }
}
