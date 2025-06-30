using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Settings")]
    private PlayerAction _actionHandler;
    private PlayerController _playerController;
    private Animator _animator;

    private Vector2 _startTouchPos, _endTouchPos;
    private float _startTouchTime;
    private float _endTouchTime;
    private float _tapTimeThreshold = 0.2f;
    private float _swipeThreshold = 30f;



    private void Awake()
    {
        _actionHandler = new PlayerAction();
        _playerController = GetComponent<PlayerController>();
        _animator = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _actionHandler.Enable();
        _actionHandler.PlayerController.TouchPress.started += ctx => StartTouch();
        _actionHandler.PlayerController.TouchPress.canceled += ctx => EndTouch();
    }

    private void StartTouch()
    {
        _startTouchTime = Time.time;
        _startTouchPos = _actionHandler.PlayerController.TouchPosition.ReadValue<Vector2>();
    }

    private void EndTouch()
    {
        _endTouchTime = Time.time;
        _endTouchPos = _actionHandler.PlayerController.TouchPosition.ReadValue<Vector2>();
        DetectSwipe(_endTouchPos - _startTouchPos, _endTouchTime - _startTouchTime);
    }

    void DetectSwipe(Vector2 delta, float duration)
    {
        Vector2 touchPos = _actionHandler.PlayerController.TouchPosition.ReadValue<Vector2>();

        StateManager.Instance.ChangeState(new IdleState(_playerController));

        // tap handler
        if (delta.magnitude < _swipeThreshold && duration < _tapTimeThreshold)
        {
            if (touchPos.x < Screen.width / 2)
            {
                StateManager.Instance.ChangeState(new LeftJabState(_playerController));
                Debug.Log("Vuốt trái → Đấm trái");
            }
            else
            {
                StateManager.Instance.ChangeState(new RightJabState(_playerController));
                Debug.Log("Vuốt phải → Đấm phải");
            }
            return;
        }


        // swipe handler

        float x = delta.x;
        float y = delta.y;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > 0)
            {
                if (touchPos.x > Screen.width / 2)
                {
                    StateManager.Instance.ChangeState(new RightHookState(_playerController));
                    Debug.Log("Vuốt phải → Đấm phải");
                }
                else
                {
                    StateManager.Instance.ChangeState(new LeftHookState(_playerController));
                    Debug.Log("Vuốt trái → Đấm trái");
                }
            }
            else
            {
                if (touchPos.x < Screen.width / 2)
                {
                    StateManager.Instance.ChangeState(new LeftHookState(_playerController));
                    Debug.Log("Vuốt trái → Đấm trái");
                }
                else
                {
                    StateManager.Instance.ChangeState(new RightHookState(_playerController));
                    Debug.Log("Vuốt phải → Đấm phải");
                }
            }
        }
        else
        {
            if (y > 0)
            {
                if (touchPos.x < Screen.width / 2)
                {
                    StateManager.Instance.ChangeState(new LeftUpperCutState(_playerController));
                    Debug.Log("Vuốt lên → muc");
                }
                else
                {
                    StateManager.Instance.ChangeState(new RightUpperCutState(_playerController));
                    Debug.Log("Vuốt xuống → Né");
                }
            }
            else
            {
                StateManager.Instance.ChangeState(new DodgeState(_playerController));
            }
        }
    }

    private float GetAnimationDuration(string clipName)
    {
        RuntimeAnimatorController controller = _animator.runtimeAnimatorController;

        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation {clipName} not found!");
        return 0.5f; // fallback
    }
}