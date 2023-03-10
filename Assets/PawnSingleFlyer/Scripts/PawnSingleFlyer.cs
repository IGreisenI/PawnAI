using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSingleFlyer : MonoBehaviour
{
    [SerializeField] private Transform model;

    [Header("Flight settings")]
    [SerializeField] private FlyerPawnMovement flyerPawnMovement;

    [Header("State settings")]
    [SerializeField] private float idleTime;
    [SerializeField] private float flyForEffectTime;
    [SerializeField] private GameObject returnTarget;
    [SerializeField] private Transform[] patrolWaypoints;

    private StateMachine _stateMachine;

    #region STATES
    private IdleState _idle;
    private PatrolState _patrol;
    private ReturnToTargetState _returnToTarget;
    private FocusTargetState _focusOnTarget;
    private FlyForEffectState _flyForEffect;
    #endregion

    void At(IState from, IState to, Func<bool> condition)
    {
        _stateMachine.AddTransition(from, to, condition);
    }

    private void Start()
    {
        _stateMachine = new StateMachine();

        //flyerPawnMovement = new FlyerPawnMovement(transform);

        _idle = new IdleState(transform, idleTime);
        _patrol = new PatrolState(transform, model, flyerPawnMovement, patrolWaypoints);
        _returnToTarget = new ReturnToTargetState(transform, returnTarget.gameObject, flyerPawnMovement);
        _focusOnTarget = new FocusTargetState(transform, model, returnTarget.transform, flyerPawnMovement);
        _flyForEffect = new FlyForEffectState(transform, model, returnTarget.transform, null, flyerPawnMovement, flyForEffectTime);

        At(_idle, _flyForEffect, _idle.IdleTimeExpired);
        //At(_idle, _patrol, _idle.IdleTimeExpired);
        At(_patrol, _idle, _patrol.PatrolFinished);

        At(_flyForEffect, _returnToTarget, _flyForEffect.FlightTimeExpired);
        At(_returnToTarget, _idle, _returnToTarget.ReturnedToTarget);

        _stateMachine.SetState(_idle);
    }

    public void Update()
    {
        _stateMachine.Tick();
    }

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
