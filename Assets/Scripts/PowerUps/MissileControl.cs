using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl : MonoBehaviour
{

    public int Stage;

    public Transform Target;
    public Vector3 TargetPoint;

    public float RotationSpeed, MovementForce;

    public GameObject ExplosionSoundPrefab;

    private float spaceshipSpeed, distToTarget;
    private Vector3 targetDirection, newDir;
    private Rigidbody rb;
    private BoxCollider boxcol;

    private Transform shootPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxcol = GetComponent<BoxCollider>();
    }

    public void SetTarget(Transform target, Vector3 point, float _spaceshipSpeed, float missileForce, Transform _shootpos)
    {
        Target = target;
        TargetPoint = point;
        MovementForce = missileForce;

        spaceshipSpeed = _spaceshipSpeed;

        rb.velocity = new Vector3(0, -missileForce/4, spaceshipSpeed + 2f)/2;

        shootPos = _shootpos;

        StartCoroutine(missileLaunch());
    }

    private void Update()
    {
        if (Target != null)
        {
            targetDirection = TargetPoint - transform.position;
            newDir = Vector3.RotateTowards(transform.forward, targetDirection, RotationSpeed * Time.deltaTime, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);

            //Debug.DrawRay(TargetPoint, transform.position, Color.red);

            distToTarget = Vector3.Distance(transform.position, TargetPoint);
            if(distToTarget < 2.0f)
            {
                DestroyTarget();
            }
        }
        else
        {
            //Debug.Log("No target! Destroying myself.");
            Destroy(gameObject);
        }

        switch (Stage)
        {
            case 0:
                PrepareMissile();
                break;
            case 1:
                MoveTowardsTarget();
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Asteroid")
        {
            Target = collision.transform;
            DestroyTarget();
        }
    }

    public void DestroyTarget()
    {
        //Destroy(Target.gameObject);
        PowerUpManager.instance.AddMoneyToCurrent(Camera.main.WorldToScreenPoint(transform.position));
        Instantiate(ExplosionSoundPrefab, transform.position, Quaternion.identity);

        LeanTween.scale(Target.gameObject, new Vector3(0, 0, 0), 0.25f).setEaseInOutBounce().setOnComplete(() => Destroy(Target.gameObject));
        Destroy(gameObject);
    }

    private void MoveTowardsTarget()
    {
        transform.rotation = Quaternion.LookRotation(newDir);
        //rb.velocity = transform.forward * Speed;
        rb.AddRelativeForce(Vector3.forward * MovementForce);
    }

    private void PrepareMissile()
    {
        rb.AddRelativeForce(Vector3.forward * MovementForce/4);
    }

    IEnumerator missileLaunch()
    {
        boxcol.enabled = false;
        yield return new WaitForSeconds(0.1f);
        Stage = 1;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, shootPos.position) > 6.0f);
        boxcol.enabled = true;

        yield break;
    }

}
