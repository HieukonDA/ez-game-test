using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MatchHUD : MonoBehaviour
{
    [Header("Player HUD")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerScoreText;

    [Header("Opponent HUD")]
    [SerializeField] private EnemyAI _opponent;
    [SerializeField] private Text opponentNameText;
    [SerializeField] private Text opponentScoreText;

    [Header("Match Timer")]
    [SerializeField] private Text _matchTimerText;
    [SerializeField] private float matchDuration = 90f; // 5 minutes
    private float _currentTimer;

    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pausePanel;
    private Button _giveUpButton;
    private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    private const string lose = "You Lose!";
    private const string win = "You Win!";
    private bool _isMatchEnded;
    int totalCoins = 0;

    void Start()
    {
        SetupUI();
        SetupMatchData();
        _currentTimer = matchDuration;
        _isMatchEnded = false;
        _matchTimerText.text = TimeSpan.FromSeconds(_currentTimer).ToString(@"mm\:ss");
    }

    private void SetupUI()
    {
        _giveUpButton = _pausePanel.transform.Find("GiveUpButton").GetComponent<Button>();
        _continueButton = _pausePanel.transform.Find("ContinueButton").GetComponent<Button>();

        _pauseButton.onClick.AddListener(() =>
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0f;
        });

        _giveUpButton.onClick.AddListener(() =>
        {
            Debug.Log("Player has given up.");
            _pausePanel.SetActive(false);
            Time.timeScale = 1f;
            _statusText.text = lose;
            _statusText.gameObject.SetActive(true);
            StateManager.Instance.ChangeState(new DefeatState(_player));
            if (_opponent != null)
                _opponent.ChangeEnemyState(new EnemyVictoryState(_opponent));
            UpdateMatchDataOnGiveUp();
            StartCoroutine(HandleMatchEnd());
        });

        _continueButton.onClick.AddListener(() =>
        {
            Debug.Log("Player has continued the game.");
            _pausePanel.SetActive(false);
            Time.timeScale = 1f;
        });
    }

    private void SetupMatchData()
    {
        var matchDataObj = new GameObject("MatchData");
        matchDataObj.AddComponent<MatchData>().PrepareForMatch();
        MatchData.Instance.PlayerName = "hieu";
        MatchData.Instance.PlayerLevel = 3;
        MatchData.Instance.OpponentName = "hieu";
        MatchData.Instance.OpponentLevel = 3;
        MatchData.Instance.RoundNumber = 1;
    }

    void Update()
    {
        if (_isMatchEnded) return;

        _currentTimer -= Time.deltaTime;
        _matchTimerText.text = TimeSpan.FromSeconds(_currentTimer).ToString(@"mm\:ss");

        if (_currentTimer <= 0f || (_player.CurrentHealth <= 0) || (_opponent != null && _opponent.CurrentHealth <= 0))
        {
            _isMatchEnded = true;
            DetermineMatchResult();
            _statusText.text = MatchData.Instance.MatchResultText;
            _statusText.gameObject.SetActive(true);
            StartCoroutine(HandleMatchEnd());
        }
    }
    
    private void DetermineMatchResult()
    {
        if (_player.CurrentHealth <= 0 && _opponent.CurrentHealth <= 0)
        {
            MatchData.Instance.MatchResultText = "Draw!";
            MatchData.Instance.IsPlayerWin = false;
        }
        else if (_player.CurrentHealth <= 0)
        {
            MatchData.Instance.MatchResultText = lose;
            MatchData.Instance.IsPlayerWin = false;
        }
        else if (_opponent.CurrentHealth <= 0)
        {
            MatchData.Instance.MatchResultText = win;
            MatchData.Instance.IsPlayerWin = true;
        }
        else
        {
            MatchData.Instance.MatchResultText = "Time's Up!";
            MatchData.Instance.IsPlayerWin = false;
        }
    }

    private IEnumerator HandleMatchEnd()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;

        // Calculate coin reward
        totalCoins = CalculateCoinReward();
        
        // Update coins and wait for completion
        if (totalCoins > 0)
        {
            yield return StartCoroutine(UpdateCoinsAndWait(totalCoins));
            MatchData.Instance.RewardText = $"Reward: {totalCoins} Coins";
            Debug.Log($"Set RewardText: {MatchData.Instance.RewardText}");
        }
        else
        {
            MatchData.Instance.RewardText = "No Reward to Obtain";
            Debug.Log($"No reward - Set RewardText: {MatchData.Instance.RewardText}");
        }

        MatchData.Instance.PersistForResult();
        SceneManager.LoadScene("ResultMatch");
    }

    private int CalculateCoinReward()
    {
        if (MatchData.Instance.IsPlayerWin)
        {
            int baseReward = 50;
            int knockOutBonus = (_opponent != null && _opponent.CurrentHealth <= 0) ? 30 : 0;
            int timeBonus = (_currentTimer / matchDuration > 0.5f) ? 20 : 0;
            int damageBonus = Mathf.FloorToInt(MatchData.Instance.TotalDamagePlayer / 10);
            int rankBonus = 0;
            return baseReward + knockOutBonus + timeBonus + damageBonus + rankBonus;
        }
        else if (MatchData.Instance.MatchResultText == "Draw!")
        {
            return 25;
        }
        else
        {
            return 10;
        }
    }

    private IEnumerator UpdateCoinsAndWait(int coinsToAdd)
    {
        // Wait for MenuManager to be ready
        while (MenuManager.Singleton == null || !MenuManager.Singleton.IsSignedIn())
        {
            Debug.Log("Waiting for MenuManager to be ready...");
            yield return null;
        }

        Debug.Log("MenuManager is ready, updating coins...");

        // Use the improved async method
        var updateTask = UpdateCoinsAsync(coinsToAdd);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception != null)
        {
            Debug.LogError($"Failed to update coins: {updateTask.Exception.GetBaseException().Message}");
        }
        else
        {
            Debug.Log($"Successfully updated coins: {updateTask.Result}");
        }
    }

    private async Task<int> UpdateCoinsAsync(int coinsToAdd)
    {
        try
        {
            // Load current coins for logging
            int currentCoins = await MenuManager.Singleton.LoadCoinsAsync();

            // Update coins
            int newCoins = await MenuManager.Singleton.UpdatePlayerCoinsAsync(coinsToAdd);
            return newCoins;
        }
        catch (Exception e)
        {
            Debug.LogError($"UpdateCoinsAsync failed: {e.Message}");
            throw;
        }
    }

    private void UpdateMatchDataOnGiveUp()
    {
        MatchData.Instance.MatchResultText = lose;
        MatchData.Instance.IsPlayerWin = false;
        MatchData.Instance.RewardText = "No Reward to Obtain";
    }
}