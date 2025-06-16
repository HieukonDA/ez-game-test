using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private EnemyLevelConfig[] _levelConfigs;
    [SerializeField] private int _currentLevel = 0;
    public int MaxLevel => _levelConfigs.Length;

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
    }

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        EnemyLevelConfig config = _levelConfigs[_currentLevel];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            ClearEnemies();
            for (int i = 0; i < config.enemyCount; i++)
            {
                Vector3 spawnPosition = playerPos + new Vector3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f));
                GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.maxHealth = config.maxHealth;
                    enemyAI.movementSpeed = config.movementSpeed;
                    enemyAI.attackCooldown = config.attackCooldown;
                    enemyAI.attackRadius = 1.5f; // Điều chỉnh nếu cần
                }
            }
            CombatManager.Instance.UpdateEnemies();
            Debug.Log($"Spawned {config.enemyCount} enemies for level {_currentLevel} at player position {playerPos}");
        }
        else
        {
            Debug.Log("player not found");
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        _currentLevel = Mathf.Min(_currentLevel + 1, _levelConfigs.Length - 1); 
        PlayerPrefs.SetInt("SelectedLevel", _currentLevel);
        ClearEnemies();
        SpawnEnemies();
    }

    private void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}