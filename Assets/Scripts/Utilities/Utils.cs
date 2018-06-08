using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utilities
{

    public static int MinMIDIIn = 0;
    public static int MaxMIDIIn = 127;


    private static string[] notes = { "C", "C<i>#</i>", "D", "D<i>#</i>", "E", "F", "F<i>#</i>", "G", "G<i>#</i>", "A", "A<i>#</i>", "B" };

    /*
    * Helper function to map value between two ranges (TODO:  Add to a utilies script maybe)
    */
    public static float MapValue(float value, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        return (value - srcMin) * (dstMax - dstMin) / (srcMax - srcMin) + dstMin;
    }

    public static float Midi2Freq(int midiNote)
    {
       return Mathf.Pow(2, (midiNote - 69) / 12.0f) * 440.0f;
    }

    public static string Midi2NoteStr(int midiNote, int octaveSize=36)
    {
        string noteStr = notes[midiNote % 12];
        int octave = midiNote / notes.Length - 1;  // - 1 becuase octaves start at -1

        noteStr += string.Format("<size={0}>{1}</size>", octaveSize, octave);

        return noteStr;
    }

    public static float MidiValue2Freq(int midiValue, int midiNoteMin, int midiNoteMax)
    {
        int midiValMin = 0;
        int midiValMax = 127;
        return MapValue(midiValue, midiValMin, midiValMax, Midi2Freq(midiNoteMin), Midi2Freq(midiNoteMin));
    }
}
