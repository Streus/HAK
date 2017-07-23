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
			string filename = args [1];
			File currentFile = FileSystem.getFile (GameManager.currentPath);
			string currentPath = GameManager.currentPath;

			if (currentFile.isDirectory) {
				if (currentFile.containsFile (currentPath + "/" + filename)) {
					return "";
				} else {
					try {
						FileSystem.createFile(filename);
						Console.log.println("Created new file \"" + filename + "\".");
					} catch (InvalidFileException e) {
						Console.log.println ("Could not create file. Reason: " + e.Message, Console.LogTag.error);
					}
					return "";
				}
			}
			return "[Error] Current file is not a directory. Somehow?";
		}
	}
}
