using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoteMessage
{
    public int NoteNumber { get; private set; }
    public int NextNoteNumber { get; private set; }
    public float Length { get; private set; }
    public bool IsStartMessage { get; private set; }
    public bool IsLastNote { get; private set; }
    public bool IsEndMessage { get; private set; }

    public NoteMessage(int noteNumber, int nextNoteNumber = -1, float length = -1)
    {
        NoteNumber = noteNumber;
        NextNoteNumber = nextNoteNumber;
        Length = length;

        IsEndMessage = NoteNumber == -1 && NextNoteNumber == -1;
        IsStartMessage = NoteNumber == -1 && NextNoteNumber != -1;
        IsLastNote = NoteNumber != -1 && NextNoteNumber == -1;
    }
}

public class NoteCtrl : MonoBehaviour, IPublisher<NoteMessage>
{

    public enum MidiStatus { Play, Stop, Pause};

    // Properties
    public static NoteCtrl Control { get; private set; }       // For Singleton..
    public bool Running { get; private set; }
    public bool ScoreLoaded { get; private set; }

    private NoteMessage currentNote;
    public NoteMessage CurrentNote {
        get
        {
            return currentNote;
        }
        private set
        {
            currentNote = value;
            SendNotifications(currentNote);
        }
    }

    private string midiScoreFile;
    public string MidiScoreFile
    {
        get
        {
            return midiScoreFile;
        }

        set
        {
            midiScoreFile = value;
            LoadScore();
        }
    }

    // Private Members
    private MidiFile mf;
    private readonly List<ISubscriber<NoteMessage>> subscribers = new List<ISubscriber<NoteMessage>>();     // List of subscribers to notify
    private IEnumerator playMidi = null;

    // assuming 4/4 time signature and 120 BPM 
    // (don't need to support any other signatures for this super simple sequencer)
    private const int TEMPO = 30;
    private int ppq;                // pulses per quarter (from MidiFile)


    private void Awake()
    {
        //Implement Psuedo-Singleton
        if(Control == null)
        {
            DontDestroyOnLoad(gameObject);
            Control = this;
            playMidi = PlayMidiTrack();
        }
        else if (Control != this)
        {
            Destroy(gameObject);
        }
    }


    private void LoadScore()
    {
        var strictMode = false;
        mf = new MidiFile(MidiScoreFile, strictMode);
        ppq = mf.DeltaTicksPerQuarterNote;
        ScoreLoaded = true;

        //Find first note
        var track = mf.Events[0];  // MIDI Files for VRMIN should only have one track
        int eventIdx = 0;
        NoteOnEvent firstNoteEvent = null;
        foreach (var midiEvent in track)
        {
            if (MidiEvent.IsNoteOn(midiEvent))
            {
                firstNoteEvent = (NoteOnEvent)midiEvent;
                break;
            }
        }

        if (firstNoteEvent != null)
        {
            CurrentNote = new NoteMessage(-1, firstNoteEvent.NoteNumber, PlayerCtrl.Control.startDelay);
        }
        else
        {
            throw new System.ArgumentException("No Data in loaded Score");
        }
        
    }

    public void PlayMidi(MidiStatus midiStatus)
    {
        switch (midiStatus)
        {
            case MidiStatus.Play:
                if (!Running)
                {
                    StartCoroutine(playMidi);
                    Running = true;
                }
                break;
            case MidiStatus.Stop:
                if (Running)
                {
                    StopCoroutine(playMidi);
                    Running = false;
                }
                break;
            case MidiStatus.Pause:
            default:
                UnityEngine.Debug.Log(string.Format("MIDI status, {0}, not available", midiStatus));
                break;
        }
    }


    private IEnumerator PlayMidiTrack()
    {
        UnityEngine.Debug.Log("Starting MIDI File");

        var track = mf.Events[0];  // MIDI Files for VRMIN should only have one track
        int eventIdx = 0;
        foreach(var midiEvent in track)
        {
            if (MidiEvent.IsNoteOn(midiEvent))
            {
                NoteOnEvent noteOn = (NoteOnEvent)midiEvent;

                //find next note on
                int nextIdx = eventIdx + 1;
                while (nextIdx < track.Count && !MidiEvent.IsNoteOn(track[nextIdx]))
                {
                    nextIdx++;
                }
                // found a note on event or end of track

                //build Note Message
                int nextNote = nextIdx != track.Count ? ((NoteOnEvent)track[nextIdx]).NoteNumber : -1;  // if we reached the end without a Note on then send -1
                float length = (noteOn.NoteLength / ppq) *  60 / TEMPO;                                 // Note length in seconds
                CurrentNote = new NoteMessage(noteOn.NoteNumber, nextNote, length);

                Debug.Log("Note Length: " + length);
                yield return new WaitForSecondsRealtime(length);
            }
            eventIdx++;
        }

        CurrentNote = new NoteMessage(-1);      // Done so let's send a final Message with note = -1
        Running = false;
        playMidi = PlayMidiTrack();             // And reset the IEnumerator to we can play another track           
    }


    // Pub Sub Interface Methods
    // #########################

    public void Subscribe(ISubscriber<NoteMessage> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber<NoteMessage> subscriber)
    {
        subscribers.Remove(subscriber);
    }

    public void SendNotifications(NoteMessage message)
    {
        foreach (var s in subscribers)
        {
            s.Notify(CurrentNote);
        }
    }
}
