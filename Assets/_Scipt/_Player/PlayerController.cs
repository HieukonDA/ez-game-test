using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } }
    [SerializeField] private HealthBar _healthBar;

    [Header("joystick")]
    [SerializeField] private GameObject _joystick;
    [SerializeField] private RectTransform _joystickRectTransform;
    private Vector2 moveDirection;

    [Header("animator")]
    [SerializeField] private Animator _animator;

    [Header("UI Buttons")]
    [SerializeField] private Button _headPunchButton;
    [SerializeField] private Button _stomachPunchButton;
    [SerializeField] private Button _kidneyPunchButton;


    void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        _headPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.HeadPunch));
        _stomachPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.StomachPunch));
        _kidneyPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.KidneyPunchLeft));

    }

    // address damage to the player
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
    }

  
}
