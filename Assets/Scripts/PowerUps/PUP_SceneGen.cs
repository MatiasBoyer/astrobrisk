using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUP_SceneGen : MonoBehaviour
{

    public GameObject PUP_ToGen;
    public float StartDistanceFromSpaceship;

    [Header("Steps")]
    public int QuantityOnGeneration = 1;
    public int MinInStep, MaxInStep;

    [Header("Generation Settings")]
    public float MinDistToGenerate = 100.0f;
    public Vector2 MinPos = new Vector2(-30, -30);
    public Vector2 MaxPos = new Vector2(30, 30);

    PlayerController PController;

    private void Awake()
    {
        PController = GameObject.FindObjectOfType<PlayerController>();

        transform.parent = null;
        transform.position = new Vector3(0, 0, StartDistanceFromSpaceship + PController.transform.position.z);

        StartCoroutine(GenerateOnProximity());
    }

    IEnumerator GenerateOnProximity()
    {
        float distToPlayer = 0.0f;

        while (true)
        {
            distToPlayer = Vector3.Distance(transform.position, PController.transform.position);

            if (distToPlayer <= MinDistToGenerate)
            {
                //Debug.Log("distToPlayer <= MinDistToGenerate");

                for (int i = 0; i < QuantityOnGeneration; i++)
                {
                    float nextStep = Random.Range(MinInStep, MaxInStep);
                    Vector3 prefabPos = new Vector3(Random.Range(MinPos.x, MaxPos.x), Random.Range(MinPos.y, MaxPos.y), transform.position.z);

                    GameObject instantiated = (GameObject)Instantiate(PUP_ToGen, prefabPos, Quaternion.identity);

                    float randTime = Random.Range(0.5f, 1.5f);

                    LeanTween.scale(instantiated, new Vector3(0, 0, 0), 0);
                    LeanTween.scale(instantiated, new Vector3(1.0f, 1.0f, 1.0f), randTime).setEaseInOutBounce();

                    transform.Translate(transform.forward * nextStep);
                }
            }

            yield return null;
        }
    }
}
