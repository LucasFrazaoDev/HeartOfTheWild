using UnityEngine;

public class SpiderIdleState : SpiderBaseState
{
    private float _idleDuration = 2f;
    private float _timer;

    public override void Enter() => _timer = 0f;

    public override void Tick()
    {
        _timer += Time.deltaTime;
        if (_timer >= _idleDuration)
            m_stateMachine.SwitchToState<SpiderWalkState>();
    }

    public override void Exit() { }
}