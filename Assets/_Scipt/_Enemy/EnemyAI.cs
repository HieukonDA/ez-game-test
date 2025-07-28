using System.Collections;
using System.Collections.Generic;
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

    //catch reference to other components
    private float _lastActionTime;
    private PlayerController _player;
    private bool _isDisabled;
    private bool _isInitialized;
    private string _currentAction;
    private Dictionary<string, AttackData> _attackLookup;
    private DamageNumber _damageNumberSystem;
    public HealthBar healthBar;

    void Awake()
    {
        InitComponents();
        CacheAttackData();
    }

    private void InitComponents()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator not found on EnemyAI!");
    }

    private void CacheAttackData()
    {
        _attackLookup = new Dictionary<string, AttackData>();
        foreach (var attack in _attacks)
        {
            if (!string.IsNullOrEmpty(attack.actionName))
                _attackLookup[attack.actionName] = attack;
        }
    }

    void Start()
    {
        InitStats();
        CacheReferences();
        ValidateComponents();

        if (_animator != null)
            _animator.SetTrigger("Idle");

        StartCoroutine(Initialize());
        Debug.Log($"Enemy Position: {transform.position}, Active: {gameObject.activeSelf}");
    }

    private void InitStats()
    {
        _currentHealth = _maxHealth;
        _currentState = State.Idle;
        _currentEnemyState = new EnemyIdleState(this);

        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        else
            Debug.LogError("HealthBar not assigned to Enemy!");

    }

    private void CacheReferences()
    {
        _player = FindObjectOfType<PlayerController>();
        _damageNumberSystem = FindObjectOfType<DamageNumber>();
    }

    private void ValidateComponents()
    {
        if (_player == null)
            Debug.LogError("PlayerController not found in scene!");
        if (healthBar == null)
            Debug.LogError("HealthBar not assigned to Enemy!");
        if (_animator == null)
            Debug.LogError("Animator not found on Enemy!");
        if (_damageNumberSystem == null)
            Debug.LogError("DamageNumber system not found in scene!");
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

        if ((playerAction == "LeftUpperCut" ||  playerAction == "RightUpperCut")  && random < 0.6f)
        {
            PerformDefensiveAction(State.Block);
        }
        else if (playerAction == "LeftJab" && random < 0.4f)
        {
            PerformDefensiveAction(State.Dodge);
        }
        //choose a random action
    
        else if (random < 0.5f)
        {
            PerformAttack();
        }
        else if (random < 0.7f)
        {
            PerformDefensiveAction(State.Dodge);
        }
        else
        {
            PerformDefensiveAction(State.Block);
        }
    }

    private void PerformAttack()
    {
        _currentState = State.Attack;
        AttackData attack = _attacks[Random.Range(0, _attacks.Length)];
        _currentAction = attack.actionName;
        ChangeEnemyState(new EnemyAttackState(this, attack));
    }

    private void PerformDefensiveAction(State defensiveState)
    {
        _currentState = defensiveState;
        IState newState = defensiveState == State.Dodge ? 
            new EnemyDodgeState(this) : 
            new EnemyBlockState(this);
        ChangeEnemyState(newState);
    }

    public bool ReceiveHit(string actionName, int damage)
    {
        if (_isDisabled || _currentState == State.KnockedOut)
            return false;

        if (_currentState == State.Dodge || _currentState == State.Block)
        {
            HandleInvulnerableHit();
            return false;
        }

        return ProcessDamage(actionName, damage);
    }

    private void HandleInvulnerableHit()
    {
        _animator?.SetTrigger(_currentState == State.Dodge ? "Dodge" : "Block");
        AudioManager.Instance?.PlaySound("Block");
        PlayHitEffect();

        Debug.Log($"Enemy is {_currentState}, attack blocked/dodged!");
    }

    private void PlayHitEffect()
    {
        _hitEffect?.Play();
    }

    private bool ProcessDamage(string actionName, int damage)
    {
        _animator?.SetTrigger("Hit" + actionName);
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        AudioManager.Instance?.PlaySound("Hit");

        PlayHitEffect();

        UpdateEnemyDamage(actionName, damage);

        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            KnockOut();
        }

        Vector3 pos = healthBar.transform.position + new Vector3(-150, -50, 0);
        _damageNumberSystem?.SpawnDamageNumber(pos, damage, false);

        return true;
    }

    private void UpdateEnemyDamage(string actionName, int damage)
    {
        if (actionName.Contains("Power"))
            MatchData.Instance.PowerDamageEnemy += damage;
        else if (actionName.Contains("Hook"))
            MatchData.Instance.HookDamageEnemy += damage;
        else if (actionName.Contains("Uppercut"))
            MatchData.Instance.UppercutDamageEnemy += damage;
        else if (actionName.Contains("MegaPunch"))
            MatchData.Instance.MegaPunchDamageEnemy += damage;

        MatchData.Instance.TotalDamageEnemy = 
            MatchData.Instance.PowerDamageEnemy + 
            MatchData.Instance.HookDamageEnemy +
            MatchData.Instance.UppercutDamageEnemy + 
            MatchData.Instance.MegaPunchDamageEnemy;
    }

    public void TakeDamage(float damage)
    {
        ReceiveHit("Hit", (int)damage);
    }

    public void OnAttackHit(string actionName) // Called by Animation Event
    {
        if (_player == null || _isDisabled || !_attackLookup.TryGetValue(actionName, out AttackData attack))
            return;
        
        bool hitSuccessful = _player.ReceiveHit(actionName, attack.damage);

        if (hitSuccessful)
        {
            PlayHitEffect();

            UpdateEnemyDamageDealt(actionName, attack.damage);

            Vector3 playerPos = _player.healthBar.transform.position + new Vector3(-150, -50, 0);
            _damageNumberSystem?.SpawnDamageNumber(playerPos, attack.damage, false);
        }
    }

    private void UpdateEnemyDamageDealt(string actionName, int damage)
    {
        MatchData.Instance.TotalDamageEnemy += damage;

        if (actionName.Contains("Power"))
            MatchData.Instance.PowerDamageEnemy += damage;
        else if (actionName.Contains("Hook"))
            MatchData.Instance.HookDamageEnemy += damage;
        else if (actionName.Contains("Uppercut"))
            MatchData.Instance.UppercutDamageEnemy += damage;
        else if (actionName.Contains("MegaPunch"))
            MatchData.Instance.MegaPunchDamageEnemy += damage;
    }

    private void KnockOut()
    {
        if (!_isDisabled)
        {
            _isDisabled = true;
            _currentState = State.KnockedOut;
            ChangeEnemyState(new EnemyKnockedOutState(this));
        }
    }

    public string GetCurrentAction() => _currentAction;

    public Animator GetAnimator() => _animator;
}