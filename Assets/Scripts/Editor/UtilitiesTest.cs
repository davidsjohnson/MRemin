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
}
