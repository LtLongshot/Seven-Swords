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

		public void Enter()
		{
			//change animation
			Debug.Log("Air");
			owner.collisionInfo.grounded = false;
		}

		public void Execute()
		{
			//gravity affecting
			owner._charVariables.velocity.y += owner._charVariables.gravity * Time.deltaTime;
			//TODO: Redo Air X Movement
			owner._charVariables.velocity.x = owner._currentXSpeed;

			
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
