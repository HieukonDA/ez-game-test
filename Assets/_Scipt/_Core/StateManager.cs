using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }
    private IState _currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(IState state)
    {
        if (_currentState != null && _currentState.GetType() == typeof(KnockedOutState)) return; // Prevent state change during KnockOut
        if (_currentState != null && state != null && state.GetType() == _currentState.GetType()) return;

        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = state;
        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }
}