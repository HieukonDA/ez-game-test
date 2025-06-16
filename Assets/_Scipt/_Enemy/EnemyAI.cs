using System.Collections;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Header("Heath Settings")]
    public float maxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } }
    [SerializeField] private HealthBar _healthBar;

    [Header("animator")]
    [SerializeField] private Animator _animator;

    [Header("Enemy Movement")]
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;
    public CharacterController _characterController;
    public Vector2 _inputDirection;

    [Header("Enemy Fight")]
    public float attackCooldown = 0.5f;
    public float dodgeDistance = 2f;
    public float attackRadius = 1f;
    [SerializeField] private PlayerController[] _playerControllers;
    [SerializeField] private Transform[] _players;
    [SerializeField] private float _lastAttackTime;
    private bool _isTakingDamage;
    private bool _isDisabled = false;

    void Start()
    {
        _currentHealth = maxHealth;
        _healthBar.UpdateHealthBar(_currentHealth, maxHealth);

        _characterController = GetComponent<CharacterController>();
        _isTakingDamage = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _players = new Transform[] { player.transform };
            _playerControllers = new PlayerController[] { player.GetComponent<PlayerController>() };
        }
    }

    void Update()
    {
        if (!_isDisabled)
        {
            for (int i = 0; i < _playerControllers.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, _players[i].position);
                if (distance > 10f) return;
                if (_players[i].gameObject.activeSelf && distance <= attackRadius)
                {
                    _animator.SetBool("Walk", false);

                    ActionType action = GenerateAction();
                    if (!_isTakingDamage)
                    {
                        PerformAction(action, _players[i]);
                    }
                }
                else
                {
                    if (_players[i].gameObject.activeSelf)
                    {
                        Vector3 movement = (_players[i].position - transform.position).normalized;
                        _characterController.Move(movement * movementSpeed * Time.deltaTime);

                        Quaternion targetRotation = Quaternion.LookRotation(movement);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                        _animator.SetBool("Walk", true);
                    }
                }
            }
        }
    }

    public ActionType GenerateAction()
    {
        return (ActionType)Random.Range(0, 4);
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
            _healthBar.UpdateHealthBar(_currentHealth, maxHealth);
        }
        Debug.Log($"{name} took {damage} damage, current health: {_currentHealth}");
    }

    public void PerformAction(ActionType action, Transform target)
    {
        if (!_isDisabled && Time.time - _lastAttackTime > attackCooldown)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 5f)
            {
                switch (action)
                {
                    case ActionType.KidneyPunchLeft:
                        _animator.SetTrigger("KidneyPunchLeft");
                        AudioManager.Instance.PlaySound("Punch");
                        break;
                    case ActionType.KidneyPunchRight:
                        _animator.SetTrigger("KidneyPunchRight");
                        AudioManager.Instance.PlaySound("Punch");
                        break;
                    case ActionType.HeadPunch:
                        _animator.SetTrigger("HeadPunch");
                        AudioManager.Instance.PlaySound("Punch");
                        break;
                    case ActionType.StomachPunch:
                        _animator.SetTrigger("StomachPunch");
                        AudioManager.Instance.PlaySound("Punch");
                        break;
                }

                int damage = IsPunch(action);
                _lastAttackTime = Time.time;

                if (Vector3.Distance(transform.position, target.position) <= attackRadius)
                {
                    PlayerController playerController = target.GetComponent<PlayerController>();
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

            Vector3 dodgeDiretion = transform.forward * dodgeDistance;

            _characterController.Move(dodgeDiretion);
        }
    }

    public void KnockOut()
    {
        if (!_isDisabled)
        {
            _animator.SetTrigger("KnockedOut");
            _currentHealth = 0;
            CombatManager.Instance.SubmitAction("enemy");
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