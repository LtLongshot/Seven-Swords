
using UnityEngine;
using Rewired;

public class CharacterInput : MonoBehaviour
{
    public int playerID = 0;
    public Rewired.Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(playerID) : null; } }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
