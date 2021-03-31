using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenSwords.Utility
{
    public struct Hitbox
    {
        public enum BladeColour { white, green, red, blue };

        public float damage;
        public float hitboxCreationTime;
        public float hitboxLingeringTime; //Time from frame it is changed to so creation time + Lingering time
        public Vector2 hitboxSize;
        public BladeColour colour;
        public float hitstun;
        public Vector2 hitboxOffset;
    }
}
