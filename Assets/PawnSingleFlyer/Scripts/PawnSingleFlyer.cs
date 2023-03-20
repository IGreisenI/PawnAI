using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSingleFlyer : MonoBehaviour
{
    [Tooltip("Reference to the visual model of the pawn, used for rotation")]
    [SerializeField] private Transform model;

    [Tooltip("Reference to the movement script for this flyer pawn")]
    [SerializeField] private FlyerPawnMovement flyerPawnMovement;

    [Header("State settings", order = 0)]
    [SerializeField] private float idleTime;
    [SerializeField] private float playTime;

    [Tooltip("Time to fly for an effect in the FlyForEffect state")]
    [SerializeField] private float flyForEffectTime;

    [Header("Return State", order = 1)]
    [SerializeField] private GameObject returnTarget;

    [Header("Patrol State", order = 1)]
    [SerializeField] private Transform[] patrolWaypoints;

    [Header("Focus State", order = 1)]
    [SerializeField] private float focusSpeed = 1f;
    [SerializeField] private float focusRadius = 5f;

    // The state machine controlling the pawn's behavior
    private StateMachine _stateMachine;

    #region STATES
    private IdleState _idle;
    private IdlePlayState _idlePlay;
    private PatrolState _patrol;
    private ReturnToTargetState _returnToTarget;
    private FocusTargetState _focusOnTarget;
    private FlyForEffectState _flyForEffect;
    #endregion

    // Helper function to add a transition between two states in the state machine
    void At(IState from, IState to, Func<bool> condition)
    {
        _stateMachine.AddTransition(from, to, condition);
    }

    private void Start()
    {
        // Initialize the movement navmesh script
        flyerPawnMovement.NavMeshInit();

        _stateMachine = new StateMachine();

        _idle = new IdleState(transform, idleTime);
        _idlePlay = new IdlePlayState(transform, playTime, model);
        _patrol = new PatrolState(transform, flyerPawnMovement, patrolWaypoints);
        _returnToTarget = new ReturnToTargetState(transform, returnTarget.gameObject, flyerPawnMovement);
        _focusOnTarget = new FocusTargetState(transform, model, returnTarget.transform, flyerPawnMovement, focusSpeed, focusRadius);
        _flyForEffect = new FlyForEffectState(transform, model, returnTarget.transform, null, flyerPawnMovement, flyForEffectTime);


        // Add transitions between states
        At(_idle, _idlePlay, _idle.IdleTimeExpired);
        At(_idlePlay, _patrol, () => _idlePlay.FinishedPlaying());
        At(_patrol, _flyForEffect, _patrol.PatrolFinished);
        At(_flyForEffect, _returnToTarget, _flyForEffect.FlightTimeExpired);
        At(_returnToTarget, _idle, _returnToTarget.ReturnedToTarget);

        // Set the initial state to idle
        _stateMachine.SetState(_idle);
    }

    public void Update()
    {
        _stateMachine.Tick();
    }

    // Draw gizmos in the editor for debugging purposes
    private void OnDrawGizmos()
    {
        if(_stateMachine != null)
            _stateMachine.DrawStateGizmo();
        if (flyerPawnMovement != null)
            flyerPawnMovement.DebugGizmo();
    }

    [ContextMenu("ReturnToTarget")]
    public void ReturnToTarget()
    {
        _stateMachine.SetState(_returnToTarget);
    }

    [ContextMenu("FlyForEffect")]
    public void FlyForEffect()
    {
        _stateMachine.SetState(_flyForEffect);
    }

    [ContextMenu("FocusOnTarget")]
    public void FocusOnTarget()
    {
        _stateMachine.SetState(_focusOnTarget);
    }

    [ContextMenu("Idle")]
    public void Idle()
    {
        _stateMachine.SetState(_idle);
    }

    [ContextMenu("Patrol")]
    public void Patrol()
    {
        _stateMachine.SetState(_patrol);
    }
}
