using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SkipPUP_Scene : MonoBehaviour
{

    public Transform Model;
    public float ModelRotationSpeed;

    [Header("SETTINGS")]
    public PowerUpSO PUP_SO;
    public float duration;
    public float maxspeed;

    bool started = false;

    Collider col;
    LevelControl lctrl;
    PlayerController pctrl;
    MusicPlayer mplayer;

    private PostProcessVolume v;
    private ChromaticAberration cab;
    private LensDistortion ldi;

    private void Awake()
    {
        col = GetComponent<Collider>();

        duration = PUP_SO.GetCurrentLevelData().pup_multiplier * PUP_SO.pup_levels.pup_base;

        lctrl = GameObject.FindObjectOfType<LevelControl>();
        pctrl = GameObject.FindObjectOfType<PlayerController>();
        mplayer = GameObject.FindObjectOfType<MusicPlayer>();

        v = Camera.main.GetComponent<PostProcessVolume>();
        v.profile.TryGetSettings(out cab);
        v.profile.TryGetSettings(out ldi);
    }

    private void Start()
    {
        StartCoroutine(chck_dist(2.0f));
    }

    private void Update()
    {
        Model.Rotate(new Vector3(0,0, 1 * ModelRotationSpeed));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.name == "PlayerModel")
        {
            Model.gameObject.SetActive(false);
            if (!started)
            {
                StartCoroutine(pup_skip());
                col.enabled = false;
            }
        }
    }


    bool upd = false;
    IEnumerator pup_skip()
    {
        upd = true;
        started = true;

        float t = 0.0f;
        float s = 0.0f;

        float startspeed = 0.0f;

        lctrl.StopAllCoroutines();
        startspeed = pctrl.MovementSpeed;
        pctrl.EnableColliders(false);

        //Debug.Log(duration);

        mplayer.AudioDistortion(maxspeed / 50, duration * 3 / 4, 1.5f);

        LeanTween.value(ldi.intensity, -50, duration * 3 / 4).setOnUpdate((float val) =>
        {
            ldi.intensity.value = val;
        }).setOnComplete(() =>
        {
            LeanTween.value(ldi.intensity, 0, duration * 1 / 4).setOnUpdate((float val) =>
            {
                ldi.intensity.value = val;
            });
        });
        LeanTween.value(cab.intensity, 1, duration * 3 / 4).setOnUpdate((float val) =>
        {
            cab.intensity.value = val;
        }).setOnComplete(() =>
        {
            LeanTween.value(cab.intensity, 0, duration * 1 / 4).setOnUpdate((float val) =>
            {
                cab.intensity.value = val;
            });
        });

        while (upd)
        {
            if (s < maxspeed)
                s += Time.deltaTime * 2.5f;

            pctrl.MovementSpeed = pctrl.MovementSpeed + s;

            t += Time.deltaTime;
            if(t >= duration)
            {
                LeanTween.value(pctrl.MovementSpeed, startspeed, 1.5f).setOnUpdate((float val) =>
                {
                    pctrl.MovementSpeed = val;
                }).setOnComplete(() =>
                {
                    lctrl.StartCoroutine(lctrl.IncrementSpeed());
                    pctrl.EnableColliders(true);
                });

                upd = false;
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator chck_dist(float t)
    {
        while(true)
        {
            if (!upd)
            {
                float dist = Vector3.Distance(transform.position, pctrl.transform.position);
                if (dist >= 1000.0f)
                    Destroy(gameObject);
            }
            yield return new WaitForSeconds(t);
        }
    }
}
