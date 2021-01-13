﻿using UnityEngine;

public interface IState
{
	void Enter();
	void Execute();
	void Exit();
}
public class StateMachine
{

	public IState currentState { get; private set; }
	public IState previousState { get; private set; }


	public void ChangeState(IState newState)
	{
		if (currentState != null)
		{
			previousState = currentState;
			currentState.Exit();
		}

		currentState = newState;

		currentState.Enter();
	}

	public void Update()
	{
		if (currentState != null)
			currentState.Execute();
	}
}

public class TestState : IState
{
	CharacterController owner;

	public TestState(CharacterController owner) { this.owner = owner; }

	public void Enter()
	{

	}

	public void Execute()
	{

	}

	public void Exit()
	{

	}
}