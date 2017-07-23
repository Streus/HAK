using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Commands
{
	public class history : Command
	{
		public override string getHelp ()
		{
			return "Get <count> amount of lines of history. Usage: history <count=10>.";
		}

		public override string getInvocation ()
		{
			return "history";
		}

		public override string execute (params string[] args)
		{
			int count = 10;
			if (args.Length > 1) {
				try {
					count = Int32.Parse(args [1]);
				} catch (Exception e) {
					e.ToString ();
					throw new ExecutionException ("Couldn't parse integer: " + args [1]);
				}
			}

			string[] history = Console.log.getHistory ();
			if (count > 0) {
				if (history.Length == 0) {
					return "\nNo history found.";
				}

				string toReturn = "";
				for (int i = Mathf.Min(count, history.Length) - 1; i >= 0; i--) {
					toReturn += "\n[" + (history.Length - i) + "] " + history [i];
				}
				return toReturn;
			} else {
				throw new ExecutionException ("Count needs to be a non-negative integer.");
			}
		}
	}
}
