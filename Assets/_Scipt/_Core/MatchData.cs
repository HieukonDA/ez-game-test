using UnityEngine;

public class MatchData : MonoBehaviour
{
    public static MatchData Instance { get; private set; }

    public string PlayerName { get; set; }
    public int PlayerLevel { get; set; }
    public string OpponentName { get; set; }
    public int OpponentLevel { get; set; }
    public int RoundNumber { get; set; }
    public bool IsPlayerWin { get; set; }
    public string MatchResultText { get; set; }
    public string RewardText { get; set; }
    public float PlayerDamageDealt { get; set; } // Thêm tổng damage gây ra
    public float PlayerDamageReceived { get; set; } // Thêm tổng damage nhận
    public float TotalDamagePlayer { get; set; }
    public float PowerDamagePlayer { get; set; }
    public float HookDamagePlayer { get; set; }
    public float UppercutDamagePlayer { get; set; }
    public float MegaPunchDamagePlayer { get; set; }
    public float TotalDamageEnemy { get; set; }
    public float PowerDamageEnemy { get; set; }
    public float HookDamageEnemy { get; set; }
    public float UppercutDamageEnemy { get; set; }
    public float MegaPunchDamageEnemy { get; set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PrepareForMatch()
    {
        PlayerName = "";
        PlayerLevel = 0;
        OpponentName = "";
        OpponentLevel = 0;
        RoundNumber = 0;
        IsPlayerWin = false;
        MatchResultText = "";
        RewardText = "";
        PlayerDamageDealt = 0f;
        PlayerDamageReceived = 0f;
        TotalDamagePlayer = 0f;
        PowerDamagePlayer = 0f;
        HookDamagePlayer = 0f;
        UppercutDamagePlayer = 0f;
        MegaPunchDamagePlayer = 0f;
        TotalDamageEnemy = 0f;
        PowerDamageEnemy = 0f;
        HookDamageEnemy = 0f;
        UppercutDamageEnemy = 0f;
        MegaPunchDamageEnemy = 0f;
    }

    public void PersistForResult()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Cleanup()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Destroy(gameObject);
    }

    // Phương thức để cập nhật tổng damage Player
    public void UpdateTotalDamagePlayer()
    {
        TotalDamagePlayer = PowerDamagePlayer + HookDamagePlayer + UppercutDamagePlayer + MegaPunchDamagePlayer;
    }

    public void UpdateTotalDamageEnemy()
    {
        TotalDamageEnemy = PowerDamageEnemy + HookDamageEnemy + UppercutDamageEnemy + MegaPunchDamageEnemy;
    }
}