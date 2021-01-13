﻿using UnityEngine;

public class Walk : IState
{
	CharacterController owner;

	public Walk(CharacterController owner) { this.owner = owner; }

	public void Enter()
	{
		//change animation
	}

	public void Execute()
	{
		owner._moveVar.velocity.x = owner._moveVar.walkspeed * owner.player.GetAxis("MoveHorizontal");
		owner._moveVar.velocity.y = Mathf.Lerp(owner._moveVar.velocity.y, owner._moveVar.gravity, owner._moveVar.gravTime);
		owner.checkJump();
		owner.checkIdle();
	}

	public void Exit()
	{

	}
}