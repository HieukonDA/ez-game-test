using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Heath Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private HealthBar _healthBar;

    [Header("joystick")]
    [SerializeField] private GameObject _joystick;
    [SerializeField] private RectTransform _joystickRectTransform;
    private Vector2 moveDirection;

    [Header("animator")]
    [SerializeField] private Animator _animator;


    void Start()
    {
        _currentHealth = _maxHealth;

        if (_healthBar != null)
        {
            _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
        }

        //set active joystick
        _joystick.SetActive(false);
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
    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
            {
                _joystick.SetActive(true);
                ActionType action = (ActionType)UnityEngine.Random.Range(0, 4);
                CombatManager.Instance.SubmitAction(action);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _joystick.SetActive(false);
                moveDirection = Vector2.zero;
            }
        }
        else
        {
            _joystick.SetActive(false);
            moveDirection = Vector2.zero;
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
    }
}
