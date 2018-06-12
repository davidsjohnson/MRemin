using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCtrl : MonoBehaviour {

    private Image progressBar;
    public Text text;

    // Use this for initialization
    void Start ()
    {
        progressBar = GetComponent<Image>(); 
    }
	

    /*
     * Used to display starting message before Midi Score kicks off
     */
    public void StartProgressBar(float delay, string message="")
    {
        StopCoroutine("UpdateProgressBar");
        text.text = message;
        StartCoroutine("UpdateProgressBar", delay);
    }

    IEnumerator UpdateProgressBar(float time)
    {
        float i = 0;
        float rate = 1f / time;

        while(i < 1)
        {
            i += Time.deltaTime * rate;
            progressBar.fillAmount = i;
            yield return null;
        }
        progressBar.fillAmount = 0;
    }
}
