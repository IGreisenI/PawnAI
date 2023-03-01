using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private readonly Transform _transform;
    private readonly float _idleTime; 
    private float _elapsedTime;

    public IdleState(Transform transform, float idleTime)
    {
        _transform = transform;
        _idleTime = idleTime;
    }

    public void Tick()
    {
        // Update the elapsed time since entering the idle state
        _elapsedTime += Time.deltaTime;

        // Check if the idle time has elapsed
        if (_elapsedTime >= _idleTime)
        {
            // If the idle time has elapsed, transition to a new state (e.g. "Patrol" state)
            var stateMachine = _transform.GetComponent<StateMachine>();
            stateMachine.SetState(new PatrolState(_transform));
        }
    }

    public void OnEnter()
    {
        // When entering the idle state, stop the flying AI from moving and facing forward
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        _transform.forward = Vector3.forward;

        // Reset the elapsed time since entering the idle state
        _elapsedTime = 0f;
    }

    public void OnExit()
    {
        // When exiting the idle state, do nothing
    }
}
