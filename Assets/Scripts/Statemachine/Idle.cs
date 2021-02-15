using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class Idle : IState
	{
		NewCharController owner;

		public Idle(NewCharController owner) { this.owner = owner; }

		public void Enter()
		{
			//change animation
			Debug.Log("Idle");
			owner._charVariables.velocity.x = 0;
		}

		public void Execute()
		{
			owner._charVariables.velocity.x = 0;
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