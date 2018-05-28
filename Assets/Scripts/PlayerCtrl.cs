using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Sanford.Multimedia.Midi;

public class PlayerCtrl : MonoBehaviour
{
    //TODO: Add a UI element for this when launching program
    public string midiScoreResource;        // Name of file containing Midi data


    public int participantID = 0;

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

    public LogWriter Logger { get; private set; }

    void Awake () {

        //Start up the Note Controller
        //And start playing the midi notes
        NoteCtrl noteCtrl = NoteCtrl.GetInstance();
        noteCtrl.Player = this;
        noteCtrl.MidiScore = midiScoreResource;
        
        //Start Up the Midi Controllers
        midiIn = new MidiInputCtrl(midiInputDeviceName);

        Logger = new LogWriter(string.Format("p{0}-midi-logger", participantID));
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
