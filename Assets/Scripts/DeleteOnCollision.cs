using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnCollision : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        LeanTween.cancel(collision.gameObject);
        Destroy(collision.gameObject);
    }

}
