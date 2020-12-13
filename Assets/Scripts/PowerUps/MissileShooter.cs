using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileShooter : MonoBehaviour
{

    public enum crosshairState { OFF, FADING, ON };

    public LayerMask TargeteableLayerMask;

    public Canvas Canvas;
    public RectTransform CrosshairTransform;

    [Header("Shooter Settings")]
    public float ClickDelay;
    public float ShootDelay;

    public GameObject MissilePrefab;
    public Transform MissileSpawnPos;

    [Header("Missile Settings")]
    public float MissileForce;

    [Header("Detector Settings")]
    public Vector3 D_Offset;
    public float D_Radius;

    //public Transform Visualizer;

    private float LastClick, LastShoot;
    private Transform selectedTarget;

    private bool crosshairEnabled = false;

    PlayerController PController;
    RectTransform CanvasRect;

    crosshairState crosshairstate;

    private void Start()
    {
        PController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Canvas.worldCamera = PController.GetComponent<PlayerUI>().UI_Cam;
        CanvasRect = Canvas.GetComponent<RectTransform>();

        StartCoroutine(TargetCalculator());

        PowerUpSO pupso = PowerUpManager.instance.FindPUPByName("MissileShooter");
        ClickDelay = pupso.pup_levels.pup_base * pupso.GetCurrentLevelData().pup_multiplier;

        CrosshairTransform.localScale = Vector3.zero;
    }

    public void Update()
    {
        //Click detection
        if (Input.GetMouseButtonDown(0))
        {
            if ((Time.time - LastClick) < ClickDelay && (Time.time - LastShoot) > ShootDelay && selectedTarget != null)
            {
                Shoot(selectedTarget);
                LastShoot = Time.time;
            }

            LastClick = Time.time;
        }


        //Crosshair Movement
        if (selectedTarget != null)
        {
            Vector2 crosshairpos = Extensions.WorldToCanvas(CanvasRect, Camera.main.WorldToScreenPoint(selectedTarget.position));

            CrosshairTransform.anchoredPosition = Vector3.Lerp(CrosshairTransform.anchoredPosition, crosshairpos, Time.deltaTime * 2.5f);

            if (!crosshairEnabled && crosshairstate == crosshairState.OFF)
            {
                CrosshairTransform.gameObject.SetActive(true);
                LeanTween.scale(CrosshairTransform, new Vector3(0.5f, 0.5f, 0.5f), .5f).setEaseInOutBounce().setOnComplete(() =>
                {
                    crosshairstate = crosshairState.ON;
                });
                crosshairEnabled = true;
            }
        }
        else
        {
            if(crosshairEnabled && crosshairstate == crosshairState.ON)
            {
                LeanTween.scale(CrosshairTransform, Vector3.zero, .5f).setEaseInOutBounce().setOnComplete(()=>
                {
                    CrosshairTransform.gameObject.SetActive(false);
                    crosshairstate = crosshairState.OFF;
                });
                crosshairEnabled = false;
            }
        }

        //Debug
        //Visualizer.position = D_Offset + transform.position;
        //Visualizer.localScale = new Vector3(1, 1, 1) * D_Radius * 2;
    }

    IEnumerator TargetCalculator()
    {
        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(D_Offset + transform.position, D_Radius, TargeteableLayerMask);

            Transform selected = null;
            float mindist = Mathf.Infinity;
            foreach(Collider h in hits)
            {
                if (h.transform.name == "PlayerModel")
                    continue;

                float dist = Vector3.Distance(transform.position, h.transform.position);
                if(dist < mindist)
                {
                    selected = h.transform;
                    mindist = dist;
                }
            }

            selectedTarget = selected;

            /*if(selectedTarget != null)
                Debug.Log(selectedTarget.name);*/

            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(D_Offset + transform.position, D_Radius);
    }

    private void Shoot(Transform target)
    {
        MissileControl g = Instantiate(MissilePrefab, MissileSpawnPos.position, Quaternion.identity).GetComponent<MissileControl>();

        //Vector3 dir = MissileSpawnPos.position - target.position;

        g.SetTarget(target, target.position, PController.MovementSpeed, MissileForce, transform);

        /*RaycastHit hit;
        if(Physics.Raycast(MissileSpawnPos.position, dir, out hit, TargeteableLayerMask))
        {
            g.SetTarget(target, hit.point, PController.MovementSpeed, MissileForce);
        }
        else
        {
            Debug.LogWarning("Couldnt raycast. WTF?");
        }*/
    }
}
