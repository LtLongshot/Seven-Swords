using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;

/// <summary>
/// Enemy Base class
/// </summary>
public class Enemy : MonoBehaviour
{
    private float health;

    private CharController.BladeColour currentWeakness;


    // Start is called before the first frame update
    void Start()
    {
        health = 60f;
        currentWeakness = CharController.BladeColour.white;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getHit(float damage, float hitstun, CharController.BladeColour bladeColour)
    {
        // if white take damage
        if (currentWeakness == CharController.BladeColour.white && bladeColour == CharController.BladeColour.white)
        {
            health -= damage;
            //Debug.Log("oof");
        }
        //if colour is correct destroy weakness
        else if (currentWeakness == bladeColour && bladeColour != CharController.BladeColour.white)
        {
            currentWeakness = CharController.BladeColour.white;
            //Debug.Log("Broken");

        }

    }
}
