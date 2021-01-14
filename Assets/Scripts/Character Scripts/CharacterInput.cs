using UnityEngine;
using Rewired;
using SevenSwords.CharacterCore;

namespace SevenSwords.Character{
    public class CharacterInput : MonoBehaviour
    {
        //Ground Speed
        public float walkspeed = 3f;
        public float runspeed = 6;
        public float jumpPower = 20f;

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
                Debug.Log("No Cahr controller dummy");
            }
        }

        // Update is called once per frame
        void Update()
        {

            CheckIdle();
            CheckHorizontalInput();
            CheckJump();

        }

        void CheckHorizontalInput()
        {
            if (player.GetAxis("MoveHorizontal") != 0)
            {
                currentSpeed = player.GetAxis("MoveHorizontal") * walkspeed;
                charController.horizontalMove(currentSpeed);
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
                charController.Jump(jumpPower);
            }
        }
    }
}
