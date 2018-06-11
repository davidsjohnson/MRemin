using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour {

    public GameObject parentCanvas;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickHandler);
    }


    private void OnClickHandler()
    {
        if (string.IsNullOrEmpty(PlayerCtrl.Control.MidiScoreResource))
            throw new System.ArgumentException("No MIDI score provided");

        PlayerCtrl.Control.StartNewScore();

        Destroy(parentCanvas);
    }
}
