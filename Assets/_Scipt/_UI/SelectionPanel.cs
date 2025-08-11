using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Services.Authentication;
using System.Net;

public class SelectionPanel : Panel
{
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _notifyButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _renameButton;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        _homeButton.onClick.AddListener(OnHomeButtonClicked);
        _shopButton.onClick.AddListener(OnShopButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _notifyButton.onClick.AddListener(OnNotifyButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _renameButton.onClick.AddListener(RenamePlayer);
        base.Initialize();
    }

    public override void Open()
    {
        base.Open();
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        _nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void OnHomeButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("selection");
    }

    private void OnSettingButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Open("setting");
    }

    private void OnShopButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Open("shop");
    }

    private void OnNotifyButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Open("notify");
    }

    private void OnCloseButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("selection");
    }

    private async void RenamePlayerConfirm(string input)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(input);
            UpdatePlayerNameUI();
        }
        catch (System.Exception)
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, "Failed to rename player. Please try again.", "OK");
        }

    }

    private void RenamePlayer()
    {
        GetInputMenu panel = (GetInputMenu)PanelManager.GetSingleton("input");
        panel.Open(RenamePlayerConfirm, GetInputMenu.Type.String, 20, "Enter your new name:", "Rename", "Cancel");
    }

    private void UpdatePlayerNameUI()
    {
        _nameText.text = AuthenticationService.Instance.PlayerName;
        // _levelText.text = $"Level: {AuthenticationService.Instance.PlayerLevel}";
    }
}