using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{

    public float TimeAfterStart = 3.0f;

    private void Start()
    {
        StartCoroutine(destroyt());
    }

    IEnumerator destroyt()
    {
        yield return new WaitForSeconds(TimeAfterStart);
        Destroy(gameObject);
        yield break;
    }

}
