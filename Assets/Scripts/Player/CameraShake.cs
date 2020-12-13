using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{

    [System.Serializable]
    public class ShakeSettings
    {
        // basic settings
        public float duration = 1.0f;
        public float magnitude = 5.0f;
        public float force = 7.0f;

        // dimming settings
        public bool dim_magnitude_overduration = true;
        public bool dim_force_overduration = true;

        // after the shake
        public float returntime = 0.4f;
    }

    private static CameraShake Instance;
    public static CameraShake instance
    {
        get
        {
            if(Instance == null)
            {
                Instance = GameObject.FindObjectOfType<CameraShake>();
                if (Instance == null)
                {
                    Debug.LogError("Couldn't find CameraShake. Did you add it to a camera?");
                }
            }

            return Instance;
        }
    }

    public void ShakeOnce(ShakeSettings _settings)
    {
        StartCoroutine(cor_ShakeOnce(_settings));
    }

    IEnumerator cor_ShakeOnce(ShakeSettings _s)
    {
        float elapsedtime = 0;

        Vector3 startpos = transform.localPosition;
        Vector3 randpos = Vector3.zero;

        if (_s.dim_force_overduration)
        {
            LeanTween.value(_s.force, 0, _s.duration).setOnUpdate((float v) =>
            {
                _s.force = v;
            });
        }
        if (_s.dim_magnitude_overduration)
        {
            LeanTween.value(_s.magnitude, 0, _s.duration).setOnUpdate((float v) =>
            {
                _s.magnitude = v;
            });
        }

        while (elapsedtime < _s.duration)
        {
            randpos = Random.insideUnitSphere * _s.magnitude;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startpos + randpos, _s.force * Time.deltaTime);

            elapsedtime += Time.deltaTime;
            yield return null;
        }

        LeanTween.moveLocal(gameObject, startpos, _s.returntime).setEaseOutBounce();
    }
}
