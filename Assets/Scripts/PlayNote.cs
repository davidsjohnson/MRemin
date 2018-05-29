using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityOSC;

public class PlayNote : MonoBehaviour, ISubscriber<int> {

    // Public members

    // Max and min Y values for the note ring
    public float minSize;
    public float maxSize;

    // Max and min midi note values that the Theremin will produce (can be configured on the Theremini)
    public int minNote = 36;
    public int maxNote = 71;

    // Number of steps for note size transition animation
    public int changeSteps = 20;

    // Material to use when the hand is in the correct locatoin
    public Material playedMaterial;

    // Private Members
    private float minFreq;
    private float maxFreq;

    private Material orgMaterial;

	private float changeSize = 0;
    private int currentStep = 0;

    /*
     * Required method for ISubscriber Interface to handle note change updates
     */
    public void Notify(int midiNote)
    {
        //NextNote(midiNote);
    }


    void Awake()
    {
        // Subscribe to Note Ctrl notifications on new notes
        NoteCtrl.GetInstance().Subscribe(this);

        // Store original material so we can swap between materials depending on play state
		orgMaterial = transform.parent.gameObject.GetComponent<Renderer> ().material;

        // Calculate min and max frequencies of the Theremin (based on max and min midi notes)
        minFreq = Utilities.Midi2Freq(minNote);
        maxFreq = Utilities.Midi2Freq(maxNote);

        NextNote(63);
    }

	void OnTriggerEnter(Collider other)
    {
        // if a hand enters target update color to indicating hand is in correct location
		if (other.tag == "Hand"){
			transform.parent.gameObject.GetComponent<Renderer>().material = playedMaterial;
		}
	}

	void OnTriggerExit(Collider other)
    {
        // if a hand leaves target update color to indicating hand is not in the correct location
        if (other.tag == "Hand"){
			transform.parent.gameObject.GetComponent<Renderer>().material = orgMaterial;
		}	
	}

	void Update()
    {
        // incrementally update the size of the Note ring if needed to provide a smooth 
        // transition to the next note
		if (currentStep < changeSteps && changeSize != 0) {
			Vector3 newScale = transform.parent.transform.localScale;
			newScale.x += changeSize;
			newScale.z += changeSize;
			currentStep++;
			transform.parent.transform.localScale = newScale;
		}
	}
		
    /*
     * Start the transition of the ring to the next note 
     */
	void NextNote(int midiNote)
    {
        //change note and note size
        transform.parent.gameObject.GetComponent<Renderer>().material = orgMaterial;    // reset to original material 
        currentStep = 0;                                                                // Reset step since we're starting size change animation 
		Vector3 newScale = CalculateNoteScale (midiNote);
		float scaleDiff = newScale.x - transform.parent.transform.localScale.x;         // Find amount ring needs to resize
		changeSize = scaleDiff / changeSteps;                                           // amount ring should change at each update until complete
	}

    /*
     * Calculate the correct scale for a given midi Note
     */ 
    Vector3 CalculateNoteScale(int midiNote)
    {
        float freq = Utilities.Midi2Freq(midiNote);
        //float newXZ = Utilities.MapValue(freq, maxFreq, minFreq, minSize, maxSize);
        float newXZ = Utilities.MapValue(midiNote, maxNote, minNote, minSize, maxSize);
        return new Vector3(newXZ, 0.1f, newXZ);
    }
}