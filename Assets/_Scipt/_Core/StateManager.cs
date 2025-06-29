using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }

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

    private IState _currentState;


    public void ChangeState(IState state)
    {
        if (_currentState != null && state.GetType() == _currentState.GetType())
            return;
            
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = state;
        if (_currentState != null)
            _currentState.Enter();
    }

    private void Update()
    {
        if(_currentState != null)
        {
            _currentState.Execute();
        }
    }




}   