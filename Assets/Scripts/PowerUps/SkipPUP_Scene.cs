using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    ShaderEffect_BleedingColors bleedcolor;

    private void Awake()
    {
        col = GetComponent<Collider>();

        duration = PUP_SO.GetCurrentLevelData().pup_multiplier * PUP_SO.pup_levels.pup_base;
    }

    private void Start()
    {
        lctrl = GameObject.FindObjectOfType<LevelControl>();
        pctrl = GameObject.FindObjectOfType<PlayerController>();
        bleedcolor = Camera.main.GetComponent<ShaderEffect_BleedingColors>();
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

    IEnumerator pup_skip()
    {
        started = true;

        bool upd = true;

        float t = 0.0f;
        float s = 0.0f;

        float startspeed = 0.0f;

        lctrl.StopAllCoroutines();
        startspeed = pctrl.MovementSpeed;
        pctrl.EnableColliders(false);

        bleedcolor.intensity = 0;
        bleedcolor.shift = 0;

        bleedcolor.enabled = true;

        while(upd)
        {
            if (s < maxspeed)
                s += Time.deltaTime * 2.5f;

            bleedcolor.intensity = Extensions.Remap(s, 0, maxspeed * .25f, 0, 3);
            bleedcolor.shift += Time.deltaTime;

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

                LeanTween.value(bleedcolor.shift, 0, 1.25f).setEaseInOutCubic().setOnUpdate((float val) =>
                {
                    bleedcolor.shift = val;
                }).setOnComplete(() => bleedcolor.enabled = false);

                upd = false;
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }

}
