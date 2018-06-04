using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ScoreDropdown : MonoBehaviour {

    public PlayerCtrl playerCtrl;
    private Dropdown dropdown;

    private List<string> midiScoreFiles;

    private const string scorePath = "Assets/Resources/";

	void Start ()
    {
        // Setup dropdown menu options and change handler
        dropdown = GetComponent<Dropdown>();
        dropdown.AddOptions(MidiScores);
        dropdown.onValueChanged.AddListener(delegate { OnValueChangedHandler(dropdown); });
	}

    private void OnValueChangedHandler(Dropdown change)
    {
        if(change.value != 0)
            playerCtrl.MidiScoreResource = Path.Combine(scorePath, change.captionText.text);
    }

    private List<string> MidiScores
    {
        get
        {
            if (midiScoreFiles == null)
            {
                midiScoreFiles = new List<string>();
                Directory.GetFiles(scorePath).Where(file => file.EndsWith(".bytes")).
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
