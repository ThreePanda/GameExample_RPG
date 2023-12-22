using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }

    [Header("Transition Info")] 
    public string sceneName;
    
    public TransitionType transitionType;
    //我可以去的点
    [Header("Where we go through this")] 
    public TransitionDestination.DestinationTag destinationTag;
    
    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //TODO:SceneController 传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
