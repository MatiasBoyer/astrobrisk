using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{

    [Header("Movement")]
    public float MovementSpeedIncrement;
    public float MovementSpeedTime;
    public float MovementSpeedExponent;

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
            {
                PController.MovementSpeed += MovementSpeedIncrement;
                PController.MovementSpeed = Mathf.Pow(PController.MovementSpeed, MovementSpeedExponent);
            }
            else
                yield break;
            yield return new WaitForSeconds(MovementSpeedTime);
        }
    }

}
