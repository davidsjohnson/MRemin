using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class PlayNote : MonoBehaviour, ISubscriber<NoteMessage> {

    
    public float minSize;               // Max and min Y values for the note ring
    public float maxSize;
    public int changeSteps = 20;        // Number of steps for note size transition animation
    public float resizeTime = 0.5f;
    public Material playedMaterial;     // Material to use when the hand is in the correct location
    public NextNoteArrowCtrl nextNoteArrow;    // Note Arrow to move when notes change

    // Private Members
    private float minFreq;
    private float maxFreq;

    private Material orgMaterial;

    /*
     * Required method for ISubscriber Interface to handle note change updates
     */
    public void Notify(NoteMessage midiNote)
    {
        if (!midiNote.IsStartMessage)
        {
            StopCoroutine("ResizeRing");
            StartCoroutine("ResizeRing", midiNote);
        }
        
    }

    public void Notify(string midiNote)
    {
        int temp = 0;
        int.TryParse(midiNote, out temp);
        Notify(new NoteMessage(temp));
    }

    private void OnDisable()
    {
        NoteCtrl.Control.Unsubscribe(this);
    }

    void Awake()
    {
        // Subscribe to Note Ctrl notifications on new notes
        NoteCtrl.Control.Subscribe(this);

        // Store original material so we can swap between materials depending on play state
		orgMaterial = transform.parent.gameObject.GetComponent<Renderer> ().material;

        // Calculate min and max frequencies of the Theremin (based on max and min midi notes)
        minFreq = Utilities.Midi2Freq(PlayerCtrl.Control.minMidiNote);
        maxFreq = Utilities.Midi2Freq(PlayerCtrl.Control.maxMidiNote);
    }

	void OnTriggerEnter(Collider other)
    {
        // if a hand enters target update color to indicating hand is in correct location
		if (other.tag == "Hand")
        {
			transform.parent.gameObject.GetComponent<Renderer>().material = playedMaterial;
		}
	}

    void OnTriggerExit(Collider other)
    {
        // if a hand leaves target update color to indicating hand is not in the correct location
        if (other.tag == "Hand")
        {
            transform.parent.gameObject.GetComponent<Renderer>().material = orgMaterial;
        }
    }

    /*
     * Calculate the correct scale for a given midi Note
     */ 
    Vector3 CalculateNoteScale(int midiNote)
    {
        float freq = Utilities.Midi2Freq(midiNote);
        float newXZ = Utilities.MapValue(freq, maxFreq, minFreq, minSize, maxSize);
        //float newXZ = Utilities.MapValue(midiNote, maxNote, minNote, minSize, maxSize);
        return new Vector3(newXZ, 0.1f, newXZ);
    }

    private IEnumerator ResizeRing(NoteMessage message)
    {
        int midiNote = message.NoteNumber;

        if (message.IsEndMessage)
        {
            PlayerCtrl.Control.MidiComplete();
            yield break;
        }
            
        //change note and note size
        transform.parent.gameObject.GetComponent<Renderer>().material = orgMaterial;    // reset to original material 

        Vector3 newScale = CalculateNoteScale(midiNote);
        float scaleDiff = newScale.x - transform.parent.transform.localScale.x;         // Find amount ring needs to resize
        float startPos = transform.parent.transform.localScale.x;

        float i = 0;
        float resizeRate = 1f / resizeTime;
        while (i < 1)
        {
            i += Time.fixedDeltaTime * resizeRate;

            Vector3 nextScale = transform.parent.transform.localScale;
            nextScale.x = startPos + (i * scaleDiff);
            nextScale.z = startPos + (i * scaleDiff);

            // Detach from parent so collider doesn't scale with NoteRing
            Transform parent = transform.parent;
            Vector3 localPos = transform.localPosition;
            transform.parent = null;
            parent.transform.localScale = nextScale;
            transform.parent = parent;
            transform.localPosition = localPos;

            yield return new WaitForFixedUpdate();
        }
        nextNoteArrow.StartRotate();
    }
}