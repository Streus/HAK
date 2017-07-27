using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class representing a user on a given NetworkNode.
 */
public class User {
	public string username;
	private string password;

	/**
	 * I know this isn't cryptographically secure, but it doesn't need to be.
	 * This is a game, and this provides the basic security we need in it.
	 */
	public bool verifyPassword(string pass) {
		return pass.Equals (password);
	}
}
