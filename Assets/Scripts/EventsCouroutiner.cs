using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsCouroutiner : MonoBehaviour
{

    private static EventsCouroutiner Instance;
    public static EventsCouroutiner instance
    {
        get
        {
            Instance = GameObject.FindObjectOfType<EventsCouroutiner>();
            if(Instance == null)
            {
                GameObject s = new GameObject();
                Instance = s.AddComponent<EventsCouroutiner>();

                s.name = "EventsCoroutiner";
            }

            return Instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CallEventAfter(float time, Action callback)
    {
        StartCoroutine(callAfter(time, callback));
    }

    public void CallEventFor(float seconds, Action callback)
    {
        StartCoroutine(callTime(seconds, callback));
    }

    public void CallEventAfterFor(float wait, float duration, Action callback)
    {
        StartCoroutine(callTimeAfter(wait, duration, callback));
    }

    IEnumerator callTimeAfter(float waitTime, float eventTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);

        float x = 0;
        while (x < eventTime)
        {
            x += Time.deltaTime;
            callback();
            yield return null;
        }
    }

    IEnumerator callTime(float eventTime, Action callback)
    {
        float x = 0;
        while(x < eventTime)
        {
            x += Time.deltaTime;
            callback();
            yield return null;
        }
    }

    IEnumerator callAfter(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
