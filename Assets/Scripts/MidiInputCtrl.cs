using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia;


public class MidiInputCtrl : IPublisher<ChannelMessage>
{

    private InputDevice midiInput = null;
    private List<ISubscriber<ChannelMessage>> subscribers = new List<ISubscriber<ChannelMessage>>();

	public MidiInputCtrl(string deviceName="Moog Theremini") {

        // Connect to Device
        UnityEngine.Debug.Log(string.Format("Num Devices: {0}", InputDevice.DeviceCount));

        bool found = false;
        for (int i = 0; i < InputDevice.DeviceCount; i++)
        {
            MidiInCaps deviceCaps = InputDevice.GetDeviceCapabilities(i);
            if (deviceName.Equals(deviceCaps.name))
            {
                found = true;
                try
                {
                    UnityEngine.Debug.Log(string.Format("Connecting to MIDI Device: {0}", deviceCaps.name));
                    midiInput = new InputDevice(i);

                    // Register Callback for Channel Messages (should be all we need to handle)
                    midiInput.ChannelMessageReceived += HandleChannelMessageReceived;
                }
                catch(Exception ex)
                {
                    //TODO: Update Exceptions
                    throw new Exception(string.Format("Unable to connect to MIDI Device: {0}", deviceName));
                }
            }
        }

        if (!found)
        {
            //TODO: Update Exceptions
            throw new Exception(string.Format("MIDI Device, {0}, Not Found", deviceName));
        }
	}

    ~MidiInputCtrl()
    {
        midiInput.Close();
    }


    public void Start()
    {
        if (midiInput != null)
        {
            midiInput.StartRecording();
        }
    }

    public void StopAndClose()
    {
        for (int i = 0; i < subscribers.Count; ++i)
        {
            subscribers[i] = null;
        }
        if (midiInput != null)
        {
            midiInput.StopRecording();
            midiInput.Close();
        }
    }



    void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
        ChannelMessage m = e.Message;
        UnityEngine.Debug.Log(string.Format("Message Recieved -  Data1={0} : Data2={1}", m.Data1, m.Data2));
        SendNotifications(m);
    }


    public void SendNotifications(ChannelMessage message)
    {
        foreach (var s in subscribers)
        {
            if (s != null)
            {
                s.Notify(message);
            }
        }
    }


    public void Subscribe(ISubscriber<ChannelMessage> subscriber)
    {
        subscribers.Add(subscriber);
    }


    public void Unsubscribe(ISubscriber<ChannelMessage> subscriber)
    {
        subscribers.Remove(subscriber);
    }
}
