using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Open : Command
	{
		public override string getHelp ()
		{
			return "Opens a file in the text editor. Usage: open <filename>";
		}

		public override string getInvocation ()
		{
			return "open";
		}

		public override string execute (params string[] args)
		{
			Menu editor = MenuManager.menusys.getMenu ("Text Editor");
			MenuManager.menusys.showMenu (editor);
			return "Opened " + args[1] + " in the Text Editor.";
		}
	}
}
