using System.Collections;
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

    [Header("Player Fight")]
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _dodgeDistance = 2f;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private Transform[] _enemys ;
    [SerializeField] private float _lastAttackTime;


    void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        CharacterController characterController = GetComponent<CharacterController>();

        _headPunchButton.onClick.AddListener(() => PerformAction(ActionType.HeadPunch));
        _stomachPunchButton.onClick.AddListener(() => PerformAction(ActionType.StomachPunch));
        _kidneyPunchButton.onClick.AddListener(() => PerformAction(ActionType.KidneyPunchLeft));
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

        // Debug.Log($"{name} took {damage} damage, current health: {_currentHealth}");
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
            _animator.SetBool("Walk", true);
        }
        else
        {
            _animator.SetBool("Walk", false);
        }
        _characterController.Move(movement * _movementSpeed * Time.deltaTime);
    }

    public void PerformAction(ActionType action)
    {
        Debug.Log($"Performing action: {action}");
        if (Time.time - _lastAttackTime > _attackCooldown)
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
                Debug.Log("Head Punch Triggered");
                    _animator.SetTrigger("HeadPunch");
                    break;
                case ActionType.StomachPunch:
                    _animator.SetTrigger("StomachPunch");
                    break;
            }

            int damage = IsPunch(action);
            _lastAttackTime = Time.time;

            // perform hit into opponents
            foreach (Transform enemy in _enemys)
            {
                if (Vector3.Distance(transform.position, enemy.position) <= _attackRadius)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.StartCoroutine(enemyAI.ReceiveHit(action, damage));
                    }
                }
            }
        }

        
    }

    public IEnumerator ReceiveHit(ActionType action, int damage)
    {
        yield return new WaitForSeconds(0.5f);
        
        switch (action)
        {
            case ActionType.KidneyPunchLeft:
                _animator.SetTrigger("KidneyHit");
                break;
            case ActionType.KidneyPunchRight:
                _animator.SetTrigger("KidneyHit");
                break;
            case ActionType.HeadPunch:
                _animator.SetTrigger("HeadHit");
                break;
            case ActionType.StomachPunch:
                _animator.SetTrigger("StomachHit");
                break;
        }
        TakeDamage(damage);
    }

    void PerformDodgeFront()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            _animator.Play("DodgeFrontAnimation");

            Vector3 dodgeDiretion = transform.forward * _dodgeDistance;

            _characterController.Move(dodgeDiretion);
        }
    }

    public void KnockOut()
    {
        _animator.SetTrigger("KnockedOut");
    }

    private int IsPunch(ActionType action)
    {
        return action switch
        {
            ActionType.HeadPunch => 10,
            ActionType.KidneyPunchLeft => 5,
            ActionType.KidneyPunchRight => 5,
            ActionType.StomachPunch => 7,
            _ => 0
        };
    }

}
