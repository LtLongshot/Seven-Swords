using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
	CharacterController owner;

	public Idle(CharacterController owner) { this.owner = owner; }

	public void Enter()
	{
		//change animation
	}

	public void Execute()
	{
		owner._moveVar.velocity.x = 0;
		owner._moveVar.velocity.y = Mathf.Lerp(owner._moveVar.velocity.y, owner._moveVar.gravity, owner._moveVar.gravTime);

		//_moveVar.velocity.y = 0;
		owner.checkMovement();
		owner.checkJump();
	}

	public void Exit()
	{

	}
}