using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //public LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //lineRenderer.SetPositions(new[] { transform.position, transform.position + transform.forward });
        if (UI.IsShow) return;
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote))
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out raycastHit))
            {
                if (raycastHit.collider.CompareTag("Book"))
                {
                    raycastHit.collider.transform.parent.parent.GetComponent<FloatBook>().ShowIntroduction();
                }
            }

        }
    }
}
