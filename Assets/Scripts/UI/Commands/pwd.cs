using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class pwd : Command
	{
		public override string getHelp ()
		{
			return "Print current working directory.. Usage: pwd.";
		}

		public override string getInvocation ()
		{
			return "pwd";
		}

		public override string execute (params string[] args)
		{
			string currentPath = GameManager.currentPath;
			if (FileSystem.getFile (currentPath) != null && FileSystem.getFile (currentPath).isDirectory) {
				return currentPath + "/";
			} else {
				throw new ExecutionException ("Detected invalid state: current directory is not a directory");
				//Console.log.println (currentPath);
			}
		}
	}
}
