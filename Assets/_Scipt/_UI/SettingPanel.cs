using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : Panel
{
    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _defaultButton;
    [SerializeField] private Button _supportButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _defaultPanel;
    [SerializeField] private GameObject _supportPanel;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    private ColorBlock _defaultColors;
    private ColorBlock _supportColors;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        _logoutButton.onClick.AddListener(SignOut);
        _defaultButton.onClick.AddListener(OnDefaultButtonClicked);
        _supportButton.onClick.AddListener(OnSupportButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);

        // Audio settings
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        _musicSlider.value = savedMusicVolume;
        _soundSlider.value = savedSoundVolume;
        AudioManager.Instance.SetMusicVolume(savedMusicVolume);
        AudioManager.Instance.SetSFXVolume(savedSoundVolume);

        // colors button
        _defaultColors = _defaultButton.colors;
        _defaultColors.normalColor = Color.white;          // Màu mặc định (xám)
        _defaultColors.highlightedColor = Color.white;    // Màu khi hover (trắng)
        _defaultColors.pressedColor = Color.grey;       // Màu khi nhấn (xanh lá)
        _defaultColors.disabledColor = Color.gray;   // Màu khi disabled (xám đậm)
        _defaultColors.colorMultiplier = 1f;
        _defaultButton.colors = _defaultColors;

        _supportColors = _supportButton.colors;
        _supportColors.normalColor = Color.gray;          // Màu mặc định (xám)
        _supportColors.highlightedColor = Color.white;    // Màu khi hover (trắng)
        _supportColors.pressedColor = Color.grey;        // Màu khi nhấn (xanh dương)
        _supportColors.disabledColor = Color.gray;   // Màu khi disabled (xám đậm)
        _supportColors.colorMultiplier = 1f;
        _supportButton.colors = _supportColors;



        base.Initialize();
    }

    void Update()
    {
        AudioManager.Instance.SetMusicVolume(_musicSlider.value);
        AudioManager.Instance.SetSFXVolume(_soundSlider.value);

        if (Mathf.Abs(_musicSlider.value - PlayerPrefs.GetFloat("MusicVolume")) > 0.01f)
        {
            PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        }
        if (Mathf.Abs(_soundSlider.value - PlayerPrefs.GetFloat("SoundVolume")) > 0.01f)
        {
            PlayerPrefs.SetFloat("SoundVolume", _soundSlider.value);
        }
    }

    public override void Open()
    {
        base.Open();
    }

    private void SignOut()
    {
        ActionConfirmMenu panel = (ActionConfirmMenu)PanelManager.GetSingleton("actionconfirm");
        panel.Open(SignOutResult, "Do you want to sign out?", "Yes", "No");
        AudioManager.Instance.PlaySound("ButtonClick");
    }

    private void SignOutResult(ActionConfirmMenu.Result result)
    {
        if (result == ActionConfirmMenu.Result.Positive)
        {
            MenuManager.Singleton.SignOut();
        }
        
    }

    private void OnSupportButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _defaultPanel.SetActive(false);
        _supportPanel.SetActive(true);
        _defaultButton.colors = _defaultColors.normalColor == Color.white ? _supportColors : _defaultColors;
    }

    private void OnDefaultButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _defaultPanel.SetActive(true);
        _supportPanel.SetActive(false);
    }
    
    private void OnCloseButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("setting");
    }
}