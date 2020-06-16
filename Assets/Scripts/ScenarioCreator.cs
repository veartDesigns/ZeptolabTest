using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class ScenarioCreator : MonoBehaviour
{

    public static ScenarioCreator Instance { get { return _instance; } }
    private static ScenarioCreator _instance;

    private int _scenarioWidth = 40;
    private int _scenarioHeight = 5;
    private int _maxMazeColSquares = 3;
    private int _minMazeColSquares = 1;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void Create()
    {
        int lastSpace = 0;
        for (int i = 0; i < _scenarioWidth; i++)
        {
            int SquaresInCol = UnityEngine.Random.Range(_minMazeColSquares, _maxMazeColSquares);
            Debug.Log(i+ " SquaresInCol " + SquaresInCol);
            List<int> colPositionsRandomized = Enumerable.Range(0, _scenarioHeight).ToList();

            colPositionsRandomized = ListsUtils.RandomizeList(colPositionsRandomized);

            for (int j = 0; j < SquaresInCol; j++)
            {
                int index = colPositionsRandomized[j];
                if (lastSpace == index) continue;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i, index, 10);
                cube.name = "col_"+ i +"_row_" + index;
            }

            lastSpace = colPositionsRandomized[colPositionsRandomized.Count-1];
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}