using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInputCtrl : MonoBehaviour {


    public string micName = "ZOOM Recording Mixer (H6)";

    // Use this for initialization
    void Start () {

        foreach(var d in Microphone.devices)
        {
            Debug.Log(string.Format("Mic: {0}", d));
        }

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(micName) > 0)) { }
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
