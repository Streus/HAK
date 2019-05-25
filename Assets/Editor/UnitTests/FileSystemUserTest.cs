using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using FileSystemNS;

[TestFixture]
public class FileSystemUserTest 
{

	[Test]
	public void CanAddUsers() {
		FileSystem fs = new FileSystem ();
		Assert.AreEqual (1, fs.allUsers.Count);

<<<<<<< HEAD
		fs.addUser ("test", "pass", SecurityLevel.ADMIN);
=======
        fs.addUser ("test", "pass", SecurityLevel.Admin);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		Assert.AreEqual (2, fs.allUsers.Count);

		fs.changeUser ("test", "pass");

		// Should not be able to add two users with the same name
		Assert.Throws (typeof(InvalidUserException), delegate {
<<<<<<< HEAD
			fs.addUser("test", "pass2", SecurityLevel.ADMIN);
		});
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.addUser("test", "pass2", SecurityLevel.NONADMIN);
=======
			fs.addUser("test", "pass2", SecurityLevel.Admin);
		});
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.addUser("test", "pass2", SecurityLevel.Nonadmin);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		});

		// Should not be able to add a second root user
		Assert.Throws (typeof(InvalidUserException), delegate {
<<<<<<< HEAD
			fs.addUser("superroot", "pass", SecurityLevel.ROOT);
=======
			fs.addUser("superroot", "pass", SecurityLevel.Root);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		});
	}

	[Test]
	public void CanChangeUsers() {
		FileSystem fs = new FileSystem ();
		Assert.AreEqual (1, fs.allUsers.Count);

<<<<<<< HEAD
		fs.addUser ("test", "pass", SecurityLevel.ADMIN);
=======
		fs.addUser ("test", "pass", SecurityLevel.Admin);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		Assert.AreEqual (2, fs.allUsers.Count);

		// Should throw exception if failed to sign in
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.changeUser ("test", "badpassword");
		});

		// Should throw exception if user doesn't exist
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.changeUser ("hello", "badpassword");
		});

		// Should do nothing if we're already that user
		fs.changeUser("root", "");

		// Should result in changed user
		fs.changeUser ("test", "pass");
		Assert.AreEqual ("test", fs.currentUser.username);
<<<<<<< HEAD
		Assert.AreEqual (SecurityLevel.ADMIN, fs.currentUser.adminLevel);
=======
		Assert.AreEqual (SecurityLevel.Admin, fs.currentUser.adminLevel);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
	}

	[Test]
	public void CanRootUsersModifyRightPermissions() {
		FileSystem fs = new FileSystem ();

		// We are root user, so we can change all permissions.
		// Try making something we can't read or write to.
		EditableFile fi = fs.createFile ("hello.txt");
		fs.setPermissions(fi, 000);
		Assert.AreEqual (000, fi.getPermissions ());

		// Can't read
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.getContents();
		});

		// Can't write
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.setContents("hello world!");	
		});

		// Can set any permissions
		fs.setPermissions (fi, 700);
		Assert.AreEqual (700, fi.getPermissions ());
		fs.setPermissions (fi, 770);
		Assert.AreEqual (770, fi.getPermissions ());
		fs.setPermissions (fi, 777);
		Assert.AreEqual (777, fi.getPermissions ());
		fs.setPermissions (fi, 700);
		Assert.AreEqual (700, fi.getPermissions ());

		// Can read + write now
		Assert.AreEqual("", fi.getContents());
		fi.setContents ("hello!");
		Assert.AreEqual("hello!", fi.getContents());		
	}

	[Test]
	public void CanAdminUsersModifyRightPermissions() {
		FileSystem fs = new FileSystem ();
		EditableFile fi = fs.createFile ("hello.txt");
		fs.setPermissions (fi, 700);

		// Make an admin user
<<<<<<< HEAD
        fs.addUser ("test", "pass", SecurityLevel.ADMIN);
=======
		fs.addUser ("test", "pass", SecurityLevel.Admin);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		Assert.AreEqual (2, fs.allUsers.Count);
		fs.changeUser ("test", "pass");

		// Can't read
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.getContents();
		});

		// Can't write
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.setContents("hello world!");	
		});

		// Can't set root permissions
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.setPermissions(fi, 000);	
		});

		// Can set admin and non-admin permissions
		fs.setPermissions(fi, 770);
		Assert.AreEqual (770, fi.getPermissions ());
		fs.setPermissions(fi, 777);
		Assert.AreEqual (777, fi.getPermissions ());

		// Can read + write now
		Assert.AreEqual("", fi.getContents());
		fi.setContents ("hello!");
		Assert.AreEqual("hello!", fi.getContents());
	}

	public void CanNonadminUsersModifyRightPermissions() {
		FileSystem fs = new FileSystem ();
		EditableFile fi = fs.createFile ("hello.txt");
		fs.setPermissions (fi, 000);

		// Make a nonadmin user
<<<<<<< HEAD
		fs.addUser("meow", "pass", SecurityLevel.NONADMIN);
=======
		fs.addUser("meow", "pass", SecurityLevel.Nonadmin);
>>>>>>> f8ade7209b69b532a52f808012fae73b6c4782d1
		Assert.AreEqual (2, fs.allUsers.Count);
		fs.changeUser ("meow", "pass");

		// Can't read
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.getContents();
		});

		// Can't write
		Assert.Throws (typeof(InvalidUserException), delegate {
			fi.setContents("hello world!");	
		});

		// Can't set root permissions
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.setPermissions(fi, 700);	
		});

		// Can't set admin permissions
		Assert.Throws (typeof(InvalidUserException), delegate {
			fs.setPermissions(fi, 070);	
		});

		// Can set nonadmin permissions
		fs.setPermissions(fi, 007);

		fi.setContents ("meowmeow");
		Assert.AreEqual("meowmeow", fi.getContents ());
	}

	[Test]
	public void CanHandleBadPermissions() {
		FileSystem fs = new FileSystem ();
		File fi = fs.createFile ("hello.txt");

		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.setPermissions(fi, 888);	
		});

		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.setPermissions(fi, -1);	
		});

		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.setPermissions(fi, -111);	
		});

		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.setPermissions(fi, 1000);	
		});		

		Assert.Throws (typeof(InvalidFileException), delegate {
			fs.setPermissions(fi, 999999999);	
		});
	}
}
