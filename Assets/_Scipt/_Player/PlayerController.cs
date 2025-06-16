using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth { get { return _maxHealth; } }
    private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public HealthBar healthBar;

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

    private bool _isDisabled = false;

    void Start()
    {
        _currentHealth = _maxHealth;
        healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        CharacterController characterController = GetComponent<CharacterController>();

        _headPunchButton.onClick.AddListener(() => PerformAction(ActionType.HeadPunch));
        _stomachPunchButton.onClick.AddListener(() => PerformAction(ActionType.StomachPunch));
        _kidneyPunchButton.onClick.AddListener(() => PerformAction(ActionType.KidneyPunchLeft));

        _enemys = GameObject.FindGameObjectsWithTag("Enemy").Select(go => go.transform).ToArray();
    }

    void Update()
    {
        if (!_isDisabled)
        {
            PerformMovement();
            UpdateEnemyArray();
        }
        
    }
    
    private void UpdateEnemyArray()
    {
        _enemys = GameObject.FindGameObjectsWithTag("Enemy").Select(go => go.transform).ToArray();
        if (_enemys.Length == 0)
        {
            Debug.LogWarning("No enemies found ");
        }
    }

    // address damage to the player
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        }

        if (_currentHealth > 0)
        {
            CombatManager.Instance.OnPlayerTakeDamage();
        }
    }

    public void InputPlayer(InputAction.CallbackContext _context)
    {
        if (!_isDisabled) 
        {
            _inputDirection = _context.ReadValue<Vector2>();
        }
        else
        {
            _inputDirection = Vector2.zero; 
        }
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
        AudioManager.Instance.PlaySound("Punch");
        if (!_isDisabled && Time.time - _lastAttackTime > _attackCooldown)
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

            bool hitSuccessful = false;
            // perform hit into opponents
            foreach (Transform enemy in _enemys)
            {
                if (Vector3.Distance(transform.position, enemy.position) <= _attackRadius)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.StartCoroutine(enemyAI.ReceiveHit(action, damage));
                        hitSuccessful = true;
                    }
                }
            }

            if (hitSuccessful)
            {
                CombatManager.Instance.OnPlayerHitEnemy(); 
            }
        }

        
    }

    public IEnumerator ReceiveHit(ActionType action, int damage)
    {
        yield return new WaitForSeconds(0.5f);
        CombatManager.Instance.OnPlayerTakeDamage();

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
        Debug.Log($"Player curenrt health: {_currentHealth}");
        
        if (_currentHealth <= 0)
        {
            KnockOut();
        }
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
        if (!_isDisabled)
        {
            _animator.SetTrigger("KnockedOut");
            _currentHealth = 0;
            CombatManager.Instance.SubmitAction("player");
            Debug.Log("Player knocked out");
            StartCoroutine(LockAfterKnockout());
        }
    }

    private IEnumerator LockAfterKnockout()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _isDisabled = true; 
        _animator.SetBool("Walk", false); 
        _characterController.enabled = false; 
    }

    public void Victory()
    {
        if (!_isDisabled)
        {
            _animator.SetTrigger("Victory");
            StartCoroutine(LockAfterVictory()); 
        }
    }

    private IEnumerator LockAfterVictory()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length); 
        _isDisabled = true; 
        _animator.SetBool("Walk", false); 
        _characterController.enabled = false; 
        Debug.Log("Player locked after victory");
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
