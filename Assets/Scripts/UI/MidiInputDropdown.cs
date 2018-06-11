using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidiInputDropdown : MonoBehaviour {

    private Dropdown dropdown;

    void Start()
    {
        // Setup dropdown menu options and change handler
        dropdown = GetComponent<Dropdown>();
        dropdown.AddOptions(MidiInputCtrl.AvailableMidiDevices());
        dropdown.onValueChanged.AddListener(delegate { OnValueChangedHandler(dropdown); });
    }

    private void OnValueChangedHandler(Dropdown change)
    {
        if (change.value != 0)
            PlayerCtrl.Control.MidiInputDeviceName = change.captionText.text;
    }
}
