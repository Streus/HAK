using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line {

    // How wide is this line?
    private int lineWidth = -1;

    // The raw contents of this Line, ignoring any wrapping.
    public string Contents;

    // The contents of this Line, split into chunks of size <= lineWidth.
    //public List<string> LimitedContents;

    // TODO pull the "128" from settings, whenever they get implemented.
    public Line() : this("", 128) { }

    public Line(int lineWidth) : this("", lineWidth) { }

    public Line(string initialContents, int lineWidth) 
    {
        this.Contents = initialContents;
        //LimitedContents = new List<string>();
        Reindex(lineWidth);
    }

    // Update this line's width. Rebuilds the entire structure.
    public void Reindex(int lineWidth) 
    {
        this.lineWidth = lineWidth;
    }

    // Add a string to the end of this line.
    public void Add(string str) 
    {
        Contents += str;
    }

    // Add a string to the end of this line.
    public static Line operator +(Line line, string str) 
    {
        line.Add(str);
        return line;
    }

    // Returns this line as a string. This will add line breaks every "lineWidth"
    // characters, followed by a tab on the newly broken line.
    public override string ToString()
    {
        string toReturn = "";
        int index = 0;
        for (; index < Contents.Length - lineWidth; index += lineWidth) 
        {
            toReturn += Contents.Substring(index, lineWidth) + "\n\t";
        }

        toReturn += Contents.Substring(index);

        return toReturn;
    }
}
