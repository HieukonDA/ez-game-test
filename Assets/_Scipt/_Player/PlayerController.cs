using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Player Movement")]
    public float _movementSpeed = 1f;
    public float _rotationSpeed = 10f;
    public CharacterController _characterController;
    public Vector2 _inputDirection;


    void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        CharacterController characterController = GetComponent<CharacterController>();

        _headPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.HeadPunch));
        _stomachPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.StomachPunch));
        _kidneyPunchButton.onClick.AddListener(() => CombatManager.Instance.SubmitAction(ActionType.KidneyPunchLeft));

    }

    void Update()
    {
        PerformMovement();
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

    public void InputPlayer(InputAction.CallbackContext _context)
    {

        _inputDirection = _context.ReadValue<Vector2>();
    }

    public void PerformMovement()
    {
        Vector3 movement = new Vector3(_inputDirection.x, 0, _inputDirection.y);
        movement.Normalize();
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            // _animator.SetBool("IsMoving", true);
        }
        else
        {
            // _animator.SetBool("IsMoving", false);
        }
        _characterController.Move(movement * _movementSpeed * Time.deltaTime);
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
