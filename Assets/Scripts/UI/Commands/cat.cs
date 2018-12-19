using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileSystemNS;


namespace Commands
{
	public class cat : Command
	{
		public override string getHelp ()
		{
			return "Print contents of a file. Usage: cat <file>.";
		}

		public override string getInvocation ()
		{
			return "cat";
		}

		public override string execute (params string[] args)
		{
			NetworkNode node = GameManager.currentHost;
			if (!(node is IFileSystem)) {
				throw new ExecutionException ("The current node does not support a file system.");
			}
			FileSystem currentFileSystem = (node as IFileSystem).fileSystem;

			string currentPath = GameManager.currentPath;
			string filename = args [1];
			File newFile = currentFileSystem.getFile (currentPath + "/" + filename);

			if (newFile == null) {
				throw new ExecutionException ("File \"" + filename + "\" does not exist.");
			}

			if (newFile is Directory) {
				throw new ExecutionException ("Can't cat file: is directory.");
			}

			if (!(newFile is EditableFile)) {
				throw new ExecutionException ("Can't cat file: contents are unreadable.");
			}

			EditableFile newFileE = newFile as EditableFile;
			try {
				return "Contents of file \"" + newFileE.getFullName() + "\":\n" + newFileE.getContents ();
			} catch (InvalidUserException iue) {
				throw new ExecutionException (iue.Message);
			}
		}
	}
}
