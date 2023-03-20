using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Transform _transform;
    private float _idleTime; 
    private float _elapsedTime = 0f;
    private float progressbarLength = 1f;
    private Vector3 progressbarSize = new Vector3(1f,1f);

    public IdleState(Transform transform, float idleTime)
    {
        _transform = transform;
        _idleTime = idleTime;
    }

    public void Tick()
    {
        // Update the elapsed time since entering the idle state
        _elapsedTime += Time.deltaTime;
        _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.identity, Time.deltaTime);
    }

    public void OnEnter()
    {
        // When entering the idle state, stop the flying AI from moving and facing forward
        var rigidbody = _transform.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Reset the elapsed time since entering the idle state
        _elapsedTime = 0f;
    }

    public void OnExit()
    {
        // When exiting the idle state, do nothing
    }

    public void DrawDebugGizmo()
    {
        Gizmos.DrawWireCube(_transform.position + _transform.up, progressbarSize);

        progressbarLength = _elapsedTime / _idleTime;
        if (progressbarLength > 0.66f) Gizmos.color = Color.green;
        else if (progressbarLength > 0.33f) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;

        Gizmos.DrawCube(_transform.position + _transform.up + new Vector3((progressbarLength / 2f) - (progressbarLength / 2f), 0, 0), new Vector3(progressbarLength, progressbarLength, progressbarSize.z));
    }

    public bool IdleTimeExpired()
    {
        return _elapsedTime >= _idleTime;
    }
}
