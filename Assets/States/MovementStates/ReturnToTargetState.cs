using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToTargetState : IState
{
    private Transform _transform;
    private GameObject _target;
    private Movement _movement;

    private Vector3 offsetPos = Vector3.zero;
    private Vector3 from = Vector3.zero;

    public ReturnToTargetState(Transform transform, GameObject target, Movement movement)
    {
        _transform = transform;
        _target = target;
        _movement = movement;
    }

    public void OnEnter()
    {
        from = _transform.position;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        // Calculate the offset position by subtracting twice the forward vector from the target's position and adding its up vector.
        offsetPos = _target.transform.position - _target.transform.forward * 2f + _target.transform.up;

        _movement.Move(from, offsetPos);
    }

    public void DrawDebugGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, offsetPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, offsetPos);
    }

    public bool ReturnedToTarget()
    {
        // Return true if the distance between the current position of the transform and the offset position is less than 0.25f
        return Vector3.Distance(_transform.position, offsetPos) < 0.25f;
    }
}
