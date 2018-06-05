using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class AudioOutputTest {

    private void AssertBuffersAreEqual(float[] buffer1, float[] buffer2, float delta)
    {
        for (int i = 0; i < buffer1.Length; i++)
        {
            Assert.AreEqual(buffer1[i], buffer2[i], delta, string.Format("Failed on test {0}", i));
        }
    }

	[Test]
	public void ToneGeneratorCreatesSineWave() {
        // Instantiate Tone Generator for testing
        // Set Freq to 1000
        ToneGenerator toneGenerator = new ToneGenerator(NoteCtrl.Control, AudioSettings.outputSampleRate, 1);
        toneGenerator.Frequency = 1000;

        // Get Data from the Generator
        int bufferLen = 10;
        float[] buffer = new float[bufferLen];
        toneGenerator.Read(buffer, 0, buffer.Length);

        // Expected buffer as manually generated in Python {Frequency: 1000, Gain: 0.7, FS: 48000}
        float[] expectedBuffer = { 0.0f,  0.09136833f,  0.18117333f,  0.2678784f,  0.35f,
                                   0.426133f, 0.49497475f,  0.55534734f,  0.60621778f,  0.64671567f };

        AssertBuffersAreEqual(expectedBuffer, buffer, .0001f);
    }

    [Test]
    public void ToneGeneratorChangesFromSubscriberUpdates()
    {
        // Instantiate Tone Generator for testing
        // Set Freq to 1000
        ToneGenerator toneGenerator = new ToneGenerator(NoteCtrl.Control, AudioSettings.outputSampleRate, 1);
        toneGenerator.Frequency = 1000;

        float precisionDelta = .0001f;
        int bufferLen = 10;

        // Get Data from the Generator
        float[] buffer1 = new float[bufferLen];
        toneGenerator.Read(buffer1, 0, buffer1.Length);

        // Expected buffer as manually generated in Python {Frequency: 1000, Gain: 0.7, FS: 48000} (samples [0:10])
        float[] expectedBuffer1000 = { 0.0f,  0.09136833f,  0.18117333f,  0.2678784f,  0.35f,
                                   0.426133f, 0.49497475f,  0.55534734f,  0.60621778f,  0.64671567f };

        AssertBuffersAreEqual(expectedBuffer1000, buffer1, precisionDelta);

        int newMidi = 69;
        toneGenerator.Notify(newMidi);

        float[] buffer2 = new float[bufferLen];
        toneGenerator.Read(buffer2, 0, buffer2.Length);

        // Expected buffer as manually generated in Python {Frequency: 440, Gain: 0.7, FS: 48000} (samples [10:20])
        float[] expectedBuffer440 = {0.38124732f, 0.41440923f, 0.44619679f, 0.47650461f, 0.50523216f,
                                     0.53228418f, 0.55757094f, 0.5810086f, 0.60251942f, 0.62203206f};

        AssertBuffersAreEqual(expectedBuffer440, buffer2, precisionDelta);
    }
}
