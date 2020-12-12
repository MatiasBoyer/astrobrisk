﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUpManager : MonoBehaviour
{

    private static PowerUpManager Instance;
    public static PowerUpManager instance
    {
        get
        {
            Instance = GameObject.FindObjectOfType<PowerUpManager>();
            if (Instance == null)
            {
                GameObject s = new GameObject();
                Instance = s.AddComponent<PowerUpManager>();

                s.name = "PowerUpManager";

                Debug.LogWarning(" [!] PowerUpManager doesn't exist. Added a new one BUT it's using DEFAULT settings.");
            }

            return Instance;
        }
    }


    [Header("CURRENT")]
    public int CurrentMoney = 1000;

    public PowerUpSO[] PowerUPS;

    private PlayerUI PUI;
    private PowerUpSO moneygrabbr_pup;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //PUI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUI>();

        //load powerups and then loadfromplayerprefs()
        PowerUPS = Resources.LoadAll<PowerUpSO>("PowerUps");

        // !!!!! CHANGE ASAP.
        // Counter measure so that it doesnt reset the level everytime you load the mainmenu.
        if (Time.frameCount <= 750)
        {
            foreach (PowerUpSO p in PowerUPS)
                p.LoadFromPlayerPrefs();
        }

        moneygrabbr_pup = FindPUPByName("MoneyGrabber");
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
        int money = 0;
        if(moneygrabbr_pup.CurrentLevel != -1)
            money = (int) (moneygrabbr_pup.pup_levels.pup_base * moneygrabbr_pup.GetCurrentLevelData().pup_multiplier);
        return money;
    }

    // new powerup implementation
    public PowerUpSO FindPUPByName(string name)
    {
        foreach(PowerUpSO pup_so in PowerUPS)
        {
            if (pup_so.name.Contains(name))
                return pup_so;
        }

        Debug.LogError(string.Format(" Couldn't find the PowerUP by the name of '{0}'", name));
        return null;
    }
}
