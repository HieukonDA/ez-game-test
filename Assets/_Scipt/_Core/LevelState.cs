using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance { get; private set; }
    [SerializeField] private int _unlockedLevel = 1;
    private ModeOption _currentMode = ModeOption.OnevsOne;

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
        _currentMode = (ModeOption)PlayerPrefs.GetInt("CurrentMode", 0);
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

    public void SetCurrentMode(ModeOption modeOption)
    {
        _currentMode = modeOption;
        PlayerPrefs.SetInt("ModeOption", (int)modeOption);
        PlayerPrefs.Save();
        Debug.Log($"Mode set to {_currentMode}");
    }

    public ModeOption GetCurrentMode()
    {
        return _currentMode;
    }
}