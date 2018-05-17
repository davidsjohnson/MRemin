using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utilities
{
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
}
