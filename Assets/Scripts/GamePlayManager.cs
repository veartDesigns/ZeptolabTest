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

        private BaseGameConfig _currentGameConfig;
        private GameObject _character;
        private int _currentSpecialCoins;
        private int _currentCoins;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        private void Start()
        {
            _currentGameConfig = MainController.Instance.GetGameConfig(LevelType.DefaultLevel);
            ScenarioCreator.Instance.OnFinishScenario += OnFinishScenario;
            ScenarioCreator.Instance.Create(_currentGameConfig);
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

        public void OnGetCoin()
        {
            _currentCoins++;
            Debug.Log("_currentCoins " + _currentCoins);

            if (CoinEarned != null) CoinEarned(_currentCoins);
        }
        public void OnGetSpecialCoin()
        {
            _currentSpecialCoins++;
            int newCoins = (int)Mathf.Round(_currentCoins * .1f);
            Debug.Log(_currentCoins + " newCoins " + newCoins);

            _currentCoins += newCoins;

            if (CoinEarned != null) CoinEarned(_currentCoins);
            if (SpecialCoinEarned != null) SpecialCoinEarned(_currentSpecialCoins);
        }

        private void OnDestroy()
        {
            ScenarioCreator.Instance.OnFinishScenario -= OnFinishScenario;
        }
    }
}