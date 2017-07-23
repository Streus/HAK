using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class FileSystemTest {
	
	[Test]
	public void BasicFileTest() {
		FileSystem.root.deleteAllFiles ();

		File f = FileSystem.createFile ("test.txt");
		Assert.IsNotNull (f);
		Assert.AreEqual ("test.txt", f.getFullName());
		Assert.AreEqual ("test", f.getName());
		Assert.AreEqual ("txt", f.getExtension ());
		Assert.AreEqual ("", f.getContents ());
		Assert.AreEqual (false, f.isDirectory);
		Assert.AreEqual ("/test.txt", f.getPath ());
		Assert.AreEqual (FileSystem.root, f.getParent ());
	}

	[Test]
	public void CanHandleGoodFileNames() {
		FileSystem.root.deleteAllFiles ();

		File f;

		f = FileSystem.createFile("test.txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile("hello-world.txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile("hello_world.txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile("test123.txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile ("test-123-hello1_new.txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile ("test", "txt");
		Assert.IsNotNull (f);

		f = FileSystem.createFile ("tes123t", "txt");
		Assert.IsNotNull (f);
	}

	[Test]
	public void CanHandleBadFileNames() {
		FileSystem.root.deleteAllFiles ();

		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile (null); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile (""); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("."); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile (".txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("test?.txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("he!llo_world123.txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("abc@.tst"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("abc.tst"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("abc", "tst"); });
		Assert.Throws(typeof(InvalidFileException), delegate { FileSystem.createFile ("1231@afa", "txt"); });
	}

	[Test]
	public void CanSetContents() {
		FileSystem.root.deleteAllFiles ();

		File f = FileSystem.createFile ("test.txt");
		Assert.AreEqual (f.getContents (), "");
		f.setContents ("hello world");
		Assert.AreEqual (f.getContents (), "hello world");
	}

	[Test]
	public void CannotSetDirectoryContents() {
		FileSystem.root.deleteAllFiles ();

		File f = FileSystem.createDirectory ("test");
		Assert.Throws(typeof(InvalidOperationException), delegate { f.setContents ("hello world"); });
		Assert.Throws(typeof(InvalidOperationException), delegate { f.getContents (); });
	}

	[Test]
	public void CanAddFilesToDirectory() {
		FileSystem.root.deleteAllFiles ();

		File f = FileSystem.createDirectory ("test");
		Assert.IsNotNull (f.getFiles ());
		Assert.AreEqual (f.getFiles (), new List<File> ());

		File fi = FileSystem.createFile("test.txt");
		f.addFile (fi);
		Assert.AreEqual (f.getFiles (), new List<File> () { fi });
		Assert.AreEqual (f.getNumFiles (), 1);
	}

	[Test]
	public void CanHandleParentsBasic() {
		FileSystem.root.deleteAllFiles ();

		File dir = FileSystem.createDirectory ("test");
		Assert.IsNotNull (dir);
		Assert.AreEqual (dir.getNumFiles (), 0);

		File file = FileSystem.createFile("hello.txt", dir);
		Assert.IsNotNull (file);

		// Are the parents showing up properly?
		Assert.AreEqual (dir.getParent (), FileSystem.root);
		Assert.AreEqual (file.getParent (), dir);
	}

	[Test]
	public void CanHandlePathsBasic() {
		FileSystem.root.deleteAllFiles ();

		File dir = FileSystem.createDirectory ("test");
		File file = FileSystem.createFile("hello.txt", dir);

		// Are the paths correct?
		Assert.AreEqual (dir.getPath (), "/test");
		Assert.AreEqual (file.getPath (), "/test/hello.txt");}

	[Test]
	public void CanHandleDirectoriesBasic() {
		FileSystem.root.deleteAllFiles ();

		File dir = FileSystem.createDirectory ("test");
		File file = FileSystem.createFile("hello.txt", dir);

		// Does the directory now contain the file?
		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir.getFiles (), new List<File>() { file });

		// Can we make a directory a child of a normal file?
		Assert.Throws (typeof(InvalidFileException), delegate {
			File file2 = FileSystem.createFile ("hello2.txt", file);
		});
	}

	[Test]
	public void CanHandleDirectoriesAdvanced() {
		FileSystem.root.deleteAllFiles ();

		// Can we create multiple files with the same path?
		Assert.Throws (typeof(InvalidFileException), delegate {
			File f1 = FileSystem.createFile("test1.txt");
			File f2 = FileSystem.createFile("test1.txt");
		});

		// Can we create directories that don't exist?
		Assert.Throws (typeof(InvalidFileException), delegate {
			File f = FileSystem.createFile ("test/testing.txt");
		});
	}

	[Test]
	public void DoesMoveFileWorkBasic() {
		FileSystem.root.deleteAllFiles ();

		File f = FileSystem.createFile ("test.txt");
		File dir = FileSystem.createDirectory ("testing");
		Assert.AreEqual (f.getParent (), FileSystem.root);
		Assert.AreEqual (f.getPath (), "/test.txt");
		Assert.AreEqual (dir.getPath (), "/testing");

		FileSystem.moveFile (f, dir);
		Assert.AreEqual (f.getParent (), dir);
		Assert.AreEqual (f.getPath (), dir.getPath () + "/" + f.getFullName ());
		Assert.AreEqual ("/testing/test.txt", dir.getPath () + "/" + f.getFullName ());
	}

	[Test]
	public void DoesMoveFileWorkAdvanced() {
		FileSystem.root.deleteAllFiles ();

		File dir = FileSystem.createDirectory ("testing");
		File dir2 = FileSystem.createDirectory ("testing2");
		File f = FileSystem.createFile ("test.txt");
		Assert.AreEqual (f.getParent (), FileSystem.root);
		Assert.AreEqual (f.getPath (), "/test.txt");
		Assert.AreEqual (dir.getPath (), "/testing");
		Assert.AreEqual (dir2.getPath (), "/testing2");

		FileSystem.moveFile (dir2, dir);
		Assert.AreEqual (dir2.getParent (), dir);
		Assert.AreEqual (dir2.getPath (), dir.getPath () + "/" + dir2.getFullName ());

		FileSystem.moveFile (f, dir2);
		Assert.AreEqual (f.getParent (), dir2);
		Assert.AreEqual (f.getParent ().getParent (), dir);
		Assert.AreEqual (f.getPath (), dir2.getPath () + "/" + f.getFullName ());
		Assert.AreEqual (f.getPath (), dir.getPath () + "/" + dir2.getFullName() + "/" + f.getFullName ());

		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir2.getNumFiles (), 1);

		File dir3 = FileSystem.createDirectory ("testing3", dir2);
		Assert.AreEqual (dir3.getPath (), dir2.getPath () + "/" + dir3.getFullName ());
		Assert.AreEqual (dir2.getNumFiles (), 2);

		FileSystem.moveFile (dir3, dir2);
		Assert.AreEqual (dir3.getPath (), dir2.getPath () + "/" + dir3.getFullName ());
		Assert.AreEqual (dir2.getNumFiles (), 2);

		FileSystem.moveFile (f, dir3);
		Assert.AreEqual (f.getParent (), dir3);
		Assert.AreEqual (f.getPath (), dir3.getPath () + "/" + f.getFullName ());
		Assert.AreEqual ("/testing/testing2/testing3/test.txt", f.getPath ());
		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir2.getNumFiles (), 1);
		Assert.AreEqual (dir3.getNumFiles (), 1);

		// Can't move files to/from null locations
		Assert.Throws (typeof(InvalidFileException), delegate {
			FileSystem.moveFile (null, dir3);
		});
		Assert.Throws (typeof(InvalidFileException), delegate {
			FileSystem.moveFile (dir3, null);
		});
		Assert.AreEqual (dir3.getParent (), dir2);

		// Can't move directory inside itself
		Assert.Throws (typeof(InvalidFileException), delegate { 
			FileSystem.moveFile (dir3, dir3);
		});

		// Can't move directories inside children of themselves
		Assert.Throws (typeof(InvalidFileException), delegate { 
			FileSystem.moveFile (dir2, dir3);
		});
	}

	[Test]
	public void DoesGetFileWorkBasic() {
		FileSystem.root.deleteAllFiles ();

		File dir = FileSystem.createDirectory ("testing");
		File f = FileSystem.createFile ("test.txt", dir);
		Assert.AreEqual (f.getParent (), dir);
		Assert.AreEqual (f.getPath (), "/testing/test.txt");

		// Basic containment tests
		Assert.AreEqual (dir.containsFile (f), true);
		Assert.AreEqual (dir.containsFile ("test.txt"), true);

		// Basic "get" containment tests
		Assert.AreEqual (dir.getFile (f), f);
		Assert.AreEqual (dir.getFile ("test.txt"), f);
		Assert.AreEqual (dir.getFile (f.getFullName()), f);
		Assert.IsNull (dir.getFile ("meow"));
		Assert.IsNull (dir.getFile (f.getName()));

		Assert.AreEqual (1, FileSystem.root.getNumFiles ());
		Assert.IsNull (FileSystem.root.getFile (f));

		// FileSystem get tests
		Assert.AreEqual(FileSystem.getFile("testing/test.txt"), f);
		Assert.AreEqual(FileSystem.getFile("/testing/test.txt"), f);
		Assert.IsNull(FileSystem.getFile("testing/meow.txt"));
		Assert.IsNull(FileSystem.getFile("/testing/meow.txt"));
		Assert.IsNull (FileSystem.getFile ("test.txt"));
		Assert.IsNull (FileSystem.getFile ("/test.txt"));

		Assert.AreEqual (FileSystem.getFile ("/"), FileSystem.root);
	}

	[Test]
	public void DoesGetFileWorkAdvanced() {
		FileSystem.root.deleteAllFiles ();

		Assert.AreEqual (FileSystem.root.getNumFiles (), 0);

		File dir = FileSystem.createDirectory ("meow");
		Assert.AreEqual (FileSystem.root.getNumFiles (), 1);
		Assert.AreEqual(FileSystem.getFile("meow"), dir);
		Assert.AreEqual(FileSystem.getFile("/meow"), dir);
		Assert.AreEqual(FileSystem.getFile("./meow"), dir);
		Assert.AreEqual(FileSystem.getFile("../meow"), dir);
		Assert.AreEqual(FileSystem.getFile("./././././././meow"), dir);
		Assert.AreEqual(FileSystem.getFile("../../../../../../../meow"), dir);
		Assert.AreEqual(FileSystem.getFile("./../././../././meow"), dir);
		Assert.IsNull(FileSystem.getFile(".../meow"));

		File dir2 = FileSystem.createDirectory ("test", dir);
		Assert.AreEqual (FileSystem.root.getNumFiles (), 1);
		Assert.IsNull(FileSystem.getFile("test"));
		Assert.AreEqual(FileSystem.getFile("meow/test"), dir2);
		Assert.AreEqual(FileSystem.getFile("/meow/test"), dir2);
		Assert.AreEqual(FileSystem.getFile("./meow/test"), dir2);
		Assert.AreEqual(FileSystem.getFile("../meow/test"), dir2);
		Assert.IsNull(FileSystem.getFile("meow/../test"));
		Assert.IsNull(FileSystem.getFile("/meow/../test"));
		Assert.IsNull(FileSystem.getFile("./meow/../test"));
		Assert.IsNull(FileSystem.getFile("../meow/../test"));
		Assert.AreEqual(FileSystem.getFile("meow/../meow/"), dir);
		Assert.AreEqual(FileSystem.getFile("meow/../meow/test"), dir2);
		Assert.AreEqual (FileSystem.getFile ("///////////////meow"), dir);
		Assert.AreEqual (FileSystem.getFile ("///meow////////////"), dir);
		Assert.AreEqual (FileSystem.getFile ("//////../meow/./test/.././test/../..////"), FileSystem.root);
	}
}