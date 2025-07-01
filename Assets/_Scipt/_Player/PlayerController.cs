using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth => _maxHealth;
    private float _currentHealth;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public HealthBar healthBar;

    [Header("Stamina Settings")]
    [SerializeField] private float _maxStamina = 100f;
    private float _currentStamina;
    [SerializeField] private float _staminaRegenRate = 10f;

    [Header("Animator")]
    public Animator Animator;

    [Header("Attack Settings")]
    [SerializeField] private AttackData[] _attacks;
    [SerializeField] private float _attackCooldown = 0.5f;
    private float _lastAttackTime;
    private bool _isDisabled;

    private EnemyAI _enemy; // Chỉ 1 enemy vì combat tĩnh

    void Start()
    {
        _currentHealth = _maxHealth;
        _currentStamina = _maxStamina;
        _enemy = FindObjectOfType<EnemyAI>(); // Lấy enemy duy nhất
        // healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        StateManager.Instance.ChangeState(new IdleState(this));
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
        AttackData attack = System.Array.Find(_attacks, a => a.actionName == actionName);
        if (attack == null || !CanAttack(attack.staminaCost)) return;

        _currentStamina -= attack.staminaCost;
        _lastAttackTime = Time.time;
        AudioManager.Instance.PlaySound("Punch");

        bool hitSuccessful = _enemy.ReceiveHit(actionName, attack.damage);
        if (hitSuccessful)
        {
            // Trigger VFX, haptic feedback
        }
    }

    public void ApplyComboBonus()
    {
        // Tăng sát thương hoặc trigger VFX đặc biệt
    }

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        if (_currentHealth <= 0)
            KnockOut();
    }

    public void TriggerAnimation(string animationName)
    {
        Animator.SetTrigger(animationName);
    }

    public void KnockOut()
    {
        if (!_isDisabled)
        {
            Animator.SetTrigger("KnockedOut");
            _isDisabled = true;
            StartCoroutine(LockAfterKnockout());
        }
    }

    private IEnumerator LockAfterKnockout()
    {
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        Animator.SetBool("Walk", false);
    }

    public void Idle() => Animator.SetTrigger("Idle");
    public void LeftJab() => Animator.SetTrigger("LeftJab");
    public void RightJab() => Animator.SetTrigger("RightJab");
    public void LeftHook() => Animator.SetTrigger("LeftHook");
    public void RightHook() => Animator.SetTrigger("RightHook");
    public void LeftUppercut() => Animator.SetTrigger("LeftUpperCut");
    public void RightUppercut() => Animator.SetTrigger("RightUpperCut");
    public void Block() => Animator.SetTrigger("Block");
    public void Dodge() => Animator.SetTrigger("Dodge");
}