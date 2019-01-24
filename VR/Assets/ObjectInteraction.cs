using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectInteraction : MonoBehaviour
{
    //THIS CLASS IS REQUIRED FOR AN ITEM TO BE PICKED UP

     private SteamVR_ActionSet[] actionSet = SteamVR_Input.actionSets;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool AttachObjectToController(GameObject controller, Vector3 AttachPoint)
    {
        //This function will put the object on it's attach point, set the parent, and disable the controller model that is on the hand by default
        rb.useGravity = false;
        rb.isKinematic = true;

        this.transform.position = AttachPoint;
        transform.SetParent(controller.transform);
        return true;
    }

    public bool DetachObjectFromController()
    {
        this.transform.SetParent(null);

        rb.useGravity = true;
        rb.isKinematic = false;
        return false;//This false will indicate to the Input script that the controller is no longer attached to an object
    }
}
