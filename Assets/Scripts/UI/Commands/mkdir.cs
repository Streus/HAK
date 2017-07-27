using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class mkdir : Command
	{
		public override string getHelp ()
		{
			return "Creates a new directory. Usage: mkdir <dirname>.";
		}

		public override string getInvocation ()
		{
			return "mkdir";
		}

		public override string execute (params string[] args)
		{
			File currentFile = GameManager.currentFileSystem.getFile (GameManager.currentPath);
			string filename = args [1];

			if (!currentFile.isDirectory) {
				throw new ExecutionException("Invalid state: Current path is not a directory.");
			}

			if (filename == null || (filename.GetType() != typeof(string))) {
				throw new ExecutionException ("Invalid argument: filename must be a string.");
			}

			try {
				GameManager.currentFileSystem.createDirectory(filename, currentFile);
				return "Created new directory \"" + filename + "\".";
			} catch (InvalidFileException e) {
				throw new ExecutionException("Could not create directory. Reason: " + e.Message);
			}
		}
	}
}
