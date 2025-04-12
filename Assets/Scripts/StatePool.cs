using System;
using System.Collections.Generic;

public class StatePool
{
    private readonly Dictionary<Type, Stack<State>> m_pool = new();
    private readonly object m_stateMachine;

    public StatePool(object stateMachine)
    {
        m_stateMachine = stateMachine;
    }

    public T GetState<T>() where T : State, new()
    {
        if (!m_pool.TryGetValue(typeof(T), out var stack) || stack.Count == 0)
        {
            var newState = new T();
            InitializeState(newState);
            return newState;
        }

        var state = (T)stack.Pop();
        if (state is IResettable resettable)
            resettable.ResetState();
        return state;
    }

    private void InitializeState(State state)
    {
        switch (m_stateMachine)
        {
            case PlayerStateMachine playerMachine when state is PlayerBaseState playerState:
                playerState.Initialize(playerMachine);
                break;

            case SpiderStateMachine spiderMachine when state is SpiderBaseState spiderState:
                spiderState.Initialize(spiderMachine);
                break;

            default:
                throw new InvalidOperationException("Combinação inválida de StateMachine e State");
        }
    }

    public void ReturnState(State state)
    {
        Type type = state.GetType();
        if (!m_pool.ContainsKey(type))
            m_pool[type] = new Stack<State>();

        m_pool[type].Push(state);
    }
}
