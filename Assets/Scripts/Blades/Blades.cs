using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenSwords.Blades
{
    public abstract class Blades
    {
        public abstract void Activate();
        
        //move the player if required
        protected void Move()
        {

        }

        protected void CreateHitbox()
        {

        }

        public abstract void BladeUpdate();
    }
}
