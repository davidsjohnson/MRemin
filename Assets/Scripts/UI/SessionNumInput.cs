using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionNumInput : MonoBehaviour {

    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(delegate { OnEndEditHandler(inputField); });
        PlayerCtrl.Control.SessionNum = null;
    }


    private void OnEndEditHandler(InputField change)
    {
        PlayerCtrl.Control.SessionNum = change.textComponent.text;
    }
}
