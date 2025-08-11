using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using System.Net;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _selectionButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _mailButton;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _diamondText;
    [SerializeField] private TextMeshProUGUI _gymPointText;
    
    private int _coins = 0;
    private int _diamonds = 0;
    private int _gymPoints = 0;
    private bool hasLoadedData = false;

    void Start()
    {
        SetupButtons();
        InitializeUI();

        if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
        {
            LoadPlayerDataAsync();
        }
    }

    private void SetupButtons()
    {
        // Button events
        _selectionButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            PanelManager.Open("selection");
        });

        _homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("ButtonClick");
            PanelManager.Close("setting");
            PanelManager.Close("selection");
        });

        _mailButton.onClick.AddListener(() => AudioManager.Instance.PlaySound("ButtonClick"));
    }

    private void InitializeUI()
    {
        // Initialize UI with default values
        UpdateCoinDisplay(0);
        UpdateDiamondDisplay(0);
        UpdateGymPointDisplay(0);
    }

    private void OnEnable()
    {
        // Subscribe to authentication event
        MenuManager.OnAuthenticationComplete += OnAuthenticationComplete;
    }

    private void OnDisable()
    {
        // Unsubscribe from authentication event
        MenuManager.OnAuthenticationComplete -= OnAuthenticationComplete;
    }

    private void OnAuthenticationComplete()
    {
        LoadPlayerDataAsync();
    }

    #region Data Loading Methods

    /// <summary>
    /// Load coins data using async/await (recommended)
    /// </summary>
    private async Task LoadPlayerDataAsync()
    {
        if (hasLoadedData) return;
        hasLoadedData = true;

        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                // Load only coins
                _coins = await MenuManager.Singleton.LoadCoinsAsync();
                UpdateCoinDisplay(_coins);

                Debug.Log($"Loaded coins in MainMenu: {_coins}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load player data in MainMenu: {e.Message}");
            // Set default values on error
            UpdateCoinDisplay(0);
        }
    }

    /// <summary>
    /// Load all currencies (coins, diamonds, gym points) - Extended version
    /// </summary>
    private async void LoadAllCurrenciesAsync()
    {
        if (hasLoadedData) return;
        hasLoadedData = true;

        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                var (coins, diamonds, gymPoints) = await MenuManager.Singleton.LoadAllCurrenciesAsync();
                
                _coins = coins;
                _diamonds = diamonds;
                _gymPoints = gymPoints;
                
                UpdateCoinDisplay(coins);
                UpdateDiamondDisplay(diamonds);
                UpdateGymPointDisplay(gymPoints);
                
                Debug.Log($"Loaded all currencies - Coins: {coins}, Diamonds: {diamonds}, Gym Points: {gymPoints}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load all currencies in MainMenu: {e.Message}");
            // Set default values on error
            InitializeUI();
        }
    }

    #endregion

    #region Public Refresh Methods

    /// <summary>
    /// Refresh player data (backward compatibility)
    /// </summary>
    public void RefreshPlayerData()
    {
        if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
        {
            MenuManager.Singleton.LoadData(coins =>
            {
                _coins = coins;
                UpdateCoinDisplay(coins);
            });
        }
    }

    /// <summary>
    /// Refresh coins using async/await (recommended)
    /// </summary>
    public async void RefreshCoinsAsync()
    {
        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                _coins = await MenuManager.Singleton.LoadCoinsAsync();
                UpdateCoinDisplay(_coins);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to refresh coins: {e.Message}");
        }
    }

    /// <summary>
    /// Refresh all currencies
    /// </summary>
    public async 
Task
RefreshAllCurrenciesAsync()
    {
        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                var (coins, diamonds, gymPoints) = await MenuManager.Singleton.LoadAllCurrenciesAsync();
                
                _coins = coins;
                _diamonds = diamonds;
                _gymPoints = gymPoints;
                
                UpdateCoinDisplay(coins);
                UpdateDiamondDisplay(diamonds);
                UpdateGymPointDisplay(gymPoints);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to refresh all currencies: {e.Message}");
        }
    }

    /// <summary>
    /// Force refresh - resets hasLoadedData flag and loads again
    /// </summary>
    public async void ForceRefreshAsync()
    {
        hasLoadedData = false;
        await LoadPlayerDataAsync();
    }

    #endregion

    #region UI Update Methods

    private void UpdateCoinDisplay(int coins)
    {
        if (_coinText != null)
        {
            _coinText.text = $"{coins:N0}"; // Format with thousands separator
        }
    }

    private void UpdateDiamondDisplay(int diamonds)
    {
        if (_diamondText != null)
        {
            _diamondText.text = $"{diamonds:N0}";
        }
    }

    private void UpdateGymPointDisplay(int gymPoints)
    {
        if (_gymPointText != null)
        {
            _gymPointText.text = $"{gymPoints:N0}";
        }
    }

    #endregion

    #region Currency Access Properties (Optional)

    public int Coins => _coins;
    public int Diamonds => _diamonds;
    public int GymPoints => _gymPoints;

    #endregion

    #region Example Usage Methods

    /// <summary>
    /// Example method showing how to add coins and refresh UI
    /// </summary>
    public async void AddCoins(int amount)
    {
        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                int newCoins = await MenuManager.Singleton.UpdatePlayerCoinsAsync(amount);
                _coins = newCoins;
                UpdateCoinDisplay(_coins);
                
                Debug.Log($"Added {amount} coins. New total: {newCoins}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add coins: {e.Message}");
        }
    }

    /// <summary>
    /// Example method showing how to set coins to specific value
    /// </summary>
    public async void SetCoins(int amount)
    {
        try
        {
            if (MenuManager.Singleton != null && MenuManager.Singleton.IsSignedIn())
            {
                int newCoins = await MenuManager.Singleton.SetCoinsAsync(amount);
                _coins = newCoins;
                UpdateCoinDisplay(_coins);
                
                Debug.Log($"Set coins to: {newCoins}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to set coins: {e.Message}");
        }
    }

    #endregion
}