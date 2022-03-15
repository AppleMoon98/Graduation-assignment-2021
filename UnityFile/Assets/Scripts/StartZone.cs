using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;
    public Player player;

    bool heTrue = false;

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < player.hasWeapons.Length; i++)
            if (player.hasWeapons[i] == true)
                heTrue = true;

        if (other.gameObject.tag == "Player" && heTrue)
            manager.StartStage();
    }
}
