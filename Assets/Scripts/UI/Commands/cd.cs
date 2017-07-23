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
			string currentPath = GameManager.currentPath;
			string filename = args [1];
			File newFile = FileSystem.getFile (currentPath + "/" + filename);

			if (newFile == null) {
				throw new ExecutionException ("Directory \"" + filename + "\" does not exist.");
			}

			if (!newFile.isDirectory) {
				throw new ExecutionException ("Can't change directory into a non-directory file.");
			}

			// This forces a simplification of paths, so things like "/test/../test/.." get turned into "/".
			GameManager.currentPath = newFile.getPath ();
			return "Current directory is now \"" + GameManager.currentPath + "/\".";
		}
	}
}
