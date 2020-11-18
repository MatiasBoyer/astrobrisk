using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SFX : MonoBehaviour
{
    public enum clipType { CLICK };
    public static UI_SFX instance;

    public AudioSource A_Source;

    [Header("AClips")]
    public AudioClip ac_Click;

    private void Awake()
    {
        instance = this;
    }

    public void PlayClip(clipType _type)
    {
        switch(_type)
        {
            case clipType.CLICK:
                A_Source.PlayOneShot(ac_Click); break;
        }
    }

}
