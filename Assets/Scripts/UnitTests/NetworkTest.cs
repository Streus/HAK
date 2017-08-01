using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class NetworkTest {

	[Test]
	public void BasicDesktopNodeTest() {
		DesktopNode node = new DesktopNode ("test.computer", "10.20.30.40");
		Assert.IsNotNull (node);

		// Some connection tests
		DesktopNode otherNode = new DesktopNode("test.otherComp", "10.20.30.41");
		Assert.IsNotNull (otherNode);

		// Some filesystem tests
		FileSystem fs = node.fileSystem;
		Assert.IsNotNull (fs);

		Directory fi = fs.createDirectory ("test");
		Assert.AreEqual (fi, fs.getFile ("test"));
		Assert.AreEqual (1, fs.root.getNumFiles ());


	}

	[Test]
	public void CanAddConnections() {
		DesktopNode node = new DesktopNode ("test.computer", "10.20.30.40");
		Assert.IsNotNull (node);

		DesktopNode otherNode = new DesktopNode("test.otherComp", "10.20.30.41");
		Assert.IsNotNull (otherNode);

		node.addConnection (otherNode);
		Assert.AreEqual (1, node.connections.Count);
		Assert.AreEqual (otherNode, node.getConnection(otherNode.hostname));

		Assert.AreEqual (1, otherNode.connections.Count);
		Assert.AreEqual (node, otherNode.getConnection(node.hostname));
	}

	[Test]
	public void CanDeleteConnections() {
		DesktopNode node = new DesktopNode ("test.computer", "10.20.30.40");
		Assert.IsNotNull (node);

		DesktopNode otherNode = new DesktopNode("test.otherComp", "10.20.30.41");
		Assert.IsNotNull (otherNode);

		node.addConnection (otherNode);
		node.deleteConnection (otherNode);

		Assert.AreEqual (0, node.connections.Count);
		Assert.IsNull (node.getConnection(otherNode.hostname));

		Assert.AreEqual (0, otherNode.connections.Count);
		Assert.IsNull (otherNode.getConnection(node.hostname));

	}

	[Test]
	public void BasicPingTest() {
		DesktopNode node1 = new DesktopNode ("test.computer", "1");
		DesktopNode node2 = new DesktopNode ("test.computer", "2");
		DesktopNode node3 = new DesktopNode ("test.computer", "3");
		DesktopNode node4 = new DesktopNode ("test.computer", "4");

		node1.addConnection (node2);
		node2.addConnection (node3);

		Assert.AreEqual (0, node1.ping (node1));
		Assert.AreEqual (1, node1.ping (node2));
		Assert.AreEqual (1, node2.ping (node1));
		Assert.AreEqual (1, node2.ping (node3));
		Assert.AreEqual (1, node3.ping (node2));
		Assert.AreEqual (2, node1.ping (node3));
		Assert.AreEqual (2, node3.ping (node1));
		Assert.AreEqual (-1, node1.ping (node4));
		Assert.AreEqual (-1, node2.ping (node4));
		Assert.AreEqual (-1, node3.ping (node4));
		Assert.AreEqual (-1, node4.ping (node1));
		Assert.AreEqual (-1, node4.ping (node2));
		Assert.AreEqual (-1, node4.ping (node3));

		// Make a circular connection. Should still return shortest hop.
		node3.addConnection(node1);

		Assert.AreEqual (0, node1.ping (node1));
		Assert.AreEqual (1, node1.ping (node2));
		Assert.AreEqual (1, node2.ping (node1));
		Assert.AreEqual (1, node2.ping (node3));
		Assert.AreEqual (1, node3.ping (node2));
		Assert.AreEqual (1, node1.ping (node3)); // Only these two change
		Assert.AreEqual (1, node3.ping (node1)); // ""
		Assert.AreEqual (-1, node1.ping (node4));
		Assert.AreEqual (-1, node2.ping (node4));
		Assert.AreEqual (-1, node3.ping (node4));
		Assert.AreEqual (-1, node4.ping (node1));
		Assert.AreEqual (-1, node4.ping (node2));
		Assert.AreEqual (-1, node4.ping (node3));
	}

	[Test]
	public void BasicNetworkTest() {
		Network net = new Network ();

	}
}
