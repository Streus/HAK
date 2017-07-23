using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForStatement : IBlockStatement
{
	private string[] operationSet;

	private int currIndex;
	private int indexStep;
	private int finalIndex;

	private int blockStart;
	private int blockEnd;

	private string varName;

	/* Constructors */
	public ForStatement(int bStart, int bEnd, string varName, string[] operationSet) : this(bStart, bEnd, varName)
	{
		this.operationSet = operationSet;

		currIndex = 0;
		indexStep = 1;
		finalIndex = operationSet.Length - 1;
	}
	public ForStatement(int bStart, int bEnd, string varName, int initIndex, int indexStep, int finalIndex) : this(bStart, bEnd, varName)
	{
		operationSet = null;

		this.currIndex = initIndex;
		this.indexStep = indexStep;
		this.finalIndex = finalIndex;
	}
	private ForStatement(int bStart, int bEnd, string varName)
	{
		this.blockStart = bStart;
		this.blockEnd = bEnd;
		this.varName = varName;

		CSEREnvironment.executing.declareVariable (varName, "_");
	}

	// Returns the starting line number of the block
	public int getBlockStart ()
	{
		return blockStart;
	}

	// Returns the ending line numer of the block
	public int getBlockEnd ()
	{
		return blockEnd;
	}

	// Returns true if the loop continues, false if the loop should end
	public bool update()
	{
		string value = operationSet == null ? currIndex + "" : operationSet [currIndex];
		CSEREnvironment.executing.setVariableValue (varName, value);

		if (currIndex > finalIndex)
			return false;
		currIndex += indexStep;
		return true;
	}
}
