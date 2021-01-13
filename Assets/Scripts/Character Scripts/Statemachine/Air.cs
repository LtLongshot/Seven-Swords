using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : IState
{
	CharacterController owner;

	public Air(CharacterController owner) { this.owner = owner; }

	public void Enter()
	{
		//change animation
	}

	public void Execute()
	{
		//gravity affecting
		owner._moveVar.velocity.y = Mathf.Lerp(owner._moveVar.velocity.y, owner._moveVar.gravity, owner._moveVar.gravTime);

		owner._moveVar.velocity.x = owner._moveVar.walkspeed * owner.player.GetAxis("MoveHorizontal");

		if (owner.collisionInfo.below)
		{
			Debug.Log("bing");
			owner._moveVar.velocity.y = 0;
			owner._stateMachine.ChangeState(new Idle(owner));
		}

	}

	public void Exit()
	{

	}
}
