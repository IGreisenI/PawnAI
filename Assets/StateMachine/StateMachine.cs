using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState _currentState;

    // Dictionary to hold transitions for each state
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();

    // List to hold transitions available in the current state
    private List<Transition> _currentTransitions = new List<Transition>();

    // List to hold transitions that can be taken from any state
    private List<Transition> _anyTransitions = new List<Transition>();

    // List to hold an empty set of transitions (for states with no transitions)
    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    /// <summary>
    /// Advance the state machine by one tick, checking for any available transitions.
    /// </summary>
    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        // Call the Tick() method on the current state
        _currentState?.Tick();
    }

    public void SetState(IState state)
    {
        // If the new state is the same as the current state, do nothing
        if (state == _currentState)
            return;

        // Call OnExit() on the current state and set the new state
        _currentState?.OnExit();
        _currentState = state;

        // Get the available transitions for the new state
        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        // Call OnEnter() on the new state
        _currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        // If there are no existing transitions for the starting state, create a new list of transitions
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }

        // Add the new transition to the list for the starting state
        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(state, predicate));
    }

    /// <summary>
    /// Private class to represent a transition from one state to another.
    /// </summary>
    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    /// <summary>
    /// Get the first available transition, either from the current state or from any state.
    /// </summary>
    private Transition GetTransition()
    {
        // Check for any available transitions that can be taken from any state
        foreach (var transition in _anyTransitions)
            if (transition.Condition())
                return transition;

        // Check for any available transitions that can be taken from the current state
        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        // If no transition is available, return null
        return null;
    }
}