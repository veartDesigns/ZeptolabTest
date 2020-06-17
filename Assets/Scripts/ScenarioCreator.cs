using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Zeptolab;

public class ScenarioCreator : MonoBehaviour
{
    public static ScenarioCreator Instance { get { return _instance; } }
    public Action<Vector3> OnFinishScenario;
    private static ScenarioCreator _instance;

    private int _scenarioWidth ;
    private int _scenarioHeight;
    private int _maxMazeColSquares;
    private int _minMazeColSquares;

    private GameObject _wallsPrefab;
    private GameObject _floorPrefab;
    private GameObject _mazeSquares;

    private GameObject _coinsPrefab;
    private GameObject _specialCoins;
    private GameObject _endGamePortal;

    private GameObject _scenarioContainer;
    private float _zDistance = 0;
    private float _specialCoinsRange;
    private Vector3 _spawnPosition;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void Create(BaseGameConfig gameConfig)
    {
        GetGameInfo(gameConfig);
        BuildScenario();
    }

    private void GetGameInfo(BaseGameConfig gameConfig)
    {
        _scenarioWidth = gameConfig.ScenarioWidth;
        _scenarioHeight = gameConfig.ScenarioHeight;
        _maxMazeColSquares = gameConfig.MaxMazeColSquares;
        _minMazeColSquares = gameConfig.MinMazeColSquares;

        _floorPrefab = gameConfig.WallsPrefab;
        _wallsPrefab = gameConfig.FloorPrefab;
        _specialCoins = gameConfig.SpecialCoinPrefab;
        _coinsPrefab = gameConfig.CoinPrefab;
        _mazeSquares = gameConfig.SquarePrefab;
        _specialCoinsRange = gameConfig.SpecialCoinsRange;
        _endGamePortal = gameConfig.EndGamePortalPrefab;
    }

    private void BuildScenario()
    {
        _scenarioContainer = new GameObject();
        _scenarioContainer.name = Constants.ScenarioContainerName;

        CreateWallLimits();
        CreateMaze();

        OnFinishScenario(_spawnPosition);
    }

    private void CreateMaze()
    {
        _spawnPosition = new Vector3(0, 0, _zDistance);

        for (int i = 0; i < _scenarioWidth - 1; i++)
        {
            int[] height = new int[_scenarioHeight];

            if ((i % 2) != 0)
            {
                int currentCase = UnityEngine.Random.Range(0, 2);

                bool ableUpAndDown = UnityEngine.Random.value < .3;

                if (currentCase == 0)
                {
                    int maxRange = (int)Mathf.Round(_scenarioHeight / 2) + 1;
                    int downSquareSizes = UnityEngine.Random.Range(1, maxRange + 1);
                    for (int n = 0; n < downSquareSizes; n++)
                    {
                        height[n] = 1;
                    }

                    if (ableUpAndDown)
                    {
                        int upSquareSizes = UnityEngine.Random.Range(0, maxRange);

                        int limit = height.Length - 1 - upSquareSizes;

                        for (int n = height.Length - 1; n > limit; n--)
                        {
                            height[n] = 1;
                        }
                    }
                }
                else
                {

                    int middleIndex = (int)Mathf.Round(_scenarioHeight / 2);
                    height[middleIndex] = 1;
                    int extraCube = UnityEngine.Random.value < .5 ? 0 : (UnityEngine.Random.value < .5 ? -1 : 1);
                    if (extraCube != 0)
                    {
                        height[middleIndex + extraCube] = 1;
                    }
                }

                for (int j = 0; j < height.Length; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int val = height[j];

                    GameObject go = null;
                    float offset = 0;

                    if (val != 0)
                    {
                        go = Instantiate(_mazeSquares);
                        go.name = "col_" + i + "_row_" + j;
                    }
                    else
                    {
                        float rndNumber = UnityEngine.Random.value;
                        if (rndNumber < _specialCoinsRange)
                        {
                            go = Instantiate(_specialCoins);
                        }
                        else
                        {
                            go = Instantiate(_coinsPrefab);
                            offset = -.3f;
                        }
                    }

                    go.transform.position = new Vector3(i, j + offset, _zDistance);
                    go.transform.parent = _scenarioContainer.transform;
                }
            }
        }

        GameObject endPortal = Instantiate(_endGamePortal);
        endPortal.transform.position = new Vector3(_scenarioWidth - 1, -0.5f, _zDistance);
    }

    private void CreateWallLimits()
    {
        float yWallPosition = _scenarioHeight - .5f;
        GameObject wallLeft = Instantiate(_wallsPrefab);
        wallLeft.transform.position = new Vector3(-1, yWallPosition, _zDistance);
        wallLeft.transform.localScale = new Vector3(1, _scenarioHeight * 2, 1);
        wallLeft.name = Constants.LeftWallName;

        GameObject wallRight = Instantiate(_wallsPrefab);
        wallRight.transform.position = new Vector3(_scenarioWidth, yWallPosition, _zDistance);
        wallRight.transform.localScale = new Vector3(1, _scenarioHeight * 2, 1);
        wallRight.name = Constants.RightWallName;

        GameObject floor = Instantiate(_floorPrefab);
        floor.transform.position = new Vector3((_scenarioWidth / 2), -1, _zDistance);
        floor.transform.localScale = new Vector3(_scenarioWidth + 2, 1, 1);
        floor.name = Constants.FloorName;

        wallRight.transform.parent = _scenarioContainer.transform;
        wallLeft.transform.parent = _scenarioContainer.transform;
        floor.transform.parent = _scenarioContainer.transform;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}