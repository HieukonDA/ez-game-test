using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave.Models.Data.Player;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;

public class MenuManager : MonoBehaviour
{
    public static event Action OnAuthenticationComplete;
    private bool isSignedIn = false;
    private bool initialized = false;
    private bool eventsInitialized = false;

    // constants
    private const string COINS_KEY = "coins";
    private const string DIAMONDS_KEY = "diamonds";
    private const string GYM_POINTS_KEY = "gym_points";
    private const string AMOUNT_FIELD = "amount";
    private const string TIMESTAMP_FIELD = "last_updated";
    private const string DEFAULT_PLAYER_NAME = "Player";

    private static MenuManager singleton = null;

    public static MenuManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<MenuManager>();
                singleton.Initialize();
            }
            return singleton;
        }
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            Application.runInBackground = true;
            StartClientService();
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (singleton == this)
        {
            if (eventsInitialized && AuthenticationService.Instance != null)
            {
                AuthenticationService.Instance.SignedIn -= OnSignedIn;
                AuthenticationService.Instance.SignedOut -= OnSignedOut;
                AuthenticationService.Instance.Expired -= OnTokenExpired;
            }
            singleton = null;
        }
    }
    
    private void Initialize()
    {
        if (initialized) return; 
        initialized = true;
    }

    public async void StartClientService()
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions();
                options.SetProfile("default_profile");
                await UnityServices.InitializeAsync();
            }

            if (!eventsInitialized)
            {
                SetupEvents();
            }

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                SignInAnonymouslyAsync();
            }
            else
            {
                PanelManager.Open("auth");
            }
        }
        catch (Exception)
        {
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network.", "Retry");
        }
    }

    public async void SignInAnonymouslyAsync()
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign in.", "OK");
        }
        catch (RequestFailedException)
        {
            ShowError(ErrorMenu.Action.SignIn, "Failed to connect to the network.", "Retry");
        }
    }

    public async void SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException )
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException )
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }

    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException )
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException )
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        PanelManager.CloseAll();
        PanelManager.Open("auth");
    }

    private void SetupEvents()
    {
        eventsInitialized = true;
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        AuthenticationService.Instance.SignedOut += OnSignedOut;
        AuthenticationService.Instance.Expired += OnTokenExpired;
    }

    private void OnSignedIn()
    {
        isSignedIn = true;
        SignInConfirmAsync();
    }

    private void OnSignedOut()
    {
        isSignedIn = false;
        PanelManager.CloseAll();
        PanelManager.Open("auth");
    }

    private void OnTokenExpired()
    {
        SignInAnonymouslyAsync();
    }

    private void ShowError(ErrorMenu.Action action = ErrorMenu.Action.None, string error = "", string button = "")
    {
        PanelManager.Close("loading");
        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
        panel.Open(action, error, button);
    }

    private async void SignInConfirmAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Player");
            }
            PanelManager.CloseAll();
            PanelManager.Open("main");

            OnAuthenticationComplete?.Invoke();
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to update player name: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to update player name.", "OK");
        }
    }

    public bool IsSignedIn()
    {
        return isSignedIn && AuthenticationService.Instance.IsSignedIn;
    }

    #region Coins Management - Following CustomizationMenu Pattern

    /// <summary>
    /// Load coins data - Callback version (backward compatibility)
    /// </summary>
    public async void LoadData(Action<int> callback)
    {
        if (!IsSignedIn())
        {
            Debug.LogError("Not signed in when trying to load coins data.");
            callback?.Invoke(0);
            return;
        }

        try
        {
            int coins = await LoadCoinsDataAsync();
            callback?.Invoke(coins);
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadData failed: {e.Message}");
            callback?.Invoke(0);
        }
    }

    /// <summary>
    /// Load coins data async - Following CustomizationMenu pattern
    /// </summary>
    private async Task<int> LoadCoinsDataAsync()
    {
        int coins = 0;
        
        try
        {
            // Load data same way as CustomizationMenu
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { "coins" }, 
                new LoadOptions(new PublicReadAccessClassOptions())
            );

            if (playerData.TryGetValue("coins", out var coinsData))
            {
                // Parse coins data similar to character data in CustomizationMenu
                var data = coinsData.Value.GetAs<Dictionary<string, object>>();
                if (data.ContainsKey("AMOUNT_FIELD"))
                {
                    coins = int.Parse(data["AMOUNT_FIELD"].ToString());
                }
                else if (data.ContainsKey("value"))
                {
                    // Support old format
                    coins = int.Parse(data["value"].ToString());
                }
            }
            
            Debug.Log($"Loaded coins: {coins}");
        }
        catch (Exception exception)
        {
            Debug.Log($"Failed to load coins: {exception.Message}");
            coins = 0;
        }

        return coins;
    }

    /// <summary>
    /// Update player coins - Callback version (backward compatibility)
    /// </summary>
    public async void UpdatePlayerCoins(int coinsToAdd)
    {
        if (!IsSignedIn())
        {
            Debug.LogError("Not signed in when trying to update coins.");
            return;
        }

        try
        {
            await UpdatePlayerCoinsAsync(coinsToAdd);
        }
        catch (Exception e)
        {
            Debug.LogError($"UpdatePlayerCoins failed: {e.Message}");
        }
    }

    /// <summary>
    /// Update player coins async - Following CustomizationMenu pattern
    /// </summary>
    public async Task<int> UpdatePlayerCoinsAsync(int coinsToAdd)
    {
        // Load current coins first
        int currentCoins = await LoadCoinsDataAsync();
        int newCoins = currentCoins + coinsToAdd;
        return await SaveCoinsAsync(newCoins);
    }

    private async Task<int> SaveCoinsAsync(int coins)
    {
        if (!IsSignedIn())
        {
            Debug.LogError("Not signed in when trying to update coins.");
            throw new InvalidOperationException("User not signed in");
        }

        try
        {
            var playerData = new Dictionary<string, object>
            {
                { "AMOUNT_FIELD", coins },
                { "TIMESTAMP_FIELD", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") }
            };

            var data = new Dictionary<string, object>
            {
                { "coins", playerData }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(
                data,
                new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions())
            );
            
            Debug.Log($"Successfully updated coins from coins to {coins}");
            return coins;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to update coins: {exception.Message}");
            throw;
        }
    }

    /// <summary>
    /// Load coins async - Public method
    /// </summary>
    public async Task<int> LoadCoinsAsync()
    {
        return await LoadCoinsDataAsync();
    }

    /// <summary>
    /// Set coins to specific value
    /// </summary>
    public async Task<int> SetCoinsAsync(int coins)
    {
        return await SaveCoinsAsync(coins);
    }


    /// <summary>
    /// Reset coins to 0
    /// </summary>
    public async Task ResetCoinsAsync()
    {
        await SetCoinsAsync(0);
    }

    #endregion

    #region Extended Currency Management (Optional)

    /// <summary>
    /// Load all currencies (coins, diamonds, gym points)
    /// </summary>
    public async Task<(int coins, int diamonds, int gymPoints)> LoadAllCurrenciesAsync()
    {
        if (!IsSignedIn())
        {
            Debug.LogError("Not signed in when trying to load currencies.");
            return (0, 0, 0);
        }

        try
        {
            var keys = new HashSet<string> { COINS_KEY, DIAMONDS_KEY, GYM_POINTS_KEY  };
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                keys, 
                new LoadOptions(new PublicReadAccessClassOptions())
            );

            int coins = ExtractCurrencyAmount(playerData, COINS_KEY);
            int diamonds = ExtractCurrencyAmount(playerData, DIAMONDS_KEY);
            int gymPoints = ExtractCurrencyAmount(playerData, GYM_POINTS_KEY);

            Debug.Log($"Loaded currencies - Coins: {coins}, Diamonds: {diamonds}, Gym Points: {gymPoints}");
            return (coins, diamonds, gymPoints);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load currencies: {exception.Message}");
            throw;
        }
    }
    
    private int ExtractCurrencyAmount(Dictionary<string, Item> playerData, string key)
    {
        if (playerData.TryGetValue(key, out var currencyData))
        {
            var data = currencyData.Value.GetAs<Dictionary<string, object>>();
            if (data.ContainsKey("AMOUNT_FIELD"))
                return int.Parse(data["AMOUNT_FIELD"].ToString());
        }
        return 0;
    }

    /// <summary>
    /// Save all currencies at once
    /// </summary>
    public async Task SaveAllCurrenciesAsync(int coins, int diamonds, int gymPoints)
    {
        if (!IsSignedIn())
        {
            Debug.LogError("Not signed in when trying to save currencies.");
            throw new InvalidOperationException("User not signed in");
        }

        try
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var data = new Dictionary<string, object>
            {
                { "coins", new Dictionary<string, object>
                    {
                        { "AMOUNT_FIELD", coins },
                        { "TIMESTAMP_FIELD", timestamp }
                    }
                },
                { "diamonds", new Dictionary<string, object>
                    {
                        { "AMOUNT_FIELD", diamonds },
                        { "TIMESTAMP_FIELD", timestamp }
                    }
                },
                { "gym_points", new Dictionary<string, object>
                    {
                        { "AMOUNT_FIELD", gymPoints },
                        { "TIMESTAMP_FIELD", timestamp }
                    }
                }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(
                data,
                new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions())
            );

            Debug.Log($"Successfully saved all currencies - Coins: {coins}, Diamonds: {diamonds}, Gym Points: {gymPoints}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to save currencies: {exception.Message}");
            throw;
        }
    }

    #endregion
}