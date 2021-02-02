using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class ClimbSlope : IState
	{
		CharController owner;

		public ClimbSlope(CharController owner) { this.owner = owner; }

		public void Enter()
		{

		}

		public void Execute()
		{
			owner._moveVar.velocity.x = owner._currentXSpeed;
			owner._moveVar.velocity.y += owner._moveVar.gravity * Time.deltaTime;
		}
		public void Exit()
		{

		}

	}
}