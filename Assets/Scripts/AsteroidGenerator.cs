using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{

    public GameObject[] Prefabs;

    public Transform GeneratedParent;

    [Header("Steps")]
    public int QuantityOnGeneration;
    public int MinInStep, MaxInStep;

    [Header("Generation Settings")]
    public float MinDistToGenerate;
    public float MinSize, MaxSize;
    public Vector2 MinPos, MaxPos;

    PlayerController PController;

    private void Awake()
    {
        PController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        StartCoroutine(GenerateOnProximity());
    }

    IEnumerator GenerateOnProximity()
    {
        float distToPlayer = 0.0f;

        while(true)
        {
            distToPlayer = Vector3.Distance(transform.position, PController.transform.position);

            if (distToPlayer <= MinDistToGenerate)
            {
                //Debug.Log("distToPlayer <= MinDistToGenerate");

                for (int i = 0; i < QuantityOnGeneration; i++)
                {
                    float nextStep = Random.Range(MinInStep, MaxInStep);
                    GameObject randomPrefab = Prefabs[Random.Range(0, Prefabs.Length - 1)];

                    Vector3 prefabPos = new Vector3(Random.Range(MinPos.x, MaxPos.x), Random.Range(MinPos.y, MaxPos.y), transform.position.z);

                    GameObject instantiated = (GameObject) Instantiate(randomPrefab, prefabPos, Random.rotation, GeneratedParent);

                    Vector3 randomScale = new Vector3(1, 1, 1) * Random.Range(MinSize, MaxSize);

                    float randTime = Random.Range(0.5f, 1.5f);

                    LeanTween.scale(instantiated, new Vector3(0, 0, 0), 0);
                    LeanTween.scale(instantiated, randomScale, randTime).setEaseInOutBounce();

                    transform.Translate(transform.forward * nextStep);
                }
            }

            yield return null;
        }
    }

}