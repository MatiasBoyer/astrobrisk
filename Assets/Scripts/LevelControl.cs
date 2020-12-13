using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{

    [Header("Movement")]
    public float MovementSpeedIncrement;
    public float MovementSpeedTime;

    PlayerController PController;

    private void Start()
    {
        PController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        StartCoroutine(IncrementSpeed());
    }

    public IEnumerator IncrementSpeed()
    {
        while(true)
        {
            if (PController.canMove)
                PController.MovementSpeed += MovementSpeedIncrement;
            else
                yield break;
            yield return new WaitForSeconds(MovementSpeedTime);
        }
    }

}
