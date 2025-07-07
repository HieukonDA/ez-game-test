using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Maimenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    [SerializeField] private Button _selectionPanelButton;
    private GameObject _selectionPanel;
    [SerializeField] private Button _settingPanelButton;
    private GameObject _settingPanel;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _mailButton;
    private Button _exitSetting;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Button _defaultButton;
    [SerializeField] private Button _supportButton;
    private GameObject _defaultPanel;
    private GameObject _supportPanel;
    private void Start()
    {

        // mo panel selections
        _selectionPanel = gameObject.transform.Find("SelectionPanel").gameObject;
        _selectionPanelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _selectionPanel.SetActive(true);
        });

        // mo setting panel
        _settingPanel = gameObject.transform.Find("SettingPanel").gameObject;
        _settingPanelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _settingPanel.SetActive(true);
            _selectionPanel.SetActive(false);
        });
        _defaultPanel = transform.Find("SettingPanel/Content/Center/DefaultPanel").gameObject;
        _supportPanel = transform.Find("SettingPanel/Content/Center/SupportPanel").gameObject;

        ColorBlock defaultColors = _defaultButton.colors;
        defaultColors.normalColor = Color.white;          // Màu mặc định (xám)
        defaultColors.highlightedColor = Color.white;    // Màu khi hover (trắng)
        defaultColors.pressedColor = Color.grey;       // Màu khi nhấn (xanh lá)
        defaultColors.disabledColor = Color.gray;   // Màu khi disabled (xám đậm)
        defaultColors.colorMultiplier = 1f;
        _defaultButton.colors = defaultColors;

        _defaultButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _defaultPanel.SetActive(true);
            _supportPanel.SetActive(false);
            
        });

        ColorBlock supportColors = _supportButton.colors;
        supportColors.normalColor = Color.gray;          // Màu mặc định (xám)
        supportColors.highlightedColor = Color.white;    // Màu khi hover (trắng)
        supportColors.pressedColor = Color.grey;        // Màu khi nhấn (xanh dương)
        supportColors.disabledColor = Color.gray;   // Màu khi disabled (xám đậm)
        supportColors.colorMultiplier = 1f;
        _supportButton.colors = supportColors;
        _supportButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _defaultPanel.SetActive(false);
            _supportPanel.SetActive(true);
            _defaultButton.colors = defaultColors.normalColor == Color.white ? supportColors : defaultColors;
        });

        // Exit button
        _exitButton.onClick.AddListener(OnExitButtonClicked);
        _exitSetting = _settingPanel.transform.Find("ExitButton").GetComponent<Button>();
        _exitSetting.onClick.AddListener(OnExitButtonClicked);
        // home button
        _homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _settingPanel.SetActive(false);
            _selectionPanel.SetActive(false);
        });
        //start button
        _startButton.onClick.AddListener(OnStartButtonClicked);

        // audio settings
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Giá trị mặc định 0.5
        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f); // Giá trị mặc định 0.5
        _musicSlider.value = savedMusicVolume;
        _soundSlider.value = savedSoundVolume;

        AudioManager.Instance.SetMusicVolume(savedMusicVolume);
        AudioManager.Instance.SetSFXVolume(savedSoundVolume);
    }

    void Update()
    {
        AudioManager.Instance.SetMusicVolume(_musicSlider.value);
        AudioManager.Instance.SetSFXVolume(_soundSlider.value);

        if (Math.Abs(_musicSlider.value - PlayerPrefs.GetFloat("MusicVolume")) > 0.01f)
        {
            PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        }
        if (Math.Abs(_soundSlider.value - PlayerPrefs.GetFloat("SoundVolume")) > 0.01f)
        {
            PlayerPrefs.SetFloat("SoundVolume", _soundSlider.value);
        }
    }

    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        SceneManager.LoadScene("PreMatch");
        Time.timeScale = 1;
    }

    private void OnExitButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _settingPanel.SetActive(false);
        _selectionPanel.SetActive(false);
    }

    

}