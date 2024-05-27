using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusTargetState : IState
{
    private Transform _transform;
    private Transform _model;
    private Transform _target;
    private Movement _movement;

    private float _focusSpeed = 1f;
    private float _focusRadius = 5f;

    private Vector3 randomPositionAroundTarget;
    private float prevSpeed;

    public FocusTargetState(Transform transform, Transform model, Transform target, Movement movement, float focusSpeed, float focusRadius)
    {
        _transform = transform;
        _target = target;
        _model = model;
        _movement = movement;
        _focusSpeed = focusSpeed;
        _focusRadius = focusRadius;
    }

    public void OnEnter()
    {
        randomPositionAroundTarget = FindNewPointOnSphere(Random.onUnitSphere * _focusRadius, 1);
        prevSpeed = _movement.GetSpeed(); // store the previous speed
        _movement.ChangeSpeed(_focusSpeed); // change the speed to the focus speed
    }

    public void OnExit()
    {
        _movement.ChangeSpeed(prevSpeed); // reset the speed to the previous speed
    }

    public void Tick()
    {
        // Check point in case it's obstructed, if it is find new one

        if (IsObstructedPoint(randomPositionAroundTarget))
        {
            randomPositionAroundTarget = FindNewPointOnSphere(randomPositionAroundTarget, 1);
        }

        if (Vector3.Distance(_transform.position, _target.position + randomPositionAroundTarget) <= 0.5f)
        {
            // Find next point on the sphere that is 0f-2f distance from last point
            randomPositionAroundTarget = FindNewPointOnSphere(randomPositionAroundTarget, 1);
            _movement.ReachedDestination();
        }

        _movement.Move(_transform.position, _target.position + randomPositionAroundTarget);
        _model.LookAt(_target.position);
    }


    public void DrawDebugGizmo()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_target.position, _focusRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_target.position + randomPositionAroundTarget, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, _target.position + randomPositionAroundTarget);
    }

    // helper function to find a new random point on the sphere
    private Vector3 FindNewPointOnSphere(Vector3 prevPoint, int maxAttempts = 10)
    {
        if (maxAttempts <= 0)
        {
            Debug.LogWarning("Max attempts reached. Could not find a valid point on the sphere.");
            return prevPoint;
        }

        Vector3 newPoint = (prevPoint + Random.onUnitSphere * 2f).normalized * _focusRadius;

        // If point is obstructed or inaccessible we try to find the point again
        if (IsObstructedPoint(newPoint))
        {
            return FindNewPointOnSphere(newPoint, maxAttempts - 1);
        }
        return newPoint;
    }

    private bool IsObstructedPoint(Vector3 point)
    {
        Ray ray = new Ray(_target.position, ((point + _target.position) - _target.position).normalized);
        RaycastHit hit;

        return Physics.Raycast(ray, out hit, ((point + _target.position) - _target.position).magnitude) || Physics.CheckSphere(_target.position + point, 1f);
    }
}