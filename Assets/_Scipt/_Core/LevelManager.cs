using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Button[] _levelButtons;
    [SerializeField] MainMenu _mainMenu;

    void Start()
    {
        InitializeButtons();
        UpdateButtonStates();
    }

    public void InitializeButtons()
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        int maxLevel = enemyManager != null ? enemyManager.MaxLevel : _levelButtons.Length;

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i < maxLevel)
            {
                int levelIndex = i;
                _levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex + 1));
                _levelButtons[i].GetComponent<Image>().color = Color.gray;
            }
            else
            {
                _levelButtons[i].gameObject.SetActive(false);
            }
            
        }
    }

    private void LoadLevel(int level)
    {
        int unlockedLevel = LevelState.Instance.GetUnlockedLevel();
        if (level <= unlockedLevel)
        {
            PlayerPrefs.SetInt("SelectedLevel", level - 1);
            SceneManager.LoadScene(_mainMenu.defaultSceneName);
            AudioManager.Instance.PlaySound("ButtonClick");
        }
        else
        {
            Debug.LogWarning($"Level {level} is not unlocked yet.");
            return;
        }
    }

    public void UpdateButtonStates()
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        int maxLevel = enemyManager != null ? enemyManager.MaxLevel : _levelButtons.Length;
        int unlockedLevel = LevelState.Instance.GetUnlockedLevel();

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i < maxLevel)
            {
                if (i < unlockedLevel)
                {
                    _levelButtons[i].interactable = true;
                    _levelButtons[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    _levelButtons[i].interactable = false;
                    _levelButtons[i].GetComponent<Image>().color = Color.gray;
                }
            } 
        }
    }
}