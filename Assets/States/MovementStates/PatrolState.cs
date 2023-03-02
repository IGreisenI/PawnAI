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
    private float _patrolSpeed;

    public PatrolState(Transform transform, Movement movement, Transform[] waypoints, float patrolSpeed)
    {
        _transform = transform;
        _waypoints = waypoints;

        _movement = movement;
        _patrolSpeed = patrolSpeed;

        // Set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
        _isPatrollingForward = true;
    }

    public void Tick()
    {
        // Get the direction towards the current waypoint
        var direction = (_waypoints[_currentWaypointIndex].position - _transform.position).normalized;

        // Use the FlyerPawnMovement instance to move towards the current waypoint
        _movement.Move(_waypoints[_currentWaypointIndex], _patrolSpeed, 180f);

        // Check if the AI has reached the current waypoint
        var distanceToWaypoint = Vector3.Distance(_transform.position, _waypoints[_currentWaypointIndex].position);
        if (distanceToWaypoint <= 0.5f)
        {
            // If the AI has reached the current waypoint, switch to the next waypoint
            if (_isPatrollingForward)
            {
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
                _currentWaypointIndex--;
            }
        }
    }

    public void OnEnter()
    {
        // When entering the patrol state, set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
        _isPatrollingForward = true;
    }

    public void OnExit()
    {
        // When exiting the patrol state, stop the AI from moving
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public bool PatrolFinished()
    {
        return _currentWaypointIndex < 0;
    }
}
