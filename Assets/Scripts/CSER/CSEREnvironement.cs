using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class CSEREnvironment
{
	/* Constants */
	public const string IF_STATEMENT = "if";
	public const string ELSE_STATEMENT = "else";
	public const string FOR_STATEMENT = "for";
	public const string END_STATEMENT = "end";
	public const char OUT_REDIRECT = '>';
	public const char SUBCOMMAND = '|';
	public const char GROUP = '\"';
	public const char LOCALVAR = '%';
	public const char GLOBVAR = '!';

	/* Static Vars */

	// All the currently executing environments
	private static Stack<CSEREnvironment> envStack;

	// Public accessor for the top of the envStack
	public static CSEREnvironment executing
	{
		get { return envStack.Peek (); }
	}

	/* Instance Vars */

	// All the variables declared in this environment
	private Dictionary<string, Variable> variables;

	// The current scope in execution
	private int scope;

	// Used to capture command output
	private string commandOuput;
	private void captureCommand(string s)
	{
		commandOuput = s;
	}

	/* Static Methods */

	// Parse an individual input line and return an array of args
	public static string[] parseLine(string line)
	{
		List<string> argsList = new List<string> ();
		string mergeString = "";
		bool merging = false;

		for (int i = 0; i < line.Length; i++)
		{
			if (line [i] == GROUP) //start or end space-ignoring group
			{
				merging = !merging;
				if (!merging)
				{
					argsList.Add (mergeString);
					mergeString = "";
				}
			}
			else if (line [i] == LOCALVAR || line [i] == GLOBVAR) //try to resolve a variable
			{
				char delim = line [i] == LOCALVAR ? LOCALVAR : GLOBVAR;
				int start = i + 1;
				int end = line.IndexOf (delim, start);
				if (end != -1)
				{
					mergeString += executing.getVariableValue (line.Substring (start, end - start), line [i] == CSEREnvironment.GLOBVAR);
					i = end;
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

	/* Constructors */
	static CSEREnvironment()
	{
		envStack = new Stack<CSEREnvironment> ();
	}

	public CSEREnvironment()
	{
		//instantiate instatnce vars
		variables = new Dictionary<string, Variable> ();
		scope = 0;

		//add self to the top of the execution stack
		envStack.Push(this);
	}

	/* Instance Methods */

	// Execute instructions
	public bool execute(string[] instructions)
	{
		Stack<IBlockStatement> scopeBounds = new Stack<IBlockStatement>();

		for(int currLine = -1; ++currLine < instructions.Length;)
		{
			if (instructions [currLine].TrimStart('\t', ' ').StartsWith (FOR_STATEMENT))
			{
				bool foundEnd = false;
				for (int i = currLine; i < instructions.Length; i++)
				{
					if (instructions [i].TrimStart('\t', ' ').StartsWith (END_STATEMENT))
					{
						foundEnd = true;
						openScope ();

						string[] args = parseLine (instructions [currLine]);
						if (args.Length != 4)
						{
							Console.log.println ("Malformed For statement on line " + currLine + ".", true);
							return false;
						}

						string varName = args [1];
						int start = 0, step = 0, end = 0;
						string[] commandOut = null;

						if (new Regex ("\\d+,?\\s+\\d+,?\\s+\\d+").IsMatch (args [2]))
						{
							string[] indexArgs = args [2].Split (' ', ',');
							try
							{
								start = int.Parse (indexArgs [0]);
								step = int.Parse (indexArgs [1]);
								end = int.Parse (indexArgs [2]);
							}
							#pragma warning disable 0168
							catch(Exception e)
							#pragma warning restore 0168
							{
								Console.log.println ("Malformed For statement on line " + currLine + ".", true);
								e.ToString (); // Supress unused warnings
								return false;
							}
						}
						else
						{
							Console.log.setStdOut (captureCommand);
							Console.log.execute (args [2]);
							commandOut = commandOuput.Split ('\n');
							Console.log.resetStdOut ();
						}

						ForStatement statement;
						if (commandOut != null)
							statement = new ForStatement (currLine, i, varName, commandOut);
						else
							statement = new ForStatement (currLine, i, varName, start, step, end);

						statement.update ();

						scopeBounds.Push (statement);
						break;
					}
				}
				if (!foundEnd)
				{
					Console.log.println ("For statement on line " + currLine + " has no corresponding" +
					" End statement.", true);
				}

				//save for loop info
			}
			else if(instructions[currLine].TrimStart('\t', ' ').StartsWith("end"))
			{
				if (scopeBounds.Peek () != null)
				{
					IBlockStatement block = scopeBounds.Peek ();
					ForStatement forStatement;
					try
					{
						forStatement = (ForStatement)block;
						if(forStatement.update())
							currLine = forStatement.getBlockStart();
						else
						{
							scopeBounds.Pop();
							closeScope();
						}
					}
					#pragma warning disable 0168
					catch(InvalidCastException ice) { }
					#pragma warning restore 0168
				}
			}
			else
			{
				Console.log.execute(instructions[currLine].TrimStart('\t', ' '));
			}
		}

		return true;
	}

	// Create a variable and store it in the console's dictionary
	public bool declareVariable(string name, string value, bool globalVar = false)
	{
		int localScope = globalVar ? 0 : scope;
		if (!variables.ContainsKey (localScope.ToString() + "~" + name))
			variables.Add (localScope.ToString() + "~" + name, new Variable (name, value, scope));
		else
			return false;
		return true;
	}

	// Set the value of a variable currently managed by the console
	public bool setVariableValue(string name, string value, bool globalVar = false)
	{
		Variable v = null;
		if (globalVar && variables.TryGetValue ("0~" + name, out v))
		{
			v.value = value;
			return true;
		}

		for(int i = scope; i >= 0; i--)
		{
			if(variables.TryGetValue (i.ToString() + "~" + name, out v))
			{
				v.value = value;
				return true;
			}
		}
		return false;
	}

	// Get the value of a variable via its name
	public string getVariableValue(string name, bool globalVar = false)
	{
		Variable v = null;

		if (globalVar && variables.TryGetValue ("0~" + name, out v))
			return v.value;

		for(int i = scope; i >= 0; i--)
			if(variables.TryGetValue (i.ToString() + "~" + name, out v))
				return v.value;

		Console.log.println (name + " does not exist in the current context.", true);
		return name;
	}

	// Open new scope
	public void openScope()
	{
		scope++;
	}

	// Close the current scope, if it's not the global scope
	public bool closeScope()
	{
		if (scope == 0)
		{
			Console.log.println ("Cannot close global scope.", true);
			return false;
		}

		// Remove all variables in the current scope
		foreach (Variable v in variables.Values)
		{
			if (v.scope == scope)
				variables.Remove (v.name);
		}

		scope--;
		return true;
	}

	// Remove this environment from the execution stack
	public void endExecution()
	{
		if(envStack.Peek() == this)
			envStack.Pop ();
	}

	/* Events + Delegates */


	/* Inner Classes */

	// To be used to save and retrive values in commands and .cser files
	public class Variable
	{
		// The name used to reference the variable
		public readonly string name;

		// The value the variable holds
		public string value;

		// The scope this variable is part of. when this scope ends, the variable is discarded
		// A value of -1 makes it global
		public readonly int scope;

		public Variable(string name, string value, int scope = -1)
		{
			this.name = name;
			this.value = value;
			this.scope = scope;
		}
	}
}
