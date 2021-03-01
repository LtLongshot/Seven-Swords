﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

namespace SevenSwords.StateMchn
{
	public class Idle : IState
	{
		NewCharController owner;

		public Idle(NewCharController owner) { this.owner = owner; }
		public void Input()
		{
			if (owner._stateMachine.stateInputs._inputList.Count > 0)
			{
				for (int i = 0; i < owner._stateMachine.stateInputs._inputList.Count; i++)
				{
					switch (owner._stateMachine.stateInputs._inputList[i].input)
					{
						case (StateInputs.Inputs.jump):
							owner.jump();
							break;

						case (StateInputs.Inputs.horizontal):
							owner._stateMachine.ChangeState(new Walk(owner));
							break;
					}
				}
			}
			owner._stateMachine.stateInputs.clearList();

		}
		public void Enter()
		{
			//change animation
			Debug.Log("Idle");
			owner._charVariables.velocity.x = 0;
			owner._charVariables.hasJumped = false;
		}

		public void Execute()
		{
			owner._charVariables.velocity.x = 0;
			owner._charVariables.velocity.y = owner._charVariables.gravity;

			if (!owner.collisionInfo.grounded)
			{
				owner._charVariables.velocity.y += owner._charVariables.gravity;
				owner._stateMachine.ChangeState(new Air(owner));
			}
		}

		public void Exit()
		{

		}
	}
}