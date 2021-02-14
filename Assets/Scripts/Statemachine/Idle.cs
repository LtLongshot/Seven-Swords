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
			if(!owner.collisionInfo.grounded)
				owner._charVariables.velocity.y = owner._charVariables.gravity;
		}

		public void Exit()
		{

		}
	}
}