using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands {
	public class ssh : Command {
		public override string getHelp () {
			return "Switch to a different node. Usage: ssh <hostname|ip>.";
		}

		public override string getInvocation () {
			return "ssh";
		}

		public override string execute (params string[] args) {
			string ipOrHostname = args [1];
			if (GameManager.currentHost.ping (ipOrHostname) < 0) {
				throw new ExecutionException ("Could not find host \"" + ipOrHostname + "\".");
			} else if (GameManager.currentHost.ping (ipOrHostname) == 0) {
				throw new ExecutionException ("Why would you want to ssh into your own computer?");
			} else {
				NetworkNode newNode = GameManager.currentHost.search (ipOrHostname);
				if (newNode == null) {
					throw new ExecutionException ("Search failed, when ping did not. Internal logical error.");
				}
				GameManager.currentHost = newNode;
				GameManager.currentPath = "";
				return "Current network node is now " + GameManager.currentHost.hostname + ".";
			}
		}
	}
}
