using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance { get; private set; }
    [SerializeField] private int _unlockedLevel = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadState();
    }

    private void LoadState()
    {
        _unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null && _unlockedLevel > enemyManager.MaxLevel)
        {
            _unlockedLevel = enemyManager.MaxLevel;
            PlayerPrefs.SetInt("UnlockedLevel", _unlockedLevel);
            PlayerPrefs.Save();
        }
    }

    public int GetUnlockedLevel()
    {
        return _unlockedLevel;
    }

    public void UnlockNextLevel()
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            int maxLevel = enemyManager.MaxLevel;
            if (_unlockedLevel < maxLevel)
            {
                _unlockedLevel++;
                PlayerPrefs.SetInt("UnlockedLevel", _unlockedLevel);
                PlayerPrefs.Save();
                Debug.Log("Unlocked next level: " + _unlockedLevel);
            }
            else
            {
                Debug.Log("Max level reached: " + maxLevel);
            }
        }
    }

    public void ResetState()
    {
        _unlockedLevel = 1;
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
    }
}