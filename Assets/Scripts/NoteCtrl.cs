using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Midi;
using System;

public class NoteCtrl : MonoBehaviour, IPublisher<int>
{

    public enum MidiStatus { Play, Stop, Pause};

    // Properties
    public static NoteCtrl Control { get; private set; }       // For Singleton..
    public bool Running { get; private set; }
    public int NextNote { get; private set; }

    private int currentNote;
    public int CurrentNote {
        get
        {
            return currentNote;
        }
        set
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
    private readonly List<ISubscriber<int>> subscribers = new List<ISubscriber<int>>();     // List of subscribers to notify
    private IEnumerator playMidi = null;

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
        while (true)
        {

            var midiEvent = track[eventIdx];
            if (MidiEvent.IsNoteOn(midiEvent))
            {
                NoteOnEvent noteOn = (NoteOnEvent)midiEvent;
                CurrentNote = noteOn.NoteNumber;
                float delay = noteOn.NoteLength / 192f * .5f;  // Verify this formula

                //find next note on
                int nextIdx = eventIdx + 1;
                while (nextIdx < track.Count && !MidiEvent.IsNoteOn(track[eventIdx]))
                {
                    nextIdx++;
                }
                // found a note on event
                NextNote = ((NoteOnEvent)track[nextIdx%track.Count]).NoteNumber;   // using Mod since we are currently looping midi track

                yield return new WaitForSecondsRealtime(delay);
            }

            eventIdx++;
            eventIdx %= track.Count;  //used to Loop probably need to send a Stop Message
        }
    }


    // Pub Sub Interface Methods
    // #########################

    public void Subscribe(ISubscriber<int> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber<int> subscriber)
    {
        subscribers.Remove(subscriber);
    }

    public void SendNotifications(int message)
    {
        foreach (var s in subscribers)
        {
            s.Notify(CurrentNote);
        }
    }
}
