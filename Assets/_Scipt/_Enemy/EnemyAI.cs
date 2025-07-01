using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Attack, Dodge, Block }
    private State _currentState;
    private Animator _animator;
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private AttackData[] _attacks;
    [SerializeField] private float _actionCooldown = 1f;
    private float _lastActionTime;
    private PlayerController _player;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _player = FindObjectOfType<PlayerController>();
        _currentState = State.Idle;
    }

    void Update()
    {
        if (Time.time - _lastActionTime > _actionCooldown)
        {
            _lastActionTime = Time.time;
            ChooseAction();
        }
    }

    private void ChooseAction()
    {
        float random = Random.value;
        if (random < 0.5f)
            _currentState = State.Attack;
        else if (random < 0.7f)
            _currentState = State.Dodge;
        else
            _currentState = State.Block;

        switch (_currentState)
        {
            case State.Attack:
                AttackData attack = _attacks[Random.Range(0, _attacks.Length)];
                _animator.SetTrigger(attack.animationTrigger);
                StartCoroutine(PerformAttack(attack));
                break;
            case State.Dodge:
                _animator.SetTrigger("Dodge");
                break;
            case State.Block:
                _animator.SetTrigger("Block");
                break;
        }
    }

    public bool ReceiveHit(string actionName, int damage)
    {
        if (_currentState == State.Dodge || _currentState == State.Block)
            return false; // Không trúng

        _animator.SetTrigger(actionName + "Hit");
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        if (_currentHealth <= 0)
            StartCoroutine(KnockOut());
        return true;
    }

    private IEnumerator PerformAttack(AttackData attack)
    {
        yield return new WaitForSeconds(0.5f); // Chờ animation hit frame
        _player.TakeDamage(attack.damage);
    }

    private IEnumerator KnockOut()
    {
        _animator.SetTrigger("KnockedOut");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        // Trigger game over hoặc victory
    }
}