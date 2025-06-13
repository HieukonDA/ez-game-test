using System.Collections;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } }
    [SerializeField] private HealthBar _healthBar;

    [Header("animator")]
    [SerializeField] private Animator _animator;

    [Header("Enemy Movement")]
    public float _movementSpeed = 1f;
    public float _rotationSpeed = 10f;
    public CharacterController _characterController;
    public Vector2 _inputDirection;

    [Header("Enemy Fight")]
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _dodgeDistance = 2f;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private PlayerController[] _playerControllers;
    [SerializeField] private Transform[] _players;
    [SerializeField] private float _lastAttackTime;
    private bool _isTakingDamage;

    void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        _characterController = GetComponent<CharacterController>();
        _isTakingDamage = false;
    }

    void Update()
    {
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            if (_players[i].gameObject.activeSelf && Vector3.Distance(transform.position, _players[i].position) <= _attackRadius)
            {
                _animator.SetBool("Walk", false);

                if (Time.time - _lastAttackTime > _attackCooldown)
                {
                    ActionType action = GenerateAction();
                    if (!_isTakingDamage)
                    {
                        PerformAction(action);
                    }

                    // _playerControllers[i].StartCoroutine(_playerControllers[i].ReceiveHit(action, 10));
                }
            }
            else
            {
                if (_players[i].gameObject.activeSelf)
                {
                    Vector3 movement = (_players[i].position - transform.position).normalized;
                    _characterController.Move(movement * _movementSpeed * Time.deltaTime);

                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                    _animator.SetBool("Walk", true);
                }
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
        Debug.Log($"{name} took {damage} damage, current health: {_currentHealth}");
    }

    public void PerformAction(ActionType action)
    {
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
                    _animator.SetTrigger("HeadPunch");
                    break;
                case ActionType.StomachPunch:
                    _animator.SetTrigger("StomachPunch");
                    break;
            }

            int damage = IsPunch(action);
            _lastAttackTime = Time.time;

            // perform hit into opponents
            foreach (Transform player in _players)
            {
                if (Vector3.Distance(transform.position, player.position) <= _attackRadius)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.StartCoroutine(playerController.ReceiveHit(action, damage));
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
        _currentHealth = 0;
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