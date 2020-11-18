using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuShop : MonoBehaviour
{

    public PowerUpManager PUP_Manager;

    [Header("UI")]
    public TextMeshProUGUI UI_CurrentMoney;

    private void Awake()
    {
        LeanTween.move(gameObject, new Vector2(Screen.width / 2, -Screen.height / 2), 0.0f);
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