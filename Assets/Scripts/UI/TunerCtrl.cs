using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class TunerCtrl : MonoBehaviour, ISubscriber<ChannelMessage>, ISubscriber<NoteMessage> {

    public GameObject pitchMarker;
    public PlayerCtrl playerCtrl;

    public float minX;
    public float maxX;

    public int pitchChannel = 20;

    private int currentNote;

    // Use this for initialization
    void Start()
    {
        playerCtrl.MidiIn.Subscribe(this);
        NoteCtrl.Control.Subscribe(this);
    }

    public void Notify(ChannelMessage message)
    {
        int channel = message.Data1;
        int value = message.Data2;       // hand position in MIDI value
        int ts = message.Timestamp;

        if (channel == pitchChannel)
        {
            // map hand location to frequency
            float minFreq = Utilities.Midi2Freq(playerCtrl.minMidiNote);
            float maxFreq = Utilities.Midi2Freq(playerCtrl.maxMidiNote);
            float handFreq = Utilities.MapValue(value, Utilities.MinMIDIIn, Utilities.MaxMIDIIn, minFreq, maxFreq);

            float minTunerFreq = Utilities.Midi2Freq(currentNote - 1);
            float maxTunerFreq = Utilities.Midi2Freq(currentNote + 1);
            float newX = Utilities.MapValue(handFreq, minTunerFreq, maxTunerFreq, minX, maxX);
            Vector3 pos = pitchMarker.transform.localPosition;
            pos.x = newX;
            pitchMarker.transform.localPosition = pos;
        }
    }

    public void Notify(NoteMessage midiNote)
    {
        currentNote = midiNote.NoteNumber;
    }
}
