using UnityEngine;
using System.Collections;
using System;
using Zeptolab;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get { return _instance; } }
    public Action OnUserTapDown;
    public Action OnUserTapUp;
    public Action<Vector2> SetDirectionalInput;
    private static InputManager _instance;
    private bool _inputEnabled = true;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        GamePlayManager.Instance.OnGameEnd += OnGameEnd;
    }

    private void OnGameEnd(bool win)
    {
        _inputEnabled = false;
    }

    void Update()
    {
        if (!_inputEnabled) return;
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnUserTapDown();
        }
#endif

        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    OnUserTapDown();
                }
            }
        }
    }

    private void OnDestroy()
    {
        GamePlayManager.Instance.OnGameEnd -= OnGameEnd;
    }
}
