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
				Console.log.println (currentPath + "/");
			} else {
				Console.log.println (currentPath);
			}
			return "";
		}
	}
}
