using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text _timerText;
    [SerializeField] private float _matchDuration = 60f;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _settingsPanel;
    private Button _exitSettingButton;
    private Button _musicButton;
    private Button _soundButton;
    private Button _BackButton;
    private float _timeLeft;
    private bool _isTimerRunning = false;
    private bool _isMusicOn = true;
    private bool _isSoundOn = true;

    void Start()
    {
        _timeLeft = _matchDuration;
        UpdateTimerText();
        StartTimer();

        _settingsPanel.SetActive(false);
        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _exitSettingButton = _settingsPanel.transform.Find("ExitSettingsButton").GetComponent<Button>();
        _musicButton = _settingsPanel.transform.Find("MusicButton").GetComponent<Button>();
        _soundButton = _settingsPanel.transform.Find("SoundButton").GetComponent<Button>();
        _BackButton = _settingsPanel.transform.Find("BackButton").GetComponent<Button>();

        _exitSettingButton.onClick.AddListener(ExitSettingButton);
        _musicButton.onClick.AddListener(OnMusic);
        _soundButton.onClick.AddListener(OnSound);
        _BackButton.onClick.AddListener(BackMainMenu);
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
}