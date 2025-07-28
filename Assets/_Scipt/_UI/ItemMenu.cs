using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ItemMenu : Panel
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _leaderBoardButton;
    [SerializeField] private LeaderboardsMenu _leaderboardsMenu;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _leaderBoardButton.onClick.AddListener(OnLeaderboardButtonClicked);
        base.Initialize();
    }

    private async void OnLeaderboardButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
        {
            // Làm mới dữ liệu trong MainMenu trước
            var mainMenu = FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                await mainMenu.RefreshAllCurrenciesAsync();
            }
        }
        PanelManager.Open("leaderboard");
    }

    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("itemmenu");
        SceneManager.LoadScene("PreMatch");
        Time.timeScale = 1;
    }
}