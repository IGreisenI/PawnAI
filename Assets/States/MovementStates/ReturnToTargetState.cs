using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToTargetState : IState
{
    private Transform _transform;
    private Transform _target;
    private Movement _movement;

    private Vector3 offsetPos = Vector3.zero;

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
        offsetPos = _target.position - _target.forward * 2f + _target.up;

        _movement.Move(_transform.position, offsetPos);
    }

    public void DrawDebugGizmo()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, offsetPos);
    }

    public bool ReturnedToTarget()
    {
        return Vector3.Distance(_transform.position, offsetPos) < 0.25f;
    }
}
