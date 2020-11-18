using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{

    public PlayerController PlayerControl;

    public void OnCollisionEnter(Collision collision)
    {
        PlayerControl.OnColEnter(collision);
    }

}
