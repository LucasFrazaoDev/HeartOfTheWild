using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State m_currentState;

    public void SwitchState(State state)
    {
        m_currentState?.Exit();
        m_currentState = state;
        m_currentState.Enter();
    }

    private void Update()
    {
        m_currentState?.Tick();
    }
}