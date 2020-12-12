using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUP_Spawner : MonoBehaviour
{

    public Transform SpawnPos;

    private void Awake()
    {
        PowerUpSO[] pup_so = PowerUpManager.instance.PowerUPS;
        foreach(PowerUpSO p in pup_so)
        {
            if (p.pup_gameobject == null)
                continue;
            if(p.CurrentLevel > -1)
                Instantiate(p.pup_gameobject, SpawnPos);
        }
    }

}
