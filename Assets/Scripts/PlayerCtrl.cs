using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public enum SceneType
{
    NoVis = 0,
    TwoD = 1,
    VR = 2
}

public enum Handedness
{
    Right = 0,
    Left = 1
}

public class PlayerCtrl : MonoBehaviour
{        
    // Leaving for now. TODO: remove this at some point?
    public bool noteCtrlOn = true;

    public int minMidiNote = 36;                                // Note Range for Theremini (Configurable in Theremini settings)
    public int maxMidiNote = 72;

    public int pitchMidiChannel = 20;
    public int volMidiChannel = 2;

    public float tempo = 45;

    public int startDelay;                                      // How long to wait before starting system

    // VR settings and Training Scenes
    public string VRDeviceName = "WindowsMR";
    public string sceneVR;
    public string scene2D;
    public string sceneNoVis;

    public GameObject completedMenuPrefab;                      // Menu Prefab to instantiate when a score is completed

    // Start Menu Input Items
    public string ParticipantID { get; set; }                   // Participant ID
    public string SessionNum { get; set; }                      // Session Number - to help keep track of log files
    public string MidiScoreResource { get; set; }               // Name of file containing Midi data

    private string midiInputDeviceName;                          // Name of Midi Device to connect to
    public string MidiInputDeviceName                            // Connect to Midi Device as soon as value set (close previous device if there is one)
    {
        get { return midiInputDeviceName; }
        set
        {
            midiInputDeviceName = value;
            MidiIn.StopAndClose(false);
            MidiIn.Connect(midiInputDeviceName);
            MidiIn.Start();
        }
    }

    private SceneType sceneType = SceneType.VR;
    public SceneType SceneType
    {
        get { return sceneType; }
        set { sceneType = value; }
    }

    public Handedness handed = Handedness.Right;
    public Handedness Handed
    {
        get { return handed; }
        set
        {
            handed = value;
            SetInterface();
        }
    }

    private GameObject thereminLH;
    private GameObject thereminRH;

    // VRMin Components
    public static PlayerCtrl Control { get; private set; }      // Singleton Accessor
    public LogWriter Logger { get; private set; }               // Logger
    public MidiInputCtrl MidiIn { get; private set; }           // Main Midi Input Controller used to position left and right hands

    private GameObject completedMenu;                           // The instantiated completed menu game object

    void Awake ()
    {
        //Implement Psuedo-Singleton
        if (Control == null)
        {
            DontDestroyOnLoad(gameObject);
            Control = this;
        }
        else if (Control != this)
        {
            Destroy(gameObject);
        }

        //Initialize Midi In (so objects can subscribe to it upon load)
        MidiIn = new MidiInputCtrl();

        //Initialize Logger
        Logger = new LogWriter();
    }

    private void Start()
    {
        // initializes and sets the correct thermin interface for the handedness
        initInterfaces();
    }

    private void initInterfaces()
    {
        // Find LH and RH Theremins and set LH to inactive
        thereminRH = GameObject.Find("Interface-RH");
        if (!thereminRH) { Debug.Log("No RH Interface Found"); }
        thereminLH = GameObject.Find("Interface-LH");
        if (!thereminLH) { Debug.Log("No LH Interface Found"); }

        SetInterface();
    }

    public void SetInterface()
    {
        if (Handed == Handedness.Right)
        {
            thereminLH.SetActive(false);
            thereminRH.SetActive(true);
        }
        else
        {
            thereminLH.SetActive(true);
            thereminRH.SetActive(false);
        }
    }

    public void StartVRMin()
    {
        switch (sceneType)
        {
            case SceneType.NoVis:
                StartCoroutine(SwitchScene(sceneNoVis, ""));
                break;
            case SceneType.TwoD:
                StartCoroutine(SwitchScene(scene2D, ""));
                break;
            case SceneType.VR:
                StartCoroutine(SwitchScene(sceneVR, VRDeviceName));
                break;
        }
    }

    private void StartSession()
    {
        Logger.Start(string.Format("p{0}-session{1}-score{2}-scene{3}", ParticipantID, SessionNum, Path.GetFileNameWithoutExtension(MidiScoreResource), SceneType));      // Start Up the Logger
        StartCoroutine(DelayedStart(startDelay));   // Start Notes on a Delay
    }

    private IEnumerator SwitchScene(string sceneName, string deviceName)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        bool enableVR = !string.IsNullOrEmpty(deviceName);

        // Only change scenes if we're not already in that scene
        if (activeScene.name != sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (sceneName == scene2D || sceneName == sceneVR)
            {
                initInterfaces();
                SetInterface();
                yield return null;
            }

            if ((enableVR && !XRSettings.enabled) || (!enableVR && XRSettings.enabled))
            {
                XRSettings.LoadDeviceByName(deviceName);
                yield return null;
                XRSettings.enabled = enableVR;
                yield return null;
            }

            // We should only reset camera positions if using VR (Non VR have specific camera locations)
            if (XRSettings.enabled)
            {
                ResetCameras();
                yield return null;
            }


        }


        StartSession();
    }

    private void ResetCameras()
    {
        foreach (var cam in Camera.allCameras)
        {
            if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {
                cam.transform.localPosition = Vector3.zero;
                cam.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private IEnumerator DelayedStart(int delay)
    {
        NoteCtrl.Control.MidiScoreFile = MidiScoreResource;     // Set Score before delay to trigger start message
        yield return new WaitForSecondsRealtime(delay);   // add an extra 5 seconds for VR scene to load

        Logger.Log("StartSession\t{0}", Path.GetFileNameWithoutExtension(MidiScoreResource));
        // Start Playing notes
        NoteCtrl.Control.PlayMidi(NoteCtrl.MidiStatus.Play);
    }

    public void MidiComplete()
    {   
        //instatiate completed menu and set player controller to this
        completedMenu = Instantiate(completedMenuPrefab);
        Logger.Stop();
    }

    void OnDisable()
    {
        // Clean up when controller is destroyed
        if(MidiIn != null)
            MidiIn.StopAndClose();
        if (Logger != null)
            Logger.Stop();
    }
}
