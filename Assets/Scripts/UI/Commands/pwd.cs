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
			NetworkNode node = GameManager.currentHost;
			if (!(node is IFileSystem)) {
				throw new ExecutionException ("The current node does not support a file system.");
			}
			FileSystem currentFileSystem = (node as IFileSystem).fileSystem;

			string currentPath = GameManager.currentPath;
			if (currentFileSystem.getFile (currentPath) != null && 
				currentFileSystem.getFile (currentPath) is Directory) {
				return currentPath + "/";
			} else {
				throw new ExecutionException ("Detected invalid state: current directory is not a directory");
				//Console.log.println (currentPath);
			}
		}
	}
}
