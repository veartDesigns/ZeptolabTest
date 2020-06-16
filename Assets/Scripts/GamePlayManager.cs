using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        ScenarioCreator.Instance.Create();
    }

   
}
