using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultMatchHUD : MonoBehaviour
{
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _playerLevelText;
    [SerializeField] private Text _opponentNameText;
    [SerializeField] private Text _opponentLevelText;
    [SerializeField] private TMP_Text _roundNumberText;
    [SerializeField] private TMP_Text _matchResultText;
    [SerializeField] private TMP_Text _rewardText;

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

    void Start()
    {
        if (MatchData.Instance == null)
        {
            Debug.LogError("MatchData not initialized!");
            return;
        }

        // Hiển thị thông tin
        _playerNameText.text = $"Player: {MatchData.Instance.PlayerName}";
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

        // Button functionality
        upgradeButton.gameObject.SetActive(MatchData.Instance.CanUpgrade);
        upgradeButton.onClick.AddListener(() =>
        {
            Debug.Log("Upgrade button clicked.");
            SceneManager.LoadScene("UpgradeMenu");
        });

        nextButton.gameObject.SetActive(MatchData.Instance.CanProceedNext);
        nextButton.onClick.AddListener(() =>
        {
            Debug.Log("Next button clicked.");
            SceneManager.LoadScene("PreMatch");
        });

        // Cleanup sau khi hiển thị
        MatchData.Instance.Cleanup();
    }
}