using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSingleFlyer : MonoBehaviour
{
    [SerializeField] private float idleTime;
    [SerializeField] private float partrolSpeed;
    [SerializeField] private Transform[] patrolWaypoints;

    private StateMachine _stateMachine;

    void At(IState to, IState from, Func<bool> condition)
    {
        _stateMachine.AddTransition(to, from, condition);
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();

        IdleState idleState = new IdleState(transform, idleTime);
        PatrolState patrolState = new PatrolState(transform, partrolSpeed, patrolWaypoints);

        At(idleState, patrolState, FinishedIdling());
        At(patrolState, idleState, FinishedPatrolling());
        
        _stateMachine.SetState(idleState);
        
        Func<bool> FinishedIdling() => () => idleState.IdleTimeExpired();
        Func<bool> FinishedPatrolling() => () => patrolState.PatrolFinished();
    }

    public void Update()
    {
        _stateMachine.Tick();
    }
}
