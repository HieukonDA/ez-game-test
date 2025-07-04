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
    [SerializeField] private float _comboWindow = 0.5f;
    [SerializeField] private float _swipeThreshold = 0.1f;
    [SerializeField] private float _tapTimeThreshold = 0.2f;
    [SerializeField] private ComboData[] _combos; // Assign in Inspector
    private Vector2 _startTouchPos, _endTouchPos;
    private float _startTouchTime;

    void Awake()
    {
        _actionHandler = new PlayerAction();
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
            Debug.LogError("PlayerController not found on this GameObject!");
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator not found on this GameObject!");
        _swipeThreshold = Screen.dpi * _swipeThreshold;
    }

    void OnEnable()
    {
        if (_actionHandler == null)
        {
            Debug.LogError("_actionHandler is null! Ensure PlayerAction asset is configured.");
            return;
        }
        _actionHandler.Enable();
        _actionHandler.PlayerController.TouchPress.started += ctx => StartTouch();
        _actionHandler.PlayerController.TouchPress.canceled += ctx => EndTouch();

        Debug.Log("PlayerInputHandler enabled and actions bound.");
    }

    void OnDisable()
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

    private void DetectSwipe(Vector2 delta, float duration)
    {
        if (_playerController == null) return;

        if (Time.time - _lastInputTime > _comboWindow)
            _inputQueue.Clear();

        string action = null;
        if (delta.magnitude < _swipeThreshold && duration < _tapTimeThreshold)
        {
            action = Random.value < 0.5f ? "LeftJab" : "RightJab";
        }
        else if (delta.magnitude >= _swipeThreshold)
        {
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            if (angle > 45 && angle <= 135)
            {
                if (delta.x < 0)
                    action = "LeftUpperCut";
                else if (delta.x > 0)
                    action = "RightUpperCut";
            }
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
        foreach (var combo in _combos)
        {
            if (_inputQueue.Count >= combo.sequence.Length &&
                _inputQueue.GetRange(_inputQueue.Count - combo.sequence.Length, combo.sequence.Length).SequenceEqual(combo.sequence))
            {
                Debug.Log($"Combo {string.Join("->", combo.sequence)} Activated!");
                _playerController.ApplyComboBonus(combo.damageBonus);
                _inputQueue.Clear();
                break;
            }
        }
    }

    private IState GetStateFromAction(string action)
    {
        Debug.Log($"Creating state for action: {action}");
        try
        {
            return StateFactory.CreateState(action, _playerController);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create state for {action}: {e.Message}");
            return new IdleState(_playerController);
        }
    }

    
}