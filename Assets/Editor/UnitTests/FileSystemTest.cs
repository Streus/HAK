using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using FileSystemNS;

[TestFixture]
public class FileSystemTest {

	[Test]
	public void BasicFileSystemTest() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);
	}
	
	[Test]
	public void BasicFileTest() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		EditableFile f = fs.createFile ("test.txt");
		Assert.IsNotNull (f);
		Assert.AreEqual ("test.txt", f.getFullName());
		Assert.AreEqual ("test", f.getName());
		Assert.AreEqual ("txt", f.getExtension ());
		Assert.AreEqual ("/test.txt", f.getPath ());
		Assert.AreEqual (fs.root, f.getParent ());
	}

	[Test]
	public void CanHandleGoodFileNames() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		File f;

		f = fs.createFile("test.txt");
		Assert.IsNotNull (f);

		f = fs.createFile("hello-world.txt");
		Assert.IsNotNull (f);

		f = fs.createFile("hello_world.txt");
		Assert.IsNotNull (f);

		f = fs.createFile("test123.txt");
		Assert.IsNotNull (f);

		f = fs.createFile ("test-123-hello1_new.txt");
		Assert.IsNotNull (f);
	}

	[Test]
	public void CanHandleBadFileNames() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile (null); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile (""); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile ("."); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile (".txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile ("test?.txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile ("he!llo_world123.txt"); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile ("abc@.tst"); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile ("abc.tst"); });
	}

	// TODO: Test this more thoroughly. Create many positive and negative cases, 
	// test for deeper nested directories, test for circular generation (if possible),
	// test for creating files in directories that don't yet exist.
	[Test]
	public void CanCreateFilesDirectlyInDirectories() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory d1 = fs.createDirectory ("test");
		Directory d2 = fs.createDirectory (d1.getPath() + "/" + "meow");

		// Can we create a file the normal way?
		File f1 = fs.createFile ("hello.txt");
		Assert.AreEqual (f1, fs.root.getFile ("hello.txt"));

		// Can we create a file inside a directory?
		File f2 = fs.createFile ("test/hello.txt");
		Assert.AreEqual (2, d1.getNumFiles ());
		Assert.AreEqual (f2, d1.getFile ("hello.txt"));
		Assert.AreEqual (f2, fs.getFile ("test/hello.txt"));
		Assert.AreEqual (f2, fs.getFile ("/test/hello.txt"));

		// How about with a leading forward slash?
		File f3 = fs.createFile ("/test/hello2.txt");
		Assert.AreEqual (f3, fs.getFile ("test/hello2.txt"));
		Assert.AreEqual (f3, fs.getFile ("/test/hello2.txt"));

		// How about nested deeper?
		File f4 = fs.createFile("test/meow/hello.txt");
		Assert.AreEqual (f4, fs.getFile ("test/meow/hello.txt"));
		Assert.AreEqual (f4, fs.getFile ("/test/meow/hello.txt"));

		File f5 = fs.createFile("/test/meow/hello2.txt");
		Assert.AreEqual (f5, fs.getFile ("test/meow/hello2.txt"));
		Assert.AreEqual (f5, fs.getFile ("/test/meow/hello2.txt"));

		// We should fail if there's ever a normal file along the way.
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile("/test/hello.txt/wrong.txt"); });

		// We should fail on bad files.
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile("hello"); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile("meow.#("); });
		Assert.Throws(typeof(InvalidFileException), delegate { fs.createFile("george's computer.txt"); });

		// We should be able to handle weird directories.
		File f6 = fs.createFile("/..////.././//./////./.././//./../test/../test//////./meow/./../meow/world.txt");
		Assert.AreEqual (f6, fs.getFile ("test/meow/world.txt"));
		Assert.AreEqual (f6, fs.getFile ("/test/meow/world.txt"));
	}

	[Test]
	public void CanSetContents() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		EditableFile f = fs.createFile ("test.txt");
		Assert.AreEqual (f.getContents (), "");
		f.setContents ("hello world");
		Assert.AreEqual (f.getContents (), "hello world");
	}

	[Test]
	public void CanAddFilesToDirectory() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory f = fs.createDirectory ("test");
		Assert.IsNotNull (f.getFiles ());
		Assert.AreEqual (f.getFiles (), new List<File> ());

		File fi = fs.createFile("test.txt");
		f.addFile (fi);
		Assert.AreEqual (f.getFiles (), new List<File> () { fi });
		Assert.AreEqual (f.getNumFiles (), 1);
	}

	[Test]
	public void CanHandleParentsBasic() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory dir = fs.createDirectory ("test");
		Assert.IsNotNull (dir);
		Assert.AreEqual (dir.getNumFiles (), 0);

		//File file = fs.createFile("hello.txt", dir);
		File file = fs.createFile("/test/hello.txt");
		Assert.IsNotNull (file);

		// Are the parents showing up properly?
		Assert.AreEqual (dir.getParent (), fs.root);
		Assert.AreEqual (file.getParent (), dir);
	}

	[Test]
	public void CanHandlePathsBasic() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory dir = fs.createDirectory ("test");
		File file = fs.createFile("/test/hello.txt");

		// Are the paths correct?
		Assert.AreEqual (dir.getPath (), "/test");
		Assert.AreEqual (file.getPath (), "/test/hello.txt");}

	[Test]
	public void CanHandleDirectoriesBasic() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory dir = fs.createDirectory ("test");
		File file = fs.createFile("test/hello.txt");

		// Does root now contain the directory?
		Assert.AreEqual(1, fs.root.getNumFiles());
		Assert.AreEqual (dir, fs.root.getFiles () [0]);

		// Does the directory now contain the file?
		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir.getFiles (), new List<File>() { file });

		// Can we make a directory a child of a normal file?
		// Compile-time error now!
		/*
		Assert.Throws (typeof(InvalidFileException), delegate {
			File file2 = fs.createFile ("hello2.txt", file);
			file2.getPath();
		});
		*/
	}

	[Test]
	public void CanHandleDirectoriesAdvanced() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		// Can we create multiple files with the same path?
		Assert.Throws (typeof(InvalidFileException), delegate {
			File f1 = fs.createFile("test1.txt");
			File f2 = fs.createFile("test1.txt");
			f1.getPath();
			f2.getPath();
		});

		// Can we create directories that don't exist?
		Assert.Throws (typeof(InvalidFileException), delegate {
			File f = fs.createFile ("test/testing.txt");
			f.getPath();
		});
	}

	[Test]
	public void DoesMoveFileWorkBasic() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		EditableFile f = fs.createFile ("test.txt");
		Directory dir = fs.createDirectory ("testing");
		Assert.AreEqual (f.getParent (), fs.root);
		Assert.AreEqual (f.getPath (), "/test.txt");
		Assert.AreEqual (dir.getPath (), "/testing");

		fs.moveFile (f, dir);
		Assert.AreEqual (f.getParent (), dir);
		Assert.AreEqual (f.getPath (), dir.getPath () + "/" + f.getFullName ());
		Assert.AreEqual ("/testing/test.txt", dir.getPath () + "/" + f.getFullName ());
	}

	[Test]
	public void DoesMoveFileWorkAdvanced() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory dir = fs.createDirectory ("testing");
		Directory dir2 = fs.createDirectory ("testing2");
		EditableFile f = fs.createFile ("test.txt");
		Assert.AreEqual (f.getParent (), fs.root);
		Assert.AreEqual (f.getPath (), "/test.txt");
		Assert.AreEqual (dir.getPath (), "/testing");
		Assert.AreEqual (dir2.getPath (), "/testing2");

		fs.moveFile (dir2, dir);
		Assert.AreEqual (dir2.getParent (), dir);
		Assert.AreEqual (dir2.getPath (), dir.getPath () + "/" + dir2.getFullName ());

		fs.moveFile (f, dir2);
		Assert.AreEqual (f.getParent (), dir2);
		Assert.AreEqual (f.getParent ().getParent (), dir);
		Assert.AreEqual (f.getPath (), dir2.getPath () + "/" + f.getFullName ());
		Assert.AreEqual (f.getPath (), dir.getPath () + "/" + dir2.getFullName() + "/" + f.getFullName ());

		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir2.getNumFiles (), 1);

		Directory dir3 = fs.createDirectory (dir2.getPath() + "/" + "testing3");
		Assert.AreEqual (dir3.getPath (), dir2.getPath () + "/" + dir3.getFullName ());
		Assert.AreEqual (dir2.getNumFiles (), 2);

		fs.moveFile (dir3, dir2);
		Assert.AreEqual (dir3.getPath (), dir2.getPath () + "/" + dir3.getFullName ());
		Assert.AreEqual (dir2.getNumFiles (), 2);

		fs.moveFile (f, dir3);
		Assert.AreEqual (f.getParent (), dir3);
		Assert.AreEqual (f.getPath (), dir3.getPath () + "/" + f.getFullName ());
		Assert.AreEqual ("/testing/testing2/testing3/test.txt", f.getPath ());
		Assert.AreEqual (dir.getNumFiles (), 1);
		Assert.AreEqual (dir2.getNumFiles (), 1);
		Assert.AreEqual (dir3.getNumFiles (), 1);

		// Can't move files to/from null locations
		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.moveFile (null, dir3);
		});
		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.moveFile (dir3, null);
		});
		Assert.AreEqual (dir3.getParent (), dir2);

		// Can't move directory inside itself
		Assert.Throws (typeof(InvalidFileException), delegate { 
			fs.moveFile (dir3, dir3);
		});

		// Can't move directories inside children of themselves
		Assert.Throws (typeof(InvalidFileException), delegate { 
			fs.moveFile (dir2, dir3);
		});
	}

	[Test]
	public void DoesGetFileWorkBasic() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Directory dir = fs.createDirectory ("testing");
		File f = fs.createFile ("testing/test.txt");

		// Basic sanity checks
		Assert.AreEqual (1, fs.root.getNumFiles ());
		Assert.AreEqual (1, dir.getNumFiles ());
		Assert.AreEqual (dir, fs.root.getFiles () [0]);
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

		Assert.AreEqual (1, fs.root.getNumFiles ());
		Assert.IsNull (fs.root.getFile (f));

		// FileSystem get tests
		Assert.AreEqual(fs.getFile("testing/test.txt"), f);
		Assert.AreEqual(fs.getFile("/testing/test.txt"), f);
		Assert.IsNull(fs.getFile("testing/meow.txt"));
		Assert.IsNull(fs.getFile("/testing/meow.txt"));
		Assert.IsNull (fs.getFile ("test.txt"));
		Assert.IsNull (fs.getFile ("/test.txt"));

		Assert.AreEqual (fs.getFile ("/"), fs.root);
	}

	[Test]
	public void DoesGetFileWorkAdvanced() {
		FileSystem fs = new FileSystem ();
		Assert.IsNotNull (fs);

		Assert.AreEqual (fs.root.getNumFiles (), 0);

		Directory dir = fs.createDirectory ("meow");
		Assert.AreEqual (fs.root.getNumFiles (), 1);

		Assert.AreEqual (fs.root.getNumFiles (), 1);
		Assert.AreEqual(fs.getFile("meow"), dir);
		Assert.AreEqual(fs.getFile("/meow"), dir);
		Assert.AreEqual(fs.getFile("./meow"), dir);
		Assert.AreEqual(fs.getFile("../meow"), dir);
		Assert.AreEqual(fs.getFile("./././././././meow"), dir);
		Assert.AreEqual(fs.getFile("../../../../../../../meow"), dir);
		Assert.AreEqual(fs.getFile("./../././../././meow"), dir);
		Assert.IsNull(fs.getFile(".../meow"));

		Directory dir2 = fs.createDirectory (dir.getPath() + "/" + "test");
		Assert.AreEqual (fs.root.getNumFiles (), 1);
		Assert.IsNull(fs.getFile("test"));
		Assert.AreEqual(fs.getFile("meow/test"), dir2);
		Assert.AreEqual(fs.getFile("/meow/test"), dir2);
		Assert.AreEqual(fs.getFile("./meow/test"), dir2);
		Assert.AreEqual(fs.getFile("../meow/test"), dir2);
		Assert.IsNull(fs.getFile("meow/../test"));
		Assert.IsNull(fs.getFile("/meow/../test"));
		Assert.IsNull(fs.getFile("./meow/../test"));
		Assert.IsNull(fs.getFile("../meow/../test"));
		Assert.AreEqual(fs.getFile("meow/../meow/"), dir);
		Assert.AreEqual(fs.getFile("meow/../meow/test"), dir2);
		Assert.AreEqual (fs.getFile ("///////////////meow"), dir);
		Assert.AreEqual (fs.getFile ("///meow////////////"), dir);
		Assert.AreEqual (fs.getFile ("//////../meow/./test/.././test/../..////"), fs.root);
	}

}