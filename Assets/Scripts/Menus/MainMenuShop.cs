using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuShop : MonoBehaviour
{

    public PowerUpManager PUP_Manager;
    public Transform ShopItemsSpawnPos;

    [Header("UI")]
    public TextMeshProUGUI UI_CurrentMoney;
    public PUP_ShopItem PUPItemPref;

    private void Awake()
    {
        LeanTween.move(gameObject, new Vector2(Screen.width / 2, -Screen.height / 2), 0.0f);
    }

    private void Start()
    {
        //spawn pups on shop
        PUP_Manager = GameObject.FindObjectOfType<PowerUpManager>();

        foreach (Transform t in ShopItemsSpawnPos)
            Destroy(t.gameObject);

        foreach (PowerUpSO pup_so in PUP_Manager.PowerUPS)
        {
            PUP_ShopItem psi = Instantiate(PUPItemPref.gameObject, ShopItemsSpawnPos).GetComponent<PUP_ShopItem>();
            psi.gameObject.name = string.Format(psi.gameObject.name, pup_so.pup_name);
            psi.UpdateData(pup_so);
        }
    }

    private void FixedUpdate()
    {
        UI_CurrentMoney.text = string.Format("${0}", PUP_Manager.CurrentMoney);
    }

    public void OpenShop(bool open)
    {
        switch (open)
        {
            case true:
                LeanTween.move(gameObject, new Vector2(Screen.width / 2, Screen.height / 2), 1.0f).setEaseInOutCubic();
                break;
            case false:
                LeanTween.move(gameObject, new Vector2(Screen.width / 2, -Screen.height / 2), 1.0f).setEaseInOutCubic();
                break;
        }
    }

}