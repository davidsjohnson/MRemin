using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextNoteArrowCtrl : MonoBehaviour, ISubscriber<NoteMessage> {


    public Text nextNoteText;
    public GameObject rotateAroundObj;

    private ProgressBarCtrl progressBar;
    private NoteMessage currentNote;

    void Awake()
    {
    }

    void Start ()
    {
        NoteCtrl.Control.Subscribe(this);
        //currentNote = NoteCtrl.Control.CurrentNote;
        progressBar = transform.Find("ArrowFill").GetComponent<ProgressBarCtrl>();
	}

    public void StartProgressBar(float delay, string message)
    {
        nextNoteText.text = message;
        progressBar.StartProgressBar(delay);
    }

    public void Notify(NoteMessage note)
    {
        currentNote = note;
        if (!note.IsEndMessage)
        {
            StartProgressBar(note.Length, !note.IsLastNote ? Utilities.Midi2NoteStr(note.NextNoteNumber, 24) :  "Done");
        }
    }

    public void StartRotate()
    {
        //change next arrow rotation
        if (currentNote != null)
        {
            float zCurrAngle = transform.localEulerAngles.z;
            float zNextAngle = currentNote.NextNoteNumber < currentNote.NoteNumber || currentNote.IsLastNote ? 180.0f : 0.0f; // Arrow should point away from center (180) if next note is smaller
            float diff = zNextAngle - zCurrAngle;                                                   

            if (diff != 0)
            {
                StopCoroutine("RotateCanvas");
                StartCoroutine("RotateCanvas", diff);
            }
        }
    }

    private IEnumerator RotateCanvas(float rotateAmount)
    {
        float timeToRotate = 0.5f;
        int i = 0;
        float totalRotated = 0;

        int numSteps = (int)(timeToRotate / Time.fixedDeltaTime);
        float amt = rotateAmount / numSteps;
        Debug.Log(string.Format("Steps: {0} | Step Amt: {1} | Total Amt: {2}", numSteps, amt, rotateAmount));

        while(i < numSteps)
        { 
            transform.RotateAround(rotateAroundObj.transform.position, Vector3.up, amt);
            totalRotated += amt;

            Vector3 textAngles = nextNoteText.transform.localEulerAngles;
            textAngles.z += amt;
            nextNoteText.transform.localEulerAngles = textAngles;

            i++;
            yield return new WaitForFixedUpdate();
        }
    }
}
