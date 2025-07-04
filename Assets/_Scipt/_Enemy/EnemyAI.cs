using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, ICombatant
{
    public enum State { Idle, Attack, Dodge, Block, KnockedOut }
    private State _currentState;
    private IState _currentEnemyState;
    private Animator _animator;
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    [SerializeField] private AttackData[] _attacks;
    [SerializeField] private float _actionCooldown = 1f;
    [SerializeField] private float _initialDelay = 2f;
    [SerializeField] private ParticleSystem _hitEffect; // VFX for hits
    private float _lastActionTime;
    private PlayerController _player;
    private bool _isDisabled;
    private bool _isInitialized;
    private string _currentAction;

    public HealthBar healthBar;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator not found on EnemyAI!");
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        _currentState = State.Idle;
        _currentEnemyState = new EnemyIdleState(this);
        _player = FindObjectOfType<PlayerController>();
        if (_player == null)
            Debug.LogError("PlayerController not found in scene!");
        _animator.SetTrigger("Idle");
        StartCoroutine(Initialize());
        Debug.Log($"Enemy Position: {transform.position}, Active: {gameObject.activeSelf}");
    }

    private IEnumerator Initialize()
    {
        _isInitialized = false;
        yield return new WaitForSeconds(_initialDelay);
        _isInitialized = true;
    }

    void Update()
    {
        if (_isDisabled || !_isInitialized) return;

        if (_currentEnemyState != null)
        {
            _currentEnemyState.Execute();
        }

        if (Time.time - _lastActionTime > _actionCooldown)
        {
            _lastActionTime = Time.time;
            ChooseAction();
        }
    }

    public void ChangeEnemyState(IState newState)
    {
        if (_currentEnemyState != null && _currentEnemyState.GetType() == typeof(EnemyKnockedOutState)) return;
        if (_currentEnemyState != null)
        {
            _currentEnemyState.Exit();
        }
        _currentEnemyState = newState;
        if (_currentEnemyState != null)
        {
            _currentEnemyState.Enter();
        }
    }

    private void ChooseAction()
    {
        if (_player == null) return;

        string playerAction = _player.GetCurrentAction();
        float random = Random.value;

        if (playerAction == "LeftUpperCut" ||  playerAction == "RightUpperCut"  && random < 0.6f)
        {
            _currentState = State.Block;
            ChangeEnemyState(new EnemyBlockState(this));
        }
        else if (playerAction == "LeftJab" && random < 0.4f)
        {
            _currentState = State.Dodge;
            ChangeEnemyState(new EnemyDodgeState(this));
        }
        else if (random < 0.5f)
        {
            _currentState = State.Attack;
            AttackData attack = _attacks[Random.Range(0, _attacks.Length)];
            _currentAction = attack.actionName;
            ChangeEnemyState(new EnemyAttackState(this, attack));
        }
        else if (random < 0.7f)
        {
            _currentState = State.Dodge;
            ChangeEnemyState(new EnemyDodgeState(this));
        }
        else
        {
            _currentState = State.Block;
            ChangeEnemyState(new EnemyBlockState(this));
        }
    }

    public bool ReceiveHit(string actionName, int damage)
    {
        if (_isDisabled || _currentState == State.Dodge || _currentState == State.Block || _currentState == State.KnockedOut)
        {
            if (_currentState != State.KnockedOut)
            {
                _animator.SetTrigger(_currentState == State.Dodge ? "Dodge" : "Block");
                AudioManager.Instance.PlaySound("Block");
                if (_hitEffect != null)
                    _hitEffect.Play();
            }
            return false;
        }

        _animator.SetTrigger("Hit" + actionName);
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        AudioManager.Instance.PlaySound("Hit");
        if (_hitEffect != null)
            _hitEffect.Play();
        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        if (_currentHealth <= 0)
        {
            ChangeEnemyState(new EnemyKnockedOutState(this));
        }
        return true;
    }

    public void TakeDamage(float damage)
    {
        ReceiveHit("Hit", (int)damage);
    }

    public void OnAttackHit(string actionName) // Called by Animation Event
    {
        if (_player == null || _isDisabled) return;
        AttackData attack = System.Array.Find(_attacks, a => a.actionName == actionName);
        if (attack != null)
        {
            _player.ReceiveHit(actionName, attack.damage);
            if (_hitEffect != null)
                _hitEffect.Play();
        }
    }

    public string GetCurrentAction() => _currentAction;

    public Animator GetAnimator() => _animator;
}