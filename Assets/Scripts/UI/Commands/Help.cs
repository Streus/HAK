using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Help : Command
	{
		public override string getHelp ()
		{
			return "Displays the help text for all available commands, or a single " +
				"command. Usage: help [command]";
		}

		public override string getInvocation ()
		{
			return "help";
		}

		public override string execute (params string[] args)
		{
			Command[] commands = Console.log.getCommandList ();
			string retString = "[HELP]\n";
			if (args.Length == 1)
			{
				foreach (Command c in commands)
					retString += c.getInvocation () + " | " + c.getHelp () + "\n";
			}
			else
			{
				foreach (Command c in commands)
				{
					if (c.getInvocation () == args [1]) 
					{
						return c.getInvocation () + " | " + c.getHelp () + "\n";
					}
				}
				throw new ExecutionException ("Unknown command: " + args [1]);
			}
			return retString;
		}
	}
}
