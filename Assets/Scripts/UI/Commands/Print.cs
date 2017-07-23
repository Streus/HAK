using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Print : Command
	{
		public override string getHelp ()
		{
			return "Print to the console. Usage: print <\"message\">";
		}

		public override string getInvocation ()
		{
			return "print";
		}

		public override string execute (params string[] args)
		{
			Console.log.println (args [1], Console.LogTag.info);
			return "";
		}
	}
}
