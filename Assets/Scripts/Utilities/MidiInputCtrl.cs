using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia;


public class MidiInputCtrl : IPublisher<ChannelMessage>
{

    private InputDevice midiInput = null;
    private List<ISubscriber<ChannelMessage>> subscribers = new List<ISubscriber<ChannelMessage>>();

	public MidiInputCtrl() {}

    public void Connect(string deviceName)
    {
        int midiIndex = MidiInputCtrl.AvailableMidiDevices().IndexOf(deviceName);
        UnityEngine.Debug.Log("Connecting to " + deviceName);
        if (midiIndex == -1)
            throw new System.ArgumentException("Invalid Midi Input Device Name");

        midiInput = new InputDevice(midiIndex);
        midiInput.ChannelMessageReceived += HandleChannelMessageReceived;
    }

    ~MidiInputCtrl()
    {
        midiInput.Close();
    }

    public void Start()
    {
        if (midiInput != null)
            midiInput.StartRecording();
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
        SendNotifications(m);
    }


    public void SendNotifications(ChannelMessage message)
    {
        foreach (var s in subscribers)
        {
            if (s != null)
                s.Notify(message);
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


    // Static Methods

    private static List<string> deviceNames;

    public static List<string> AvailableMidiDevices()
    {
        if (deviceNames == null)
        {
            deviceNames = new List<string>();
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                MidiInCaps deviceCaps = InputDevice.GetDeviceCapabilities(i);
                deviceNames.Add(deviceCaps.name);
                UnityEngine.Debug.Log(string.Format("Midi Device {0}: {1}", i, deviceCaps.name));
            }
        }
        return deviceNames;
    }
}
