using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class RightHandCtrl : MonoBehaviour, ISubscriber<ChannelMessage> {

    public float minPosition;
    public float maxPosition;

    public int pitchChannel = 20;

    private MidiInputCtrl midiIn;
    private int minMidiVal = 0;
    private int maxMidiVal = 127;

	// Use this for initialization
	void Start ()
    {
        // Change Right Hand to "OK" position
        Animator rhAnimator = GetComponent<Animator>();
        rhAnimator.SetTrigger(Animator.StringToHash("OK"));

        midiIn = GetComponentInParent<PlayerCtrl>().MidiIn;
        midiIn.Subscribe(this);
	}

    void onDisable()
    {
        midiIn.Unsubscribe(this);
    }


    public void Notify(ChannelMessage message)
    {
        int channel = message.Data1;
        int value = message.Data2;

        if (channel == pitchChannel)
        {
            UnityEngine.Debug.Log(string.Format("Pitch: {0}", value));
            UnityEngine.Debug.Log(string.Format("Volume: {0}", value));
            // Calc new position value from Midi data
            float newPosition = Utilities.MapValue(value, maxMidiVal, minMidiVal, minPosition, maxPosition);

            // Update hand location
            Vector3 tmp = transform.localPosition;
            tmp.x = newPosition;
            transform.localPosition = tmp;
        }
    }
}
