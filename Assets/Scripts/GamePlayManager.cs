using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptolab;

public class GamePlayManager : MonoBehaviour
{
    private BaseGameConfig _currentGameConfig;
    void Start()
    {
        _currentGameConfig = MainController.Instance.GetGameConfig(LevelType.DefaultLevel);
        ScenarioCreator.Instance.OnFinishScenario += OnFinishScenario;
        ScenarioCreator.Instance.Create(_currentGameConfig);
    }

    private void OnFinishScenario()
    {
        Debug.Log("Scenario Finished");
        Debug.Log("Spawn Character");
        ScenarioCreator.Instance.OnFinishScenario -= OnFinishScenario;
    }

    private void OnDestroy()
    {
        ScenarioCreator.Instance.OnFinishScenario -= OnFinishScenario;
    }
}
