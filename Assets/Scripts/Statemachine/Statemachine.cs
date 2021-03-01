﻿using UnityEngine;
namespace SevenSwords.CharacterCore
{
	public interface IState
	{
		void Enter();
		void Input();
		void Execute();
		void Exit();
	}

	public class StateMachine
	{
		//initialisation state
		public void Start()
		{
			stateInputs = new StateInputs();
		}

		public IState currentState { get; private set; }
		public IState previousState { get; private set; }

		private bool stateLocked = false;
		private float timeUnlock;

		public StateInputs stateInputs;

		public void ChangeState(IState newState)
		{
			if (!stateLocked)
			{
				if (currentState != null)
				{
					previousState = currentState;
					currentState.Exit();
				}

				currentState = newState;

				currentState.Enter();
			}
		}

		public void InputRead()
		{
			if (currentState != null)
				currentState.Input();
		}

		public void Update()
		{
			if (currentState != null)
				currentState.Execute();

			if (stateLocked)
			{
				if(Time.time >= timeUnlock)
				{
					stateLocked = false;
				}
			}
		}

		//lock a state for a set period of time (Cannot be changed)
		public void LockState(float time)
		{
			stateLocked = true;
			timeUnlock = Time.time + time;
		}
	}

	public class TestState : IState
	{
		NewCharController owner;

		public TestState(NewCharController owner) { this.owner = owner; }

		public void Input()
		{

		}

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
}