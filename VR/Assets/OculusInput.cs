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
    private GameObject currentlyAttachedObject;

    public GameObject teleTarget;

    bool actionSetChanged = false;
    private SteamVR_ActionSet currentActionSet;

    private SteamVR_Action_Boolean gripClickAction;
    private SteamVR_Action_Single triggerGripAction; 
    private SteamVR_Action_Vector2 joyStickAction;
    private SteamVR_Action_Boolean xDetachAction;
    private SteamVR_Action_Boolean aDetachAction;

    bool gripClick;
    bool gripClickUp;
    float triggerValue;
    Vector2 moveValue;
    bool xDetach;
    bool aDetach;

    bool pressFlag = true;

    Hover leftHover;
    Hover rightHover;

    bool leftAttached = false;
    bool rightAttached = false;

    void Start()
    {
        leftHover = leftController.GetComponent<Hover>();
        rightHover = rightController.GetComponent<Hover>();

        currentActionSet = SteamVR_Input.GetActionSet("MySet");
        gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MySet", "GripClick", false, false);
        triggerGripAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("MySet", "Teleport", false, false);
        joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("MySet", "Move", false, false);

        //Sword specific actions that need to be defined
        xDetachAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "Detach", false, false);
        aDetachAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "Detach", false, false);
    }

    void Update()
    {
        if (actionSetChanged)
        {
            switch (currentActionSet.ToString())
            {
                case "MySet":
                    gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MySet", "GripClick", false, false);
                    triggerGripAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("MySet", "Teleport", false, false);
                    joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("MySet", "Move", false, false);
                    break;

                case "Sword":
                    gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "InitiateSlash", false, false);
                    triggerGripAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Sword", "Teleport", false, false);
                    joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Sword", "Move", false, false);
                    xDetachAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "Detach", false, false);
                    aDetachAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "Detach", false, false);
                    break;

                default:
                    break;
            }

            actionSetChanged = false;
        }

        gripClick = gripClickAction.GetStateDown(SteamVR_Input_Sources.Any);
        gripClickUp = gripClickAction.GetStateUp(SteamVR_Input_Sources.Any);
        triggerValue = triggerGripAction.GetAxis(SteamVR_Input_Sources.Any);
        xDetach = xDetachAction.GetStateDown(SteamVR_Input_Sources.LeftHand);
        aDetach = aDetachAction.GetStateDown(SteamVR_Input_Sources.RightHand);
        moveValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);


        //gripClick = gripClickAction.GetLastStateDown(SteamVR_Input_Sources.Any);
        if (gripClick && (rightAttached || leftAttached) && currentActionSet.ToString() == "Sword"){
            ParticleSystem slashParticles = currentlyAttachedObject.GetComponent<ParticleSystem>();
            var emiss = slashParticles.emission;
            emiss.rateOverDistance = 10f;
        }
        if(gripClickUp && (rightAttached || leftAttached) && currentActionSet.ToString() == "Sword")
        {
            ParticleSystem slashParticles = currentlyAttachedObject.GetComponent<ParticleSystem>();
            var emiss = slashParticles.emission;
            emiss.rateOverDistance = 0f;
        }

        //OBJECT DETACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        //aDetach = aDetachAction.GetStateDown(SteamVR_Input_Sources.RightHand);//Will this work?
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
                currentlyAttachedObject = null;
                actionSetChanged = true;
            }
        }

        //xDetach = xDetachAction.GetStateDown(SteamVR_Input_Sources.LeftHand);//THis might get the state (boolean) of the x button on the LEFT touch controller
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
                currentlyAttachedObject = null;
                actionSetChanged = true;
            }
        }

        //OBJECT ATTACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        //gripClick = gripClickAction.GetStateDown(SteamVR_Input_Sources.Any);
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
                currentlyAttachedObject = rightHover.closestHoverObj;
                actionSetChanged = true;
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
                actionSetChanged = true;
            }
        }

        //TRIGGER PRESS CODE ---------------------------------------------------------------------------------------------------------------------------------
        //triggerValue = triggerGripAction.GetAxis(SteamVR_Input_Sources.Any);
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
        //moveValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);
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
