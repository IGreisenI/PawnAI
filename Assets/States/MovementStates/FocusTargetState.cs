using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusTargetState : IState
{
    private Transform _transform;
    private Transform _model;
    private Transform _target;
    private Movement _movement;

    private Vector3 randomPositionAroundTarget;

    public FocusTargetState(Transform transform, Transform model, Transform target, Movement movement)
    {
        _transform = transform;
        _target = target;
        _model = model;
        _movement = movement;
    }

    public void OnEnter()
    {
        randomPositionAroundTarget = Random.onUnitSphere * 5;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        if (Vector3.Distance(_transform.position, _target.position + randomPositionAroundTarget) <= 0.2f)
        {
            // Find next point on the sphere that is 0f-2f distance from last point
            randomPositionAroundTarget = (randomPositionAroundTarget + Random.onUnitSphere * 2).normalized * 5;
        }

        _movement.Move(_transform.position, _target.position + randomPositionAroundTarget);
        _model.LookAt(_target);
    }

    public void DrawDebugGizmo()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_target.position, 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_target.position + randomPositionAroundTarget, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, _target.position + randomPositionAroundTarget);
    }
}