using UnityEngine;

public class PlayerStart : ThingController 
{
    public static PlayerStart[] PlayerStarts = new PlayerStart[4];

    public int PlayerNumber;

    void Awake()
    {
        if (PlayerNumber < 1 || PlayerNumber > 4)
            Debug.LogError("PlayerStart: Awake: PlayerNumber out of range");

        PlayerStarts[PlayerNumber - 1] = this;
    }
}
