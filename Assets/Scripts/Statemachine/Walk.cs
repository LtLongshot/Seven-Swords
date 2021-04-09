using UnityEngine;
using SevenSwords.CharacterCore;
using SevenSwords.Utility;

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
						case (StateInputs.Inputs.attack):
							owner._stateMachine.ChangeState(new PlayerAttack(owner, (Hitbox)owner._stateMachine.stateInputs._inputList[i].arg));
							break;
						case (StateInputs.Inputs.jump):
							owner._charVariables.hasJumped = true;
							owner._charVariables.velocity.y = owner._charVariables.jumpVel;
							Debug.Log("Ooof");
							owner._stateMachine.ChangeState(new Air(owner));
							break;
						case (StateInputs.Inputs.horizontal):
							xVel = (float)owner._stateMachine.stateInputs._inputList[i].arg;
							break;
						case (StateInputs.Inputs.bladeAttack):
							owner._stateMachine.ChangeState(new BladeTransition((Hitbox)owner._stateMachine.stateInputs._inputList[i].arg, owner).Transition());
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
			owner._charVariables.velocity.x = xVel;
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