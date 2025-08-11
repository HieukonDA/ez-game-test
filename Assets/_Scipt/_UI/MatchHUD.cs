using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

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

    [Header("Match Skill item inventory")]
    public List<ItemPlayerEquiped> itemEquipped = new List<ItemPlayerEquiped>();
    [SerializeField] private GameObject _itemEquippedPrefab;
    [SerializeField] private Transform _itemSkillPlayerContainer;
    [SerializeField] private GameObject _skillTimer;
    [SerializeField] private Image _skillTimerImage;
    [SerializeField] private TextMeshProUGUI _skillTimerText;
    [SerializeField] private ItemEquipped _itemEquipped;

    [Header("Match Status")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pausePanel;
    private Button _giveUpButton;
    private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _infoActionText;
    private const string lose = "You Lose!";
    private const string win = "You Win!";
    private bool _isMatchEnded;
    int totalCoins = 0;

    void Start()
    {
        SetupUI();
        SetupMatchData();
        LoadItemFromCloud();
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

        _player._onAttackHitCallBack += OnPlayerActionReceived;
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

    #region Action info
    private void OnPlayerActionReceived(string actionInfo)
    {
        _infoActionText.text = actionInfo;
    }
    
    #endregion

    #region Skill 

    public async void LoadItemFromCloud()
    {
        var equippedItems = await MenuManager.Singleton.LoadItemEquippedAsync();
        LoadItemEquippedAsync(equippedItems);
    }

    public void LoadItemEquippedAsync(List<ItemData> itemDataList)
    {
        if (itemDataList == null || itemDataList.Count == 0)
        {
            Debug.LogWarning("No items to load for ItemPreMatch.");
            return;
        }

        for (int i = _itemSkillPlayerContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_itemSkillPlayerContainer.GetChild(i).gameObject);
        }
        itemEquipped.Clear();

        foreach (var itemData in itemDataList)
        {
            if (itemData == null)
            {
                Debug.LogError("ItemData is null in LoadItemEquippedAsync.");
                continue;
            }
            AddItemEquipped(itemData);
        }
    }

    public void AddItemEquipped(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("ItemPlayerEquiped or ItemData is null in AddItemEquipped.");
            return;
        }
        ItemPlayerEquiped itemEquipped = Instantiate(_itemEquippedPrefab, _itemSkillPlayerContainer).GetComponent<ItemPlayerEquiped>();
        itemEquipped.Setup(item);
        this.itemEquipped.Add(itemEquipped);

        itemEquipped.GetComponent<Button>().onClick.AddListener(() => HandleItemClicked(item, itemEquipped));
    }

    private void HandleItemClicked(ItemData item, ItemPlayerEquiped itemEquipped)
    {
        _skillTimer.SetActive(true);
        _skillTimerImage.fillAmount = 1f;
        _skillTimerText.text = item.coolDownTime.ToString("F1");
        InteractableWithItem(false,"#917b7bff");
        var button = itemEquipped.GetComponent<Button>();
        var colors = button.colors;
        colors.disabledColor = Color.white;
        button.colors = colors;

        if (item.coolDownTime > 0)
        {
            StartCoroutine(SkillCooldownCoroutine(item.coolDownTime, item));
        }
        else
        {
            Debug.LogWarning("ItemData's coolDownTime is zero or negative, cannot start cooldown.");
        }
    }

    private IEnumerator SkillCooldownCoroutine(float cooldownTime, ItemData itemData)
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = 1f - (elapsedTime / cooldownTime);
            _skillTimerImage.fillAmount = fillAmount;
            _skillTimerText.text = (cooldownTime - elapsedTime).ToString("F1");

            _itemEquipped.EquipItem(itemData);
            _player.UpdateDamage(itemData.bonusDamagePercent);

            yield return null;
        }
        _skillTimer.SetActive(false);
        _skillTimerImage.fillAmount = 0f;
        InteractableWithItem(true, "#917b7bff");
        _itemEquipped.ClearItem();
        _player.UpdateDamage(0f);
    }

    public void InteractableWithItem(bool interactable, string color)
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(color, out newColor);

        foreach (var itemEquipped in itemEquipped)
        {
            if (itemEquipped != null)
            {
                itemEquipped.GetComponent<Button>().interactable = interactable;
                var colors = itemEquipped.GetComponent<Button>().colors;
                colors.disabledColor = newColor;
                itemEquipped.GetComponent<Button>().colors = colors;
            }
            else
            {
                Debug.LogError("ItemPlayerEquiped is null in InteractableWithItem.");
            }
        }
    }
    

    #endregion
}