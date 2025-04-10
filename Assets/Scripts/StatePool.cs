using System;
using System.Collections.Generic;

public class StatePool
{
    private readonly Dictionary<Type, Stack<State>> _pool = new();
    private readonly object _stateMachine;

    public StatePool(object stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public T GetState<T>() where T : State, new()
    {
        if (!_pool.TryGetValue(typeof(T), out var stack) || stack.Count == 0)
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
        switch (_stateMachine)
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
        if (!_pool.ContainsKey(type))
            _pool[type] = new Stack<State>();

        _pool[type].Push(state);
    }
}

public interface IResettable
{
    void ResetState();
}