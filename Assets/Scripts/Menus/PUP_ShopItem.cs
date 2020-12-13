using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PUP_ShopItem : MonoBehaviour
{

    [Header("UI")]
    public TextMeshProUGUI UI_Name;
    public TextMeshProUGUI UI_Description;
    public TextMeshProUGUI UI_ItemCost;
    public Button UI_UpgradeButton;
    public RawImage UI_Image;

    PowerUpSO PowerUP_SO;

    public void UpdateUICost()
    {
        PowerUpSO._PUP_LevelSettings._Level l = PowerUP_SO.GetNextLevelData();
        if (l != null)
            UI_ItemCost.text = string.Format("UPGRADE\n${0}", l.UpgradeCost);
        else
        {
            UI_ItemCost.text = "NO MORE UPS!";
            UI_UpgradeButton.interactable = false;
        }
    }

    private void Start()
    {
        EventsCouroutiner.instance.CallEventAfter(0.5f, () => { UpdateUICost();});
    }

    public void UpdateData(PowerUpSO pup_so)
    {
        PowerUP_SO = pup_so;
        UI_Name.text = pup_so.name;
        UI_Description.text = pup_so.pup_desc;
        UpdateUICost();
        UI_Image.texture = pup_so.pup_img;
    }

    public void Upgrade()
    {
        PowerUpManager pupmgr = PowerUpManager.instance;
        PowerUpSO._PUP_LevelSettings._Level l = PowerUP_SO.GetNextLevelData();
        if (l != null)
        {
            if (pupmgr.CurrentMoney >= l.UpgradeCost)
            {
                pupmgr.CurrentMoney -= l.UpgradeCost;
                PowerUP_SO.CurrentLevel++;

                Vector3 dest = new Vector3(1.0f, 1.0f, 1.0f);
                LeanTween.scale(UI_UpgradeButton.gameObject, dest * 1.2f, .05f).setOnComplete(() =>
                {
                    LeanTween.scale(UI_UpgradeButton.gameObject, dest, .5f);
                });

                UpdateUICost();
            }
            else
            {
                Debug.Log(string.Format("Not enough money to buy {0}", PowerUP_SO.pup_name));
            }
        }
    }

}