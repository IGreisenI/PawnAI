using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private readonly Transform _transform;
    private readonly float _patrolSpeed;
    private readonly Vector3[] _waypoints;
    private int _currentWaypointIndex;
    private bool _isPatrollingForward = true;

    public PatrolState(Transform transform, float patrolSpeed, Vector3[] waypoints)
    {
        _transform = transform;
        _patrolSpeed = patrolSpeed;
        _waypoints = waypoints;

        // Set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
    }

    public void Tick()
    {
        // Get the direction towards the current waypoint
        var direction = (_waypoints[_currentWaypointIndex] - _transform.position).normalized;

        // Update the AI's velocity to move in the direction of the current waypoint at the patrol speed
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = direction * _patrolSpeed;

        // Rotate the AI to face the direction of movement
        _transform.forward = direction;

        // Check if the AI has reached the current waypoint
        var distanceToWaypoint = Vector3.Distance(_transform.position, _waypoints[_currentWaypointIndex]);
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
                if (_currentWaypointIndex < 0)
                {
                    // If the AI has reached the beginning of the list of waypoints, start patrolling forward
                    _isPatrollingForward = true;
                    _currentWaypointIndex = 1;
                }
            }
        }
    }

    public void OnEnter()
    {
        // When entering the patrol state, set the initial waypoint to patrol towards
        _currentWaypointIndex = _isPatrollingForward ? 0 : _waypoints.Length - 1;
    }

    public void OnExit()
    {
        // When exiting the patrol state, stop the AI from moving
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
