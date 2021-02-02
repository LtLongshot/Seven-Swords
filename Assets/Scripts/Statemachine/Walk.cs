﻿using UnityEngine;
using SevenSwords.CharacterCore;
namespace SevenSwords.StateMchn
{
	public class Walk : IState
	{
		CharController owner;
		float xVel;

		public Walk(CharController owner, float xVel) { this.owner = owner; this.xVel = xVel; }

		public void Enter()
		{
			//change animation
			Debug.Log("Air");
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