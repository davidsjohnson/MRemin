using NAudio.Wave;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class AsioSampleProvider : ISampleProvider
{
    private AsioOut asio;
    private BufferedWaveProvider waveProvider;
    private ISampleProvider sampleProvider;

    

    private int sampleRate;
    private int numChannels;

    public AsioSampleProvider(string driverName, int sampleRate=44100, int numChannels = 2)
    {
        this.sampleRate = sampleRate;
        this.numChannels = numChannels;

        //For now assuming ASIO in is PCM 16 (H6 input is PCM 16bit and not planning to use another interface)
        //In future may need some sort of data conversion
        this.WaveFormat = new WaveFormat(sampleRate, numChannels);
        waveProvider = new BufferedWaveProvider(this.WaveFormat);

        sampleProvider = waveProvider.ToSampleProvider();

        try
        {
            asio = new AsioOut(driverName);
            asio.InitRecordAndPlayback(null, numChannels, sampleRate);
            asio.AudioAvailable += OnAudioAvailable;
            asio.Play();
            Debug.Log(string.Format("Asio Playing: {0}", asio.PlaybackState));
        }
        catch(System.ArgumentException e)
        {
            Debug.Log(string.Format("Invalid ASIO Driver Name: {0}", e));
        }
        catch(System.Exception e)
        {
            Debug.Log(string.Format("Unknown ASIO Error: {0}", e));
        }
    }

    private void OnAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
    {
        int dataSize = sizeof(short);

        byte[] interleavedBuffer = new byte[e.SamplesPerBuffer * dataSize * e.InputBuffers.Length];
        byte[] inputBuffer = new byte[e.SamplesPerBuffer * dataSize];

        for (int i = 0; i < e.InputBuffers.Length; i++)
        {
            Marshal.Copy(e.InputBuffers[i], inputBuffer, 0, e.SamplesPerBuffer * dataSize);            //Copying to managed array for easier access (can 
            for (int j = 0; j < inputBuffer.Length; j += dataSize)
            {
                interleavedBuffer[j * e.InputBuffers.Length + i * dataSize] = inputBuffer[j];
                interleavedBuffer[j * e.InputBuffers.Length + i * dataSize + 1] = inputBuffer[j + 1];
            }
        }
        waveProvider.AddSamples(interleavedBuffer, 0, interleavedBuffer.Length);
    }

    public void Close()
    {
        if (asio.PlaybackState != PlaybackState.Stopped) asio.Stop();
        asio.Dispose();
    }

    public WaveFormat WaveFormat { get; private set; }

    public int Read(float[] buffer, int offset, int count)
    {
        return sampleProvider.Read(buffer, offset, count);
    }
}