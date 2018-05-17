using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

[RequireComponent(typeof(AudioSource))]
public class ToneCtrl : MonoBehaviour {

    public int channels = 2;

    private ToneGenerator toneGenerator;

    private void Awake()
    {
        toneGenerator = new ToneGenerator(NoteCtrl.GetInstance(), AudioSettings.outputSampleRate, channels);

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            SpatialSoundSettings.SetRoomSize(audioSource, SpatialSoundRoomSizes.None);
        }   
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        toneGenerator.Read(data, 0, data.Length);
    }
}
