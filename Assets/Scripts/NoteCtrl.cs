using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sanford.Multimedia.Midi;


public enum MidiStatus { Play, Stop, Pause };


public class NoteCtrl : IPublisher<int>
{
    // Private Members
    private static NoteCtrl singleton = null;

    private int eventIdx;
    private Track midiTrack;

    private static List<ISubscriber<int>> subscribers = new List<ISubscriber<int>>();

    private IEnumerator playMidi = null;

    // Properties
    public bool Running { get; private set; }
    public int CurrentNote { get; private set; }
    public int NextNote { get; private set; }

    public PlayerCtrl Player { get; set; }

    private string midiScore;
    public string MidiScore
    {
        get
        {
            return midiScore;
        }
        set
        {
            midiScore = value;
            LoadScore();
        }
    }

    private NoteCtrl()
    {
        playMidi = PlayMidiTrack();
    }

    // Methods

    public static NoteCtrl GetInstance()
    {
        if (singleton == null)
        {
            singleton = new NoteCtrl();
        }
        return singleton;
    }

    /*
     * Loads a score from a MIDI file for a teaching a melody to a student
     */
    private void LoadScore()
    {
        Sequence s = new Sequence();
        s.Load(MidiScore);
        midiTrack = s[0];  // For now assume only 1 track per Sequence (TODO: Review Sequences in new Midi Toolkit)
    }


    public void PlayMidi(MidiStatus midiStatus)
    {
        switch (midiStatus)
        {
            case MidiStatus.Play:
                if (!Running)
                {
                    Player.StartChildCoroutine(playMidi);
                    Running = true;
                }
                break;
            case MidiStatus.Stop:
                if (Running)
                {
                    Player.StopChildCoroutine(playMidi);
                    Running = false;
                }

                break;
            case MidiStatus.Pause:
            default:
                UnityEngine.Debug.Log("MIDI status not yet implemented");
                break;
        }
    }

    /*
     * Plays a MIDI Track and updates all subscribers when new note is played.
     * MIDI Track is assumed to be monophonic
     * (TODO: Look into a better method for playing MIDI. This seems a bit of a hack)
     */
    private IEnumerator PlayMidiTrack()
    {
        UnityEngine.Debug.Log("Starting Midi Track");

        eventIdx = 0;
        while (true)
        {
            MidiEvent e = midiTrack.GetMidiEvent(eventIdx);         // Load next event
            float delay = 0;                                        // NoteOff has been called so reset Delay

            if (e.MidiMessage.GetType() == typeof(ChannelMessage))  // Make sure its a Channel Message
            {
                // Get Note Info from Message
                ChannelMessage m = e.MidiMessage as ChannelMessage;
                int note = m.Data1;

                if (m.Command == ChannelCommand.NoteOn)
                {
                    // Received a Note On so set current note and notify all subscribers
                    CurrentNote = note;
                    SendNotifications(CurrentNote);

                    // Check next events in MidiTrack for a note off  (TODO: Review MIDI docs for better method)
                    int j = eventIdx + 1;
                    MidiEvent nextE = midiTrack.GetMidiEvent(j);
                    ChannelMessage nextM = nextE.MidiMessage as ChannelMessage;

                    // Find note off for current note
                    while (j < midiTrack.Count && nextM.Command != ChannelCommand.NoteOff && nextM.Data1 != CurrentNote)
                    {
                        j++;
                        nextE = midiTrack.GetMidiEvent(j);
                        nextM = nextE.MidiMessage as ChannelMessage;
                    }

                    // Now that we found the note off Set delay for IEnumerator based on number of Ticks
                    delay = nextE.DeltaTicks / 192f * .5f; //TODO: Verify this calculation
                }
            }
            // Move to next MIDI event to find next note on
            eventIdx++;
            eventIdx %= midiTrack.Count;  // used to Loop...Probably don't want this in real setting...

            yield return new WaitForSecondsRealtime(delay);
        }
    }


    public void SendNotifications(int midiNote)
    {
        foreach (var s in subscribers)
        {
            s.Notify(CurrentNote);
        }
    }

    public void Subscribe(ISubscriber<int> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber<int> subscriber)
    {
        subscribers.Remove(subscriber);
    }
}
