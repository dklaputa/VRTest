using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    // Input module 
    private static bool isShow;
    public static bool IsShow { get { return isShow; } }

    private OVRInputModule inputModule;
    public OVRManager manager { get; private set; }
    public OVRPlayerController playerController { get; private set; }
    public Transform centerEyeTransform { get; private set; }
    private EventSystem eventSystemPrefab;
    // Current max possible scale for Inspector without intersecting world geometry
    private float maxInspectorScale = 1;
    public float eyeToGUIDistance = 1.5f;

    private bool isUITwoShown;
    private bool isWaitForAnime;
    private bool isReady;

    //public GameObject docsPanel;
    //OVRGazeScroller docsScroller;
    //Text docsPanelText;
    //string docText = "";

    static public OVRCameraRig cameraRig
    {
        get
        {
            return GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        }
    } 


    public void Show(){
        if (isShow) return;
        canvas.SetActive(true);
        //Reposition(false, 0);
        isShow = true;
        StartCoroutine(Ready());
    }

    IEnumerator Ready(){
        yield return new WaitForSeconds(1);
        isReady = true;
    }
    public GameObject canvas;

	// Use this for initialization
	void Awake () { 
        // Setup canvas and canvas panel builders  
        eventSystemPrefab = (EventSystem)Resources.Load("Prefabs/EventSystem", typeof(EventSystem));
        AssignCameraRig();
 
        //docsPanelText = docsPanel.GetComponentInChildren<Text>();
        //docsPanelText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        //docsScroller = docsPanel.GetComponentInChildren<OVRGazeScroller>();

        //SetDocText("Test Test");
	}

    // Update is called once per frame
    void Update()
    {
        if (canvas.activeSelf == false || !isReady) return;
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        Transform activeTransform = cameraRig.centerEyeAnchor;

        if ((activeController == OVRInput.Controller.LTouch) || (activeController == OVRInput.Controller.LTrackedRemote))
            activeTransform = cameraRig.leftHandAnchor;

        if ((activeController == OVRInput.Controller.RTouch) || (activeController == OVRInput.Controller.RTrackedRemote))
            activeTransform = cameraRig.rightHandAnchor;

        if (activeController == OVRInput.Controller.Touch)
            activeTransform = cameraRig.rightHandAnchor;

        OVRGazePointer.instance.rayTransform = activeTransform;
        inputModule.rayTransform = activeTransform;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) && !isUITwoShown)
        {
            Debug.Log("Pressed");
            if (isWaitForAnime) return;
            transform.GetChild(0).GetComponent<Animator>().SetBool("ToTwo", true);
            StartCoroutine(WaitForAnimation(true));
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTrackedRemote))
        {
            if (isWaitForAnime) return;
            if (isUITwoShown)
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("ToTwo", false);
                StartCoroutine(WaitForAnimation(false));
            }
            else
            {
                transform.GetChild(0).GetComponent<Animator>().SetTrigger("Close");
                StartCoroutine(CloseUI());
            }
        }
    }

    IEnumerator CloseUI(){
        isWaitForAnime = true;
        yield return new WaitForSeconds(0.5f);
        canvas.SetActive(false);
        isShow = false;
        isReady = false;
        isWaitForAnime = false;
    }

    IEnumerator WaitForAnimation(bool UITwoShown){
        isWaitForAnime = true;
        yield return new WaitForSeconds(1);
        isUITwoShown = UITwoShown;
        isWaitForAnime = false;
    }

    public void AssignCameraRig()
    {
        if (cameraRig)
        {
            Transform t = cameraRig.transform.Find("TrackingSpace");
            centerEyeTransform = t.Find("CenterEyeAnchor");
        }

        manager = FindObjectOfType<OVRManager>();
        // There has to be an event system for the GUI to work
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.Log("Creating EventSystem");
            eventSystem = (EventSystem)GameObject.Instantiate(eventSystemPrefab);

        }
        else
        {
            //and an OVRInputModule
            if (eventSystem.GetComponent<OVRInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<OVRInputModule>();
            }
        }
        inputModule = eventSystem.GetComponent<OVRInputModule>();

        cameraRig.EnsureGameObjectIntegrity();
        canvas.GetComponent<Canvas>().worldCamera = cameraRig.leftEyeCamera;
    } 

    //public void SetDocText(string text)
    //{
    //    if (text == null || text == "")
    //        return;
 
    //    docText = text;

    //    UpdateDocsVisibility();
    //    docsScroller.GotoTop();

    //}

    //void UpdateDocsVisibility()
    //{
    //    if (docsPanel != null)
    //    {
    //        bool active = docText.Length > 0;
    //        docsPanel.SetActive(active);
    //        docsScroller.MarkContentChanged();
    //        if (active)
    //        {
    //            docsPanelText.text = docText;
    //        }

    //    }
    //}

    //public void Reposition(bool toOrigin = false, float rotationAroundPlayer = 0)
    //{
    //    if (centerEyeTransform)
    //    {
    //        if (toOrigin)
    //        {
    //            transform.position = cameraRig.transform.position + cameraRig.transform.forward * eyeToGUIDistance * maxInspectorScale * centerEyeTransform.transform.lossyScale.x;
    //            transform.localScale = new Vector3(maxInspectorScale, maxInspectorScale, maxInspectorScale) * centerEyeTransform.transform.lossyScale.x;
    //            transform.rotation = Quaternion.LookRotation(cameraRig.transform.TransformVector(Vector3.forward), cameraRig.transform.TransformVector(Vector3.up));
    //        }
    //        else
    //        {
    //            //Orientate facing the player but keep upright
    //            Vector3 forward = centerEyeTransform.forward;
    //            forward = Quaternion.Euler(new Vector3(0, rotationAroundPlayer, 0)) * forward;
    //            //remove any up/down component relative to cameraRig
    //            forward -= (Vector3.Dot(forward, cameraRig.transform.up) * cameraRig.transform.up);

    //            if (forward.sqrMagnitude == 0)
    //                forward = Vector3.forward;
    //            forward.Normalize();
    //            transform.position = centerEyeTransform.position + forward * eyeToGUIDistance * maxInspectorScale * centerEyeTransform.transform.lossyScale.x;
    //            transform.localScale = new Vector3(maxInspectorScale, maxInspectorScale, maxInspectorScale) * centerEyeTransform.transform.lossyScale.x;
    //            transform.rotation = Quaternion.LookRotation(forward, cameraRig.transform.TransformVector(Vector3.up));
    //        }
    //    }
    //}
}
