using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class touch : Command
	{
		public override string getHelp ()
		{
			return "Updates access time of a file, or creates it if it doesn't exist. Usage: touch <filename>.";
		}

		public override string getInvocation ()
		{
			return "touch";
		}

		public override string execute (params string[] args)
		{
			//string currentPath = GameManager.currentPath;
			Directory currentFile = GameManager.currentFileSystem.getFile (GameManager.currentPath) as Directory;
			string filename = args [1];

			if (!(currentFile is Directory)) {
				throw new ExecutionException("Invalid state: Current path is not a directory.");
			}

			if (filename == null || (filename.GetType() != typeof(string))) {
				throw new ExecutionException ("Invalid argument: filename must be a string.");
			}
				
			try {
				GameManager.currentFileSystem.createFile(filename, currentFile);
				return "Created new file \"" + filename + "\".";
			} catch (InvalidFileException e) {
				throw new ExecutionException("Could not create file. Reason: " + e.Message);
			}
		}
	}
}
