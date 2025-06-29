using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    [SerializeField] private Button _selectionPanelButton;
    private GameObject _selectionPanel;
    [SerializeField] private Button _settingPanelButton;
    private GameObject _settingPanel;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _mailButton;

    public string defaultSceneName = "MainScene";
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

        // Exit button
        _exitButton.onClick.AddListener(OnExitButtonClicked);
        // home button
        _homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            _settingPanel.SetActive(false);
            _selectionPanel.SetActive(false);
        });
        //start button
        _startButton.onClick.AddListener(OnStartButtonClicked);


    }

    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        SceneManager.LoadScene(defaultSceneName);
        Time.timeScale = 1;
    }

    private void OnExitButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        _settingPanel.SetActive(false);
        _selectionPanel.SetActive(false);
    }

}