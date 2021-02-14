using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

public class AICtrl : MonoBehaviour
{
    //Ground Speed
    public float slowWalkSpeed = 2f;
    public float walkspeed = 5f;
    public float runspeed = 6;
    public float jumpPower = 20f;

    const float jumpApexTime = 0.3f;
    const float jumpHeights = 1f;

    //private CharController charController;
    // Start is called before the first frame update
    void Start()
    {
        //charController = gameObject.GetComponent<CharController>();
        //if (charController == null)
        //{
        //    Debug.Log("No Char controller dummy");
        //}

        //charController.setJumpValues(jumpHeights, jumpApexTime);
    }

    // Update is called once per frame
    void Update()
    {
        //charController.checkIdle();
    }
}
