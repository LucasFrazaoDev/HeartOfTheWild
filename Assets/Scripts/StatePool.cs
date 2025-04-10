using System;
using System.Collections.Generic;

public class StatePool
{
    private readonly Dictionary<Type, Stack<PlayerBaseState>> _pool = new();
    private readonly PlayerStateMachine _stateMachine;

    public StatePool(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public T GetState<T>() where T : PlayerBaseState, new()
    {
        if (!_pool.TryGetValue(typeof(T), out var stack) || stack.Count == 0)
        {
            var newState = new T();
            newState.Initialize(_stateMachine);
            return newState;
        }

        var state = (T)stack.Pop();
        state.ResetState();
        return state;
    }

    public void ReturnState(PlayerBaseState state)
    {
        Type type = state.GetType();
        if (!_pool.ContainsKey(type))
        {
            _pool[type] = new Stack<PlayerBaseState>();
        }
        _pool[type].Push(state);
    }
}