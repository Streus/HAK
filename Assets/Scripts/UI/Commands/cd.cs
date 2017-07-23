using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class cd : Command
	{
		public override string getHelp ()
		{
			return "Change directory. Usage: cd <directory>.";
		}

		public override string getInvocation ()
		{
			return "cd";
		}

		public override string execute (params string[] args)
		{
			string filename = args [1];
			//File currentFile = FileSystem.getFile (GameManager.currentPath);
			string currentPath = GameManager.currentPath;

			File newFile = FileSystem.getFile (currentPath + "/" + filename);
			Console.log.println ("Trying to change to directory " + currentPath + filename + ". Status=" + newFile);
			if (newFile != null) {
				// This forces a simplification of paths, so things like "/test/../test/.." get turned into "/".
				GameManager.currentPath = FileSystem.getFile(currentPath + "/" + filename).getPath();
			}

			return "";
		}
	}
}
