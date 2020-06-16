using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Zeptolab;

public class ScenarioCreator : MonoBehaviour
{

    public static ScenarioCreator Instance { get { return _instance; } }
    public Action OnFinishScenario;
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

    private GameObject _scenarioContainer;
    private float _zDistance = 10;
    private float _specialCoinsRange;

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
    }

    private void BuildScenario()
    {
        _scenarioContainer = new GameObject();
        _scenarioContainer.name = Constants.ScenarioContainerName;

        CreateWallLimits();
        CreateMaze();

        OnFinishScenario();
    }

    private void CreateMaze()
    {
        int lastSpace = 0;

        for (int i = 0; i < _scenarioWidth; i++)
        {
            int SquaresInCol = UnityEngine.Random.Range(_minMazeColSquares, _maxMazeColSquares);
            //Debug.Log(i + " SquaresInCol " + SquaresInCol);
            List<int> colPositionsRandomized = Enumerable.Range(0, _scenarioHeight).ToList();

            colPositionsRandomized = ListsUtils.RandomizeList(colPositionsRandomized);

            for (int j = 0; j < colPositionsRandomized.Count; j++)
            {
                int index = colPositionsRandomized[j];
                GameObject go = null;

                if (SquaresInCol<j && lastSpace != index)
                {
                    go  = Instantiate(_mazeSquares);
                    go.name = "col_" + i + "_row_" + index;
                }
                else
                {
                    float rndNumber = UnityEngine.Random.value;
                    if(rndNumber < _specialCoinsRange)
                    {
                        go = Instantiate(_specialCoins);
                    }
                    else
                    {
                        go = Instantiate(_coinsPrefab);
                    }

                    go.name = "coin_col_" + i + "_row_" + index;
                }

                go.transform.position = new Vector3(i, index, _zDistance);
                go.transform.parent = _scenarioContainer.transform;
            }

            lastSpace = colPositionsRandomized[colPositionsRandomized.Count - 1];
        }
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
        floor.transform.position = new Vector3((_scenarioWidth / 2)-.5f, -1, _zDistance);
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