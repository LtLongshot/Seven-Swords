using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using SevenSwords.CharacterCore;

public class NewCharacterInput : MonoBehaviour
{
    //Ground Speed
    public float slowWalkSpeed = 2f;
    public float walkspeed = 5f;
    public float runspeed = 6;

    public float jumpApexTime = 0.3f;
    public float jumpHeights = 1f;

    private float currentSpeed;

    private NewCharController charController;

    public int playerID = 0;
    public Rewired.Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(playerID) : null; } }

       

    void Start()
    {
        //put this here for now needs to go into startup
        //Application.targetFrameRate = 60;

        charController = gameObject.GetComponent<NewCharController>();
        if (charController == null)
        {
            Debug.Log("No Char controller dummy");
        }

        charController.setJumpValues(jumpHeights, jumpApexTime);

        HitboxSetup();

    }

    // Update is called once per frame
    void Update()
    {
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
                currentSpeed = Mathf.Sign(player.GetAxis("MoveHorizontal")) * slowWalkSpeed;
                charController._stateMachine.stateInputs.horizontalAxis(player.GetAxis("MoveHorizontal"));
            }
            else
            {
                currentSpeed = Mathf.Sign(player.GetAxis("MoveHorizontal")) * walkspeed;
                charController._stateMachine.stateInputs.horizontalAxis(player.GetAxis("MoveHorizontal"));
            }
        }
    }

    void CheckJump()
    {
        if (player.GetButtonDown("Jump"))
        {
                charController.jump();
        }
    }

        #region Player Attacks

        private NewCharController.HitboxData basicAttack1;
        void HitboxSetup()
        {
            basicAttack1.damage = 10f;
            basicAttack1.hitboxCreationTime = 0.1f;
            basicAttack1.hitboxLingeringTime = 0.2f;
            basicAttack1.hitboxSize = new Vector2(0.2f, 0.2f);
            basicAttack1.colour = NewCharController.BladeColour.white;
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

