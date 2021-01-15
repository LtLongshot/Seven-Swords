using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class Idle : IState
	{
		CharController owner;

		public Idle(CharController owner) { this.owner = owner; }

		public void Enter()
		{
			//change animation
		}

		public void Execute()
		{
			owner._moveVar.velocity.x = 0;
			owner._moveVar.velocity.y += owner._moveVar.gravity*Time.deltaTime;
		}

		public void Exit()
		{

		}
	}
}