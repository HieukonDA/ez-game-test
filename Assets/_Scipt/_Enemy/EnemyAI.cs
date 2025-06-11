
using System.Collections;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } }
    [SerializeField] private HealthBar _healthBar;

    private ActionType _actionType;
    [SerializeField] private Animator _animator;

    void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        StartCoroutine(EnemyAction());
    }

    private IEnumerator EnemyAction()
    {
        while (_currentHealth > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            if (_currentHealth > 0) 
            {
                ActionType action = (ActionType)Random.Range(0, 4); 
                CombatManager.Instance.SubmitAction(ActionType.None); 
            }
        }
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
        if (_currentHealth <= 0) return;
        _animator.SetTrigger(hitType.ToString());
        TakeDamage(damage);
    }

    public void KnockOut()
    {
        _animator.SetTrigger("KnockedOut");
        _currentHealth = 0;
    }
}