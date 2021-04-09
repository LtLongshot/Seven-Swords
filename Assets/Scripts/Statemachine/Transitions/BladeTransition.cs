using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.Utility;
using SevenSwords.StateMchn;
using SevenSwords.CharacterCore;

namespace SevenSwords
{
    public class BladeTransition : TransitionState
	{
		Hitbox hitbox;
		NewCharController owner;
		public BladeTransition(Hitbox hitbox, NewCharController owner) { this.hitbox= hitbox; this.owner = owner; }
		public IState Transition()
		{
			//do stuff with vars and return the state to be used
			switch (hitbox.colour)
			{
				case (Hitbox.BladeColour.green):
					return new GreenBlade(owner, hitbox);
					
			}
			return new TestState(owner);
		}
	}
}
