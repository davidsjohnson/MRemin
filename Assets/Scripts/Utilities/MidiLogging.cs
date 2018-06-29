using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

public class MidiLogging : MonoBehaviour, ISubscriber<ChannelMessage>, ISubscriber<NoteMessage> {


    // Use this for initialization
    void Start ()
    {
        PlayerCtrl.Control.MidiIn.Subscribe(this);
        NoteCtrl.Control.Subscribe(this);
	}
	

    public void Notify(ChannelMessage message)
    {
        int channel = message.Data1;
        int value = message.Data2;
        int ts = message.Timestamp;

        PlayerCtrl.Control.Logger.Log("{0}\t{1}\t{2}", channel, value, ts);
    }

    public void Notify(NoteMessage message)
    {
        PlayerCtrl.Control.Logger.Log("NoteOn\t{0}\t{1}", message.NoteNumber, message.Length);
    }
}
