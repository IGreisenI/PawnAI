using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private Transform _transform;
    private Transform[] _waypoints;
    private int _currentWaypointIndex;
    private bool _isPatrollingForward;

    private Movement _movement;
    private Vector3 _startingPoint;
    private Vector3 from;

    public PatrolState(Transform transform, Movement movement, Transform[] waypoints)
    {
        _transform = transform;
        _waypoints = waypoints;

        _movement = movement;

        // Set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
        _isPatrollingForward = true;
    }

    public void Tick()
    {
        // Use the FlyerPawnMovement instance to move towards the current waypoint
        _movement.Move(from, _waypoints[_currentWaypointIndex].position);

        // Check if the AI has reached the current waypoint
        float distanceToWaypoint = Vector3.Distance(_transform.position, _waypoints[_currentWaypointIndex].position);
        if (distanceToWaypoint <= 0.5f)
        {
            _movement.ReachedDestination();
            // If the AI has reached the current waypoint, switch to the next waypoint
            if (_isPatrollingForward)
            {
                from = _waypoints[_currentWaypointIndex].position;
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= _waypoints.Length)
                {
                    // If the AI has reached the end of the list of waypoints, start patrolling backward
                    _isPatrollingForward = false;
                    _currentWaypointIndex = _waypoints.Length - 2;
                }
            }
            else
            {
                from = _waypoints[_currentWaypointIndex].position;
                _currentWaypointIndex--;
            }
        }
    }

    public void OnEnter()
    {
        // When entering the patrol state, set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
        _isPatrollingForward = true;
        _startingPoint = _transform.position;
        from = _startingPoint;
    }

    public void OnExit()
    {
        // When exiting the patrol state, stop the AI from moving
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void DrawDebugGizmo()
    {
        if (PatrolFinished()) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, _waypoints[_currentWaypointIndex].position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_transform.position, _waypoints[_currentWaypointIndex].position);

        Gizmos.color = Color.white;
        foreach (Transform point in _waypoints)
        {
            Gizmos.DrawSphere(point.position, 0.1f);
        }
    }

    public bool PatrolFinished()
    {
        return _currentWaypointIndex < 0;
    }
}
