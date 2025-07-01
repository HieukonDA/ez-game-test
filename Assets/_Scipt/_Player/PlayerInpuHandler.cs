using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Settings")]
    private PlayerAction _actionHandler;
    private PlayerController _playerController;
    private Animator _animator;
    private List<string> _inputQueue = new List<string>();
    private float _lastInputTime;
    private float _comboWindow = 0.5f;
    private float _swipeThreshold;
    private float _tapTimeThreshold = 0.2f;
    private Vector2 _startTouchPos, _endTouchPos;
    private float _startTouchTime;



    private void Awake()
    {
        _actionHandler = new PlayerAction();
        _playerController = GetComponent<PlayerController>();
        _animator = this.GetComponent<Animator>();
        _swipeThreshold = Screen.dpi * 0.1f;
    }

    private void OnEnable()
    {
        _actionHandler.Enable();
        _actionHandler.PlayerController.TouchPress.started += ctx => StartTouch();
        _actionHandler.PlayerController.TouchPress.canceled += ctx => EndTouch();
    }

    private void OnDisable()
    {
        _actionHandler.Disable();
    }

    private void StartTouch()
    {
        _startTouchTime = Time.time;
        _startTouchPos = _actionHandler.PlayerController.TouchPosition.ReadValue<Vector2>();
    }

    private void EndTouch()
    {
        _endTouchPos = _actionHandler.PlayerController.TouchPosition.ReadValue<Vector2>();
        float duration = Time.time - _startTouchTime;
        Vector2 delta = _endTouchPos - _startTouchPos;
        DetectSwipe(delta, duration);
    }

    void DetectSwipe(Vector2 delta, float duration)
    {
        if (Time.time - _lastInputTime > _comboWindow)
            _inputQueue.Clear();

        string action = null;
        if (delta.magnitude < _swipeThreshold && duration < _tapTimeThreshold)
        {
            action = UnityEngine.Random.value < 0.5f ? "LeftJab" : "RightJab";
        }
        else if (delta.magnitude >= _swipeThreshold)
        {
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            if (angle > 45 && angle <= 135)
                action = "Uppercut";
            else if (angle > -135 && angle <= -45)
                action = "Dodge";
            else if (angle > 135 || angle <= -135)
                action = "LeftHook";
            else
                action = "RightHook";
        }

        if (action != null)
        {
            _inputQueue.Add(action);
            _lastInputTime = Time.time;
            StateManager.Instance.ChangeState(GetStateFromAction(action));
            CheckCombo();
        }
    }

    private void CheckCombo()
    {
        if (_inputQueue.Count >= 3 && _inputQueue.GetRange(_inputQueue.Count - 3, 3).SequenceEqual(new[] { "LeftJab", "RightHook", "Uppercut" }))
        {
            Debug.Log("Combo Activated!");
            _playerController.ApplyComboBonus();
            _inputQueue.Clear();
        }
    }

    private IState GetStateFromAction(string action)
    {
        switch (action)
        {
            case "LeftJab": return new LeftJabState(_playerController);
            case "RightJab": return new RightJabState(_playerController);
            case "LeftHook": return new LeftHookState(_playerController);
            case "RightHook": return new RightHookState(_playerController);
            case "Uppercut": return new LeftUpperCutState(_playerController);
            case "Dodge": return new DodgeState(_playerController);
            default: return new IdleState(_playerController);
        }
    }
}