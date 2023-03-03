using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToTargetState : IState
{
    private Transform _transform;
    private Transform _target;
    private Movement _movement;

    public ReturnToTargetState(Transform transform, Transform target, Movement movement)
    {
        _transform = transform;
        _target = target;
        _movement = movement;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        _movement.Move(_transform.position, _target.position - _target.forward);
    }

    public void DrawDebugGizmo()
    {

    }
}
