using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;

public class LeaderboardsMenu : Panel
{
    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
    [SerializeField] private RectTransform playersContainer = null;
    [SerializeField] public TextMeshProUGUI pageText = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button addScoreButton = null;

    private int currentPage = 1;
    private int totalPages = 0;
    private bool isLoading = false;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearPlayersList();
        closeButton.onClick.AddListener(ClosePanel);
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        addScoreButton.onClick.AddListener(() => AddScoreAsync());
        base.Initialize();
    }

    public override void Open()
    {
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        base.Open();
        ClearPlayersList();
        currentPage = 1;
        totalPages = 0;
        LoadPlayersAsync(1);
    }

    public async void AddScoreAsync()
    {
        if (isLoading) return;

        addScoreButton.interactable = false;
        isLoading = true;

        try
        {
            if (!ValidateUserSignedIn()) return;

            // Load current coins using the new async method
            int coins = await MenuManager.Singleton.LoadCoinsAsync();

            // Add coins as score to leaderboard
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("LeaderBoard", coins);

            Debug.Log($"Successfully added score to leaderboard: {coins} coins");

            // Refresh leaderboard to show updated scores
            await LoadPlayersAsync(currentPage);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add score to leaderboard: {e.Message}");
        }
        finally
        {
            addScoreButton.interactable = true;
            isLoading = false;
        }
    }

    private async Task LoadPlayersAsync(int page)
    {
        if (isLoading) return;

        isLoading = true;
        nextButton.interactable = false;
        prevButton.interactable = false;

        try
        {
            GetScoresOptions options = new GetScoresOptions
            {
                Offset = (page - 1) * playersPerPage,
                Limit = playersPerPage
            };

            var scores = await LeaderboardsService.Instance.GetScoresAsync("LeaderBoard", options);

            // Clear existing players
            ClearPlayersList();

            // Create new player items
            for (int i = 0; i < scores.Results.Count; i++)
            {
                LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                item.Initialize(scores.Results[i]);
            }

            // Update pagination
            totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
            currentPage = page;

            Debug.Log($"Loaded leaderboard page {currentPage}/{totalPages} with {scores.Results.Count} players");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load leaderboard players: {exception.Message}");

            // Reset to default state on error
            totalPages = 0;
            currentPage = 1;
        }
        finally
        {
            // Update UI
            UpdatePaginationUI();
            isLoading = false;
        }
    }

    private void NextPage()
    {
        if (isLoading) return;

        int targetPage = currentPage + 1;
        if (targetPage > totalPages)
        {
            targetPage = 1; // Wrap to first page
        }

        LoadPlayersAsync(targetPage);
    }

    private void PrevPage()
    {
        if (isLoading) return;

        int targetPage = currentPage - 1;
        if (targetPage <= 0)
        {
            targetPage = totalPages; // Wrap to last page
        }

        LoadPlayersAsync(targetPage);
    }

    private void UpdatePaginationUI()
    {
        // Update page text
        if (totalPages > 0)
        {
            pageText.text = $"{currentPage}/{totalPages}";
        }
        else
        {
            pageText.text = "0/0";
        }

        // Update button states
        nextButton.interactable = !isLoading && totalPages > 1;
        prevButton.interactable = !isLoading && totalPages > 1;
    }
    private void ClosePanel()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        Close();
    }

    private void ClearPlayersList()
    {
        LeaderboardsPlayerItem[] items = playersContainer.GetComponentsInChildren<LeaderboardsPlayerItem>();
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    DestroyImmediate(items[i].gameObject);
                }
            }
        }
    }

    public async void RefreshAsync()
    {
        await LoadPlayersAsync(currentPage);
    }

    public async void GoToPageAsync(int pageNumber)
    {
        if (pageNumber < 1 || (totalPages > 0 && pageNumber > totalPages) || isLoading)
            return;

        await LoadPlayersAsync(pageNumber);
    }

    public (int currentPage, int totalPages, bool isLoading) GetLeaderboardState()
    {
        return (this.currentPage, this.totalPages, this.isLoading);
    }
    
    private bool ValidateUserSignedIn()
    {
        if (MenuManager.Singleton?.IsSignedIn() != true)
        {
            Debug.LogError("User not signed in");
            return false;
        }
        return true;
    }
}