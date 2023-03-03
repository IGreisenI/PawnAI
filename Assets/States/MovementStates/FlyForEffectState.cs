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
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        _elapsedTime += Time.deltaTime;

        Vector3 targetOffset = _target.position + _target.forward + _target.up * 0.2f;

        _movement.Move(_transform.position, targetOffset);

        if (_focusTarget != null)
        {
            _model.LookAt(_target);
        }
    }

    public void DrawDebugGizmo()
    {

    }

    public bool FlightTimeExpired()
    {
        return _elapsedTime >= _flightTime;
    }
}
