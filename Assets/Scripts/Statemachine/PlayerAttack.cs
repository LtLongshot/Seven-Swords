using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class PlayerAttack : IState
	{
		CharController owner;
		CharController.HitboxData hitboxData;

		public PlayerAttack(CharController owner, CharController.HitboxData hitboxData) { this.owner = owner; this.hitboxData = hitboxData; }

		private float hitboxCreationTime;
		private bool hitboxCreated;

		public void Enter()
		{
			//change animation

			hitboxCreationTime = hitboxData.hitboxCreationTime + Time.time;
		}

		public void Execute()
		{
			//gravity affecting and can move L/R
			owner._moveVar.velocity.y += owner._moveVar.gravity * Time.deltaTime;
			owner._moveVar.velocity.x = owner._currentXSpeed;
			

		}

		public void Exit()
		{

		}
	}
}