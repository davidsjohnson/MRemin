using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ScoreDropdown : MonoBehaviour {

    private Dropdown dropdown;

    private List<string> midiScoreFiles;

    private string scorePath;

	void Start ()
    {
        scorePath = "C:\\Users\\david\\Documents\\projects\\VRmin-UserStudy\\Scores\\";

        // Setup dropdown menu options and change handler
        dropdown = GetComponent<Dropdown>();
        dropdown.AddOptions(MidiScores);
        dropdown.onValueChanged.AddListener(delegate { OnValueChangedHandler(dropdown); });
        PlayerCtrl.Control.MidiScoreResource = null;
	}

    private void OnValueChangedHandler(Dropdown change)
    {
        if(change.value != 0)
            PlayerCtrl.Control.MidiScoreResource = Path.Combine(scorePath, change.captionText.text);
    }

    private List<string> MidiScores
    {
        get
        {
            if (midiScoreFiles == null)
            {
                midiScoreFiles = new List<string>();
                Directory.GetFiles(scorePath).Where(file => file.EndsWith(".bytes") || file.EndsWith(".midi") || file.EndsWith(".mid")).
                    ToList().ForEach(path => midiScoreFiles.Add(Path.GetFileName(path)));
                //foreach (var path in Directory.GetFiles(scorePath).Where(file => file.EndsWith(".bytes")))
                //{
                //    midiScoreFiles.Add(Path.GetFileName(path));
                //}
            }

            return midiScoreFiles;
        }
    }

}
