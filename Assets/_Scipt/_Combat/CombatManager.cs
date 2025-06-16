using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyAI[] _enemyAIs;
    [SerializeField] private HUD _hud;
    private int _totalScore = 0;
    private int _matchScore = 0;
    public int highScore = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadScore();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _matchScore = 0;
        FindPlayerAndEnemiesAndHUD();
        StartCoroutine(InitializeGameWithPause());
    }

    private IEnumerator InitializeGameWithPause()
    {
        EnemyLevelConfig config = EnemyManager.Instance.LevelConfigs[EnemyManager.Instance.CurrentLevel];
        string info = $"Level {config.levelNumber}\n" +
                     $"Enemy Count: {config.enemyCount}\n" +
                     $"Health: {config.maxHealth}\n" +
                     $"Speed: {config.movementSpeed:F1}\n" +
                     $"Attack Cooldown: {config.attackCooldown:F1}s\n";
        if (_hud != null)
        {
            _hud.ShowEnemyInfo(info);
        }
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(0f); 
    }

    public void UpdateEnemies()
    {
        _enemyAIs = FindObjectsOfType<EnemyAI>();
        Debug.Log($"Updated enemies count: {_enemyAIs.Length}");
    }

    private void FindPlayerAndEnemiesAndHUD()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _enemyAIs = FindObjectsOfType<EnemyAI>();
        _hud = FindObjectOfType<HUD>();
        if (_hud != null && _playerController != null)
        {
            _hud.gameObject.SetActive(true);
            _hud.UpdateScoreText(_totalScore);
        }
    }

    private void LoadScore()
    {
        _totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("TotalScore", _totalScore);
        PlayerPrefs.Save();
    }

    public void OnPlayerHitEnemy()
    {
        _matchScore += 10;
        _totalScore += 10;
        SaveScore();
        if (_hud != null)
        {
            _hud.UpdateScoreDisplay(10, true);
        }
    }

    public void OnPlayerTakeDamage()
    {
        _matchScore -= 10;
        _totalScore -= 10;
        SaveScore();
        if (_hud != null)
        {
            _hud.UpdateScoreDisplay(10, false);
        }
    }

    public void SubmitAction(string loser)
    {
        if (loser == "enemy")
        {
            OnPlayerHitEnemy();
            CheckWinCondition();
        }
        else if (loser == "player")
        {
            OnPlayerTakeDamage();
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (_playerController.CurrentHealth <= 0)
        {
            EndMatch(false);
        }
        else if (AreAllEnemiesDefeated())
        {
            EndMatch(true);
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        foreach (var enemy in _enemyAIs)
        {
            if (enemy != null && enemy.CurrentHealth > 0)
                return false;
        }
        return true;
    }

    private void EndMatch(bool playerWon)
    {
        Time.timeScale = 1f;
        highScore = _matchScore;
        if (playerWon)
        {
            _playerController.Victory();
        }
        StartCoroutine(DelayShowGameOverPanel(playerWon));       
    }

    private IEnumerator DelayShowGameOverPanel(bool playerWon)
    {
        yield return new WaitForSecondsRealtime(5f);
        if (_hud != null)
        {
            _hud.ShowGameOverPanel(playerWon);
            _hud.UpdateScoreText(_totalScore);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (_hud != null)
        {
            _hud.HideEnemyInfo();
        }
        Debug.Log("Game started");
    }

    void OnEnable()
    {
        FindPlayerAndEnemiesAndHUD();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
}
