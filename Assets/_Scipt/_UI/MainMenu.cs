using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ModeOption
{
    OnevsOne,
    OnevsMany, 
    ManyvsMany,
}
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private Button _aboutButton;
    [SerializeField] private GameObject _ModeOptionPanel;
    [SerializeField] private GameObject _AboutPanel;
    [SerializeField] private GameObject _LevelPanel;
    private Button _backMainMenuButton;
    private Button _exitAboutButton;
    private Button _exitModeButton;
    private Button _onevsOneButton;
    private Button _onevsManyButton;
    private Button _manyvsManyButton;
    public string defaultSceneName = "Mode1";

    private void Start()
    {
        _ModeOptionPanel.SetActive(false);
        _AboutPanel.SetActive(false);
        _LevelPanel.SetActive(false);
        // Initialize main menu buttons
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _modeButton.onClick.AddListener(OnModeOptionClicked);
        _aboutButton.onClick.AddListener(OnAboutClicked);

        // Initialize mode option buttons
        _exitModeButton = _ModeOptionPanel.transform.Find("ExitModeButton").GetComponent<Button>();
        _onevsOneButton = _ModeOptionPanel.transform.Find("OnevsOneButton").GetComponent<Button>();
        _onevsManyButton = _ModeOptionPanel.transform.Find("OnevsManyButton").GetComponent<Button>();
        _manyvsManyButton = _ModeOptionPanel.transform.Find("ManyvsManyButton").GetComponent<Button>();

        _exitModeButton.onClick.AddListener(OnExitModeClicked);
        _onevsOneButton.onClick.AddListener(() => OnModeSelected(ModeOption.OnevsOne));
        _onevsManyButton.onClick.AddListener(() => OnModeSelected(ModeOption.OnevsMany));
        _manyvsManyButton.onClick.AddListener(() => OnModeSelected(ModeOption.ManyvsMany));

        _exitAboutButton = _AboutPanel.transform.Find("ExitAboutButton").GetComponent<Button>();
        _exitAboutButton.onClick.AddListener(OnExitModeClicked);

        _backMainMenuButton = _LevelPanel.transform.Find("BackMainMenuButton").GetComponent<Button>();
        _backMainMenuButton.onClick.AddListener(OnExitModeClicked);

    }

    private void OnModeSelected(ModeOption mode)
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        switch (mode)
        {
            case ModeOption.OnevsOne:
                Debug.Log("Selected One vs One mode");
                defaultSceneName = "Mode1"; // Change to your One vs One scene name
                _onevsOneButton.GetComponent<Image>().color = Color.green;
                _onevsManyButton.GetComponent<Image>().color = Color.white;
                _manyvsManyButton.GetComponent<Image>().color = Color.white;
                break;
            case ModeOption.OnevsMany:
                Debug.Log("Selected One vs Many mode");
                defaultSceneName = "Mode2"; // Change to your One vs Many scene name
                _onevsOneButton.GetComponent<Image>().color = Color.white;
                _onevsManyButton.GetComponent<Image>().color = Color.green;
                _manyvsManyButton.GetComponent<Image>().color = Color.white;
                break;
            case ModeOption.ManyvsMany:
                Debug.Log("Selected Many vs Many mode");
                defaultSceneName = "Mode3"; // Change to your Many vs Many scene name
                _onevsOneButton.GetComponent<Image>().color = Color.white;
                _onevsManyButton.GetComponent<Image>().color = Color.white;
                _manyvsManyButton.GetComponent<Image>().color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void OnExitModeClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _ModeOptionPanel.SetActive(false);
        _AboutPanel.SetActive(false);
        _LevelPanel.SetActive(false);
    }

    private void OnAboutClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _AboutPanel.SetActive(true);
    }

    private void OnModeOptionClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _ModeOptionPanel.SetActive(true);
    }

    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        // SceneManager.LoadScene(_defaultSceneName);
        _LevelPanel.SetActive(true);
        Time.timeScale = 1;
    }

    private void OnExitButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        Application.Quit();
    }

}