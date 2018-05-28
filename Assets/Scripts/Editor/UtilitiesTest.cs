using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UtilitiesTest {

	[Test]
	public void Midi2FreqIsCorrect() {
        int midiNote = 69;   // A4 440hz
        float expectedFreq = 440f;
        float precisionDelta = 0.009f;

        Assert.AreEqual(expectedFreq, Utilities.Midi2Freq(midiNote), precisionDelta,
                        string.Format("Midi Note {0} doesn't convert to {1}", midiNote, expectedFreq));

        midiNote = 21;      // A0 27.5Hz
        expectedFreq = 27.5f;

        Assert.AreEqual(expectedFreq, Utilities.Midi2Freq(midiNote), precisionDelta,
                        string.Format("Midi Note {0} doesn't convert to {1}", midiNote, expectedFreq));

        midiNote = 108;      // C8 4186.0
        expectedFreq = 4186f;

        Assert.AreEqual(expectedFreq, Utilities.Midi2Freq(midiNote), precisionDelta,
                        string.Format("Midi Note {0} doesn't convert to {1}", midiNote, expectedFreq));
    }

    [Test]
    public void MapValueIsCorrect()
    {
        float srcMin = 0;
        float srcMax = 100;
        float dstMin = 0;
        float dstMax = 10;

        float ogValue = 90;
        float expectedOutput = 9;
        Assert.AreEqual(expectedOutput, Utilities.MapValue(ogValue, srcMin, srcMax, dstMin, dstMax), "Incorrect Mapping");


        // Works when src is reverse order
        srcMin = 100;
        srcMax = 0;

        ogValue = 90;
        expectedOutput = 1;
        Assert.AreEqual(expectedOutput, Utilities.MapValue(ogValue, srcMin, srcMax, dstMin, dstMax), "Incorrect Mapping");

        //Works when dst is in reverse order
        srcMin = 0;
        srcMax = 100;
        dstMin = 10;
        dstMax = 0;

        ogValue = 90;
        expectedOutput = 1;
        Assert.AreEqual(expectedOutput, Utilities.MapValue(ogValue, srcMin, srcMax, dstMin, dstMax), "Incorrect Mapping");
    }

    [Test]
    public void Midi2NoteStrIsCorrect()
    {
        int midiNote = 69;  // A4
        string expectedStr = "A<size=36>4</size>";
        string result = Utilities.Midi2NoteStr(midiNote);
        Assert.AreEqual(expectedStr, result, "Note Str result is incorrect");

        midiNote = 0;  // C-1
        expectedStr = "C<size=36>-1</size>";
        result = Utilities.Midi2NoteStr(midiNote);
        Assert.AreEqual(expectedStr, result, "Note Str result is incorrect");

        midiNote = 127;  // G9
        expectedStr = "G<size=36>9</size>";
        result = Utilities.Midi2NoteStr(midiNote);
        Assert.AreEqual(expectedStr, result, "Note Str result is incorrect");
    }

    [Test]
    public void TestAngle()
    {
        Vector2 v1 = new Vector2(-2, -1);
        Vector2 v2 = new Vector2(1, -2);

        float angle = Vector2.SignedAngle(v2 - v1, Vector2.right);

        float xDelta = Mathf.Sin(Mathf.Deg2Rad * angle) * .5f;
        float yDelta = Mathf.Cos(Mathf.Deg2Rad * angle) * .5f;

        Assert.AreEqual(45.0f, yDelta);

    }

    [Test]
    public void ListMidiDevices()
    {
        UnityEngine.Debug.unityLogger.logEnabled = true;
        MidiInputCtrl.ListMidiDevices();
    }
}
