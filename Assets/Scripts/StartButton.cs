using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {

    public PlayerCtrl playerCtrl;
    public GameObject parentCanvas;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickHandler);
    }


    private void OnClickHandler()
    {
        if (string.IsNullOrEmpty(playerCtrl.MidiInputDeviceName))
            throw new System.ArgumentException("No MIDI device provided");

        if (string.IsNullOrEmpty(playerCtrl.MidiScoreResource))
            throw new System.ArgumentException("No MIDI score provided");

        if (string.IsNullOrEmpty(playerCtrl.ParticipantID))
            throw new System.ArgumentException("No participant ID provided");

        playerCtrl.StartVRMin();

        Destroy(parentCanvas);
    }
}