using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using SevenSwords.StateMchn;
using SevenSwords.Utility;


namespace SevenSwords
{
    public class GreenBlade : IState
    {
		NewCharController owner;
		Hitbox hitboxData;
		GameObject hitboxCreationPoint;

		public GreenBlade(NewCharController owner, Hitbox hitboxData) { this.owner = owner; this.hitboxData = hitboxData; }

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
			owner._stateMachine.LockState();

			hitboxCreationTime = hitboxData.hitboxCreationTime + Time.time;
			hitboxActiveTime = hitboxData.hitboxLingeringTime + Time.time;
			enemyHit = false;
			Debug.Log("Attacking");

			//gravity affecting and can move L/R
			owner._charVariables.velocity.y = 0;
			owner._charVariables.velocity.x = 0;

		}

		public void Execute()
		{
			//create hitbox if not created and ready to be created
			if (!hitboxCreated && Time.time >= hitboxCreationTime)
			{
				//hitbox creation
				Collider2D[] collisions = owner.manager.CreatePlayerHitbox(hitboxData);
				hitboxCreated = true;
				hitboxCreatedThisFrame = true;
				owner.manager.HitboxDebug(hitboxData, Color.green);
				if (collisions.Length > 0)
				{
					foreach (Collider2D hit in collisions)
					{
						owner._stateMachine.UnlockState();
						owner._stateMachine.ChangeState(new Idle(owner));
						hit.GetComponent<Enemy>().getHit(hitboxData.damage, hitboxData.hitstun, hitboxData.colour);
					}
					enemyHit = true;
				}
			}
			else if (hitboxCreated && Time.time < hitboxActiveTime && !hitboxCreatedThisFrame && !enemyHit)
			{
				//still looking for hitbox
				Collider2D[] collisions = owner.manager.CreatePlayerHitbox(hitboxData);
				owner.manager.HitboxDebug(hitboxData, Color.green);

				if (collisions.Length > 0)
				{
					foreach (Collider2D hit in collisions)
					{
						owner._stateMachine.UnlockState();
						owner._stateMachine.ChangeState(new Idle(owner));

						hit.GetComponent<Enemy>().getHit(hitboxData.damage, hitboxData.hitstun, hitboxData.colour);
					}
					enemyHit = true;
				}
			}
			else if (hitboxCreated && Time.time >= hitboxActiveTime && !enemyHit)
			{
				//wiffed
				owner._stateMachine.UnlockState();
				owner._stateMachine.ChangeState(new Idle(owner));
			}
			hitboxCreatedThisFrame = false;
		}

		public void Exit()
		{

		}
	}
}
