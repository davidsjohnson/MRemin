using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartcipantIDInput : MonoBehaviour {

    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(delegate { OnEndEditHandler(inputField); });
        PlayerCtrl.Control.ParticipantID = null;
    }


    private void OnEndEditHandler(InputField change)
    {
        PlayerCtrl.Control.ParticipantID = change.textComponent.text;
    }
 
}
