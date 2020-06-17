using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zeptolab;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _coinsText;
    [SerializeField] private Text _specialCoinsText;
    [SerializeField] private Text _time;
    [SerializeField] private Animator _specialCoinsAnimator;
    [SerializeField] private Animator _coinsAnimator;

    private void Awake()
    {
        GamePlayManager.Instance.CoinEarned += OnGetCoin;
        GamePlayManager.Instance.SpecialCoinEarned += OnGetSpecialCoin;
        GamePlayManager.Instance.RemainingTimeChange += RemainingTimeChange;
        GamePlayManager.Instance.OnGameEnd += OnGameEnd;
    }

    private void OnGameEnd(bool win)
    {
        string msg;
        if(win){
            msg = "Level Passed";
        }
        else
        {
            msg = "Game Over";
        }
        //.text = msg;
    }

    private void RemainingTimeChange(int time)
    {
        _time.text = time + "'";
    }

    private void OnGetSpecialCoin(int specialCoins)
    {
        _specialCoinsText.text = specialCoins.ToString();
        //specialCoinsAnimator.Play("SpecialCoinsEarned");
    }

    private void OnGetCoin(int coins)
    {
        _coinsText.text = coins.ToString();
        //specialCoinsAnimator.Play("CoinsEarned");
    }
}
