using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Sanford.Multimedia.Midi;

public class PlayerCtrl : MonoBehaviour {

    // Name of file containing Midi data
    //TODO: Add a UI element for this when launching program
    public string midiScoreResource;

    // Name of Midi Device to connect to
    public string midiInputDeviceName;

    private MidiInputCtrl midiIn;
    public MidiInputCtrl MidiIn
    {
        get
        {
            return midiIn;
        }
    }


    void Awake () {

        //Start up the Note Controller
        //And start playing the midi notes
        NoteCtrl noteCtrl = NoteCtrl.GetInstance();
        noteCtrl.Player = this;
        noteCtrl.MidiScore = midiScoreResource;
        
        //Start Up the Midi Controllers
        midiIn = new MidiInputCtrl(midiInputDeviceName);
    }

    void Start()
    {
        NoteCtrl.GetInstance().PlayMidi(MidiStatus.Play);
        midiIn.Start();
    }

    void OnDisable()
    {
        midiIn.StopAndClose();
    }


    public void StartChildCoroutine(IEnumerator coroutineMethod)
    {
        StartCoroutine(coroutineMethod);
    }

    public void StopChildCoroutine(IEnumerator coroutineMethod)
    {
        StopCoroutine(coroutineMethod);
    }
}
