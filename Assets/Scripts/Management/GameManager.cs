using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static string currentPath = "";

	// TODO: When we have a network system, chance this to be the current network host.
	// That should hold on to a FileSystem reference on its own.
	//public static FileSystem currentFileSystem = new FileSystem();

	public static Network currentLevel = null;

	public static NetworkNode currentHost = null;

	// Use this for initialization
	void Start () {
		//TODO: Make this into a Network object, and serialize it into a level.
		RouterNode route1 = new RouterNode ("test1", "1");
		RouterNode route2 = new RouterNode ("test2", "2");

		DesktopNode desk1 = new DesktopNode ("George's computer", "1.1");
		DesktopNode desk2 = new DesktopNode ("George's server", "1.2");
		desk2.active = false;

		DesktopNode desk3 = new DesktopNode ("Remote server", "2.1");

		route1.addConnection(route2);
		desk1.addConnection (route1);
		desk2.addConnection (route1);
		desk3.addConnection (route2);

		currentHost = desk1;

		//currentLevel = Network.getLevel1 ();
		//currentHost = currentLevel.getStart ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
