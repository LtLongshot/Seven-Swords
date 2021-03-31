using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.Utility;
using SevenSwords.Blades;

namespace SevenSwords.CharacterCore
{
    public class CharacterManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private NewCharController charController;
        public int enemyMask = 1 << 10;//intrinsic
        public Hitbox.BladeColour currentColour;

        void Start()
        {
            charController = gameObject.GetComponent<NewCharController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //hitbox creation
        public Collider2D[] CreatePlayerHitbox(Hitbox hitbox)
        {
            //this is a lil jank for garbage collection
            //for right facing only ATM
            if (charController._charVariables.isRight)
                return Physics2D.OverlapBoxAll(transform.position + (charController._charVariables.hitboxCreationPos + new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize*2, 0, enemyMask);
            else
                return Physics2D.OverlapBoxAll(transform.position + (charController._charVariables.hitboxCreationNeg - new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize*2, 0, enemyMask);
        }

        public void HitboxDebug(Hitbox hitbox, Color color)
        {
            if (charController._charVariables.isRight)
                ExtDebug.DrawBox(transform.position + (charController._charVariables.hitboxCreationPos + new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize, Quaternion.identity, color);
            else
                ExtDebug.DrawBox(transform.position + (charController._charVariables.hitboxCreationNeg - new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize, Quaternion.identity, color);
        }
    }
}
