using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class FileSystemTest {
	
	[Test]
	public void InstantiateProperly() {
		File f = new File ("test", "txt");
		Assert.IsNotNull (f);
		Assert.AreEqual (f.getName(), "test.txt");
		Assert.AreEqual (f.getContents (), "");
	}

	[Test]
	public void CanSetContents() {
		File f = new File ("test", "txt");
	}
}