using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Zeptolab
{
    public class MainController : MonoBehaviour
    {
        public static MainController Instance { get { return _instance; } }
        private static MainController _instance;
        private AppState _currentAppState;
        private StatsData currentStats;

        public void SetAppState(AppState appState)
        {
            _currentAppState = appState;
        }

        public List<UserData> GetStats()
        {
            return currentStats.UsersStats;
        }

        public void SaveStats(string currentName, int coins)
        {
            currentStats.AddUserStat(new UserData(currentName, coins));

            for (int i = 0; i < currentStats.UsersStats.Count; i++)
            {
                UserData userData = currentStats.UsersStats[i];
            }

            string jsonInfo = JsonUtility.ToJson(currentStats);
            Debug.Log("jsonInfo " + jsonInfo);
            PlayerPrefs.SetString(Constants.GameStatsKey, jsonInfo);
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            bool statsExists = PlayerPrefs.HasKey(Constants.GameStatsKey);

            if (statsExists)
            {
                currentStats = JsonUtility.FromJson<StatsData>(PlayerPrefs.GetString(Constants.GameStatsKey));
            }
            else
            {
                currentStats = new StatsData();
            }

            SceneManager.LoadScene(AppState.MainMenu.ToString());

        }
    }
}