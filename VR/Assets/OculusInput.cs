using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusInput : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject teleTarget;
    public Camera mainCam;

    private GameObject currentlyAttachedObject;
    public static bool attached;

    bool actionSetChanged = false;
    private SteamVR_ActionSet currentActionSet;

    private SteamVR_Action_Boolean gripClickAction;
    private SteamVR_Action_Single triggerPullAction;
    private SteamVR_Action_Boolean triggerClickAction;
    private SteamVR_Action_Vector2 joyStickAction;
    private SteamVR_Action_Boolean detachAction;

    bool triggerClick;
    bool triggerClickUp;
    bool gripClick;
    float triggerValue;
    Vector2 moveValue;
    bool detach;

    bool pressFlag = true;

    Hover hover;
    SteamVR_Behaviour_Pose pose;
    ParticleSystem slashParticles;
    SteamVR_Input_Sources currentSource;

    void Start()
    {

        if (GameObject.FindGameObjectWithTag("leftHand").Equals(this.gameObject))
        {
            currentSource = SteamVR_Input_Sources.LeftHand;
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("rightHand").Equals(this.gameObject))
            {
                currentSource = SteamVR_Input_Sources.RightHand;
            }
            else
            {
                print("This script is not on right or left hand");
            }
        }

        pose = GetComponent<SteamVR_Behaviour_Pose>();
        hover = GetComponent<Hover>();

        currentActionSet = SteamVR_Input.GetActionSet("MySet");
        gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MySet", "GripClick", false, false);
        triggerPullAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("MySet", "Teleport", false, false);
        joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("MySet", "Move", false, false);

        //Sword specific actions that need to be defined (They won't do anything until we activate the correct action set)
        detachAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "Detach", false, false);
        triggerClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "InitiateSlash", false, false);
    }

    void Update()
    {
        if (actionSetChanged)
        {
            switch (currentActionSet.GetShortName())
            {
                case "MySet":
                    currentActionSet.Activate(currentSource, 0, false);
                    pose.poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("MySet", "Pose", false, false);
                    gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MySet", "GripClick", false, false);
                    triggerPullAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("MySet", "Teleport", false, false);
                    joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("MySet", "Move", false, false);
                    break;

                case "Sword":
                    currentActionSet.Activate(currentSource, 0, false);
                    pose.poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Sword", "Pose", false, false);
                    //gripClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "InitiateSlash", false, false);
                    triggerClickAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Sword", "InitiateSlash", false, false);
                    joyStickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Sword", "Move", false, false);
                    break;

                default:
                    break;
            }

            actionSetChanged = false;
        }

        gripClick = gripClickAction.GetStateDown(currentSource);
        triggerClickUp = triggerClickAction.GetStateUp(currentSource);
        triggerValue = triggerPullAction.GetAxis(currentSource);
        triggerClick = triggerClickAction.GetLastStateDown(currentSource);
        detach = detachAction.GetStateDown(currentSource);
        moveValue = joyStickAction.GetAxis(currentSource);

        //INITIATESLASH CODE ----------------------------------------------------------------------------------------------------------------------------------
        if (triggerClick && attached && currentActionSet.GetShortName().Equals("Sword"))
        {
            slashParticles = currentlyAttachedObject.GetComponent<ParticleSystem>();
            var emiss = slashParticles.emission;
            emiss.rateOverDistance = 100f;
        }
        if (triggerClickUp && attached && currentActionSet.GetShortName().Equals("Sword"))
        {
            slashParticles = currentlyAttachedObject.GetComponent<ParticleSystem>();
            var emiss = slashParticles.emission;
            emiss.rateOverDistance = 0f;
        }

        //OBJECT DETACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        if (attached && detach && hover.closestHoverObj != null)
        {
            //Grab a reference to the current closestHoverObject that is obtained by running Hover() in Hover.cs
            try
            {
                ObjectInteraction detachCall = hover.closestHoverObj.GetComponent<ObjectInteraction>();

                attached = detachCall.DetachObjectFromController();
                currentActionSet.Deactivate(currentSource);
                currentActionSet = SteamVR_Input.GetActionSet("MySet");
                currentlyAttachedObject = null;
                actionSetChanged = true;
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("detachCall Hand is null in OculusInput.cs");
                return;
            }
        }

        //OBJECT ATTACH CODE ---------------------------------------------------------------------------------------------------------------------------------
        if(gripClick && hover.closestHoverObj != null && !attached)
        {
            try
            {
                ObjectInteraction attachCall = hover.closestHoverObj.GetComponent<ObjectInteraction>();

                attached = attachCall.AttachObjectToController(this.gameObject, hover.hoverPoint);
                currentlyAttachedObject = hover.closestHoverObj;
                currentActionSet.Deactivate(currentSource);
                currentActionSet = SteamVR_Input.GetActionSet("Sword"); //CHANGE THIS LATER WHEN WE HAVE MORE THAN ONE ITEM TO PICK UP
                actionSetChanged = true;
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("attachCall Right Hand is null in OculusInput.cs");
                return;
            }
        }

        //TRIGGER PRESS CODE ---------------------------------------------------------------------------------------------------------------------------------
        if(triggerValue > 0.1f)
        {
            int layerMask = (1 << 9);
            if (Physics.Raycast(transform.position, transform.forward - transform.up, out RaycastHit hit, 50f, layerMask))
            {
                teleTarget.transform.position = hit.point;
                Debug.DrawRay(transform.position, (transform.forward - transform.up) * 100f, Color.blue,5f);
            }
        }
        if(triggerValue > 0.75f && pressFlag)//Teleport using raycast
        {
            print("Teleport");
            Debug.DrawRay(transform.position, transform.forward * 100f, Color.red);
            if (Physics.Raycast(transform.position, transform.forward - transform.up, out RaycastHit hit, 100f))
            {
                cameraRig.transform.position = hit.point;

            }
            pressFlag = false;
        }
        if (triggerValue < 0.1f)//Must release or almost release trigger before teleporting again
            pressFlag = true;

        //JOYSTICK MOVEMENT CODE ---------------------------------------------------------------------------------------------------------------------------------
        if (moveValue.y > 0.1f || moveValue.y < -0.1f)
        {//Forward and Backward
            Vector3 controllerMove = new Vector3(((transform.forward - transform.up).normalized).x,
                0f,
                (((transform.forward - transform.up).normalized).z));

            cameraRig.transform.Translate(controllerMove*moveValue.y);
        }
        if (moveValue.x > 0.1f || moveValue.x < -0.1f)
        {//Right and Left
            Vector3 controllerMove = new Vector3(transform.right.x, 0f, transform.right.z);
            cameraRig.transform.Translate(controllerMove * moveValue.x);
        }
    }
}
