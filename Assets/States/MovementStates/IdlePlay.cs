using System.Collections.Generic;
using UnityEngine;

public class IdlePlayState : IState
{
    private Transform _transform;
    private Transform _model;
    private float _playTime;
    private float _elapsedTime = 0f;
    private float progressbarLength = 1f;
    private Vector3 progressbarSize = new Vector3(1f, 1f);

    public IdlePlayState(Transform transform, float playTime, Transform model)
    {
        _transform = transform;
        _playTime = playTime;
        _model = model;
    }

    public void Tick()
    {
        // Update the elapsed time since entering the idle play state
        _elapsedTime += Time.deltaTime;

        _model.RotateAround(_transform.position, Vector3.up, 60 * Time.deltaTime);
        _model.RotateAround(_transform.position, Vector3.right, 60 * Time.deltaTime);
    }

    public void OnEnter()
    {
        // Reset the elapsed time since entering the idle play state
        _elapsedTime = 0f;
    }

    public void OnExit()
    {
        _model.rotation = Quaternion.identity;
        _transform.rotation = Quaternion.identity;
    }

    public bool FinishedPlaying()
    {
        return _elapsedTime > _playTime;
    }

    public void DrawDebugGizmo()
    {
        Gizmos.DrawWireCube(_transform.position + _transform.up, progressbarSize);

        progressbarLength = _elapsedTime / _playTime;
        if (progressbarLength > 0.66f) Gizmos.color = Color.black;
        else if (progressbarLength > 0.33f) Gizmos.color = Color.gray;
        else Gizmos.color = Color.white;

        Gizmos.DrawCube(_transform.position + _transform.up + new Vector3((progressbarLength / 2f) - (progressbarLength / 2f), 0, 0), new Vector3(progressbarLength, progressbarLength, progressbarSize.z));
    }
}