using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICombatant
{
    public enum State { Idle, Attack, Dodge, Block, KnockedOut }
    private State _currentState;

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth => _maxHealth;
    [SerializeField] private float _currentHealth;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public HealthBar healthBar;

    [Header("Stamina Settings")]
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _staminaRegenRate = 10f;
    [Header("Damage Numbers")]
    [Header("Animator")]
    public Animator Animator;

    [Header("Attack Settings")]
    [SerializeField] private AttackData[] _attacks;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private ParticleSystem _hitEffect; // VFX for hits

    // catch reference to other components
    private float _lastAttackTime;
    private bool _isDisabled;
    private EnemyAI _enemy;
    private string _currentAction;
    private Dictionary<string, AttackData> _attackLookup;
    private DamageNumber _damageNumberSystem;
    public event Action<string> _onAttackHitCallBack;

    void Awake()
    {
        InitComponents();
        CacheAttackData();
    }

    private void InitComponents()
    {
        var characterController = GetComponent<CharacterController>();
        if (characterController != null)
            characterController.enabled = false; // Disable movement
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

        if (Animator != null)
            Animator.SetTrigger("Idle");
        
        StateManager.Instance.ChangeState(new IdleState(this));
        Debug.Log($"Player Position: {transform.position}, Active: {gameObject.activeSelf}");
    }

    private void InitStats()
    {
        _currentHealth = _maxHealth;
        _currentStamina = _maxStamina;
        _currentState = State.Idle;

        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        else
            Debug.LogError("HealthBar not assigned!");
    }

    private void CacheReferences()
    {
        _enemy = FindObjectOfType<EnemyAI>();
        _damageNumberSystem = FindObjectOfType<DamageNumber>();
    }

    private void ValidateComponents()
    {
        if (_enemy == null)
            Debug.LogError("EnemyAI not found in scene!");
        if (healthBar == null)
            Debug.LogError("HealthBar not assigned!");
        if (Animator == null)
            Debug.LogError("Animator not found on Player!");
        if (_damageNumberSystem == null)
            Debug.LogError("DamageNumber system not found in scene!");
    }

    public void UpdateDamage(float newBonusDamagePercent)
    {
        foreach (var attack in _attacks)
        {
            if (attack != null && attack.damage > 0)
            {
                attack.bonusDamagePercent = newBonusDamagePercent;
            }
        }
    }

    void Update()
    {
        if (!_isDisabled)
            _currentStamina = Mathf.Min(_currentStamina + _staminaRegenRate * Time.deltaTime, _maxStamina);
    }

    public bool CanAttack(float staminaCost)
    {
        return !_isDisabled && Time.time - _lastAttackTime > _attackCooldown && _currentStamina >= staminaCost;
    }

    public void PerformAttack(string actionName) //player calls this to attack
    {
        if (_enemy == null || !_attackLookup.TryGetValue(actionName, out AttackData attack)) return;

        if (!CanAttack(attack.staminaCost))
        {
            _onAttackHitCallBack?.Invoke("Stamina is too low!");
            return;
        }

        _currentStamina -= attack.staminaCost;
        _currentAction = actionName;
        _lastAttackTime = Time.time;


        // Animator.speed = 2f;
        StateManager.Instance.ChangeState(GetStateFromAction(_currentAction));
        _onAttackHitCallBack?.Invoke(_currentAction);
        
    }

    private IState GetStateFromAction(string action)
    {
        Debug.Log($"Creating state for action: {action}");
        try
        {
            return StateFactory.CreateState(action, this);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create state for {action}: {e.Message}");
            return new IdleState(this);
        }
    }

    public void OnAttackHit(string actionName) // Called by Animation Event
    {
        if (_enemy == null || !_attackLookup.TryGetValue(actionName, out AttackData attack))
            return;
        // change this to use the FinalDamage method
        bool hitSuccessful = _enemy.ReceiveHit(actionName, attack.FinalDamage());
        Debug.LogError("OnAttackHit called with action: " + actionName + ", damage: " + attack.FinalDamage());

        if (hitSuccessful)
        {
            PlayHitEffect();

            Vector3 enemyPos = _enemy.healthBar.transform.position + new Vector3(150, -50, 0);
            _damageNumberSystem?.SpawnDamageNumber(enemyPos, attack.FinalDamage(), false);
            UpdatePlayerDamage(actionName, attack.FinalDamage(), false);
        }

        _currentAction = null;
    }

    private void PlayHitEffect()
    {
        _hitEffect?.Play();
    }

    private void UpdatePlayerDamage(string actionName, int damage, bool isReceived)
    {
        if (isReceived)
            MatchData.Instance.PlayerDamageReceived += damage;
        else
            MatchData.Instance.PlayerDamageDealt += damage;

        if (actionName.Contains("Power"))
            MatchData.Instance.PowerDamagePlayer += damage;
        else if (actionName.Contains("Hook"))
            MatchData.Instance.HookDamagePlayer += damage;
        else if (actionName.Contains("Uppercut"))
            MatchData.Instance.UppercutDamagePlayer += damage;
        else if (actionName.Contains("MegaPunch"))
            MatchData.Instance.MegaPunchDamagePlayer += damage;

        MatchData.Instance.TotalDamagePlayer = 
            MatchData.Instance.PowerDamagePlayer + 
            MatchData.Instance.HookDamagePlayer +
            MatchData.Instance.UppercutDamagePlayer + 
            MatchData.Instance.MegaPunchDamagePlayer;
    }

    public void ApplyComboBonus(int damageBonus = 20)
    {
        if (_enemy != null)
            _enemy.ReceiveHit("Combo", damageBonus);
    }

    public void TakeDamage(float damage)
    {
        ReceiveHit("Hit", (int)damage);
    }

    public bool ReceiveHit(string actionName, int damage)
    {
        if (_isDisabled || _currentState == State.Dodge || _currentState == State.Block)
        {
            HandleInvulnerableHit();
            
            return false;
        }

        return ProcessDamage(actionName, damage);
    }

    private void HandleInvulnerableHit()
    {
        Animator.SetTrigger(_currentState == State.Dodge ? "Dodge" : "Block");
        AudioManager.Instance.PlaySound("Block");
        PlayHitEffect();

        Debug.LogWarning($"Player is {_currentState}, cannot receive hit with action .");
    }

    private bool ProcessDamage(string actionName, int damage)
    {
        Animator?.SetTrigger("Hit" + actionName);
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        AudioManager.Instance?.PlaySound("Hit");

        PlayHitEffect();

        healthBar?.UpdateHealthBar(_currentHealth, _maxHealth);

        Vector3 pos = healthBar.transform.position + new Vector3(-150, -50, 0);
        _damageNumberSystem?.SpawnDamageNumber(pos, damage, true);

        UpdatePlayerDamage(actionName, damage, true);

        if (_currentHealth <= 0)
            KnockOut();
        Debug.Log($"Player received hit with action {actionName}, current health: {_currentHealth}.");
        return true;

    }

    public void TriggerAnimation(string animationName)
    {
        // if (_currentState != State.Idle && animationName != "Idle") return;
        Animator.SetTrigger(animationName);
        _currentState = animationName switch
        {
            "LeftJab" or "RightJab" or "LeftHook" or "RightHook" or "LeftUpperCut" or "RightUpperCut" => State.Attack,
            "Dodge" => State.Dodge,
            "Block" => State.Block,
            "KnockedOut" => State.KnockedOut,
            _ => State.Idle
        };
    }

    public void KnockOut()
    {
        if (!_isDisabled)
        {
            _isDisabled = true;
            _currentState = State.KnockedOut;
            StateManager.Instance.ChangeState(new KnockedOutState(this));
        }
    }

    public string GetCurrentAction() => _currentAction;

    public Animator GetAnimator() => Animator;
}