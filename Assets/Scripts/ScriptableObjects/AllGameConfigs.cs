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

    public BaseGameConfig GetGameConfig(LevelType levelType )
    {
        return _gameConfigs.Find(x => x.LevelType == levelType);
    }
}
