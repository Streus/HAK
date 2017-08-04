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
		return history.ToArray ();
	}

	// Every individual 
	private List<string> history;
	private int historyIndex;

	[SerializeField]
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

			history = new List<string> ();
			historyIndex = -1;

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
		if (!_enabled || input.text == "")
			return;
		
		//use the text from input to execute a command
		string output = "";
		bool success = execute(input.text, out output);
		LogTag outTag = !success ? LogTag.error : LogTag.buildConOut();
		if (output != "")
			println (output, outTag);
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
			println ("Failed to load " + fileName + ".", LogTag.error);
			return false;
		}

		new CSEREnvironment ().execute (instructions);

		return true;
	}

	// Attempt to execute a command. Fills output with the result of a successful command
	public bool execute(string command, out string output)
	{
		history.Insert (0, input.text);

		bool success = true;

		//parse input
		string[] args = CSEREnvironment.parseLine(command);
		args [0] = args [0].ToLower ();

		Command c = null;
		if (commands.TryGetValue (args [0], out c))
		{
			try
			{
				output = c.execute (args);
			}
			catch (Command.ExecutionException ee)
			{
				output = ee.Message;
				success = false;
			}
			#pragma warning disable 0168
			catch (System.IndexOutOfRangeException ioore)
			#pragma warning restore 0168
			{
				output = "Provided too few arguments.\n" + c.getHelp ();
				success = false;
			}
		}
		else
		{
			output = "Command \"" + args[0] + "\" not found.  Try \"help\" for a list of commands";
			success = false;
		}

		return success;
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
		output.text += tag.getFullTag() + " " + message;

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
	public struct LogTag
	{
		/* Basic pre-defined tags */
		public static readonly LogTag none = new LogTag ("", Color.white);
		public static readonly LogTag error = new LogTag ("[ERR]", Color.red);
		public static readonly LogTag warning = new LogTag ("[WARN]", Color.yellow);
		public static readonly LogTag info = new LogTag ("[INFO]", Color.green);

		/* Static Methods */
		public static LogTag buildConOut()
		{
			return new LogTag (GameManager.currentPath + " $", Color.cyan);
		}

		/* Instance Vars */
		private string tagValue;
		private Color tagColor;

		public LogTag(string value, Color color)
		{
			tagValue = value;
			tagColor = color;
		}

		public string getFullTag()
		{
			if (tagValue == "")
				return tagValue;
			string colorValue = "<color=#";
			Vector4 colVals = (Vector4)tagColor;
			colorValue += ((int)(255f * colVals.x)).ToString ("x2");
			colorValue += ((int)(255f * colVals.y)).ToString ("x2");
			colorValue += ((int)(255f * colVals.z)).ToString ("x2");
			colorValue += ((int)(255f * colVals.w)).ToString ("x2");
			return colorValue + "><b>" + tagValue + "</b></color>";
		}
	}
}
