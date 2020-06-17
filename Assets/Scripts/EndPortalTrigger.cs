using UnityEngine;
using System.Collections;
using System;
using Zeptolab;

public class EndPortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<Collider>().enabled = false;
        GamePlayManager.Instance.PassLevel();
    }
}