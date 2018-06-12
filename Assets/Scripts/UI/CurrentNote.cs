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
        noteTxt.text = currentNote.NoteNumber != -1 ? string.Format("{0}", Utilities.Midi2NoteStr(currentNote.NoteNumber)) 
                                                    : string.Format("Session Complete");
        nextNoteTxt.text = currentNote.NoteNumber != -1 && currentNote.NextNoteNumber != -1  ? string.Format("next: {0}", Utilities.Midi2NoteStr(currentNote.NextNoteNumber, 24)) 
                                                                                             : string.Format("");
    }
}
