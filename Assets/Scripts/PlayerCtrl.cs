using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Sanford.Multimedia.Midi;

public class PlayerCtrl : MonoBehaviour
{        
    // Leaving for now. TODO: remove this at some point?
    public bool noteCtrlOn = true;
   
    public string ParticipantID { get; set; }           // Participant ID
    public string MidiScoreResource { get; set; }       // Name of file containing Midi data
    public string MidiInputDeviceName { get; set; }     // Name of Midi Device to connect to

    public LogWriter Logger { get; private set; }
    public MidiInputCtrl MidiIn { get; private set; }   // Main Midi Input Controller used to position left and right hands

    private NoteCtrl noteCtrl;

    void Awake ()
    {
        // Initialize the Note Controller
        noteCtrl = NoteCtrl.GetInstance();
        noteCtrl.Player = this;

        //Initialize Midi In (so objects can subscribe to it upon load)
        MidiIn = new MidiInputCtrl();

        //Initialize Logger
        Logger = new LogWriter();
    }

    public bool StartVRMin()
    {
        // Start Up the Logger
        Logger.Start(string.Format("p{0}-midi-logger", ParticipantID));

        //Start Up the Midi Controllers
        MidiIn.Connect(MidiInputDeviceName);
        MidiIn.Start();

        // Start Playing
        noteCtrl.MidiScore = MidiScoreResource;
        noteCtrl.PlayMidi(MidiStatus.Play);

        return true;
    }

    void OnDisable()
    {
        if(MidiIn != null)
            MidiIn.StopAndClose();
        if (Logger != null)
            Logger.Stop();
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
