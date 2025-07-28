using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Authentication;

public class ItemPreMatch : Panel
{
    [SerializeField] private Button _fightButton;
    [SerializeField] private Button _backButton;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        _fightButton.onClick.AddListener(OnFightButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        base.Initialize();
    }

    private void OnFightButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("itemprematch");
        SceneManager.LoadScene("Match");
        Time.timeScale = 1;
    }

    private void OnBackButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("itemprematch");
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}