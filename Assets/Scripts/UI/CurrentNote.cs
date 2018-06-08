using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentNote : MonoBehaviour, ISubscriber<NoteMessage> {

    public GameObject nextNoteObj;

    public Vector3 nextNoteStart;
    public Vector3 nextNoteStop;

    private float nextScaleMin = .1f;
    private float nextScaleMax = .45f;

    private Text noteTxt;
    private Text nextNoteTxt;
    private NoteMessage currentNote;

    private IEnumerator moveNextNote;

    private float distPerFrame;
    private float distCovered;
    private int numFrames;
    private Vector3 scale;
    private float scaleInc;
    private bool started = false;

    // Use this for initialization
    void Start()
    {
        NoteCtrl.Control.Subscribe(this);

        // Get Text Components for updating
        noteTxt = GetComponent<Text>();
        noteTxt.text = "";

        nextNoteTxt = nextNoteObj.GetComponent<Text>();
        nextNoteTxt.text = "";
    }

    public void Notify(NoteMessage midiNote)
    {
        //Update the Note string whenever a new midi note is received
        currentNote  = midiNote;
        updateNotes();
    }

    // Temp function for manually changing note from UI
    public void Notify(string midiNote)
    {
        int midiNumber;
        int.TryParse(midiNote, out midiNumber);
        currentNote = new NoteMessage(midiNumber);
    }

    private void updateNotes()
    {
        noteTxt.text = string.Format("{0}", Utilities.Midi2NoteStr(currentNote.NoteNumber));

        int nextNote = currentNote.NextNoteNumber;
        nextNoteTxt.text = nextNote != -1 ? string.Format("{0}", Utilities.Midi2NoteStr(nextNote)) : string.Format("Fin!");
        StartMove();
    }


    private void StartMove()
    {
        distCovered = 0;
        numFrames = (int)(currentNote.Length / Time.fixedDeltaTime);
        distPerFrame = distPerFrame = Vector3.Distance(nextNoteStart, nextNoteStop) / numFrames;
        started = true;

        scale = new Vector3(.1f, .1f, .1f);
        scaleInc = (.45f - .1f) / numFrames; 
    }

    private void FixedUpdate()
    {
        if (started){
            distCovered += distPerFrame;
            RectTransform rect = nextNoteObj.GetComponent<RectTransform>();
            rect.localPosition = Vector3.Lerp(nextNoteStart, nextNoteStop, distCovered / (numFrames * distPerFrame));

            scale.x += scaleInc;
            scale.y += scaleInc;
            scale.z += scaleInc;

            rect.localScale = scale;
        }
    }
}
