using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyForEffectState : IState
{
    private Transform _transform;
    private Transform _model;
    private Transform _target;
    private Transform _focusTarget;
    private Movement _movement;
    private float _flightTime;
    private float _elapsedTime = 0f;

    private Vector3 _targetOffset;
    private Vector3 from = Vector3.zero;

    public FlyForEffectState(Transform transform, Transform model, Transform target, Transform focusTarget, Movement movement, float flightTime)
    {
        _transform = transform;
        _target = target;
        _model = model;
        _focusTarget = focusTarget;
        _movement = movement;
        _flightTime = flightTime;
    }

    public void OnEnter()
    {
        _elapsedTime = 0f;
        from = _transform.position;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        _elapsedTime += Time.deltaTime;

        _targetOffset = _target.position + _target.forward * 2f + _target.up;

        if(Vector3.Distance(from, _targetOffset) > 0.05f)
            _movement.Move(from, _targetOffset);

        if (_focusTarget != null)
        {
            _model.LookAt(_target);
        }
    }

    public void DrawDebugGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, _targetOffset);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, _targetOffset);

        Gizmos.color = Color.cyan;
        if (_focusTarget != null)
        {
            Gizmos.DrawLine(_transform.position, _focusTarget.position);
        }
    }

    public bool FlightTimeExpired()
    {
        return _elapsedTime >= _flightTime;
    }
}
