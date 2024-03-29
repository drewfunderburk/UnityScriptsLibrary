using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine built from a provided enum
/// </summary>
public class EnumStateMachine<T> where T : System.Enum
{
    /**
     * Explicitly casting to and from the given enum type is problematic in a templated class, 
     * but casting first to object seems to work
     */

    private class State
    {
        public delegate void TransitionEvent();
        public TransitionEvent OnStateEnter;
        public TransitionEvent OnStateExit;
    }

    private State[] _states = new State[System.Enum.GetValues(typeof(T)).Length];

    /// <summary>
    /// The StateMachine's current state
    /// </summary>
    public T CurrentState { get; private set; }

    /// <summary>
    /// Create a new StateMachine, defaulting CurrentState to the first entry in the enum
    /// </summary>
    public EnumStateMachine() : this((T)(object)0) { }

    /// <summary>
    /// Create a new StateMachine, defaulting CurrentState to the provided state
    /// </summary>
    public EnumStateMachine(T startingState)
    {
        // Initialize the state array
        for (int i = 0; i < _states.Length; i++)
        {
            _states[i] = new State();
        }

        CurrentState = startingState;
    }

    /// <summary>
    /// Return a state object that matches the provided enum entry
    /// </summary>
    private State GetState(T state)
    {
        return _states[(int)(object)state];
    }

    /// <summary>
    /// Returns CurrentState as a State object
    /// </summary>
    /// <returns></returns>
    private State GetCurrentState()
    {
        return _states[(int)(object)CurrentState];
    }

    /// <summary>
    /// Bind a function to the given state's OnStateEnter delegate
    /// </summary>
    public void BindOnStateEnter(T state, State.TransitionEvent function)
    {
        GetState(state).OnStateEnter += function;
    }

    /// <summary>
    /// Unbind a function from the given state's OnStateEnter delegate
    /// </summary>
    public void UnbindOnStateEnter(T state, State.TransitionEvent function)
    {
        GetState(state).OnStateEnter -= function;
    }

    /// <summary>
    /// Bind a function to the given state's OnStateExit delegate
    /// </summary>
    public void BindOnStateExit(T state, State.TransitionEvent function)
    {
        GetState(state).OnStateExit += function;
    }

    /// <summary>
    /// Unbind a function from the given state's OnStateExit delegate
    /// </summary>
    public void UnbindOnStateExit(T state, State.TransitionEvent function)
    {
        GetState(state).OnStateExit -= function;
    }

    /// <summary>
    /// Transition to the given state, invoking OnStateExit for the previous state and OnStateEnter for the given state
    /// </summary>
    public void TransitionTo(T state)
    {
        GetCurrentState().OnStateExit?.Invoke();
        GetState(state).OnStateEnter?.Invoke();
        CurrentState = state;
    }
}
