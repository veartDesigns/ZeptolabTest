using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get { return _instance; } }
    public Action OnUserTapDown;
    public Action OnUserTapUp;
    public Action<Vector2> SetDirectionalInput;
    private static InputManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Update()
    {

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
}
