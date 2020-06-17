using UnityEngine;
using System.Collections;
using System;
using Zeptolab;

public class EndPortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GamePlayManager.Instance.PassLevel();
        this.GetComponent<BoxCollider>().enabled = false;
    }
}