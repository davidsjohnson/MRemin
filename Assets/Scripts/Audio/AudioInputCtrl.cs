using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInputCtrl : MonoBehaviour {

    public string driverName = "ZOOM H and F Series ASIO";

    private int numChannels = 2;
    private AsioSampleProvider sampleProvider;

    private void Awake()
    {
        sampleProvider = new AsioSampleProvider(driverName, AudioSettings.outputSampleRate, numChannels);
    }

    private void OnDisable()
    {
        sampleProvider.Close();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        Debug.Log("Data Length: " + data.Length);
        sampleProvider.Read(data, 0, data.Length);
    }
}
