using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using UnityEngine;

public class ToneGenerator : ISubscriber<int>, ISampleProvider
{
    private double frequency = 440.0;
    public double Frequency
    {
        get
        {
            return frequency;
        }
        set
        {
            frequency = value;
            signalGenerator.Frequency = value;
        }
    }

    public double gain = 0.5;
    public double Gain
    {
        get
        {
            return gain;
        }
        set
        {
            gain = value;
            signalGenerator.Gain = value;
        }
    }

    public SignalGenerator signalGenerator;

    public ToneGenerator(NoteCtrl noteCtrl, int sampleRate, int channels)
    {
        noteCtrl.Subscribe(this);

        signalGenerator = new SignalGenerator(sampleRate, channels)
        {
            Gain = Gain,
            Frequency = Frequency,
            Type = SignalGeneratorType.Sin
        };
    }

    // ISubscriber Interface
    public void Notify(int midiNote)
    {
        Frequency = Utilities.Midi2Freq(midiNote);
        signalGenerator.Frequency = Frequency;
    }

    // ISample Provider Interface

    public WaveFormat WaveFormat { get; private set; }

    public int Read(float[] buffer, int offset, int count)
    {
        float[] tmpBuffer = new float[count];
        signalGenerator.Read(buffer, offset, count);

        return 0;
    }
}