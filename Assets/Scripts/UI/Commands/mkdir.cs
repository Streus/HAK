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
			string filename = args [1];
			File currentFile = FileSystem.getFile (GameManager.currentPath);
			string currentPath = GameManager.currentPath;

			if (currentFile.isDirectory) {
				if (currentFile.containsFile (currentPath + "/" + filename)) {
					return "";
				} else {
					try {
						FileSystem.createDirectory(filename);
						Console.log.println("Created new directory \"" + filename + "\".");
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
