using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToTargetState : IState
{
    private Transform _transform;
    private GameObject _target;
    private Movement _movement;

    private Vector3 offsetPos = Vector3.zero;

    public ReturnToTargetState(Transform transform, GameObject target, Movement movement)
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
        offsetPos = _target.transform.position - _target.transform.forward * 2f + _target.transform.up;

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
