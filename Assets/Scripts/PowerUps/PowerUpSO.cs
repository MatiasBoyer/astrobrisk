using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_powerup", menuName = "Create new PowerUp")]
public class PowerUpSO : ScriptableObject
{

    [System.Serializable]
    public class _PUP_LevelSettings
    {
        [System.Serializable]
        public class _Level
        {
            public int UpgradeCost;
            public float pup_multiplier;
        }

        public float pup_base;
        public List<_Level> Levels = new List<_Level>();
    }

    [Tooltip("Name displayed on the shop.")]
    public string pup_name;

    [Tooltip("Description displayed on the shop.")]
    public string pup_desc;

    [Tooltip("Levels you can buy on the shop.")]
    public _PUP_LevelSettings pup_levels;
    public int CurrentLevel = -1;
    public int DefaultLevel = -1; // DefaultLevel >= -1

    [Tooltip("Gameobject instantiated in the spaceship. (It CAN be null!)")]
    public GameObject pup_gameobject;

    public void LoadFromPlayerPrefs()
    {
        CurrentLevel = PlayerPrefs.GetInt(pup_name, DefaultLevel);
    }
    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt(pup_name, CurrentLevel);
    }
    public _PUP_LevelSettings._Level GetCurrentLevelData()
    {
        return pup_levels.Levels[CurrentLevel];
    }
    public _PUP_LevelSettings._Level GetNextLevelData()
    {
        if (CurrentLevel >= pup_levels.Levels.Count - 1)
            return null;

        return pup_levels.Levels[CurrentLevel + 1];
    }
}