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

    public SteamVR_Action_Single grabGripAction;
    public SteamVR_Action_Single triggerGripAction; 
    public SteamVR_Action_Vector2 joyStickAction;
    float gripValue;
    float triggerValue;
    Vector2 moveValue;

    bool pressFlag = true;
    // Update is called once per frame
    void Update()
    {
        //if (SteamVR_Input.GetStateDown("MySet", "InteractUI", SteamVR_Input_Sources.Any, false))
        //{
        //    print("InteractUI");
        //}

        gripValue = grabGripAction.GetAxis(SteamVR_Input_Sources.Any);
        if (gripValue > 0.2f)
        {
            print(gripValue);
        }


        //print(triggerValue);
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


        moveValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);
        if (moveValue.y > 0.1f || moveValue.y < -0.1f)
        {//Forward and Backward
            Vector3 controllerMove = new Vector3(((rightController.transform.forward - rightController.transform.up).normalized).x,
                0f,
                (((rightController.transform.forward - rightController.transform.up).normalized).z));

            cameraRig.transform.Translate(controllerMove*moveValue.y);
        }
        if(moveValue.x > 0.1f || moveValue.x < -0.1f)
        {//Right and Left
            Vector3 controllerMove = new Vector3(rightController.transform.right.x, 0f, rightController.transform.right.z);
            cameraRig.transform.Translate(controllerMove * moveValue.x);
        }

        //Vector2 joyStickValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (joyStickValue != Vector2.zero)
        //{
        //    print(joyStickValue);
        //}
    }
}
