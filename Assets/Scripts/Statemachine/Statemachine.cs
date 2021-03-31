using UnityEngine;
namespace SevenSwords.CharacterCore
{
	public interface IState
	{
		void Enter();
		void Input();
		void Execute();
		void Exit();
	}


	//Takes in a obj and inhereted members can take information and point towards the correct state accordingly
	public interface TransitionState
	{
		public IState Transition();
	}

	//test example
	class TestTransition: TransitionState
	{
		object vars;
		NewCharController owner;
		public TestTransition(object vars, NewCharController owner) { this.vars = vars; this.owner = owner; }
		public IState Transition()
		{
			//do stuff with vars and return the state to be used
			return new TestState(owner);
		}
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
		public void LockState()
		{
			stateLocked = true;
		}

		public void UnlockState()
		{
			stateLocked = false;
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