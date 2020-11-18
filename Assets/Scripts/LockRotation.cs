using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{

    public Vector3 EulerOffset;

    private void LateUpdate()
    {
        transform.eulerAngles = EulerOffset;
    }

}
