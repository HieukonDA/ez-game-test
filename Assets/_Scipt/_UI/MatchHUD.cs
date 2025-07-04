using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        _giveUpButton = _pausePanel.transform.Find("GiveUpButton").GetComponent<Button>();
        _continueButton = _pausePanel.transform.Find("ContinueButton").GetComponent<Button>();

        _pauseButton.onClick.AddListener(() =>
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        });
        _giveUpButton.onClick.AddListener(() =>
        {
            Debug.Log("Player has given up.");
            _statusText.text = "You have given up!";
            _pausePanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
            _statusText.text = lose;
            _statusText.gameObject.SetActive(true);
            StateManager.Instance.ChangeState(new DefeatState(_player));
            if (_opponent != null)
                _opponent.ChangeEnemyState(new EnemyVictoryState(_opponent));
            StartCoroutine(EndMatch());
        });
        _continueButton.onClick.AddListener(() =>
        {
            Debug.Log("Player has continued the game.");
            _pausePanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        });

        _currentTimer = matchDuration;
        _isMatchEnded = false;
        _matchTimerText.text = TimeSpan.FromSeconds(_currentTimer).ToString(@"mm\:ss");
    }

    void Update()
    {
        if (_isMatchEnded) return;

        _currentTimer -= Time.deltaTime;
        _matchTimerText.text = TimeSpan.FromSeconds(_currentTimer).ToString(@"mm\:ss");

        if (_currentTimer <= 0f || (_player.CurrentHealth <= 0) || (_opponent != null && _opponent.CurrentHealth <= 0))
    {
        _isMatchEnded = true;
        if (_player.CurrentHealth <= 0 && _opponent.CurrentHealth <= 0)
            _statusText.text = "Draw!";
        else if (_player.CurrentHealth <= 0)
            _statusText.text = lose;
        else if (_opponent.CurrentHealth <= 0)
            _statusText.text = win;
        else
            _statusText.text = "Time's Up!";
        _statusText.gameObject.SetActive(true);
        StartCoroutine(EndMatch());
    }
    }

    private IEnumerator EndMatch()
    {
        yield return new WaitForSeconds(2f); 
        Time.timeScale = 1f; 
        SceneManager.LoadScene("ResultMatch"); 
    }
}