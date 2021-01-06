using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPUP : MonoBehaviour
{

    public Collider ownCollision;

    [Header("SETTINGS")]
    public PowerUpSO PUP_SO;
    public int Lives;

    PlayerController PCTRL;
    bool isdying = false;

    public void Start()
    {
        PCTRL = GameObject.FindObjectOfType<PlayerController>();

        Lives = (int) (PUP_SO.GetCurrentLevelData().pup_multiplier * PUP_SO.pup_levels.pup_base);

        foreach (Collider p in PCTRL.ReturnPlayerColliders())
            Physics.IgnoreCollision(p, ownCollision);
    }

    private void LateUpdate()
    {
        if(Lives <= 0 && !isdying)
        {
            PowerUpManager.instance.DestroyGameobjectWithAnim(gameObject, 0);
            isdying = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (Lives <= 0)
        {
            return;
        }

        switch(collision.transform.tag)
        {
            case "Asteroid":
                PowerUpManager.instance.DestroyGameobjectWithAnim(collision.gameObject, 0);
                Lives--;
                break;
            default:
                break;
        }
    }
}
