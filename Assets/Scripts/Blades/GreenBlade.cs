using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.Utility;

namespace SevenSwords.Blades
{
    public class GreenBlade : Blades
    {
        public Hitbox greenHitbox = new Hitbox
        {
            damage = 10f,
            hitboxCreationTime = 0.1f,
            hitboxLingeringTime = 0.2f,
            hitboxSize = new Vector2(0.2f, 0.2f),
            colour = Hitbox.BladeColour.white,
            hitstun = 1f
        };

        public override void Activate()
        {
            throw new System.NotImplementedException();
        }

        public override void BladeUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
