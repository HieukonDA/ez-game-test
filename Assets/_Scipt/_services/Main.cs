using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class Main : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    [SerializeField] private Button leaderBoardButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        leaderBoardButton.onClick.AddListener(LeaderBoard);
        base.Initialize();
    }

    public override void Open()
    {
        UpdatePlayerNameUI();
        base.Open();
    }

    private void SignOut()
    {
        MenuManager.Singleton.SignOut();
    }

    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }
    
    private void LeaderBoard()
    {
        PanelManager.Open("leaderboard");
    }
    
}