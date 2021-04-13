using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using SevenSwords.Utility;

namespace SevenSwords.StateMchn
{
	public class Idle : IState
	{
		NewCharController owner;

		public Idle(NewCharController owner) { this.owner = owner; }
		public void Input()
		{
			if (owner._stateMachine.stateInputs._inputList.Count > 0)
			{
				for (int i = 0; i < owner._stateMachine.stateInputs._inputList.Count; i++)
				{
					switch (owner._stateMachine.stateInputs._inputList[i].input)
					{
						case (StateInputs.Inputs.attack):
							owner._stateMachine.ChangeState(new PlayerAttack(owner, (Hitbox) owner._stateMachine.stateInputs._inputList[i].arg));
							break;
						case (StateInputs.Inputs.jump):
							owner._charVariables.hasJumped = true;
							owner._charVariables.velocity.y = owner._charVariables.jumpVel;
							owner._stateMachine.ChangeState(new Air(owner));
							break;

						case (StateInputs.Inputs.horizontal):
							owner._stateMachine.ChangeState(new Walk(owner));
							break;
						case (StateInputs.Inputs.bladeAttack):
							owner._stateMachine.ChangeState(new BladeTransition((Hitbox) owner._stateMachine.stateInputs._inputList[i].arg, owner).Transition());
							break;
					}
				}
			}
			owner._stateMachine.stateInputs.clearList();

		}
		public void Enter()
		{
			//change animation
			owner._charVariables.velocity.x = 0;
			owner._charVariables.hasJumped = false;
		}

		public void Execute()
		{
			owner._charVariables.velocity.y = owner._charVariables.gravity;

			if (!owner.collisionInfo.grounded)
			{
				owner._charVariables.velocity.y += owner._charVariables.gravity;
				owner._stateMachine.ChangeState(new Air(owner));
			}
		}

		public void Exit()
		{

		}
	}
}