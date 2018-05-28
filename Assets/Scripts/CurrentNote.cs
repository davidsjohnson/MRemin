using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentNote : MonoBehaviour, ISubscriber<int> {

    public GameObject rightHand;

    private Text noteTxt;
    private int currentNote;

    public void Notify(int midiNote)
    {
        //Update the Note string whenever a new midi note is received
        
        currentNote  = midiNote;

    }

    // Use this for initialization
    void Awake ()
    {
        NoteCtrl.GetInstance().Subscribe(this);
        noteTxt = GetComponent<Text>();
        noteTxt.text = "";
	}

    void Update()
    {
        float x = rightHand.transform.localPosition.x;
        string message = Utilities.Midi2NoteStr(currentNote);
        noteTxt.text = string.Format("{0}\nx Pos:{1}", message, x);
    }
}
