using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusInput : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject leftController;
    public GameObject rightController;


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

        triggerValue = triggerGripAction.GetAxis(SteamVR_Input_Sources.Any);
        print(triggerValue);

        if(triggerValue > 0.5f && pressFlag)//Teleport using raycast
        {
            print("Teleport");
            RaycastHit hit;
            if(Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f))
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
            cameraRig.transform.Translate(Vector3.forward * moveValue.y);
        }

        if(moveValue.x > 0.1f || moveValue.x < -0.1f)
        {//Right and Left
            cameraRig.transform.Translate(Vector3.right * moveValue);
        }

        //Vector2 joyStickValue = joyStickAction.GetAxis(SteamVR_Input_Sources.Any);

        //if (joyStickValue != Vector2.zero)
        //{
        //    print(joyStickValue);
        //}
    }
}
