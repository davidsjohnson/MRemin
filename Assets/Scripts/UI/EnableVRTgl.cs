using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableVRTgl : MonoBehaviour {

    private void Awake()
    {
        Toggle tgl = GetComponent<Toggle>();
        tgl.onValueChanged.AddListener(delegate { OnValueChangedHandler(tgl); });
        PlayerCtrl.Control.UseVRmin = tgl.isOn;   // set to toggles default value
    }

    public void OnValueChangedHandler(Toggle change)
    {
        PlayerCtrl.Control.UseVRmin = change.isOn;
    }
}
