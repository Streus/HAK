using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line {

    // How wide is this line?
    private int lineWidth = -1;

    // The raw contents of this Line, ignoring any wrapping.
    public string Contents;

    public int Length { get { return Contents.Length; } }

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

    private void Clear() 
    {
        this.Contents = "";
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

    /// <summary>
    /// Splits this logical line at the nearest possible even break point.
    /// 
    /// If this Line is equal to one physical line, no split occurs; this Line will
    /// either remain unchanged or be returned, depending on which side of the split
    /// it logically belongs to.
    /// 
    /// Otherwise, if this Line is larger than one physical line, then the closest
    /// break point is computed. This Line will retain every block to the left of
    /// the break point, and this method returns every block to the right of the 
    /// break point. 
    /// 
    /// It is possible that either this Line becomes empty or the returned Line 
    /// is empty. 
    /// </summary>
    /// <param name="index">Index.</param>
    public Line Split(int index) 
    {
        // We're equivalent to one physical line.
        if (Contents.Length <= lineWidth) 
        {
            if (index < Contents.Length / 2)
            {
                Line toReturn = new Line(Contents, lineWidth);
                Clear();
                return toReturn;
            }
            return new Line("", lineWidth);
        }

        // Case 1.
        // This Line should become empty; return a copy of this Line
        if (index < lineWidth / 2)
        {
            Line toReturn = new Line(Contents, lineWidth);
            Clear();
            return toReturn;
        }

        // Leftover is how many characters are in the last non-full block of this line.
        int leftover = Contents.Length % lineWidth;

        // Case 4.
        // This Line is unchanged; return an empty Line.
        if (index >= Contents.Length - (leftover / 2))
        {
            return new Line("", lineWidth);
        }

        // Case 3.
        // This Line keeps all but the last block; return the last block.
        if (index >= Contents.Length - leftover - (lineWidth / 2))
        {
            Line toReturn = new Line(Contents.Substring(Contents.Length - leftover), lineWidth);
            Contents = Contents.Substring(0, Contents.Length - leftover);
            return toReturn;
        }

        // Case 2 (General case).
        // This Line keeps all blocks (roughly) left of the index; return the rest.
        int splitIndex = (index + (lineWidth / 2)) / lineWidth * lineWidth;
        Line toReturnGeneral = new Line(Contents.Substring(splitIndex), lineWidth);
        Contents = Contents.Substring(0, splitIndex);

        return toReturnGeneral;
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
