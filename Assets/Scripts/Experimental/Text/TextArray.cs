using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class TextArray
{

    public List<TextArrayEntry> entries;

    public TextArray()
    {
        entries = new List<TextArrayEntry>();
        entries.Add(new TextArrayEntry());
    }

    public int Count
    {
        get 
        {
            return entries.Count;
        }
    }

    public void Insert(int index, Line line) 
    {
        int lastCount = 0;
        int currentCount = 0;

        TextArrayEntry entry = null;
        for (int i = 0; i < entries.Count; i++) 
        {
            currentCount += entries[i].Count;
            if (index >= lastCount && index < currentCount) {
                entry = entries[i];
                break;
            }
        }

        if (entry == null) {
            throw new System.Exception("Index " + index + " is invalid. Valid range: [0, " + currentCount + "].");
        }

        // Last count will hold the starting index of the current entry.
        int relativeIndex = index - lastCount;
        entry.Insert(relativeIndex, line);

        HandleSplit(entry);
    }

    public void Append(Line line) 
    {
        int lastIndex = entries.Count - 1;
        //Debug.Log("Appending to entry index " + lastIndex);
        entries[lastIndex].Add(line);


        HandleSplit(entries[lastIndex]);
    }

    private void HandleSplit(TextArrayEntry entry) 
    {
        if (entry.ShouldSplit()) 
        {
            // Grab the newly generated split and insert it after the current one
            TextArrayEntry nextSplit = entry.Split();
            if (nextSplit == null) {
                throw new System.Exception("Failed to split.");
            }
            entries.Insert(entries.IndexOf(entry) + 1, nextSplit);
        }
    }

    // TODO change me to use Lines instead of strings.
    public class TextArrayEntry : List<Line> {

        public const int MAX = 1 << 14;
        //public const int MAX = 128;

        // Soft max is determined by 3/4 the max number of characters (65536 / 4).
        public const int SOFT_MAX = MAX * 3 / 4;

        // Soft min is determined by 1/4 the max number of characters.
        public const int SOFT_MIN = MAX * 1 / 4;

        // Gets the count of characters. You usually want to use this 
        // instead of Count.
        public int CharCount
        {
            get
            {
                int charCount = 0;
                for (int i = 0; i < base.Count; i++) 
                {
                    charCount += this[i].Contents.Length;
                }
                return charCount;
            }
        }

        public TextArrayEntry() : base(MAX) { }

        /// <summary>
        /// Returns true if this TextArrayEntry is eligible for splitting.
        /// This is the case when our Count is greater than (3/4) * MAX.
        /// </summary>
        /// <returns><c>true</c>, if we can split, <c>false</c> otherwise.</returns>
        public bool ShouldSplit() 
        {
            return CharCount >= SOFT_MAX;
        }

        /// <summary>
        /// If size > SOFT_MAX, splits this this entry into two. Will return null
        /// if this is not necessary.
        /// 
        /// The first half of the strings will be retained in this object. The
        /// second half will be moved to a new TextArrayEntry object, and returned.
        /// </summary>
        /// <returns>The split.</returns>
        public TextArrayEntry Split() 
        {
            if (!ShouldSplit()) 
            {
                return null;
            }

            // Find the middle logical line.
            int estimatedMidpoint = CharCount / 2;
            int currentMidpoint = 0;
            Line middleLine = null;
            int middleIndex = 0;
            for (int i = 0; i < this.Count; i++) 
            {
                if (estimatedMidpoint >= currentMidpoint && estimatedMidpoint < currentMidpoint + this[i].Length)
                {
                    // Middle string is the current one
                    middleLine = this[i];
                    middleIndex = i;
                    break;
                }
                currentMidpoint += this[i].Length;
            }

            // Split the logical line.
            int splitIndex = estimatedMidpoint - currentMidpoint;
            Line afterMiddleLine = middleLine.Split(splitIndex);

            // Create the split list. Add the second half of the split line to it
            // first, then add every line after that. Note that the first half of
            // the split line is index "middleIndex", so we start 1 higher.
            TextArrayEntry newList = new TextArrayEntry();

            // If the second half of the split line was empty, then don't add it.
            if (!afterMiddleLine.Contents.Equals(""))
            {
                newList.Add(afterMiddleLine);
            }

            for (int i = middleIndex + 1; i < Count; i++) 
            {
                newList.Add(this[i]);
            }

            // Remove all the lines we added to the split. 
            // If the first half of the split line was empty, don't include it.
            if (middleLine.Contents.Equals(""))
            {
                this.RemoveRange(middleIndex, Count - middleIndex);
            }
            else
            {
                this.RemoveRange(middleIndex + 1, Count - middleIndex - 1);
            }

            return newList;
        }
    }
}
