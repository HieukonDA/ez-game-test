using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Services.Authentication;

public class SelectionPanel : Panel
{
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _notifyButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _closeButton;
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
}