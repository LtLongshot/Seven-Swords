using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class PlayerAttack : IState
	{
		CharController owner;
		CharController.HitboxData hitboxData;
		GameObject hitboxCreationPoint;

		public PlayerAttack(CharController owner, CharController.HitboxData hitboxData) { this.owner = owner; this.hitboxData = hitboxData; }

		private float hitboxCreationTime;
		private float hitboxActiveTime;
		private bool hitboxCreated;
		private bool hitboxCreatedThisFrame;

		public void Enter()
		{
			//change animation

			hitboxCreationTime = hitboxData.hitboxCreationTime + Time.time;
			hitboxActiveTime = hitboxData.hitboxLingeringTime + Time.time;
		}

		public void Execute()
		{
			//gravity affecting and can move L/R
			owner._moveVar.velocity.y = 0;
			owner._moveVar.velocity.x = 0;

			//create hitbox if not created and ready to be created
			if (!hitboxCreated && Time.time >= hitboxCreationTime)
			{
				//hitbox creation
				owner.CreatePlayerHitbox(hitboxData);
				hitboxCreated = true;
				hitboxCreatedThisFrame = true;
				if (owner.CheckHitbox())
				{
					//on initial frame hit
					Debug.Log("Hit Enemy");
				}
			}
			else if (hitboxCreated && !owner.CheckHitbox() && Time.time < hitboxActiveTime)
			{
				//still looking for hitbox
				owner.CreatePlayerHitbox(hitboxData);
				if (owner.CheckHitbox())
				{
					//hitbox found
					Debug.Log("Hit Enemy2");
				}
			}
			else if (hitboxCreated && !owner.CheckHitbox() && Time.time >= hitboxActiveTime)
			{
				//wiffed
				Debug.Log("Wiffed");
				owner._stateMachine.ChangeState(new Idle(owner));
			}
		}



		public void Exit()
		{

		}
	}
}