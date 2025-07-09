using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Maimenu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _selectionButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _mailButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _leaderBoardButton;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _diamondText;
    [SerializeField] private TextMeshProUGUI _gymPointText;

    void Start()
    {
        // Button events
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _selectionButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            PanelManager.Open("selection");
        });
        _exitButton.onClick.AddListener(OnExitButtonClicked);
        _homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            PanelManager.Close("setting");
            PanelManager.Close("selection");
        });
        _leaderBoardButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            PanelManager.Open("leaderboard");
        });
        _mailButton.onClick.AddListener(() => AudioManager.Instance.PlaySound("ButtonClick")); 
    }

    void Update()
    {
       
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
        PanelManager.Close("setting");
        PanelManager.Close("selection");
    }

}