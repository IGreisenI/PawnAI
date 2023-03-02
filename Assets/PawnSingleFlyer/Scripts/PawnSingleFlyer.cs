using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSingleFlyer : MonoBehaviour
{
    [Header("Flight settings")]
    [SerializeField] private float partrolSpeed;
    [SerializeField] private float noiseFrequency;
    [SerializeField] private float noiseMagnitude;

    [Header("State settings")]
    [SerializeField] private float idleTime;
    [SerializeField] private Transform[] patrolWaypoints;

    private StateMachine _stateMachine;
    private FlyerPawnMovement flyerPawnMovement;

    void At(IState to, IState from, Func<bool> condition)
    {
        _stateMachine.AddTransition(to, from, condition);
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();

        flyerPawnMovement = new FlyerPawnMovement(transform, noiseFrequency, noiseMagnitude);

        IdleState idleState = new IdleState(transform, idleTime);
        PatrolState patrolState = new PatrolState(transform, flyerPawnMovement, patrolWaypoints, partrolSpeed);

        Func<bool> FinishedIdling() => () => idleState.IdleTimeExpired();
        At(idleState, patrolState, FinishedIdling());

        Func<bool> FinishedPatrolling() => () => patrolState.PatrolFinished();
        At(patrolState, idleState, FinishedPatrolling());
        
        _stateMachine.SetState(idleState);
    }

    public void Update()
    {
        _stateMachine.Tick();
    }
}
