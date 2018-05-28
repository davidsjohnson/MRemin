using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class LeftHandCtrl : MonoBehaviour, ISubscriber<ChannelMessage>
{
    public PlayerCtrl playerCtrl;

    public float minPosition;
    public float maxPosition;

    public int volumeChannel = 2;

    private int minMidiVal = 0;
    private int maxMidiVal = 127;

    // Use this for initialization
    void Start()
    {
        playerCtrl.MidiIn.Subscribe(this);
    }

    void onDisable()
    {
        playerCtrl.MidiIn.Unsubscribe(this);
    }

    public void Notify(ChannelMessage message)
    {
        int channel = message.Data1;
        int value = message.Data2;
        int ts = message.Timestamp;

        if (channel == volumeChannel)
        {
            // UnityEngine.Debug.Log(string.Format("Volume: {0}", value));
            // Calc new position value from Midi data
            float newPosition = Utilities.MapValue(value, minMidiVal, maxMidiVal, minPosition, maxPosition);

            // Update hand location
            Vector3 tmp = transform.localPosition;
            tmp.y = newPosition;
            transform.localPosition = tmp;

            playerCtrl.Logger.Log("{0}\t{1}\t{2}", channel, value, ts);
        }
    }
}
