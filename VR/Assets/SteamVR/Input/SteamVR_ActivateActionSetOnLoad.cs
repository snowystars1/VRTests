//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;

namespace Valve.VR
{
    /// <summary>
    /// Automatically activates an action set on Start() and deactivates the set on OnDestroy(). Optionally deactivating all other sets as well.
    /// </summary>
    public class SteamVR_ActivateActionSetOnLoad : MonoBehaviour
    {
        public SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet("default");

        public SteamVR_Input_Sources rHand = SteamVR_Input_Sources.RightHand;
        public SteamVR_Input_Sources lHand = SteamVR_Input_Sources.LeftHand;

        public bool disableAllOtherActionSets = false;

        public bool activateOnStart = true;
        public bool deactivateOnDestroy = true;


        private void Start()
        {
            if (actionSet != null && activateOnStart)
            {
                //Debug.Log(string.Format("[SteamVR] Activating {0} action set.", actionSet.fullPath));
                actionSet.Activate(rHand, 0, disableAllOtherActionSets);
                actionSet.Activate(lHand, 0, disableAllOtherActionSets);
            }
        }

        private void OnDestroy()
        {
            if (actionSet != null && deactivateOnDestroy)
            {
                //Debug.Log(string.Format("[SteamVR] Deactivating {0} action set.", actionSet.fullPath));
                actionSet.Deactivate(rHand);
                actionSet.Deactivate(lHand);
            }
        }
    }
}