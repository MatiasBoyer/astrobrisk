using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    public Transform _Transform;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(-_Transform.forward, _Transform.up);
    }

}
