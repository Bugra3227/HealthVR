using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPlayerTrigger : MonoBehaviour
{
   private bool _isFinishGame;
   private void OnTriggerEnter(Collider other)
   {
      if(_isFinishGame)
         return;
      TargetPointTrigger targetPointTrigger = other.GetComponent<TargetPointTrigger>();
      if (targetPointTrigger)
      {
         _isFinishGame = true;
         targetPointTrigger.HandleReachedGoal();
      }
        
         
   }
}
