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

    private RectTransform rtransform;

    private void Awake()
    {
        rtransform = GetComponent<RectTransform>();
        LeanTween.move(rtransform, new Vector2(0, -1920), 0.0f);
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
                LeanTween.move(rtransform, new Vector2(0, 0), 1.0f).setEaseInOutCubic();
                break;
            case false:
                LeanTween.move(rtransform, new Vector2(0, -1920), 1.0f).setEaseInOutCubic();
                break;
        }
    }

}