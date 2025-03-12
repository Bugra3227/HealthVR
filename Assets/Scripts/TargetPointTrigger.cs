using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointTrigger : MonoBehaviour
{
    [SerializeField] private PathTracking pathTracking;
    [SerializeField] private OVRManager ovrManager;

    public void HandleReachedGoal()
    {
        ovrManager.GetComponent<CharacterController>().enabled = false;
        ovrManager.GetComponent<OVRPlayerController>().enabled = false;
        pathTracking.ReachedGoal();
    }
}
