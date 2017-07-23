using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class cd : Command
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
			int count = args [1];
			string[] history = Console.getHistory ();

			if (count > 0) {
				string toReturn = "";
				for (int i = 0; i < Mathf.Min(count, history.Length); i++) {
					toReturn += history [i];
				}
				return toReturn;
			} else {
				throw new ExecutionException ("Count needs to be a non-negative integer.");
			}
		}
	}
}
