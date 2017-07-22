using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.IO;

public class Console : MonoBehaviour
{
	/* Static Vars */
	public static Console log;

	private static Assembly assembly = Assembly.GetExecutingAssembly();

	// Tags that are appended to print calls
	private static string[] tags = new string[]
	{
		"",
		"<color=#ff1111ff><b>[ERROR]</b></color>", //red
		"<color=#ffff11ff><b>[WARN]</b></color>", //yellow
		"<color=#11ffffff><b>[INFO]</b></color>", //cyan
		"<color=#11ff11ff><b>[OUT]</b></color>" //green
	};

	/* Instance Vars */

	// The input line for the Console
	[SerializeField]
	private InputField input;

	// The output display for the Console
	[SerializeField]
	private Text output;

	// The base RectTransform of the Console
	[SerializeField]
	private RectTransform root;

	// The Menu of which this console is a part
	[SerializeField]
	private Menu menu;

	// The maximum number of lines output will display (also history size)
	public int linesMax;

	// A list of all available commands
	private List<Command> commands;

	public Command[] getCommandList()
	{
		return commands.ToArray ();
	}

	// Every individual 
	private List<string> history;
	private int historyIndex;

	private bool _enabled;
	public bool isEnabled { get { return _enabled; } }
	public void setEnabled(bool enabled)
	{
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

			commands = new List<Command> ();
			buildCommandList ();

			history = new List<string> ();
			historyIndex = -1;
		}
		else
			Destroy (transform.root.gameObject);
	}
	private void buildCommandList()
	{
		string baseDir = Directory.GetCurrentDirectory ();
		string[] files = Directory.GetFiles (baseDir + "/Assets/Scripts/UI/Commands");
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
				commands.Add (c);
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
		if (!isEnabled)
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
			historyIndex = (historyIndex + 1) % history.Count;
			input.text = history [historyIndex];
		}

		if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			historyIndex--;
			if (historyIndex < 0)
				historyIndex = history.Count - 1;
			input.text = history [historyIndex];
		}
	}

	// Invoked when the user presses enter and the console is active
	private void inputEntered()
	{
		if (!isEnabled || input.text == "")
			return;
		
		//use the text from input to execute a command
		execute(input.text);
	}

	// Execute a file of prepared commands, treating each line as an indiv. command
	public bool executeFile(string fileName)
	{
		return true; //TODO lua file execution
	}

	// Attempt to execute a command. Fills output with the result of a successful command
	public bool execute(string command)
	{
		string commOut = "";
		bool success = false;

		//parse input
		string[] args = parseLine(command);
		args [0] = args [0].ToLower ();

		foreach (Command c in commands)
		{
			if(c.getInvocation().Equals(args[0]))
			{
				try
				{
					commOut = c.execute (args);
				}
				catch(Command.ExecutionException ee)
				{
					println (ee.Message, LogTag.error);
				}
				#pragma warning disable 0168
				catch(System.IndexOutOfRangeException ioore)
				#pragma warning restore 0168
				{
					println ("Provided too few arguments.\n" + c.getHelp(), LogTag.error);
				}

				if (commOut != "")
					println (commOut, LogTag.command_out);

				success = true;
			}
		}
		if(!success)
			println ("Command not found.  Try \"help\" for a list of commands", LogTag.error);
		history.Insert (0, input.text);

		return success;
	}

	// Parse an individual input line and return an array of args
	private string[] parseLine(string line)
	{
		List<string> argsList = new List<string> ();
		string mergeString = "";
		bool merging = false;

		for (int i = 0; i < line.Length; i++)
		{
			if (line [i] == '\"') //start or end space-ignoring group
			{
				merging = !merging;
				if (!merging)
				{
					argsList.Add (mergeString);
					mergeString = "";
				}
			}
			else if (line [i] == ' ' && !merging) //end of a regular term
			{
				if(mergeString != "")
					argsList.Add (mergeString);
				mergeString = "";
			}
			else //add any other character to the mergeString
				mergeString += line [i];
		}

		//if the merge string is not empty, add it the the args list
		if(mergeString != "")
			argsList.Add (mergeString);

		//return the parsed result
		return argsList.ToArray ();
	}

	// Print a message to the console
	public void println(string message)
	{
		print (message + "\n");
	}
	public void println(string message, LogTag tag)
	{
		print (message + "\n", tag);
	}
	public void print(string message)
	{
		print (message, LogTag.none);
	}
	public void print(string message, LogTag tag)
	{
		output.text += tags [(int)tag] + " " + message;

		if (output.cachedTextGenerator.lineCount > linesMax)
		{
			string currOutput = output.text;
			output.text = currOutput.Substring (output.cachedTextGenerator.lines [1].startCharIdx);
		}
	}

	// Clear the console output window
	public void clear()
	{
		output.text = "";
	}

	/* Inner Classes */

	// Used to tag output with appending strings and colors
	public enum LogTag
	{
		none, error, warning, info, command_out
	}
}
