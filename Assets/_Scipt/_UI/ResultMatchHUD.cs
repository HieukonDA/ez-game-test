using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Threading.Tasks;

public class ResultMatchHUD : MonoBehaviour
{
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _playerLevelText;
    [SerializeField] private Text _opponentNameText;
    [SerializeField] private Text _opponentLevelText;
    [SerializeField] private TMP_Text _roundNumberText;
    [SerializeField] private TMP_Text _matchResultText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private TextMeshProUGUI _rewardCoinsText;

    [Header("Player Damage Stats")]
    [SerializeField] private Text _totalDamageTextPlayer;
    [SerializeField] private Text _powerDamageTextPlayer;
    [SerializeField] private Text _hookDamageTextPlayer;
    [SerializeField] private Text _uppercutDamageTextPlayer;
    [SerializeField] private Text _megaPunchDamageTextPlayer;

    [Header("Enemy Damage Stats")]
    [SerializeField] private Text _totalDamageTextEnemy;
    [SerializeField] private Text _powerDamageTextEnemy;
    [SerializeField] private Text _hookDamageTextEnemy;
    [SerializeField] private Text _uppercutDamageTextEnemy;
    [SerializeField] private Text _megaPunchDamageTextEnemy;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button nextButton;

    [Header("Leaderboard Integration")]
    [SerializeField] private bool autoSubmitScore = true;
    private bool hasLoadedCoins = false;
    private bool hasSubmittedScore = false;
    private int rewardCoins = 0;

    void Start()
    {
        if (MatchData.Instance == null)
        {
            Debug.LogError("MatchData not initialized!");
            return;
        }

        // Display match information first
        DisplayMatchInfo();

        // Setup buttons
        SetupButtons();

        StartCoroutine(HandleMatchResult());
    }

    private void DisplayMatchInfo()
    {
        // Hiển thị thông tin
        _playerNameText.text = $"==Player: {MatchData.Instance.PlayerName}";
        _playerLevelText.text = $"Level: {MatchData.Instance.PlayerLevel}";
        _opponentNameText.text = $"Opponent: {MatchData.Instance.OpponentName}";
        _opponentLevelText.text = $"Level: {MatchData.Instance.OpponentLevel}";
        _roundNumberText.text = $"Round: {MatchData.Instance.RoundNumber}";
        _matchResultText.text = MatchData.Instance.MatchResultText;
        _rewardText.text = MatchData.Instance.RewardText;

        // Player Damage Stats
        _totalDamageTextPlayer.text = $"{MatchData.Instance.TotalDamagePlayer:F2}";
        _powerDamageTextPlayer.text = $"{MatchData.Instance.PowerDamagePlayer:F2}";
        _hookDamageTextPlayer.text = $"{MatchData.Instance.HookDamagePlayer:F2}";
        _uppercutDamageTextPlayer.text = $"{MatchData.Instance.UppercutDamagePlayer:F2}";
        _megaPunchDamageTextPlayer.text = $"{MatchData.Instance.MegaPunchDamagePlayer:F2}";

        // Enemy Damage Stats
        _totalDamageTextEnemy.text = $"{MatchData.Instance.TotalDamageEnemy:F2}";
        _powerDamageTextEnemy.text = $"{MatchData.Instance.PowerDamageEnemy:F2}";
        _hookDamageTextEnemy.text = $"{MatchData.Instance.HookDamageEnemy:F2}";
        _uppercutDamageTextEnemy.text = $"{MatchData.Instance.UppercutDamageEnemy:F2}";
        _megaPunchDamageTextEnemy.text = $"{MatchData.Instance.MegaPunchDamageEnemy:F2}";

        // Tính toán reward coins dựa trên kết quả match - CHỈ ĐỂ HIỂN THỊ
        // Coins đã được update trong MatchHUD rồi
        ExtractRewardCoinsFromText();

    }

    private void ExtractRewardCoinsFromText()
    {
        string rewardText = MatchData.Instance.RewardText;
        
        Debug.Log($"=== RESULT MATCH DEBUG ===");
        Debug.Log($"RewardText from MatchData: '{rewardText}'");
        Debug.Log($"IsPlayerWin: {MatchData.Instance.IsPlayerWin}");
        Debug.Log($"MatchResultText: '{MatchData.Instance.MatchResultText}'");
        
        // Parse reward coins from text like "Reward: 120 Coins"
        if (rewardText.Contains("Reward:") && rewardText.Contains("Coins"))
        {
            try
            {
                string[] parts = rewardText.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == "Reward:" && i + 1 < parts.Length)
                    {
                        if (int.TryParse(parts[i + 1], out int coins))
                        {
                            rewardCoins = coins;
                            Debug.Log($"Successfully parsed reward coins: {coins}");
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse reward coins: {e.Message}");
            }
        }
        
        Debug.Log($"Final extracted reward coins: {rewardCoins}");
    }

    private void SetupButtons()
    {
        // Button functionality
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(() =>
            {
                Debug.Log("Upgrade button clicked.");
                CleanupAndLoadScene("UpgradeMenu");
            });
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySound("ButtonClick");
                Debug.Log("Next button clicked.");
                CleanupAndLoadScene("MainMenu");
            });
        }
    }

    /// <summary>
    /// Main coroutine - load coins and auto submit if enabled
    /// </summary>
    private IEnumerator HandleMatchResult()
    {
        // Wait for MenuManager
        while (MenuManager.Singleton == null || !MenuManager.Singleton.IsSignedIn())
        {
            yield return null;
        }

        // Load and display coins
        yield return StartCoroutine(LoadAndDisplayCoins());

        // Auto submit to leaderboard if enabled
        if (autoSubmitScore && !hasSubmittedScore)
        {
            yield return StartCoroutine(AutoSubmitToLeaderboard());
        }
    }

    /// <summary>
    /// Auto submit current coins to leaderboard
    /// </summary>
    private IEnumerator AutoSubmitToLeaderboard()
    {
        Debug.Log("Auto submitting to leaderboard...");
        
        var submitTask = SubmitToLeaderboardAsync();
        yield return new WaitUntil(() => submitTask.IsCompleted);

        if (submitTask.Exception != null)
        {
            Debug.LogError($"Auto submit failed: {submitTask.Exception.GetBaseException().Message}");
        }
        else
        {
            Debug.Log("✅ Auto submit to leaderboard successful!");
            hasSubmittedScore = true;
        }
    }

    /// <summary>
    /// Submit current total coins to leaderboard
    /// </summary>
    private async Task SubmitToLeaderboardAsync()
    {
        try
        {
            // Load current total coins
            int totalCoins = await MenuManager.Singleton.LoadCoinsAsync();
            
            // Submit to leaderboard using Unity Services
            await Unity.Services.Leaderboards.LeaderboardsService.Instance.AddPlayerScoreAsync("LeaderBoard", rewardCoins);

            Debug.Log($"✅ Successfully submitted {rewardCoins} coins to leaderboard!");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Failed to submit to leaderboard: {e.Message}");
            throw;
        }
    }

    private IEnumerator LoadAndDisplayCoins()
    {
        if (hasLoadedCoins) yield break;
        hasLoadedCoins = true;

        Debug.Log("Loading coins data for display...");

        // Use the improved async method
        var loadTask = LoadCoinsAsync();
        yield return new WaitUntil(() => loadTask.IsCompleted);

        if (loadTask.Exception != null)
        {
            Debug.LogError($"Failed to load coins: {loadTask.Exception.GetBaseException().Message}");
            UpdateCoinsDisplay(0);
        }
        else
        {
            UpdateCoinsDisplay(loadTask.Result);
        }
    }

    private async Task<int> LoadCoinsAsync()
    {
        try
        {
            return await MenuManager.Singleton.LoadCoinsAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadCoinsAsync failed: {e.Message}");
            throw;
        }
    }

    private void UpdateCoinsDisplay(int totalCoins)
    {
        if (_rewardCoinsText != null && this != null)
        {
            if (rewardCoins > 0)
            {
                _rewardCoinsText.text = $"Reward: +{rewardCoins} | Total: {totalCoins}";
                Debug.Log($"Displayed coins with reward - Reward: {rewardCoins}, Total: {totalCoins}");
            }
            else
            {
                _rewardCoinsText.text = $"Total Coins: {totalCoins}";
                Debug.Log($"Displayed coins without reward - Total: {totalCoins}");
            }
        }
        else
        {
            Debug.LogError("_rewardCoinsText is null or this object is destroyed");
        }
    }

    private void CleanupAndLoadScene(string sceneName)
    {
        // Cleanup MatchData before loading new scene
        if (MatchData.Instance != null)
        {
            MatchData.Instance.Cleanup();
        }
        
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        // Cleanup button listeners
        if (upgradeButton != null)
            upgradeButton.onClick.RemoveAllListeners();
        if (nextButton != null)
            nextButton.onClick.RemoveAllListeners();
    }
    

}