﻿using System.Collections;
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
			if (!currentFile.isDirectory) {
				throw new ExecutionException("Invalid state: Current path is not a directory.");
			}

			List<File> files = currentFile.getFiles ();
			if (files.Count == 0) {
				return "No files found.";
			} else {
				string toReturn = "";
				if (files.Count > 0) {
					toReturn = files [0].getFullName ();
					if (files [0].isDirectory) {
						toReturn += "/";
					}
				}

				for (int i = 1; i < files.Count; i++) {
					toReturn += "\n" + files [i].getFullName ();

					// Possibly display in a different color
					if (files [i].isDirectory) {
						toReturn += "/";	
					}
				}
				return toReturn;
			}
		}
	}
}