using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using SevenSwords.Utility;

/// <summary>
/// Enemy Base class
/// </summary>
public class Enemy : MonoBehaviour
{
    private float health;

    private Hitbox.BladeColour currentWeakness;


    // Start is called before the first frame update
    void Start()
    {
        health = 60f;
        currentWeakness = Hitbox.BladeColour.white;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getHit(float damage, float hitstun, Hitbox.BladeColour bladeColour)
    {
        // if white take damage
        if (currentWeakness == Hitbox.BladeColour.white && bladeColour == Hitbox.BladeColour.white)
        {
            health -= damage;
            Debug.Log("oof");
        }
        //if colour is correct destroy weakness
        else if (currentWeakness == bladeColour && bladeColour != Hitbox.BladeColour.white)
        {
            currentWeakness = Hitbox.BladeColour.white;
            Debug.Log("Broken");

        }

    }
}
