using System.Collections;
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
    private float _currentStamina;
    [SerializeField] private float _staminaRegenRate = 10f;
    [Header("Damage Numbers")]
    [Header("Animator")]
    public Animator Animator;

    [Header("Attack Settings")]
    [SerializeField] private AttackData[] _attacks;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private ParticleSystem _hitEffect; // VFX for hits
    private float _lastAttackTime;
    private bool _isDisabled;
    private EnemyAI _enemy;
    private string _currentAction;

    void Awake()
    {
        var characterController = GetComponent<CharacterController>();
        if (characterController != null)
            characterController.enabled = false; // Disable movement
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        _currentStamina = _maxStamina;
        _currentState = State.Idle;
        _enemy = FindObjectOfType<EnemyAI>();
        if (_enemy == null)
            Debug.LogError("EnemyAI not found in scene!");
        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        else
            Debug.LogWarning("HealthBar not assigned!");
        if (Animator == null)
            Debug.LogError("Animator not found on Player!");
        Animator.SetTrigger("Idle");
        StateManager.Instance.ChangeState(new IdleState(this));
        Debug.Log($"Player Position: {transform.position}, Active: {gameObject.activeSelf}");
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

    public void PerformHit(string actionName)
    {
        if (_enemy == null) return;
        _currentAction = actionName;
        AttackData attack = System.Array.Find(_attacks, a => a.actionName == actionName);
        if (attack == null || !CanAttack(attack.staminaCost)) return;

        _currentStamina -= attack.staminaCost;
        _lastAttackTime = Time.time;
        AudioManager.Instance.PlaySound("Punch");
        Animator.SetTrigger(attack.animationTrigger);
        // Hit detection triggered by Animation Event
    }

    public void OnAttackHit(string actionName) // Called by Animation Event
    {
        if (_enemy == null) return;
        AttackData attack = System.Array.Find(_attacks, a => a.actionName == actionName);
        if (attack == null) return;

        bool hitSuccessful = _enemy.ReceiveHit(actionName, attack.damage);
        if (hitSuccessful && _hitEffect != null)
            _hitEffect.Play();

        if (hitSuccessful)
        {
            Vector3 enemyPos = _enemy.healthBar.transform.position + new Vector3(150, -50, 0);
            FindObjectOfType<DamageNumber>().SpawnDamageNumber(enemyPos, attack.damage, false);
        }

        _currentAction = null;
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
            Animator.SetTrigger(_currentState == State.Dodge ? "Dodge" : "Block");
            AudioManager.Instance.PlaySound("Block");
            if (_hitEffect != null)
                _hitEffect.Play();
            Debug.LogWarning($"Player is {_currentState}, cannot receive hit with action {actionName}.");
            return false;
        }

        Animator.SetTrigger("Hit" + actionName);
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        AudioManager.Instance.PlaySound("Hit");
        if (_hitEffect != null)
            _hitEffect.Play();
        if (healthBar != null)
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        Vector3 pos = healthBar.transform.position + new Vector3(-150, -50, 0);
        FindObjectOfType<DamageNumber>().SpawnDamageNumber(pos, damage, true);

        if (_currentHealth <= 0)
            KnockOut();
        Debug.Log($"Player received hit with action {actionName}, current health: {_currentHealth}.");
        return true;
    }

    public void TriggerAnimation(string animationName)
    {
        if (_currentState != State.Idle && animationName != "Idle") return;
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