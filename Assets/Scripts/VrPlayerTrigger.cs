using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPlayerTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      TargetPointTrigger targetPointTrigger = other.GetComponent<TargetPointTrigger>();
      if(targetPointTrigger)
         targetPointTrigger.HandleReachedGoal();
         
   }
}
