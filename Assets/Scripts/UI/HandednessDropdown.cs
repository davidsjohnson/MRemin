using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandednessDropdown : MonoBehaviour
{


    private void Start()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { OnValueChangedHandler(dropdown); });
    }

    private void OnValueChangedHandler(Dropdown changed)
    {
        Debug.Log("here: " + (Handedness)changed.value);
        PlayerCtrl.Control.Handed = (Handedness)changed.value;
    }
}
