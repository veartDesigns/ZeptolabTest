using System;
using UnityEngine;
using Zeptolab;

[CreateAssetMenu(fileName = "BaseGameConfig", menuName = "ScriptableObjects/BaseGameConfig", order = 1)]
[Serializable]
public class BaseGameConfig : ScriptableObject
{
    public LevelType LevelType;
    public int ScenarioWidth;
    public int ScenarioHeight;
    public int MaxMazeColSquares;
    public int MinMazeColSquares;
    public float SpecialCoinsRange;
    public int Speed;
    public int JumpForce;

    public GameObject SquarePrefab;
    public GameObject WallsPrefab;
    public GameObject CharacterPrefab;
    public GameObject FloorPrefab;
    public GameObject CoinPrefab;
    public GameObject SpecialCoinPrefab;
    public GameObject EndGamePortalPrefab;
}