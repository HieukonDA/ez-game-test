
using System.Collections;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private HealthBar _healthBar;

    private ActionType _actionType;
    [SerializeField] private Animator _animator;

    void Start()
    {
        
    }

    public ActionType GenerateAction()
    {
        return (ActionType)Random.Range(0, System.Enum.GetValues(typeof(ActionType)).Length);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

        }

        if (_healthBar != null)
        {
            _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        }
    }

    public void PerformAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.KidneyPunchLeft:
                _animator.SetTrigger("KidneyPunchLeft");
                break;
            case ActionType.KidneyPunchRight:
                _animator.SetTrigger("KidneyPunchRight");
                break;
            case ActionType.HeadPunch:
                _animator.SetTrigger("HeadPunch");
                break;
            case ActionType.StomachPunch:
                _animator.SetTrigger("StomachPunch");
                break;
        }
    }
    
    public void ReceiveHit(ActionType hitType, int damage)
    {
        _animator.SetTrigger(hitType.ToString());
        TakeDamage(damage);
    }

    public void KnockOut()
    {
        _animator.SetTrigger("KnockedOut");
        _currentHealth = 0;
    }
}