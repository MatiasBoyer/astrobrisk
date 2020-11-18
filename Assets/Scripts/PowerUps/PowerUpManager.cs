using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUpManager : MonoBehaviour
{

    public static PowerUpManager instance;

    [Header("CURRENT")]
    public int CurrentMoney = 1000;

    [Header("MONEY GIVER")]
    public int MG_MoneyBase = 100;
    public float MG_MoneyMultiplier = 1.0f;

    private PlayerUI PUI;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        //PUI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUI>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            PUI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUI>();
        }
    }

    public void AddMoneyToCurrent(Vector2 screenPos)
    {
        int m = ReturnMoneyToGive();
        CurrentMoney += m;
        if(PUI != null)
            PUI.SpawnMoneyTextOnPos(m, screenPos, 1.5f);
    }

    public int ReturnMoneyToGive()
    {
        return (int) (MG_MoneyBase * MG_MoneyMultiplier);
    }
}
