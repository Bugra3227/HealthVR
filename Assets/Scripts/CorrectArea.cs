using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectArea : MonoBehaviour
{
    private bool _pasiveArea;
    private PathTracking _pathTracking;

    private void Start()
    {
        _pathTracking=PathTracking.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        VrPlayerTrigger playerTrigger = other.GetComponent<VrPlayerTrigger>();
        if (playerTrigger && !_pasiveArea)
        {
            _pasiveArea = true;
            _pathTracking.IncreaseCorrectPathCount();
           
        }
            
    }
}
