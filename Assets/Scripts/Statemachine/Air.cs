using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;


namespace SevenSwords.StateMchn
{
	public class Air : IState
	{
		NewCharController owner;

		public Air(NewCharController owner) { this.owner = owner; }
		float xVel;
		public void Input()
		{

			xVel = 0f;
			if (owner._stateMachine.stateInputs._inputList.Count > 0)
			{
				for (int i = 0; i < owner._stateMachine.stateInputs._inputList.Count; i++)
				{
					switch (owner._stateMachine.stateInputs._inputList[i].input)
					{
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
			Debug.Log("Air");
			owner.collisionInfo.grounded = false;
		}

		public void Execute()
		{
			
			if(owner._charVariables.hasJumped && owner._charVariables.velocity.y < 0)
				owner._charVariables.velocity.y += owner._charVariables.gravity*2 * Time.deltaTime; //More gravity when falling from peak of jump
			else
				owner._charVariables.velocity.y += owner._charVariables.gravity * Time.deltaTime;

			//TODO: Redo Air X Movement
			owner._charVariables.velocity.x = 5*xVel;

			
			if (owner.collisionInfo.grounded)
			{
				owner._charVariables.velocity.y = 0;
				owner._stateMachine.ChangeState(new Idle(owner));
			}


		}

		public void Exit()
		{

		}
	}
}
