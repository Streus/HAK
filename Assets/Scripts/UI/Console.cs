using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.IO;
using System;

public class Console : MonoBehaviour
{
	/* Static Vars */
	public static Console log;

	private static Assembly assembly = Assembly.GetExecutingAssembly();

	/* Instance Vars */

	// The input line for the Console
	[SerializeField]
	private InputField input;

	// The output display for the Console
	[SerializeField]
	private Transform output;
	private List<string> outHistory;

	// The base RectTransform of the Console
	[SerializeField]
	private RectTransform root;

	// The Menu of which this console is a part
	[SerializeField]
	private Menu menu;

	// The maximum number of lines a message can contain
	public int messageLengthMax;

	// The maximum number of messages that can be active at a time
	public int activeMessagesMax;

	// Output stream stuff
	public delegate void OutStream(string msg);
	private OutStream stdOut;
	private OutStream stdErr;

	// A list of all available commands
	private Dictionary<string, Command> commands;

	public Command getCommand(string invocaton)
	{
		Command c = null;
		commands.TryGetValue (invocaton, out c);
		return c;
	}

	public Command[] getCommandList()
	{
		List<Command> cs = new List<Command> ();
		foreach (Command c in commands.Values)
			cs.Add (c);
		return cs.ToArray ();
	}

	public string[] getHistory()
	{
		return inHistory.ToArray ();
	}

	// Every individual line set in through this console's input
	private List<string> inHistory;
	private int historyIndex;

	private bool _enabled;
	public bool getEnabled() { return _enabled; }
	public void setEnabled(bool enabled)
	{
		_enabled = enabled;
		if (!_enabled)
		{
			input.text = "";
			input.DeactivateInputField ();
		}
		else
			input.ActivateInputField ();
	}

	public bool isFocused
	{
		get { return input.isFocused; }
	}

	/* Instance Methods */
	public void Awake()
	{
		if (log == null) 
		{
			log = this;
			DontDestroyOnLoad (transform.root.gameObject);

			commands = new Dictionary<string, Command> ();
			buildCommandList ();

			inHistory = new List<string> ();
			outHistory = new List<string> ();
			historyIndex = -1;

			stdOut = sendToOut;
			stdErr = sendToErr;

			new CSEREnvironment (); //DEBUG
		}
		else
			Destroy (transform.root.gameObject);
	}
	private void buildCommandList()
	{
		string baseDir = System.IO.Directory.GetCurrentDirectory ();
		string[] files = System.IO.Directory.GetFiles (baseDir + "/Assets/Scripts/UI/Commands");
		foreach (string file in files)
		{
			if (file.EndsWith (".cs"))
			{
				int start = 1 + Mathf.Max (file.LastIndexOf ('/'), file.LastIndexOf ('\\'));
				int end = file.LastIndexOf ('.');

				string rawClass = file.Substring (start, end - start);

				Command c = (Command)assembly.CreateInstance ("Commands." + rawClass);
				if (c == null)
					Debug.LogError ("Non-Command file in Commands folder."); //DEBUG
				commands.Add(c.getInvocation(), c);
			}
		}
	}

	public void Start()
	{
		_enabled = menu.IsOpen;
		menu.changedFocus += setEnabled;

		input.onEndEdit.AddListener (delegate{inputEntered();});
	}

	public void Update()
	{
		if (!_enabled)
			return;

		if (!isFocused && (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.UpArrow)))
		{
			input.ActivateInputField ();
			input.Select ();
			input.text = "";
			historyIndex = -1;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow))
		{
			historyIndex = (historyIndex + 1) % inHistory.Count;
			input.text = inHistory [historyIndex];
		}

		if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			historyIndex--;
			if (historyIndex < 0)
				historyIndex = inHistory.Count - 1;
			input.text = inHistory [historyIndex];
		}
	}

	// --- Output Stream Management ---

	// Set the destination of the stdOut stream
	public void setStdOut(OutStream stream)
	{
		stdOut = stream;
	}
	public void resetStdOut()
	{
		stdOut = sendToOut;
	}

	// Set the destination of the stdErr stream
	public void setStdErr(OutStream stream)
	{
		stdErr = stream;
	}
	public void resetStdErr()
	{
		stdErr = sendToErr;
	}

	// Invoked when the user presses enter and the console is active
	private void inputEntered()
	{
		if (!_enabled || input.text == "")
			return;
		
		//use the text from input to execute a command
		execute(input.text);
	}

	// Execute a file of prepared commands, treating each line as an indiv. command
	public bool executeFile(string fileName)
	{
		string[] instructions;

		try
		{
			instructions = System.IO.File.ReadAllLines (fileName);
		}
		#pragma warning disable 0168
		catch(IOException ioe)
		#pragma warning restore 0168
		{
			println ("Failed to load " + fileName + ".", true);
			return false;
		}

		new CSEREnvironment ().execute (instructions);

		return true;
	}

	// Attempt to execute a command. Fills output with the result of a successful command
	public bool execute(string command)
	{
		inHistory.Insert (0, input.text);

		bool success = true;
		string result = "";

		//parse input
		string[] args = CSEREnvironment.parseLine(command);
		args [0] = args [0].ToLower ();

		Command c = null;
		if (commands.TryGetValue (args [0], out c))
		{
			try
			{
				result = c.execute (args);
			}
			catch (Command.ExecutionException ee)
			{
				result = ee.Message;
				success = false;
			}
			#pragma warning disable 0168
			catch (System.IndexOutOfRangeException ioore)
			#pragma warning restore 0168
			{
				result = "Provided too few arguments.\n" + c.getHelp ();
				success = false;
			}
		}
		else
		{
			result = "Command \"" + args[0] + "\" not found.  Try \"help\" for a list of commands";
			success = false;
		}

		//send the output text to the appropriate available stream
		if (success && stdOut != null)
		{
			stdOut (result);
		}
		else if(!success && stdErr != null)
		{
			stdErr (result);
		}

		return success;
	}

	// This is for backwards-compatability and also ease-of use.
	private string outputIntercept;
	private void setOutputIntercept(string s) { outputIntercept = s; }
	public bool execute(string command, out string result)
	{
		setStdOut (setOutputIntercept);
		setStdErr (setOutputIntercept);
		bool success = execute (command);
		result = outputIntercept;
		resetStdOut ();
		resetStdErr ();
		return success;
	}

	// Print a message to the console
	public void println(string message, bool error = false)
	{
		if (error)
			sendToErr (message);
		else
			sendToOut (message);
	}
	public void print(string message, bool error = false)
	{
		//TODO attempt to append to an existing message
	}

	// Send the passed text to this console's output pane
	private void sendToOut(string msg)
	{
		GameObject messagePfb = Resources.Load<GameObject> ("UI/ConsoleMessage");

		while (true)
		{
			GameObject message = Instantiate (messagePfb, output);
			Text box = message.GetComponent<Text> ();
			//TODO message splitting is hard
			break;
		}
	}

	//Send the passed text to this console's error pane
	private void sendToErr(string msg)
	{
		sendToOut (markWithColor (msg, Color.red));
	}
	private string markWithColor(string source, Color c)
	{
		string colorValue = "<color=#";
		Vector4 colVals = (Vector4)c;
		colorValue += ((int)(255f * colVals.x)).ToString ("x2");
		colorValue += ((int)(255f * colVals.y)).ToString ("x2");
		colorValue += ((int)(255f * colVals.z)).ToString ("x2");
		colorValue += ((int)(255f * colVals.w)).ToString ("x2");
		return colorValue + "><b>" + source + "</b></color>";
	}

	// Clear the console output window
	public void clear()
	{
		
	}

	// Clear the history buffer
	public void clearHistory()
	{
		inHistory.Clear ();
	}
}
