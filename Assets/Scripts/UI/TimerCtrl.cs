using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCtrl : MonoBehaviour, ISubscriber<NoteMessage> {

    private Image progressBar;

    // Use this for initialization
    void Start ()
    {
        NoteCtrl.Control.Subscribe(this);
        progressBar = GetComponent<Image>(); 
    }
	

    IEnumerator UpdateProgressBar(float time)
    {
        float i = 0;
        float rate = 1f / time;
        Debug.Log("Starting PRogress Bar");

        while(i < 1)
        {
            i += Time.deltaTime * rate;
            progressBar.fillAmount = i;
            Debug.Log(string.Format("Amount = {0}", i));
            yield return null;
        }
    }

    public void Notify(NoteMessage message)
    {
        StopCoroutine("UpdateProgressBar");
        if (message.NoteNumber != -1)
        {
            StartCoroutine("UpdateProgressBar", message.Length);
        }
        else
        {
            progressBar.fillAmount = 0;
        }
    }
}
