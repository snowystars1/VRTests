using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusInput : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject leftController;
    public GameObject rightController;
    public Camera mainCam;

    public GameObject teleTarget;

    public SteamVR_Action_Boolean gripClickAction;
    public SteamVR_Action_Single triggerGripAction; 
    public SteamVR_Action_Vector2 joyStickAction;
    public SteamVR_Action_Boolean xDetachAction;
    public SteamVR_Action_Boolean aDetachAction;

    bool gripClick;
    float triggerValue;
    Vector2 moveValue;
    bool xDetach;
    bool aDetach;

    bool pressFlag = true;

    Hover leftHover;
    Hover rightHover;

    bool leftAttached = false;
    bool rightAttached = false;

    private void Start()
    {
        leftHover = leftController.GetComponent<Hover>();
        rightHover = rightController.GetComponent<Hover>();
    }

    void Update()
    {
        //if (SteamVR_Input.GetStateDown("MySet", "InteractUI", SteamVR_Input_Sources.Any, false))
        //{
        //    print("InteractUI");
        //}

        //gripValue = grabGripAction.GetAxis(SteamVR_Input_Sources.Any);
        //if (gripValue > 0.2f)
        //{
        //    print(gripValue);
        //}

        //We WILL need different input code for each action set. Oof

        //OBJECT DETACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        aDetach = aDetachAction.GetLastStateDown(SteamVR_Input_Sources.RightHand);//Will this work?
        if (rightAttached && aDetach && rightHover.closestHoverObj != null)
        {
            //Grab a reference to the current closestHoverObject that is obtained by running Hover() in Hover.cs
            ObjectInteraction detachCall = rightHover.closestHoverObj.GetComponent<ObjectInteraction>();
            if (detachCall == null)//This is technically impossible, but, just for safety
            {
                Debug.LogWarning("detachCall Right Hand is null in OculusInput.cs");
                return;
            }
            else
            {
                rightAttached = detachCall.DetachObjectFromController();
            }
        }

        xDetach = xDetachAction.GetLastStateDown(SteamVR_Input_Sources.LeftHand);//THis might get the state (boolean) of the x button on the LEFT touch controller
        if(leftAttached && xDetach && leftHover.closestHoverObj != null)
        {
            ObjectInteraction detachCall = leftHover.closestHoverObj.GetComponent<ObjectInteraction>();
            if (detachCall == null)
            {
                Debug.LogWarning("detachCall Left Hand is null in OculusInput.cs");
            }
            else
            {
                leftAttached = detachCall.DetachObjectFromController();
            }
        }

        //OBJECT ATTACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        gripClick = gripClickAction.GetStateDown(SteamVR_Input_Sources.Any);
        if(gripClick && rightHover.closestHoverObj != null && !rightAttached)
        {
            ObjectInteraction attachCall = rightHover.closestHoverObj.GetComponent<ObjectInteraction>();
            if (attachCall == null)
            {
                Debug.LogWarning("attachCall Right Hand is null in OculusInput.cs");
            }
            else
            {
                rightAttached = attachCall.AttachObjectToController(rightController, rightHover.hoverPoint);
            }
        }
        if (gripClick && leftHover.closestHoverObj != null && !leftAttached)
        {
            ObjectInteraction attachCall = leftHover.closestHoverObj.GetComponent<ObjectInteraction>();
            if (attachCall == null)
            {
                Debug.LogWarning("attachCall Left Hand is null in OculusInput.cs");
            }
            else
            {
                leftAttached = attachCall.AttachObjectToController(leftController, leftHover.hoverPoint);
            }
        }

        //TRIGGER PRESS CODE ---------------------------------------------------------------------------------------------------------------------------------
        triggerValue = triggerGripAction.GetAxis(SteamVR_Input_Sources.Any);
        if(triggerValue > 0.1f)
        {
            int layerMask = (1 << 9);
            if (Physics.Raycast(rightController.transform.position, rightController.transform.forward - rightController.transform.up, out RaycastHit hit, 50f, layerMask))
            {
                teleTarget.transform.position = hit.point;
                Debug.DrawRay(rightController.transform.position, (rightController.transform.forward - rightController.transform.up) * 100f, Color.blue,5f);
            }
        }
        if(triggerValue > 0.75f && pressFlag)//Teleport using raycast
        {
            print("Teleport");
            Debug.DrawRay(rightController.transform.position, rightController.transform.forward * 100f, Color.red);
            if (Physics.Raycast(rightController.transform.position, rightController.transform.forward - rightController.transform.up, out RaycastHit hit, 100f))
            {
                cameraRig.transform.position = hit.point;

            }
            pressFlag = false;
        }
        if (triggerValue < 0.1f)//Must release or almost release trigger before teleporting again
            pressFlag = true;

        //JOYSTICK MOVEMENT CODE ---------------------------------------------------------------------------------------------------------------------------------
        moveValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);
        if (moveValue.y > 0.1f || moveValue.y < -0.1f)
        {//Forward and Backward
            Vector3 controllerMove = new Vector3(((rightController.transform.forward - rightController.transform.up).normalized).x,
                0f,
                (((rightController.transform.forward - rightController.transform.up).normalized).z));

            cameraRig.transform.Translate(controllerMove*moveValue.y);
        }
        if (moveValue.x > 0.1f || moveValue.x < -0.1f)
        {//Right and Left
            Vector3 controllerMove = new Vector3(rightController.transform.right.x, 0f, rightController.transform.right.z);
            cameraRig.transform.Translate(controllerMove * moveValue.x);
        }
    }
}
