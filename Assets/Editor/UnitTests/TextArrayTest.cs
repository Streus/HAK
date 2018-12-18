using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

// TODO add more complex testing.
[TestFixture]
public class TextArrayTest {

    [Test]
    public void TextArrayBasicTest() {
        TextArray text = new TextArray();
        // Should result in 2 splits
        for (int i = 0; i < 95; i++) {
            string toAdd = "" + i;
            //Debug.Log("Adding: " + toAdd);
            text.Append(new Line(toAdd, 128));
        }


        Debug.Log(TextArray.TextArrayEntry.MAX);
        Debug.Log(TextArray.TextArrayEntry.SOFT_MIN);
        Debug.Log(TextArray.TextArrayEntry.SOFT_MAX);

        Debug.Log(text.Count);
        //Debug.Log("Last string in first entry: " + text.entries[0][text.entries[0].Count - 1]);

        for (int i = 0; i < text.entries.Count; i++) {
            //for (int j = 0; j < text.entries[i].Count; j++) {
            //Debug.Log("Entry " + i + " index " + j + " : " + text.entries[i][j]);
            //}
            Debug.Log(text.entries[i].CharCount);
        }


        //Debug.Log("Last string in second entry: " + text.entries[1][text.entries[1].Count - 1]);
        //Debug.Log(text.entries[0].ShouldSplit());
    }

    [Test]
    public void CanTextArraySplitBasic()
    {
        TextArray text = new TextArray();

        for (int i = 0; i < 96; i++) {
            string toAdd = new string('*', 128);
            text.Append(new Line(toAdd, 128));
        }

        // We should have split.
        Assert.AreEqual(2, text.entries.Count);

        // This test is set up so the split index is even. 
        Assert.AreEqual(96 / 2, text.entries[0].Count);
        Assert.AreEqual(128 * 96 / 2, text.entries[0].CharCount);
        Assert.AreEqual(text.entries[0].Count, text.entries[1].Count);
        Assert.AreEqual(text.entries[0].CharCount, text.entries[1].CharCount);
    }

    [Test]
    public void CanTextArraySplitLongStrings()
    {
        TextArray text = new TextArray();

        for (int i = 0; i < 3; i++)
        {
            // Lines now represent many physical rows.
            string toAdd = new string('*', 4096);
            text.Append(new Line(toAdd, 128));
        }

        // We should have split.
        Assert.AreEqual(2, text.entries.Count);

        // Even though we split a Line in half, the actual physical lines is
        // designed to be the same as above. Logical line count differs.
        Assert.AreEqual(2, text.entries[0].Count);
        Assert.AreEqual(128 * 96 / 2, text.entries[0].CharCount);
        Assert.AreEqual(text.entries[0].Count, text.entries[1].Count);
        Assert.AreEqual(text.entries[0].CharCount, text.entries[1].CharCount);

    }
}
