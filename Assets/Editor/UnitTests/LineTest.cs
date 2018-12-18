using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

// TODO add more complex testing.
[TestFixture]
public class LineTest
{

    [Test]
    public void BasicLineTest() 
    {
        //line += " This is a long string that should logically wrap in fun ways!";
        //Debug.Log(line);

        Line line = new Line("", 12);
        line += "Hello world!";

        Assert.AreEqual(line.ToString(), "Hello world!");
    }

    int main()
    {
        return 0;
    }
}
