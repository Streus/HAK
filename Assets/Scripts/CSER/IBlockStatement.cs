using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockStatement
{
	// Returns the starting line number of the block
	int getBlockStart ();

	// Returns the ending line numer of the block
	int getBlockEnd ();
}
