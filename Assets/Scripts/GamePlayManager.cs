using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zeptolab
{
    public class GamePlayManager : MonoBehaviour
    {
        public static GamePlayManager Instance { get { return _instance; } }
        private static GamePlayManager _instance;

        public Action<int> CoinEarned;
        public Action<int> SpecialCoinEarned;
        public Action<int> RemainingTimeChange;
        public Action<bool> OnGameEnd;
        public Action<string> OnUserChange;

        private BaseGameConfig _currentGameConfig;
        private GameObject _character;
        private int _currentSpecialCoins;
        private int _currentCoins;

        private IEnumerator _coroutine;
        private int _levelTime = 20;
        private string _currentName;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            _currentName = "User_" + UnityEngine.Random.Range(0,999);
        }

        private void Start()
        {
            _currentGameConfig = MainController.Instance.GetGameConfig(LevelType.DefaultLevel);
            ScenarioCreator.Instance.OnFinishScenario += OnFinishScenario;
            ScenarioCreator.Instance.Create(_currentGameConfig);

            if (OnUserChange != null) OnUserChange(_currentName);
            StartTimer();
        }

        public void PassLevel()
        {
            Debug.Log("Level Passed");
            StopTimer();
            MainController.Instance.SaveStats(_currentName,_currentCoins);
            if (OnGameEnd != null) OnGameEnd(true);
        }

        public void OnGetCoin()
        {
            StopTimer();

            _currentCoins++;

            if (CoinEarned != null) CoinEarned(_currentCoins);

            StartTimer();
        }
        public void OnGetSpecialCoin()
        {
            StopTimer();

            _currentSpecialCoins++;
            int newCoins = (int)Mathf.Round(_currentCoins * .1f);
            _currentCoins += newCoins;

            if (CoinEarned != null) CoinEarned(_currentCoins);
            if (SpecialCoinEarned != null) SpecialCoinEarned(_currentSpecialCoins);
            StartTimer();
        }

        private void OnFinishScenario(Vector3 CharaterSpawnPos)
        {
            Debug.Log("Scenario Finished");
            Debug.Log("Spawn Character");
            SpawnCharacter(CharaterSpawnPos);
            ScenarioCreator.Instance.OnFinishScenario -= OnFinishScenario;
        }

        private void SpawnCharacter(Vector3 charaterSpawnPos)
        {
            _character = Instantiate(_currentGameConfig.CharacterPrefab);
            _character.transform.position = charaterSpawnPos;
            CameraController.Instance.SetTarget(_character);
        }

        private IEnumerator Timer(int waitTime)
        {
            for (int i = 0; i < waitTime; i++)
            {
                RemainingTimeChange(waitTime - i);
                yield return new WaitForSeconds(1f);
            }

            GameOver();
        }

        private void GameOver()
        {
            StopTimer();
           if(OnGameEnd != null) OnGameEnd(false);
        }

        private void StartTimer()
        {
            _coroutine = Timer(_levelTime);
            StartCoroutine(_coroutine);
        }

        private void StopTimer()
        {
            if(_coroutine != null) StopCoroutine(_coroutine);
        }

        private void OnDestroy()
        {
            ScenarioCreator.Instance.OnFinishScenario -= OnFinishScenario;
        }
    }
}