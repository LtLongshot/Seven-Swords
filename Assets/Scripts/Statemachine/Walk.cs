using UnityEngine;
using SevenSwords.CharacterCore;
namespace SevenSwords.StateMchn
{
	public class Walk : IState
	{
		NewCharController owner;
		float xVel;

		public Walk(NewCharController owner, float xVel) { this.owner = owner; this.xVel = xVel; }

		public void Enter()
		{
			//change animation
			Debug.Log("walk");
		}

		public void Execute()
		{
			owner._charVariables.velocity.x = owner._currentXSpeed;
			owner._charVariables.velocity.y = owner._charVariables.gravity;

			if (!owner.collisionInfo.grounded)
			{
				owner._charVariables.velocity.y = owner._charVariables.gravity;
				owner._stateMachine.ChangeState(new Air(owner));
			}		
		}

		public void Exit()
		{

		}
	}
}