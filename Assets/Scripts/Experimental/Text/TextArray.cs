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

    public void Insert(int index, string line) 
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

    public void Append(string line) 
    {
        int lastIndex = entries.Count - 1;
        Debug.Log("Appending to entry index " + lastIndex);
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
    public class TextArrayEntry : List<string> {

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
                    charCount += this[i].Length;
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

            // The midpoint is (roughly) the index where CharCount(0 ... midpoint) == CharCount(midpoint + 1 ... Count).
            //int midpoint = Count / 2;
            int midpoint = 0;

            TextArrayEntry newList = new TextArrayEntry();
            for (int i = midpoint; i < Count; i++)
            {
                newList.Add(this[i]);
                //this.Remove(this[i]);
            }

            this.RemoveRange(midpoint, Count - midpoint);

            //Debug.Log("Is new list null?: " + newList == null);
            //this.RemoveRange(midpoint, Count - midpoint);
            //TextArrayEntry newThis = GetRange(0, midpoint) as TextArrayEntry;

            return newList;
        }
    }
}
