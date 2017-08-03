using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands {
	public class ping : Command {
		public override string getHelp () {
			return "Pings a given hostname or ip. Usage: ping <hostname|ip>.";
		}

		public override string getInvocation () {
			return "ping";
		}

		public override string execute (params string[] args) {
			string ipOrHostname = args [1];
			try {
				long hops = GameManager.currentHost.ping (ipOrHostname);
				return "Found \"" + ipOrHostname + "\" in " + hops + " hop" + (hops == 1 ? "." : "s.");
			} catch (InvalidOperationException ioe) {
				throw new ExecutionException (ioe.Message);
			}
		}
	}
}
