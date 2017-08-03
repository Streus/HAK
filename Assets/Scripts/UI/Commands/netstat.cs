using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands {
	public class netstat : Command {
		public override string getHelp () {
			return "Displays current active connections. Usage: netstat.";
		}

		public override string getInvocation () {
			return "netstat";
		}

		public override string execute (params string[] args) {
			if (GameManager.currentHost == null) {
				throw new ExecutionException("Current host is null.");
			}
			List<NetworkNode> activeConnections = new List<NetworkNode> ();
			foreach (NetworkNode n in GameManager.currentHost.connections) if (n.active) activeConnections.Add (n);

			string toReturn = "Found " + activeConnections.Count + " connection" + (activeConnections.Count == 1 ? ":" : "s:");
			foreach (NetworkNode n in activeConnections) {
				toReturn += "\n";
				toReturn += "host \"" + n.hostname + "\" at ip \"" + n.ip + "\"";
			}
			return toReturn;
		}
	}
}
