using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class PlayerAttack : IState
	{
		NewCharController owner;
		NewCharController.HitboxData hitboxData;
		GameObject hitboxCreationPoint;

		public PlayerAttack(NewCharController owner, NewCharController.HitboxData hitboxData) { this.owner = owner; this.hitboxData = hitboxData; }

		private float hitboxCreationTime;
		private float hitboxActiveTime;
		private bool hitboxCreated;
		private bool hitboxCreatedThisFrame;
		private bool enemyHit;

		public void Input()
		{

		}

		public void Enter()
		{
			//change animation

			hitboxCreationTime = hitboxData.hitboxCreationTime + Time.time;
			hitboxActiveTime = hitboxData.hitboxLingeringTime + Time.time;
			enemyHit = false;
		}

		public void Execute()
		{
			//gravity affecting and can move L/R
			owner._charVariables.velocity.y = 0;
			owner._charVariables.velocity.x = 0;

			//create hitbox if not created and ready to be created
			if (!hitboxCreated && Time.time >= hitboxCreationTime)
			{
				//hitbox creation
				Collider2D[] collisions = owner.CreatePlayerHitbox(hitboxData);
				hitboxCreated = true;
				hitboxCreatedThisFrame = true;
				if (collisions.Length > 0)
				{
					foreach (Collider2D hit in collisions)
					{
						//hit.GetComponent<Enemy>().getHit(hitboxData.damage, hitboxData.hitstun, hitboxData.colour);
					}
					enemyHit = true;
				}
			}
			else if (hitboxCreated && Time.time < hitboxActiveTime && !hitboxCreatedThisFrame && !enemyHit)
			{
				//still looking for hitbox
				Collider2D[] collisions = owner.CreatePlayerHitbox(hitboxData);
				if (collisions.Length > 0)
				{
					foreach (Collider2D hit in collisions)
					{
						//hit.GetComponent<Enemy>().getHit(hitboxData.damage, hitboxData.hitstun, hitboxData.colour);
					}
					enemyHit = true;
				}
			}
			else if (hitboxCreated && Time.time >= hitboxActiveTime && !enemyHit)
			{
				//wiffed
				owner._stateMachine.ChangeState(new Idle(owner));
			}

			hitboxCreatedThisFrame = false;
		}



		public void Exit()
		{

		}
	}
}