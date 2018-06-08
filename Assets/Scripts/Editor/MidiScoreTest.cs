using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class MidiScoreTest {

	[Test]
	public void MidiScoreLoadsSuccessfully() {
        // Use the Assert class to test conditions.

        NoteCtrl.Control.MidiScoreFile = "Assets/Resources/score_NIME.bytes";
        Assert.IsTrue(NoteCtrl.Control.ScoreLoaded);
    }

}
