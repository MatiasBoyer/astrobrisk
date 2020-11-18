using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpeed : MonoBehaviour
{

    public float PlaybackSpeed;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        anim.speed = PlaybackSpeed;
    }
}
