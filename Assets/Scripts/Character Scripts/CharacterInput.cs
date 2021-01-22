using UnityEngine;
using Rewired;

namespace SevenSwords.CharacterCore{
    public class CharacterInput : MonoBehaviour
    {
        //Ground Speed
        public float slowWalkSpeed = 2f;
        public float walkspeed = 5f;
        public float runspeed = 6;
        public float jumpPower = 20f;

        const float jumpApexTime = 0.3f;
        const float jumpHeights = 1f;

        private float currentSpeed;

        private CharController charController;

        public int playerID = 0;
        public Rewired.Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(playerID) : null; } }

        void Start()
        {
            //put this here for now needs to go into startup
            Application.targetFrameRate = 60;

            charController = gameObject.GetComponent<CharController>();
            if(charController == null)
            {
                Debug.Log("No Char controller dummy");
            }

            charController.setJumpValues(jumpHeights, jumpApexTime);

            HitboxSetup();

        }

        // Update is called once per frame
        void Update()
        {

            CheckIdle();
            CheckHorizontalInput();
            CheckJump();
            PlayerBasicAttack();

        }

        void CheckHorizontalInput()
        {
            if (player.GetAxis("MoveHorizontal") != 0)
            {
                if (!player.GetButton("WalkMod"))
                {
                    currentSpeed = player.GetAxis("MoveHorizontal") * slowWalkSpeed;
                    charController.horizontalMove(currentSpeed);
                }
                else
                {
                    currentSpeed = player.GetAxis("MoveHorizontal") * walkspeed;
                    charController.horizontalMove(currentSpeed);
                }
            }
        }

        void CheckIdle()
        {
            if (player.GetAxis("MoveHorizontal") == 0 && player.GetAxis("MoveVertical") == 0)
            {
                charController.checkIdle();
            }

        }

        void CheckJump()
        {
            if (player.GetButtonDown("Jump"))
            {
                charController.Jump();
            }
        }

        #region Player Attacks

        private CharController.HitboxData basicAttack1;
        void HitboxSetup()
        {
            basicAttack1.damage = 10f;
            basicAttack1.hitboxCreationTime = 0.1f;
            basicAttack1.hitboxLingeringTime = 0.2f;
            basicAttack1.hitboxSize = new Vector2(0.2f, 0.2f);
            basicAttack1.colour = CharController.BladeColour.white;
            basicAttack1.hitstun = 1f;
        }

		void PlayerBasicAttack()
        {
            if (player.GetButtonDown("BasicAttack"))
            {
                charController.PlayerBasicAttack(basicAttack1);
            }
        }
		#endregion
	}
}
