using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class ls : Command
	{
		public override string getHelp ()
		{
			return "Lists files in current directory. Usage: ls.";
		}

		public override string getInvocation ()
		{
			return "ls";
		}

		public override string execute (params string[] args)
		{
			File currentFile = FileSystem.getFile (GameManager.currentPath);
			if (currentFile.isDirectory) {
				List<File> files = currentFile.getFiles ();
				string toReturn = "";
				if (files.Count == 0) {
					Console.log.println ("No files found.");
				} else {
					for (int i = 0; i < files.Count; i++) {
						// Possibly display in a different color
						if (files [i].isDirectory) {
							toReturn += files [i].getFullName () + "/\n";
						} else {
							toReturn += files [i].getFullName () + "/\n";
						}
					}
					Console.log.println (toReturn);
				}
				return "";
			}
			return "[Error] Current file is not a directory. Somehow?";
		}
	}
}
