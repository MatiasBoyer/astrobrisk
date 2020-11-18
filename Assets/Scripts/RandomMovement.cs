using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{

    public float Radius, Force, Frequency;

    private float Timer;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * Force);

        if (Timer >= Frequency)
        {
            targetPosition = (Random.insideUnitSphere * Radius) + transform.parent.position;
            Timer = 0;
        }
        else
        {
            Timer += Time.deltaTime;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.parent.position, Radius);
    }
#endif
}
