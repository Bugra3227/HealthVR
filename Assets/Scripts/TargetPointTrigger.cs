using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointTrigger : MonoBehaviour
{
    [SerializeField] private PathTracking pathTracking;

    public void HandleReachedGoal()
    {
        pathTracking.ReachedGoal();
    }
}
