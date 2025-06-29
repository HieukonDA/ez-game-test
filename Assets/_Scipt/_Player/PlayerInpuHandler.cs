using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    public Vector2 InputDirection { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsDodging { get; private set; }
    [Header("Input Settings")]
    [SerializeField] private InputAction _actionHandler;
    private PlayerController _playerController;

    private Vector2 _startTouchPos, _endTouchPos;
    private float _swipeThreshold = 50f;


    private void Awake()
    {
        _actionHandler.Enable();
    }

    private void OnEnable()
    {
        _actionHandler.started += ctx => StartTouch();
        _actionHandler.canceled += ctx => EndTouch();
    }

    private void StartTouch()
    {
        _startTouchPos = _actionHandler.ReadValue<Vector2>();
    }

    private void EndTouch()
    {
        _endTouchPos = _actionHandler.ReadValue<Vector2>();
        DetectSwipe(_endTouchPos - _startTouchPos);
    }

    void DetectSwipe(Vector2 delta)
    {
        Vector2 touchPos = _actionHandler.ReadValue<Vector2>();

        StateManager.Instance.ChangeState(new IdleState(_playerController));

        if (delta.magnitude < _swipeThreshold)
        {
            if (touchPos.x < Screen.width / 2)
            {
                Debug.Log("Vuốt trái → Đấm trái");
            }
            else
            {
                Debug.Log("Vuốt phải → Đấm phải");
            }
            return;
        }

        float x = delta.x;
        float y = delta.y;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > 0)
            {
                if (touchPos.x > Screen.width / 2)
                {
                    Debug.Log("Vuốt phải → Đấm phải");
                }
                else
                {
                    Debug.Log("Vuốt trái → Đấm trái");
                }
            }
            else
            {
                if (touchPos.x < Screen.width / 2)
                {
                    Debug.Log("Vuốt trái → Đấm trái");
                }
                else
                {
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
                    Debug.Log("Vuốt lên → Né");
                }
                else
                {
                    Debug.Log("Vuốt xuống → Né");
                }
            }
            else
            {
                if (touchPos.x < Screen.width / 2)
                {
                    Debug.Log("Vuốt xuống → Né");
                }
                else
                {
                    Debug.Log("Vuốt lên → Né");
                }
            }
        }
    }
}