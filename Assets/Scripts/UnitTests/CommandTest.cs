using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class CommandTest {
	[Test]
	public void FileSystemCommandsBasic() {
		string output;
		Console.log.execute ("pwd", out output);
		Assert.Equals ("/", output);
	}
}
