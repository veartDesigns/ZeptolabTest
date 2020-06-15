using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zeptolab;

public class HomeMenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _statsUI;
    [SerializeField] private Button PlayButton;
    void Start()
    {
        MainController.Instance.SetAppState(AppState.MainMenu);
        PlayButton.onClick.AddListener(PlayGame);
        ShowStats();
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(AppState.Game.ToString());
    }

    private void ShowStats()
    {
        bool statsExists = PlayerPrefs.HasKey(Constants.GameStatsKey);

        if (statsExists)
        {
            List<UserData> stats = MainController.Instance.GetStats();
           
            for (int i = 0; i < stats.Count; i++){
                UserData userData = stats[i];
                GameObject statsGo = _statsUI[i];
                Text[] statsUIText = statsGo.transform.GetComponentsInChildren<Text>();
                statsUIText[0].text = userData.UserName;
                statsUIText[1].text = userData.Coins.ToString();
            }
        }
        else
        {
            Debug.Log("No User Stats");
            //Show Start filling stats text;
        }
    }
}
