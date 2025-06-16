using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private EnemyLevelConfig[] _levelConfigs;
    [SerializeField] private int _currentLevel = 0;
    public EnemyLevelConfig[] LevelConfigs => _levelConfigs;
    public int MaxLevel => _levelConfigs.Length;
    public int CurrentLevel => _currentLevel;
    private ObjectPool<GameObject> _enemyPool;
    private const int PoolSize = 50; 
    private Transform _poolParent;

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
        _currentLevel = PlayerPrefs.GetInt("SelectedLevel", 0);
        GenerateLevelConfig();

        _poolParent = new GameObject("EnemyPool").transform; 
        _enemyPool = new ObjectPool<GameObject>(() => Instantiate(_enemyPrefab, _poolParent), PoolSize, _poolParent);
    }

    public void GenerateLevelConfig()
    {
        _levelConfigs = new EnemyLevelConfig[10];
        ModeOption currentMode = LevelState.Instance.GetCurrentMode();
        for (int i = 0; i < 10; i++)
        {
            _levelConfigs[i] = ScriptableObject.CreateInstance<EnemyLevelConfig>();
            _levelConfigs[i].levelNumber = i + 1;
            if (currentMode == ModeOption.OnevsOne)
            {
                _levelConfigs[i].enemyCount = 1;
                _levelConfigs[i].maxHealth = 100 + (i * 20);
                _levelConfigs[i].movementSpeed = 1f + (i * 0.1f);
                _levelConfigs[i].attackCooldown = Mathf.Max(2f - (i * 0.1f), 0.5f);
                _levelConfigs[i].attackDamage = 5 + (i * 2);
            }
            else if (currentMode == ModeOption.OnevsMany)
            {
                _levelConfigs[i].enemyCount = Mathf.Clamp(2 + i, 2, 10);
                _levelConfigs[i].maxHealth = 100;
                _levelConfigs[i].movementSpeed = 1f;
                _levelConfigs[i].attackCooldown = Mathf.Max(2f - (i * 0.15f), 0.5f);
                _levelConfigs[i].attackDamage = 5;
            }
            else if (currentMode == ModeOption.ManyvsMany)
            {
                _levelConfigs[i].enemyCount = PoolSize; 
                _levelConfigs[i].maxHealth = 80 + (i * 10);
                _levelConfigs[i].movementSpeed = 1f + (i * 0.1f);
                _levelConfigs[i].attackCooldown = Mathf.Max(2f - (i * 0.1f), 0.5f);
                _levelConfigs[i].attackDamage = 5 + (i * 1);
            }
        }
    }

    void Start()
    {
        SpawnEnemies();
    }
    // geenerate enemy level config
    void SpawnEnemies()
    {
        EnemyLevelConfig config = _levelConfigs[_currentLevel];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            ClearEnemies();
            int enemiesToSpawn = Mathf.Min(config.enemyCount, PoolSize - _enemyPool.CountInactive);
            for (int i = 0; i < config.enemyCount; i++)
            {
                GameObject enemy = _enemyPool.Get();
                enemy.transform.position = playerPos + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.maxHealth = config.maxHealth;
                    enemyAI.movementSpeed = config.movementSpeed;
                    enemyAI.attackCooldown = config.attackCooldown;
                    enemyAI.attackRadius = 0.7f; // Điều chỉnh nếu cần
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ResetState();
            }
        }
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