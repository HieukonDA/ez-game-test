using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _levelText;
    [SerializeField] private float _matchDuration = 60f;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _GameOverPanel;
    [SerializeField] private Text _displayScoreText;
    [SerializeField] private Text _enemyInfoText;
    [SerializeField] private GameObject _infoPanel;
    private Text _gameOverText;
    private Text _highScoreText;
    private Text _pauseText;
    private Button _backToMainMenuButton;
    private Button _nextMatchButton;
    private Button _ResetMatchButton;
    private Button _exitSettingButton;
    private Button _musicButton;
    private Button _soundButton;
    private Button _BackButton;
    private Button _PauseButton;
    private float _timeLeft;
    private bool _isTimerRunning = false;
    private bool _isMusicOn = true;
    private bool _isSoundOn = true;
    private int _highScore = 0;

    void Start()
    {
        _timeLeft = _matchDuration;
        UpdateTimerText();
        StartTimer();
        UpdateLevelText();

        _settingsPanel.SetActive(false);
        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _exitSettingButton = _settingsPanel.transform.Find("ExitSettingsButton").GetComponent<Button>();
        _musicButton = _settingsPanel.transform.Find("MusicButton").GetComponent<Button>();
        _soundButton = _settingsPanel.transform.Find("SoundButton").GetComponent<Button>();
        _BackButton = _settingsPanel.transform.Find("BackButton").GetComponent<Button>();
        _PauseButton = _settingsPanel.transform.Find("PauseButton").GetComponent<Button>();
        _pauseText = _PauseButton.transform.Find("PauseText").GetComponent<Text>();


        _exitSettingButton.onClick.AddListener(ExitSettingButton);
        _musicButton.onClick.AddListener(OnMusic);
        _soundButton.onClick.AddListener(OnSound);
        _BackButton.onClick.AddListener(BackMainMenu);
        _PauseButton.onClick.AddListener(PauseGame);

        _GameOverPanel.SetActive(false);
        _gameOverText = _GameOverPanel.transform.Find("GameOverText").GetComponent<Text>();
        _highScoreText = _GameOverPanel.transform.Find("HighScoreText").GetComponent<Text>();
        _backToMainMenuButton = _GameOverPanel.transform.Find("BackToMainMenuButton").GetComponent<Button>();
        _nextMatchButton = _GameOverPanel.transform.Find("NextMatchButton").GetComponent<Button>();
        _ResetMatchButton = _GameOverPanel.transform.Find("ResetMatchButton").GetComponent<Button>();

        _backToMainMenuButton.onClick.AddListener(BackMainMenu);
        _ResetMatchButton.onClick.AddListener(RestartMatch);
        _nextMatchButton.gameObject.SetActive(false);

    }

    private void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0f;
            _PauseButton.GetComponent<Image>().color = Color.red;
            _pauseText.text = "Resume";
        }
        else
        {
            Time.timeScale = 1f;
            _PauseButton.GetComponent<Image>().color = Color.green;
            _pauseText.text = "Pause";
        }
    }

    private void BackMainMenu()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        SceneManager.LoadScene("MainMenu");
    }

    private void OnSettingsButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _settingsPanel.SetActive(true);
    }

    private void ExitSettingButton()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _settingsPanel.SetActive(false);
    }

    private void OnSound()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _isSoundOn = !_isSoundOn;
        AudioManager.Instance.SetSoundEnabled(_isSoundOn);
        if (_isSoundOn)
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _soundButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            AudioManager.Instance.StopSound();
            _soundButton.GetComponent<Image>().color = Color.red;
        }
    }

    private void OnMusic()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _isMusicOn = !_isMusicOn;
        if (_isMusicOn)
        {
            AudioManager.Instance.PlayMusic("MusicTheme");
            _musicButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            AudioManager.Instance.StopMusic();
            _musicButton.GetComponent<Image>().color = Color.red;
        }
    }

    void Update()
    {
        if (_isTimerRunning && _timeLeft > 0)
        {
            UpdateTimerText();
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                _timeLeft = 0;
                _isTimerRunning = false;
                OnTimerEnd();
            }
        }
    }

    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            _timerText.text = $"{Mathf.CeilToInt(_timeLeft)}";
        }
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = $"$ {score}";
    }

    public void UpdateScoreDisplay(int score, bool PlayerHit)
    {
        if (_displayScoreText != null)
        {
            if (PlayerHit)
            {
                _displayScoreText.text = $"+{score}";
            }
            else
            {
                _displayScoreText.text = $"-{score}";
            }
        }
    }

    private void StartTimer()
    {
        _timeLeft = _matchDuration;
        _isTimerRunning = true;
        UpdateTimerText();
    }

    private void OnTimerEnd()
    {
        // Logic to handle what happens when the timer ends
        Debug.Log("Match time ended!");
        // You can trigger end match logic here, like showing results or ending the game
    }

    private void UpdateLevelText()
    {
        if (_levelText != null)
        {
            int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 0) + 1;
            _levelText.text = $"Level {currentLevel}";
        }
    }

    public void ShowGameOverPanel(bool playerWon)
    {
        Time.timeScale = 0f;
        _GameOverPanel.SetActive(true);
        _gameOverText.text = playerWon ? "You Win!" : "You Lose!";
        _highScoreText.text = CombatManager.Instance.highScore.ToString();
        _nextMatchButton.gameObject.SetActive(playerWon);
        if (playerWon && EnemyManager.Instance != null)
        {
            _nextMatchButton.onClick.RemoveAllListeners();
            _nextMatchButton.onClick.AddListener(() =>
            {
                EnemyManager.Instance.NextLevel();
                ResetHealthPlayer();
                UpdateLevelText();
                _GameOverPanel.SetActive(false);
            });
            _ResetMatchButton.onClick.RemoveAllListeners();
            _ResetMatchButton.onClick.AddListener(RestartMatch);
            LevelState.Instance.UnlockNextLevel();
            AudioManager.Instance.PlaySound("Victory");
        }
        if (!playerWon)
        {
            AudioManager.Instance.PlaySound("GameOver");
        }
    }

    private void ResetHealthPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.CurrentHealth = player.MaxHealth;
            player.healthBar.UpdateHealthBar(player.MaxHealth, player.MaxHealth);
        }
    }

    private void RestartMatch()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UpdateLevelText();
        Time.timeScale = 1;
    }

    public void ShowEnemyInfo(string info)
    {
        if (_enemyInfoText != null && _infoPanel != null)
        {
            _enemyInfoText.text = info;
            _infoPanel.SetActive(true);
        }
    }

    public void HideEnemyInfo()
    {
        if (_infoPanel != null)
        {
            _infoPanel.SetActive(false);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        CombatManager.Instance.StartGame();
    }
}