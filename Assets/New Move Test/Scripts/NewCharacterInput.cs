using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using SevenSwords.CharacterCore;
using SevenSwords.Utility;

public class NewCharacterInput : MonoBehaviour
{
    //Ground Speed
    public float slowWalkSpeed = 2f;
    public float walkspeed = 5f;

    public float jumpApexTime = 0.3f;
    public float jumpHeights = 1f;

    private float currentSpeed;

    private NewCharController charController;
    public CharacterManager charManager;

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

        charManager = gameObject.GetComponent<CharacterManager>();
        if (charManager == null)
        {
            Debug.Log("No Char manager dummy");
        }
        charController.setJumpValues(jumpHeights, jumpApexTime);
    }

    // Update is called once per frame
    void Update()
    {
        CheckHorizontalInput();
        CheckJump();
        PlayerBasicAttack();
        PlayerBladeAttack();
    }   

    void CheckHorizontalInput()
    {
        if (player.GetAxis("MoveHorizontal") != 0)
        {
            if (!player.GetButton("WalkMod"))
            {
                currentSpeed = Mathf.Sign(player.GetAxis("MoveHorizontal")) * slowWalkSpeed;
                charController._stateMachine.stateInputs.horizontalAxis(currentSpeed);
            }
            else
            {
                currentSpeed = Mathf.Sign(player.GetAxis("MoveHorizontal")) * walkspeed;
                charController._stateMachine.stateInputs.horizontalAxis(currentSpeed);
            }
        }
    }

    void CheckJump()
    {
        if (player.GetButtonDown("Jump"))
        {
            charController._stateMachine.stateInputs.jump(1f);
        }
    }

    #region Player Attacks

    private Hitbox basicAttack1 = new Hitbox {
        damage = 10f,
        hitboxCreationTime = 0.1f,
        hitboxLingeringTime = 0.2f,
        hitboxSize = new Vector2(0.4f, 0.4f),
        colour = Hitbox.BladeColour.white,
        hitstun = 1f,
        hitboxOffset = new Vector2(0f, 0f)
    };

    private Hitbox greenAttack = new Hitbox
    {
        damage = 20f,
        hitboxCreationTime = 0.5f,
        hitboxLingeringTime = 0.9f,
        hitboxSize = new Vector2(1f, 0.5f),
        colour = Hitbox.BladeColour.green,
        hitstun = 5f,
        hitboxOffset = new Vector2(0f, 0f)
    };

    void PlayerBasicAttack()
        {
            if (player.GetButtonDown("BasicAttack"))
            {
                charController._stateMachine.stateInputs.attack(basicAttack1);
            }
        }

    void PlayerBladeAttack()
    {
        if (player.GetButtonDown("BladeAttack"))
        {
            switch (charManager.currentColour)
            {
                case (Hitbox.BladeColour.green):
                    charController._stateMachine.stateInputs.bladeAttack(greenAttack);
                    break;
            }
        }
    }
    #endregion
}

