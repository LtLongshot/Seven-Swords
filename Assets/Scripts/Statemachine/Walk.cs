using UnityEngine;
using SevenSwords.CharacterCore;
namespace SevenSwords.StateMchn
{
	public class Walk : IState
	{
		NewCharController owner;
		float xVel;

		public Walk(NewCharController owner) { this.owner = owner; }

		public void Input()
		{
			if (owner._stateMachine.stateInputs._inputList.Count == 0)
			{
				owner._stateMachine.ChangeState(new Idle(owner));
			}
			else {
				for (int i = 0; i < owner._stateMachine.stateInputs._inputList.Count; i++)
				{
					switch (owner._stateMachine.stateInputs._inputList[i].input)
					{
						case (StateInputs.Inputs.jump):
							owner.jump();
							break;

						case (StateInputs.Inputs.horizontal):
							xVel = owner._stateMachine.stateInputs._inputList[i].value;
							break;
					}
				}
			}
			owner._stateMachine.stateInputs.clearList();
		}

		public void Enter()
		{
			//change animation
			Debug.Log("walk");
		}

		public void Execute()
		{
			owner._charVariables.velocity.x = 5 * xVel;
			owner._charVariables.velocity.y = owner._charVariables.gravity;

			if (!owner.collisionInfo.grounded)
			{
				owner._charVariables.velocity.y = owner._charVariables.gravity;
				owner._stateMachine.ChangeState(new Air(owner));
			}		
		}

		public void Exit()
		{

		}
	}
}