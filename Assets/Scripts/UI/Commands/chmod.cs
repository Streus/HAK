using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FileSystemNS;

namespace Commands
{
	public class chmod : Command
	{
		public override string getHelp ()
		{
			return "Change permissions. Usage: chmod [permissions] <file>.";
		}

		public override string getInvocation ()
		{
			return "chmod";
		}

		public override string execute (params string[] args)
		{
			NetworkNode node = GameManager.currentHost;
			if (!(node is IFileSystem)) {
				throw new ExecutionException ("The current node does not support a file system.");
			}
			FileSystem currentFileSystem = (node as IFileSystem).fileSystem;

			int permissions = 0;
			string filename = "";
			if (args.Length == 2) {
				filename = args [1];
			} else if (args.Length >= 3) {
				try {
					permissions = Int32.Parse (args [1]);
				} catch (Exception e) {
					e.ToString ();
					throw new ExecutionException ("Couldn't parse integer: " + args [1]);
				}
				filename = args [2];
			} else {
				throw new IndexOutOfRangeException ();
			}

			string currentPath = GameManager.currentPath;
			File file = currentFileSystem.getFile (currentPath + "/" + filename);

			if (file == null) {
				throw new ExecutionException ("File \"" + filename + "\" does not exist.");
			}

			if (args.Length == 2) {
				permissions = file.getPermissions ();
				int R = permissions / 100;
				int A = (permissions / 10) % 10;
				int N = permissions % 10;

				string prettyString = "";
				prettyString += (file is Directory) ? "d" : "-";
				prettyString += ((R & 4) == 4) ? "r" : "-";
				prettyString += ((R & 2) == 2) ? "w" : "-";
				prettyString += ((R & 1) == 1) ? "x" : "-";
				prettyString += ((A & 4) == 4) ? "r" : "-";
				prettyString += ((A & 2) == 2) ? "w" : "-";
				prettyString += ((A & 1) == 1) ? "x" : "-";
				prettyString += ((N & 4) == 4) ? "r" : "-";
				prettyString += ((N & 2) == 2) ? "w" : "-";
				prettyString += ((N & 1) == 1) ? "x" : "-";
				prettyString += " " + file.getFullName ();
				return prettyString;
			} else {			
				try {
					currentFileSystem.setPermissions (file, permissions);
					return "Successfully set permissions of \"" + file.getFullName() + "\" to " + args[1] + ".";
				} catch (InvalidFileException ife) {
					throw new ExecutionException(ife.Message);
				} catch (InvalidUserException iue) {
					throw new ExecutionException(iue.Message);
				}
			}


		}
	}
}
