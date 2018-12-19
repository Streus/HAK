using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Routers connect directly to other routers, as well as have nodes
/// "underneath" them in their local area network.
/// </summary>
public class RouterNode : NetworkNode
{
    public RouterNode(string hostname, string ip) : base(hostname, ip) { }
}
