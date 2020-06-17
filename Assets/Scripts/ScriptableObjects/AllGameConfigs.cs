using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zeptolab;
using System;

[CreateAssetMenu(fileName = "AllGameConfigs", menuName = "ScriptableObjects/AllGameConfigs", order = 1)]
[Serializable]
public class AllGameConfigs : ScriptableObject
{
    [SerializeField]private List<BaseGameConfig> _gameConfigs;

    //TODO: fill each scriptable object with the correct prefabs and new values to have great different levels. I had no time to fill it.
    public BaseGameConfig GetGameConfig(LevelType levelType )
    {
        return _gameConfigs.Find(x => x.LevelType == levelType);
    }
}
