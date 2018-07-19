using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class LeftHandCtrl : MonoBehaviour, ISubscriber<ChannelMessage>
{
    public float minPosition;
    public float maxPosition;

    private int minMidiVal = 0;
    private int maxMidiVal = 127;

    // Use this for initialization
    void Start()
    {
        PlayerCtrl.Control.MidiIn.Subscribe(this);
    }

    void OnDestroy()
    {
        PlayerCtrl.Control.MidiIn.Unsubscribe(this);
    }

    public void Notify(ChannelMessage message)
    {
        int channel = message.Data1;
        int value = message.Data2;
        int ts = message.Timestamp;

        if (channel == PlayerCtrl.Control.volMidiChannel)
        {
            // UnityEngine.Debug.Log(string.Format("Volume: {0}", value));
            // Calc new position value from Midi data
            float newPosition = Utilities.MapValue(value, minMidiVal, maxMidiVal, minPosition, maxPosition);

            // Update hand location
            Vector3 tmp = transform.localPosition;
            tmp.y = newPosition;
            transform.localPosition = tmp;
        }
    }
}
