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
        // Basic test. Wrap length > line length, so this is just one physical line.
        // Thus, the ToString() method should just return the input string.
        Line line = new Line("Hello world!", 12);
        Assert.AreEqual(line.ToString(), "Hello world!");

    }

    [Test]
    public void DoesLineWrap() 
    {
        // Wrapping test. Only characters over wrap limit should wrap.
        Line wrappedLine = new Line("Hello world!", 6);
        Assert.AreEqual(wrappedLine.ToString(), "Hello \n\tworld!");

        // Adding one more character should force a wrap.
        Line wrappedLine2 = new Line("Hello world!!", 6);
        Assert.AreEqual(wrappedLine2.ToString(), "Hello \n\tworld!\n\t!");
    }

    [Test]
    public void CanAddLines()
    {
        Line fullLine = new Line("Hello world!", 6);

        Line partialLine = new Line("Hello", 6);
        partialLine += " world!";

        Assert.AreEqual(fullLine.Contents, partialLine.Contents);
        Assert.AreEqual(fullLine.ToString(), partialLine.ToString());
    }

    [Test]
    public void LineSplitCase0High() 
    {
        string testString = "test";
        Line line = new Line(testString, 7);
        Line split = line.Split(7);

        // Split above the midpoint, so left keeps the string
        Assert.AreEqual(line.Contents, testString);
        Assert.AreEqual(split.Contents, "");
    }

    [Test]
    public void LineSplitCase0Low()
    {
        string testString = "test";
        Line line = new Line(testString, 7);
        Line split = line.Split(0);

        // Split below the midpoint, so right gets the string
        Assert.AreEqual(line.Contents, "");
        Assert.AreEqual(split.Contents, testString);
    }

    [Test]
    public void LineSplitCase0Middle()
    {
        string testString = "test";
        Line line = new Line(testString, 7);
        Line split = line.Split(3);

        // If we're on a boundary, the left should be larger.
        Assert.AreEqual(testString, line.Contents);
        Assert.AreEqual("", split.Contents);
    }

    [Test]
    public void LineSplitCase1()
    {
        string testString1 = "testing testing testing testing ";
        // Testing from here^    ^to here
        // (We test before the start of the string; <0 is same as =0.)

        for (int i = -20; i < 4; i++)
        {
            Line line = new Line(testString1, 8);
            Line split = line.Split(i);

            Assert.AreEqual("", line.Contents, "Nearest split index should be 0.");
            Assert.AreEqual(testString1, split.Contents);
        }
    }

    [Test]
    public void LineSplitCase2()
    {

        string testString1 = "testing testing testing testing ";
        // 1. Testing splits from ^here  ^to here
        // 2. Testing splits from         ^here   ^to here

        for (int i = 4; i < 12; i++) 
        {
            // All of these should split at index 8.
            Line line1 = new Line(testString1, 8);
            Line split1 = line1.Split(i);

            Assert.AreEqual(8, line1.Length, "Nearest split index should be 8. [i=" + i + "]");
            Assert.AreEqual(testString1.Length - 8, split1.Length);

            Assert.AreEqual("testing ", line1.Contents, "Nearest split index should be 8. [i=" + i + "]");
            Assert.AreEqual("testing testing testing ", split1.Contents);
        }

        for (int i = 12; i < 20; i++) 
        {
            // All of these should split at index 16.
            Line line2 = new Line(testString1, 8);
            Line split2 = line2.Split(i);

            Assert.AreEqual("testing testing ", line2.Contents, "Nearest split index should be 16. [i=" + i + "]");
            Assert.AreEqual("testing testing ", split2.Contents);
        }
    }

    [Test]
    public void LineSplitCase3() 
    {
        // We make the last block is a bit shorter. This was, cases 3 and 4 are hit.
        string testString = "testing testing testing test";
        // We're testing splits from             ^here ^to here

        for (int i = 20; i < 26; i++)
        {
            Line line1 = new Line(testString, 8);
            Line split1 = line1.Split(i);

            Assert.AreEqual("testing testing testing ", line1.Contents, "Nearest split index should be 24.");
            Assert.AreEqual("test", split1.Contents);
        }
    }

    [Test]
    public void LineSplitCase4()
    {
        // We make the last block is a bit shorter. This was, cases 3 and 4 are hit.
        string testString = "testing testing testing test";
        // We're testing splits from             here^      ^past the end.

        for (int i = 26; i < 50; i++) {
            Line line1 = new Line(testString, 8);
            Line split1 = line1.Split(i);

            Assert.AreEqual(testString, line1.Contents, "Nearest split index should be 28.");
            Assert.AreEqual("", split1.Contents);
        }
    }
}
