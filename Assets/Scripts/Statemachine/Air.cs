using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;


namespace SevenSwords.StateMchn
{
	public class Air : IState
	{
		CharController owner;

		public Air(CharController owner) { this.owner = owner; }

		public void Enter()
		{
			//change animation

		}

		public void Execute()
		{
			//gravity affecting
			owner._moveVar.velocity.y += owner._moveVar.gravity * Time.deltaTime;


			//TODO: Redo Air X Movement
			owner._moveVar.velocity.x = owner._currentXSpeed;



			if (owner.collisionInfo.below && owner._moveVar.velocity.y <= 0)
			{
				owner._moveVar.velocity.y = 0;
				owner._stateMachine.ChangeState(new Idle(owner));
			}

		}

		public void Exit()
		{
			//Debug.Log("YA");
		}
	}
}
