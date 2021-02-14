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
		}

		public void Execute()
		{
			//gravity affecting
			owner._charVariables.velocity.y = owner._charVariables.gravity;
			//TODO: Redo Air X Movement
			owner._charVariables.velocity.x = owner._currentXSpeed;



			if (owner.collisionInfo.below && owner._charVariables.velocity.y <= 0)
			{
				owner._charVariables.velocity.y = 0;
				owner._stateMachine.ChangeState(new Idle(owner));
			}

		}

		public void Exit()
		{
			//Debug.Log("YA");
		}
	}
}
