using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectInteraction : MonoBehaviour
{
    //THIS CLASS IS REQUIRED FOR AN ITEM TO BE PICKED UP

    //private SteamVR_ActionSet swordSet = SteamVR_Input.GetActionSet("Sword");
    //private SteamVR_ActionSet mySet = SteamVR_Input.GetActionSet("MySet");
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool AttachObjectToController(GameObject controller, Vector3 AttachPoint)
    {
        //This function will put the object on it's attach point, set the parent, and disable the controller model that is on the hand by default
        //Also enable the sword action set.
        transform.SetParent(controller.transform, true);
        rb.useGravity = false;
        rb.isKinematic = true;

        //AttachPoint is just equal to the hoverPoint at the moment.
        this.transform.position = AttachPoint;
        this.transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,-90f));

        //swordSet.Activate(SteamVR_Input_Sources.Any, 0, true);

        return true;
    }

    public bool DetachObjectFromController()
    {
        //mySet.Activate(SteamVR_Input_Sources.Any, 0, true);

        //This function will detach this object from whatever parent it currently has, (controller) and reapply gravity.
        this.transform.SetParent(null, true);

        rb.useGravity = true;
        rb.isKinematic = false;
        return false;//This false will indicate to the Input script that the controller is no longer attached to an object
    }
}
