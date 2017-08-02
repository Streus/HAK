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

		// We check that adding affects both nodes.
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

		// We check that deleting affects both nodes.
		node.addConnection (otherNode);
		node.deleteConnection (otherNode);

		Assert.AreEqual (0, node.connections.Count);
		Assert.IsNull (node.getConnection(otherNode.hostname));

		Assert.AreEqual (0, otherNode.connections.Count);
		Assert.IsNull (otherNode.getConnection(node.hostname));
	}

	[Test]
	public void CanGetConnections() {
		DesktopNode node = new DesktopNode ("test.computer", "10.20.30.40");
		Assert.IsNotNull (node);

		DesktopNode otherNode = new DesktopNode("test.otherComp", "10.20.30.41");
		Assert.IsNotNull (otherNode);

		node.addConnection (otherNode);
		Assert.AreEqual (1, node.connections.Count);
		Assert.AreEqual (1, otherNode.connections.Count);

		Assert.AreEqual (otherNode, node.getConnection ("test.otherComp"));
		Assert.AreEqual (otherNode, node.getConnection ("10.20.30.41"));

		// What happens if a node appears identical to us? We shouldn't find ourselves.
		DesktopNode selfSimilarNode = new DesktopNode ("test.computer", "10.20.30.40");
		node.addConnection (selfSimilarNode);

		Assert.AreEqual (selfSimilarNode, node.getConnection ("test.computer"));
		Assert.AreEqual (selfSimilarNode, node.getConnection ("10.20.30.40"));
		Assert.AreNotEqual (node, node.getConnection ("test.computer"));
		Assert.AreNotEqual (node, node.getConnection ("10.20.30.40"));
	}

	[Test]
	public void BasicPingTest() {
		DesktopNode node1 = new DesktopNode ("test.computer", "1");
		DesktopNode node2 = new DesktopNode ("test.computer", "2");
		DesktopNode node3 = new DesktopNode ("test.computer", "3");
		DesktopNode node4 = new DesktopNode ("test.computer", "4");

		// 1 <-> 2 <-> 3
		node1.addConnection (node2);
		node2.addConnection (node3);

		Assert.AreEqual (0, node1.ping (node1)); // Can you ping yourself?
		Assert.AreEqual (1, node1.ping (node2)); // Can you ping direct connections?
		Assert.AreEqual (1, node2.ping (node1));
		Assert.AreEqual (1, node2.ping (node3));
		Assert.AreEqual (1, node3.ping (node2));
		Assert.AreEqual (2, node1.ping (node3)); // Can you ping distant connections?
		Assert.AreEqual (2, node3.ping (node1));
		Assert.AreEqual (-1, node1.ping (node4)); // Does an invalid ping fail?
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
		Assert.AreEqual (1, node1.ping (node3)); // There is now
		Assert.AreEqual (1, node3.ping (node1)); // a shorter path.
		Assert.AreEqual (-1, node1.ping (node4));
		Assert.AreEqual (-1, node2.ping (node4));
		Assert.AreEqual (-1, node3.ping (node4));
		Assert.AreEqual (-1, node4.ping (node1));
		Assert.AreEqual (-1, node4.ping (node2));
		Assert.AreEqual (-1, node4.ping (node3));

		//    1
		//   / \
		//  2-- 3-- 4
		node3.addConnection (node4);

		Assert.AreEqual (0, node1.ping (node1));
		Assert.AreEqual (1, node1.ping (node2));
		Assert.AreEqual (1, node2.ping (node1));
		Assert.AreEqual (1, node2.ping (node3));
		Assert.AreEqual (1, node3.ping (node2));
		Assert.AreEqual (1, node1.ping (node3));
		Assert.AreEqual (1, node3.ping (node1));
		Assert.AreEqual (2, node1.ping (node4)); // Below these change
		Assert.AreEqual (2, node2.ping (node4));
		Assert.AreEqual (1, node3.ping (node4));
		Assert.AreEqual (2, node4.ping (node1));
		Assert.AreEqual (2, node4.ping (node2));
		Assert.AreEqual (1, node4.ping (node3));
	}

	[Test]
	public void BasicNetworkTest() {
		Network net = new Network ();

	}
}
