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
        for (int i = 0; i < 100; i++) {
            string toAdd = "" + i;
            //Debug.Log("Adding: " + toAdd);
            text.Append(toAdd);
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
}
